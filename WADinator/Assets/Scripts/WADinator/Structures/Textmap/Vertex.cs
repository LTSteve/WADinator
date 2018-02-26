using System.IO;
using System;
using UnityEngine;
using System.Collections.Generic;

namespace WADinator.Structures.Textmap
{
    public class Vertex : BlockDef
    {
        public float x { get; set; }
        public float y { get; set; }

        public Vertex(List<Property> properties) : base(properties)
        {
            SetStuff(this, GetType());
        }
    }
}
