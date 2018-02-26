using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace WADinator.Structures
{
    public class Lump
    {
        public int location;

        public int size;

        public string name;//A-Z0-9[]-_
        
        public new string ToString()
        {
            var output = string.Empty;

            output += "[" + location + "]:";
            output += name + " (" + size + ")";

            return output;
        }
    }
}
