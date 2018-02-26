using System.IO;
using System;
using UnityEngine;
using UnityEditor;
using WADinator.Structures.Textmap;
using WADinator.Controllers;
using System.Collections.Generic;
using WADinator.Util;

namespace WADinator.Structures
{
    [Serializable]
    public class WAD
    {
        [NonSerialized]
        public readonly Header header;

        public List<TextMap> textmaps;

        public List<string> textmapNames;
        
        public string filePath;
        public string fileName;

        private Material defaultMaterial;

        private IEnumerable<WADHooks> hooks;

        public WAD(Header h, List<string> tms, string filepath, Material mat)
        {
            header = h;
            textmapNames = tms;
            filePath = filepath;
            var fpath = filepath.Split('\\');
            fileName = fpath[fpath.Length-1];

            defaultMaterial = mat;
        }

        public void Create(int tmId, WADController wadMap)
        {
            textmaps = new List<TextMap>();

            var stream = File.OpenRead(filePath);

            var workingTitle = string.Empty;

            hooks = ReflectiveEnumerator.GetEnumerableOfType<WADHooks>();

            EditorUtility.DisplayProgressBar("Loading Map...", "Finding Lumps", 0.05f);

            for (var i = 0; i < header.entriesCount; i++)
            {
                stream.Seek(header.lumpDefsLocation + i * WADLoad.LUMP_HEADER_SIZE, SeekOrigin.Begin);

                var lumpHeaderData = new byte[WADLoad.LUMP_HEADER_SIZE];

                stream.Read(lumpHeaderData, 0, lumpHeaderData.Length);

                var lump = new Lump
                {
                    location = WADLoad.IntFromBytes(lumpHeaderData, 0),
                    size = WADLoad.IntFromBytes(lumpHeaderData, 4),
                    name = WADLoad.StringFromBytes(lumpHeaderData, 8, 8)
                };

                //TODO: figure out what to do with the rest of the lumps
                if (lump.name == "TEXTMAP")
                {
                    stream.Seek(lump.location, SeekOrigin.Begin);

                    var textMapData = new byte[lump.size];

                    stream.Read(textMapData, 0, textMapData.Length);

                    textmaps.Add(new TextMap(WADLoad.StringFromBytes(textMapData), workingTitle));
                }
                else
                {
                    workingTitle = lump.name;
                }
            }

            var textmap = textmaps[tmId];

            var sectorLines = new Dictionary<int, List<LineDef>>();

            for (var i = 0; i < textmap.sectors.Count; i++)
            {
                var sector = textmap.sectors[i];

                var sectorGo = new GameObject("Sector"+i);

                sectorGo.transform.parent = wadMap.transform;
            }

            EditorUtility.DisplayProgressBar("Loading Map...", "Building Walls", 0.5f);

            for (var i = 0; i < textmap.lineDefs.Count; i++)
            {
                var line = textmap.lineDefs[i];
                

                if (!sectorLines.ContainsKey((int)line.sidefrontRef.sector))
                {
                    sectorLines[(int)line.sidefrontRef.sector] = new List<LineDef>();
                }
                sectorLines[(int)line.sidefrontRef.sector].Add(line);
                
                if (line.sidebackRef != null)
                {
                    if (!sectorLines.ContainsKey((int)line.sidebackRef.sector))
                    {
                        sectorLines[(int)line.sidebackRef.sector] = new List<LineDef>();
                    }
                    sectorLines[(int)line.sidebackRef.sector].Add(line);
                }

                var miny = line.sidefrontRef.sectorRef.heightfloor;
                var maxy = line.sidefrontRef.sectorRef.heightceiling;

                var position = new Vector3((line.v1Ref.x + line.v2Ref.x) / 2f, miny + (maxy - miny) / 2f, (line.v1Ref.y + line.v2Ref.y) / 2f) / 64f;

                var rotation = Quaternion.FromToRotation(new Vector3(1, 0, 0),
                    new Vector3(line.v2Ref.x - line.v1Ref.x, 0, line.v2Ref.y - line.v1Ref.y).normalized);

                var scale = new Vector3(new Vector2(line.v2Ref.x - line.v1Ref.x, line.v2Ref.y - line.v1Ref.y).magnitude, maxy - miny, 1) / 64f;

                var textures = new Material[2];
                
                if(line.sidefrontRef.texturemiddle != "-" && !string.IsNullOrEmpty(line.sidefrontRef.texturemiddle))
                {
                    textures[0] = FileUtils.GetMaterial(line.sidefrontRef.texturemiddle);
                    textures[0] = textures[0] == null ? defaultMaterial : textures[0];
                }
                if (line.sidebackRef != null && line.sidebackRef.texturemiddle != "-" && !string.IsNullOrEmpty(line.sidebackRef.texturemiddle))
                {
                    textures[1] = FileUtils.GetMaterial(line.sidebackRef.texturemiddle);
                    textures[1] = textures[1] == null ? defaultMaterial : textures[1];
                }

                var walls = DrawUtils.CreateWall(position, rotation, scale, 
                    new[] { textures[0] == null ? null : GameObject.Find("Sector" + line.sidefrontRef.sector).transform,
                        textures[1] == null ? null : wadMap.transform.Find("Sector" + line.sidebackRef.sector) }, textures);

                if(walls[0] != null)
                {
                    walls[0].name = "LineDef " + line.v1Ref.x + "," + line.v1Ref.y + " - " + line.v2Ref.x + "," + line.v2Ref.y;
                }
                if(walls[1] != null)
                {
                    walls[1].name = "LineDef " + line.v1Ref.x + "," + line.v1Ref.y + " - " + line.v2Ref.x + "," + line.v2Ref.y + " i";
                }

                //draw texturetop and texturebottom
                if(line.sidefrontRef != null && line.sidebackRef != null)
                {
                    Material mat = null;
                    GameObject result = null;
                    if (!string.IsNullOrEmpty(line.sidefrontRef.texturetop))
                    {
                        mat = FileUtils.GetMaterial(line.sidefrontRef.texturetop);
                        result = DrawUtils.CreateTopWall(line.v1Ref, line.v2Ref, 
                            GameObject.Find("Sector" + line.sidefrontRef.sector).transform,
                            mat == null ? defaultMaterial : mat,
                            line.sidefrontRef.sectorRef, line.sidebackRef.sectorRef);
                        result.name = "TextureTop " + line.v1Ref.x + "," + line.v1Ref.y + " - " + line.v2Ref.x + "," + line.v2Ref.y;
                    }
                    if (!string.IsNullOrEmpty(line.sidefrontRef.texturebottom))
                    {
                        mat = FileUtils.GetMaterial(line.sidefrontRef.texturebottom);
                        result = DrawUtils.CreateBottomWall(line.v1Ref, line.v2Ref,
                            GameObject.Find("Sector" + line.sidefrontRef.sector).transform,
                            mat == null ? defaultMaterial : mat,
                            line.sidefrontRef.sectorRef, line.sidebackRef.sectorRef);
                        result.name = "TextureBottom " + line.v1Ref.x + "," + line.v1Ref.y + " - " + line.v2Ref.x + "," + line.v2Ref.y;
                    }
                }
            }

            EditorUtility.DisplayProgressBar("Loading Map...", "Building Floors", .7f);
            for (var i = 0; i < textmap.sectors.Count; i++)
            {
                var sector = textmap.sectors[i];

                var miny = sector.heightfloor;
                var maxy = sector.heightceiling;

                sectorLines[i] = DrawUtils.LineUpVerts(sectorLines[i]);
                
                try
                {
                    sector.drawablePolygon = new DrawablePolygon(textmap, sectorLines[i]);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    continue;
                }

                var floortexture = FileUtils.GetMaterial(sector.texturefloor);
                var ceilingtexture = FileUtils.GetMaterial(sector.textureceiling);

                var floor = DrawUtils.CreateFloor(sector.drawablePolygon, miny, wadMap.transform.Find("Sector" + i), textmap, floortexture, "Sector " + i + " Floor");
                floor.AddComponent<MeshCollider>();

                var ceil = DrawUtils.CreateCeiling(sector.drawablePolygon, maxy, wadMap.transform.Find("Sector" + i), textmap, ceilingtexture, "Sector " + i + " Ceiling");
                ceil.AddComponent<MeshCollider>();
            }


            EditorUtility.DisplayProgressBar("Loading Map...", "Placing Things", .9f);

            for (var i = 0; i < textmap.things.Count; i++)
            {
                var thing = textmap.things[i];

                var thingObject = GameObject.Instantiate(WADController.objectPrefab);

                var sector = textmap.FindSector(thing.x, thing.y);

                var height = (thing.height/64f) + 
                    (sector != null ? sector.heightfloor/64f : 0);

                thingObject.transform.position = new Vector3(thing.x / 64f, height + 0.5f, thing.y / 64f);

                var thingController = thingObject.GetComponent<ThingController>();
                thingController.thing = thing;
                thingController.thingString = thing.ToString();

                thingObject.SetParent(wadMap.transform);
                
                foreach(var hook in hooks)
                {
                    hook.CreateThing(thing, thingObject.gameObject);
                }
            }
        }
    }
}
