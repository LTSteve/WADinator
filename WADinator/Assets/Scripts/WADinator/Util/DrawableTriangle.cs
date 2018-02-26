using System.IO;
using System;
using UnityEngine;
using UnityEditor;
using WADinator.Structures.Textmap;
using WADinator.Controllers;
using System.Collections.Generic;

namespace WADinator.Util
{
    public class DrawableTriangle
    {
        public List<Vector2> points;
        public List<int> pointIds;

        public DrawableTriangle(List<Vector2> points, List<int> pointIds)
        {
            this.points = points;
            this.pointIds = pointIds;
        }

        public DrawableTriangle(Vector2 p1, Vector2 p2, Vector2 p3, int p1id, int p2id, int p3id)
        {
            points = new List<Vector2>();

            points.Add(p1);
            points.Add(p2);
            points.Add(p3);

            pointIds = new List<int>();

            pointIds.Add(p1id);
            pointIds.Add(p2id);
            pointIds.Add(p3id);
        }

        public bool ContainsPoint(float x, float y)
        {
            return DrawUtils.PointInTriangle(new Vector2(x, y), points);
        }
    }
}
