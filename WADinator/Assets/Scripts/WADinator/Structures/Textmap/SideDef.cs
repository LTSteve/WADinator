using System.IO;
using System;
using UnityEngine;
using System.Collections.Generic;

namespace WADinator.Structures.Textmap
{
    public class SideDef : BlockDef
    {
        public int offsetx { get; set; }
        public int offsety { get; set; }

        public string texturetop { get; set; }
        public string texturebottom { get; set; }
        public string texturemiddle { get; set; }

        public int sector { get; set; }
        public Sector sectorRef;

        public string comment { get; set; }

        public SideDef(List<Property> properties) : base(properties)
        {
            //set non-standard defaults
            texturetop = "-";
            texturebottom = "-";
            texturemiddle = "-";

            //assign values
            SetStuff(this, GetType());
        }
    }
}
