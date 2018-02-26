using System.IO;
using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

namespace WADinator.Structures.Textmap
{
    [Serializable]
    public class TextMap
    {
        [SerializeField]
        public string name;

        public readonly List<Block> blocks;

        public readonly List<LineDef> lineDefs = new List<LineDef>();
        public readonly List<SideDef> sideDefs = new List<SideDef>();
        public readonly List<Vertex> vertecies = new List<Vertex>();
        public readonly List<Sector> sectors = new List<Sector>();
        public readonly List<Thing> things = new List<Thing>();

        public TextMap(string data, string name)
        {
            this.name = name;

            data = TextMapFormatParseTools.StripComments(data);

            EditorUtility.DisplayProgressBar("Loading Map...", "Building Blocks", 0.1f);

            blocks = TextMapFormatParseTools.GetBlocks(data);

            for(var i = 0; i < blocks.Count;i++)
            {
                EditorUtility.DisplayProgressBar("Loading Map...", "Defining Blocks", 0.1f + (i / (float)blocks.Count) * 0.4f);

                var block = blocks[i];

                switch (block.type.ToLowerInvariant())
                {
                    case "linedef":
                        lineDefs.Add(new LineDef(block.properties));
                        break;
                    case "sidedef":
                        sideDefs.Add(new SideDef(block.properties));
                        break;
                    case "vertex":
                        vertecies.Add(new Vertex(block.properties));
                        break;
                    case "sector":
                        sectors.Add(new Sector(block.properties));
                        break;
                    case "thing":
                        things.Add(new Thing(block.properties));
                        break;
                }
            }

            //setup refs
            foreach(var sideDef in sideDefs)
            {
                sideDef.sectorRef = FindSector(sideDef.sector);
            }
            foreach(var lineDef in lineDefs)
            {
                lineDef.v1Ref = FindVertex(lineDef.v1);
                lineDef.v2Ref = FindVertex(lineDef.v2);
                lineDef.sidefrontRef = FindSideDef(lineDef.sidefront);
                lineDef.sidebackRef = FindSideDef(lineDef.sideback);
            }
        }

        public Vertex FindVertex(int id)
        {
            if(id < 0)
            {
                return null;
            }

            return vertecies[id];
        }

        public Sector FindSector(int id)
        {
            if (id < 0)
            {
                return null;
            }

            return sectors[id];
        }

        public Sector FindSector(float x, float y)
        {
            foreach(var sector in sectors)
            {
                if(sector.drawablePolygon == null)
                {
                    continue;
                }

                if (sector.drawablePolygon.ContainsPoint(x, y))
                {
                    return sector;
                }
            }

            return null;
        }

        public SideDef FindSideDef(int id)
        {
            if (id < 0)
            {
                return null;
            }

            return sideDefs[id];
        }

        public new string ToString()
        {
            var output = string.Empty;
            
            output += "lineDefs:\n";

            foreach(var def in lineDefs)
            {
                output += def.ToString() + "\n";
            }

            output += "sideDefs:\n";

            foreach (var def in sideDefs)
            {
                output += def.ToString() + "\n";
            }

            output += "vertecies:\n";

            foreach (var def in vertecies)
            {
                output += def.ToString() + "\n";
            }

            output += "sectors:\n";

            foreach (var def in sectors)
            {
                output += def.ToString() + "\n";
            }

            output += "things:\n";

            foreach (var def in things)
            {
                output += def.ToString() + "\n";
            }

            return output;
        }
    }
}
