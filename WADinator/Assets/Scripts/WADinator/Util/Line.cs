using System.IO;
using System;
using UnityEngine;
using UnityEditor;
using WADinator.Structures.Textmap;
using WADinator.Controllers;
using System.Collections.Generic;

namespace WADinator.Util
{
    public class Line
    {
        private bool vertical = false;

        private float slope = 0f;
        private Vector3 point = Vector3.zero;

        public Line(Vector2 point, Vector2 slope)
        {
            this.point = point;

            if(slope.x == 0)
            {
                vertical = true;
            }
            else
            {
                this.slope = slope.y/slope.x;
            }
        }

        private Line(Vector2 point, float slope)
        {
            this.point = point;
            this.slope = slope;
        }

        private Line(Vector2 point, bool vertical)
        {
            this.vertical = vertical;
            this.point = point;
        }

        public Line Inverse()
        {
            if (vertical)
            {
                return new Line(point, Vector2.zero);
            }
            else if(slope == 0)
            {
                return new Line(point, true);
            }
            else
            {
                return new Line(point, -1f / slope);
            }
        }

        public void SetPoint(Vector2 newPoint)
        {
            point = newPoint;
        }

        //returns true if near and far are on opposite sides of this
        public bool OppositeSides(Vector2 near, Vector2 far)
        {
            if (!vertical)
            {
                var nearSide = YAt(near.x) > near.y;
                var farSide = YAt(far.x) > far.y;

                return nearSide != farSide;
            }
            else
            {
                var nearSide = near.x > point.x;
                var farSide = far.x > point.x;

                return nearSide != farSide;
            }
        }

        public float YAt(float x)
        {
            if (vertical)
            {
                return 0f;
            }

            return slope * (x - point.x) + point.y;
        }

        //returns true if vec is between bound1 and bound2
        public static bool PointIsBetween(Line bound1, Line bound2, Vector2 testVec)
        {
            if(bound1.vertical != bound2.vertical)
            {
                return false;
            }

            if (bound1.vertical)
            {
                return
                    !(bound1.point.x > testVec.x && bound2.point.x > testVec.x) &&
                    !(bound1.point.x < testVec.x && bound2.point.x < testVec.x);
            }

            var bound1Side = bound1.YAt(testVec.x) > testVec.y;
            var bound2Side = bound2.YAt(testVec.x) > testVec.y;

            return bound1Side != bound2Side;
        }
    }
}
