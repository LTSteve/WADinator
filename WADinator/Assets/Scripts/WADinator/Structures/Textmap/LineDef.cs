using System.IO;
using System;
using UnityEngine;
using System.Collections.Generic;

namespace WADinator.Structures.Textmap
{
    public class LineDef : BlockDef
    {
        public int id { get; set; }

        public int v1 { get; set; }
        public Vertex v1Ref;
        public int v2 { get; set; }
        public Vertex v2Ref;

        public bool blocking { get; set; }
        public bool blockmonsters { get; set; }
        public bool twosided { get; set; }
        public bool dontpegtop { get; set; }
        public bool dontpegbottom { get; set; }
        public bool secret { get; set; }
        public bool blocksound { get; set; }
        public bool dontdraw { get; set; }
        public bool mapped { get; set; }

        public bool passuse { get; set; }

        public bool translucent { get; set; }
        public bool jumpover { get; set; }
        public bool blockfloaters { get; set; }

        public bool playercross { get; set; }
        public bool playeruse { get; set; }
        public bool monstercross { get; set; }
        public bool monsteruse { get; set; }
        public bool impact { get; set; }
        public bool playerpush { get; set; }
        public bool monsterpush { get; set; }
        public bool missilecross { get; set; }
        public bool repeatspecial { get; set; }

        public int special { get; set; }
        public int arg0 { get; set; }
        public int arg1 { get; set; }
        public int arg2 { get; set; }
        public int arg3 { get; set; }
        public int arg4 { get; set; }

        public int sidefront { get; set; }
        public SideDef sidefrontRef;
        public int sideback { get; set; }
        public SideDef sidebackRef;

        public string comment { get; set; }

        public LineDef(List<Property> properties) : base(properties)
        {
            //set non-standard defaults
            id = -1;
            sideback = -1;

            //assign values
            SetStuff(this, GetType());
        }
    }
}
