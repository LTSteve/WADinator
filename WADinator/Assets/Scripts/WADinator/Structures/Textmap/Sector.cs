using System.IO;
using System;
using UnityEngine;
using System.Collections.Generic;
using WADinator.Util;

namespace WADinator.Structures.Textmap
{
    public class Sector : BlockDef
    {
        public int heightfloor { get; set; }
        public int heightceiling { get; set; }

        public string texturefloor { get; set; }
        public string textureceiling { get; set; }

        public int lightlevel { get; set; }

        public int special { get; set; }
        public int id { get; set; }

        public string comment { get; set; }

        public DrawablePolygon drawablePolygon;

        public Sector(List<Property> properties) : base(properties)
        {
            //set non-standard defaults
            lightlevel = 160;

            //assign values
            SetStuff(this, GetType());
        }
    }
}
