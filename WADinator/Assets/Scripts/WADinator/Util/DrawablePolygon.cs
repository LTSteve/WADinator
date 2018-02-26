using System.IO;
using System;
using UnityEngine;
using UnityEditor;
using WADinator.Structures.Textmap;
using WADinator.Controllers;
using System.Collections.Generic;

namespace WADinator.Util
{
    public class DrawablePolygon
    {
        public List<LineDef> lines;
        public List<Vector2> vecs;
        public List<DrawableTriangle> triangles;

        private float height = 0f;

        public DrawablePolygon(TextMap parent, List<LineDef> input)
        {
            lines = DrawUtils.LineUpVerts(input);

            if (lines.Count < 3)
            {
                return;
            }

            vecs = new List<Vector2>();

            for (var i = 0; i < input.Count; i++)
            {
                vecs.Add(DrawUtils.LineToVec2(input[i]));
            }

            //make vecs ccw
            if (VecsAreClockwise())
            {
                vecs.Reverse();
            }

            triangles = DrawUtils.GetTriangles(vecs);
        }

        public void SetHeight(float height)
        {
            this.height = height;
        }

        public Vector3[] GetVertices()
        {
            var output = new Vector3[vecs.Count];

            for (var i = 0; i < vecs.Count; i++)
            {
                var vec = vecs[i];

                output[i] = new Vector3(vec.x / 64f, height / 64f, vec.y / 64f);
            }

            return output;
        }

        public Vector2[] GetUVs()
        {
            var uvs = new Vector2[vecs.Count];

            var pxAdjustment = 1f / 64f;

            for(var i = 0; i < triangles.Count; i++)
            {
                var v1 = triangles[i].pointIds[0];
                var v2 = triangles[i].pointIds[1];
                var v3 = triangles[i].pointIds[2];

                uvs[v1] = new Vector2(vecs[v1].x * pxAdjustment, vecs[v1].y * pxAdjustment);
                uvs[v2] = new Vector2(vecs[v2].x * pxAdjustment, vecs[v2].y * pxAdjustment);
                uvs[v3] = new Vector2(vecs[v3].x * pxAdjustment, vecs[v3].y * pxAdjustment);
            }

            return uvs;
        }

        public int[] GetTriangles(bool up)
        {
            var tris = new int[3 * (vecs.Count - 2)];    //3 verts per triangle * num triangles
            
            for(var i = 0; i < triangles.Count; i++)
            {
                var i3 = i * 3;

                tris[i3] = triangles[i].pointIds[0];
                if (up)
                {
                    tris[i3 + 1] = triangles[i].pointIds[1];
                    tris[i3 + 2] = triangles[i].pointIds[2];
                }
                else
                {
                    tris[i3 + 1] = triangles[i].pointIds[2];
                    tris[i3 + 2] = triangles[i].pointIds[1];
                }
            }

            return tris;
        }

        private bool VecsAreClockwise()
        {
            var sum = 0f;

            for(var i = 0; i < vecs.Count;i++)
            {
                var j = (i + 1) % vecs.Count;
                sum += (vecs[j].x - vecs[i].x) * (vecs[j].y + vecs[i].y);
            }

            return sum > 0;
        }

        public bool ContainsPoint(float x, float y)
        {
            foreach(var triangle in triangles)
            {
                if (triangle.ContainsPoint(x, y))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
