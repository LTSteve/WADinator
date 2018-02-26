using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace WADinator.Structures
{
    public class Header
    {
        public WADType wadType;

        public int entriesCount;

        public int lumpDefsLocation;

        public new string ToString()
        {
            var output = string.Empty;

            output += wadType + "!\n";
            output += "lumpCount: " + entriesCount + "\n";
            output += "lumpDefsLocation: " + lumpDefsLocation;

            return output;
        }
    }
}
