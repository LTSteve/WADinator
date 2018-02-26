using System.IO;
using System;
using UnityEngine;
using System.Collections.Generic;

namespace WADinator.Structures.Textmap
{
    [Serializable]
    public class Thing : BlockDef
    {
        public int id { get; set; }

        public float x { get; set; }
        public float y { get; set; }

        public float height { get; set; }

        public int angle { get; set; }

        public int type { get; set; }

        public bool skill1 { get; set; }
        public bool skill2 { get; set; }
        public bool skill3 { get; set; }
        public bool skill4 { get; set; }
        public bool skill5 { get; set; }
        public bool ambush { get; set; }
        public bool single { get; set; }
        public bool dm { get; set; }
        public bool coop { get; set; }

        public bool friend { get; set; }

        public bool dormant { get; set; }
        public bool class1 { get; set; }
        public bool class2 { get; set; }
        public bool class3 { get; set; }

        public bool standing { get; set; }
        public bool strifeally { get; set; }
        public bool translucent { get; set; }
        public bool invisible { get; set; }

        public int special { get; set; }
        public int arg0 { get; set; }
        public int arg1 { get; set; }
        public int arg2 { get; set; }
        public int arg3 { get; set; }
        public int arg4 { get; set; }

        public string comment { get; set; }

        public Thing(List<Property> properties) : base(properties)
        {
            //set non-standard defaults
            skill1 = true;
            skill2 = true;
            skill3 = true;
            skill4 = true;
            skill5 = true;
            ambush = true;
            single = true;
            dm = true;
            coop = true;
            class1 = true;
            class2 = true;
            class3 = true;

            //assign values
            SetStuff(this, GetType());
        }
    }
}
