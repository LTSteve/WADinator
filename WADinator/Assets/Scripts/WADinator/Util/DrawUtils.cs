using System.IO;
using System;
using UnityEngine;
using UnityEditor;
using WADinator.Structures.Textmap;
using WADinator.Controllers;
using System.Collections.Generic;

namespace WADinator.Util
{
    public static class DrawUtils
    {
        public static List<LineDef> LineUpVerts(List<LineDef> unsorted)
        {
            var linedUp = new List<LineDef>();

            var prevSelection = unsorted[0];
            linedUp.Add(prevSelection);

            //Line up all linedefs
            while (linedUp.Count < unsorted.Count)
            {
                LineDef nextSelection = null;

                for (var i = 0; i < unsorted.Count; i++)
                {
                    var lineDef = unsorted[i];

                    //flip check
                    if ((lineDef.v1 == prevSelection.v1 && lineDef.v2 != prevSelection.v2) ||
                        (lineDef.v2 == prevSelection.v2 && lineDef.v1 != prevSelection.v1))
                    {
                        var t = lineDef.v1;
                        var tRef = lineDef.v1Ref;
                        lineDef.v1 = lineDef.v2;
                        lineDef.v1Ref = lineDef.v2Ref;
                        lineDef.v2 = t;
                        lineDef.v2Ref = tRef;
                    }

                    if (lineDef.v1 == prevSelection.v2 && lineDef.v2 != prevSelection.v1)
                    {
                        nextSelection = lineDef;
                        break;
                    }
                }
                if (nextSelection == null)
                {
                    Debug.LogError("Failed to work with some verticies for a floor/ceiling the file format may be off");
                    return new List<LineDef>();
                }

                prevSelection = nextSelection;

                linedUp.Add(nextSelection);
            }
            
            return linedUp;
        }
        
        public static List<DrawableTriangle> GetTriangles(List<Vector2> vecsIn)
        {
            //copy vecsIn so as not to change working vec
            var vecs = new List<Vector2>();

            for(var i = 0; i < vecsIn.Count; i++)
            {
                vecs.Add(vecsIn[i]);
            }

            //create a reference list of ids for each of the vectors
            var vecIds = new Dictionary<Vector2, int>();

            for (var i = 0; i < vecs.Count; i++)
            {
                vecIds[vecs[i]] = i;
            }

            var output = new List<DrawableTriangle>();

            var iteration = 0;

            while(vecs.Count > 2)
            {
                for(var i = 0; i < vecs.Count; i++)
                {
                    //try and get a triangle from i-1, i, i+1
                    var triangle = TryGetTriangle(vecs, i, vecIds);

                    if (triangle != null)
                    {
                        output.Add(triangle);
                        vecs.Remove(vecs[i]);
                        break;
                    }
                }
                iteration++;
                if(iteration > vecsIn.Count * vecsIn.Count && iteration > 1000)
                {
                    throw new Exception("Failed to triangulate a floor or ceiling! The WAD data may be a bit off");
                }
            }

            return output;
        }

        private static DrawableTriangle TryGetTriangle(List<Vector2> vecs, int index, Dictionary<Vector2, int> ids)
        {
            var corner = new List<Vector2>();
            var cornerIds = new List<int>();

            //add corners at i-1, i, i+1
            corner.Add(vecs[(index + vecs.Count - 1) % vecs.Count]);
            corner.Add(vecs[index]);
            corner.Add(vecs[(index + 1) % vecs.Count]);

            cornerIds.Add(ids[corner[0]]);
            cornerIds.Add(ids[corner[1]]);
            cornerIds.Add(ids[corner[2]]);

            var rest = new List<Vector2>();

            for(var i = 0; i < vecs.Count; i++)
            {
                if(i != index &&
                    (index + vecs.Count - 1) % vecs.Count != i &&
                    (index + 1) % vecs.Count != i)
                {
                    rest.Add(vecs[i]);
                }
            }

            //make sure the corner is convex
            bool straight;
            var angleTest = AngleTest(corner, out straight);
            
            if (straight)
            {
                vecs.Remove(vecs[index]);
                return null;
            }

            var clipTest = TriangleTest(rest, corner);

            if(angleTest && clipTest)
            {
                return new DrawableTriangle(corner, cornerIds);
            }

            return null;
        }

        //returns true if points make a 180 degree angle or less in the order presented
        private static bool AngleTest(List<Vector2> corner, out bool straight)
        {
            var side1 = corner[1] - corner[0];
            var side2 = corner[2] - corner[1];

            var angle = Vector2.SignedAngle(side1, side2);

            straight = angle == 0f;

            return angle > 0;
        }

        private static bool TriangleTest(List<Vector2> rest, List<Vector2> triangle)
        {
            foreach(var vec in rest)
            {
                if(PointInTriangle(vec, triangle))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool PointInTriangle(Vector2 point, List<Vector2> triangle)
        {
            return SameSide(point, triangle[0], triangle[1], triangle[2]) &&
                    SameSide(point, triangle[1], triangle[0], triangle[2]) &&
                    SameSide(point, triangle[2], triangle[0], triangle[1]);
        }

        private static bool SameSide(Vector2 p1, Vector2 p2, Vector2 a, Vector2 b)
        {
            var cp1 = Vector3.Cross(b - a, p1 - a);
            var cp2 = Vector3.Cross(b - a, p2 - a);

            return Vector3.Dot(cp1, cp2) > 0;
        }

        //returns true if points are on the far side of the line (inclusive) while they are between the bounds
        private static bool LineTest(List<Vector2> toTest, Vector2 near, Line line, Line bound1, Line bound2)
        {
            for(var i = 0; i < toTest.Count; i++)
            {
                var testVec = toTest[i];

                var between = Line.PointIsBetween(bound1, bound2, testVec);

                var past = line.OppositeSides(near, testVec);

                if(!between || past)
                {
                    continue;
                }

                return false;
            }

            return true;
        }

        public static Vector2 LineToVec2(LineDef input)
        {
            return new Vector2(input.v1Ref.x, input.v1Ref.y);
        }

        public static GameObject[] CreateWall(Vector3 position, Quaternion rotation, Vector3 scale, Transform[] parents, Material[] materials)
        {

            GameObject newQuad1 = null;
            GameObject newQuad2 = null;

            if (materials[0] != null)
            {
                newQuad1 = GameObject.CreatePrimitive(PrimitiveType.Quad);

                newQuad1.transform.SetParent(parents[0]);

                newQuad1.transform.localScale = scale;
                newQuad1.transform.SetPositionAndRotation(position, rotation);
                
                var meshRenderer = newQuad1.GetComponent<MeshRenderer>();
                var material = new Material(materials[0]);
                material.mainTextureScale = new Vector2(scale.x / 2f, scale.y / 2f);
                meshRenderer.material = material;
            }

            if (materials[1] != null)
            {
                newQuad2 = GameObject.CreatePrimitive(PrimitiveType.Quad);

                newQuad2.transform.SetParent(parents[1]);

                newQuad2.transform.localScale = scale;
                newQuad2.transform.SetPositionAndRotation(position, rotation * Quaternion.Euler(0, 180f, 0));

                var meshRenderer = newQuad2.GetComponent<MeshRenderer>();
                var material = new Material(materials[0]);
                material.mainTextureScale = new Vector2(scale.x / 2f, scale.y / 2f);
                meshRenderer.material = material;
            }

            return new[] { newQuad1, newQuad2 };
        }

        public static GameObject CreateBottomWall(Vertex vert1, Vertex vert2, Transform parent, Material material, Sector from, Sector to)
        {
            var miny = from.heightfloor;
            var maxy = to.heightfloor;

            var position = new Vector3((vert1.x + vert2.x) / 2f, miny + (maxy - miny) / 2f, (vert1.y + vert2.y) / 2f) / 64f;

            var rotation = Quaternion.FromToRotation(new Vector3(1, 0, 0),
                new Vector3(vert2.x - vert1.x, 0, vert2.y - vert1.y).normalized);

            var scale = new Vector3(new Vector2(vert2.x - vert1.x, vert2.y - vert1.y).magnitude, maxy - miny, 1) / 64f;

            var newQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);

            newQuad.transform.SetParent(parent);

            newQuad.transform.localScale = scale;
            newQuad.transform.SetPositionAndRotation(position, rotation);

            newQuad.GetComponent<MeshRenderer>().material = material;

            return newQuad;
        }

        public static GameObject CreateTopWall(Vertex vert1, Vertex vert2, Transform parent, Material material, Sector from, Sector to)
        {
            var miny = to.heightceiling;
            var maxy = from.heightceiling;

            var position = new Vector3((vert1.x + vert2.x) / 2f, miny + (maxy - miny) / 2f, (vert1.y + vert2.y) / 2f) / 64f;

            var rotation = Quaternion.FromToRotation(new Vector3(1, 0, 0),
                new Vector3(vert2.x - vert1.x, 0, vert2.y - vert1.y).normalized);

            var scale = new Vector3(new Vector2(vert2.x - vert1.x, vert2.y - vert1.y).magnitude, maxy - miny, 1) / 64f;

            var newQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);

            newQuad.transform.SetParent(parent);

            newQuad.transform.localScale = scale;
            newQuad.transform.SetPositionAndRotation(position, rotation);

            newQuad.GetComponent<MeshRenderer>().material = material;

            return newQuad;
        }

        public static GameObject CreateFloor(DrawablePolygon poly, int height, Transform parent, TextMap textmap, Material mat, string name = "Floor")
        {
            var meshObject = new GameObject();
            meshObject.transform.parent = parent;
            meshObject.name = name;

            var meshFilter = meshObject.AddComponent<MeshFilter>();
            var meshRenderer = meshObject.AddComponent<MeshRenderer>();

            var mesh = CreateMesh(poly, height, textmap);

            meshRenderer.material = mat;

            meshFilter.mesh = mesh;

            return meshObject;
        }

        public static GameObject CreateCeiling(DrawablePolygon poly, int height, Transform parent, TextMap textmap, Material mat, string name = "Floor")
        {
            var meshObject = new GameObject();
            meshObject.transform.parent = parent;
            meshObject.name = name;

            var meshFilter = meshObject.AddComponent<MeshFilter>();
            var meshRenderer = meshObject.AddComponent<MeshRenderer>();

            var mesh = CreateMesh(poly, height, textmap, true);

            meshRenderer.material = mat;

            meshFilter.mesh = mesh;

            return meshObject;
        }

        public static Mesh CreateMesh(DrawablePolygon poly, int height, TextMap textmap, bool reverseVerts = false)
        {
            //Create a new mesh
            var mesh = new Mesh();

            poly.SetHeight(height);

            //Vertices
            var vertex = poly.GetVertices();

            //UVs
            var uvs = poly.GetUVs();

            //Triangles
            var tris = poly.GetTriangles(reverseVerts);

            //Assign data to mesh
            mesh.vertices = vertex;
            mesh.uv = uvs;
            mesh.triangles = tris;

            //Recalculations
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            //mesh.Optimize();

            //Name the mesh
            mesh.name = "DynamicMesh";

            //Return the mesh
            return mesh;
        }
    }
}
