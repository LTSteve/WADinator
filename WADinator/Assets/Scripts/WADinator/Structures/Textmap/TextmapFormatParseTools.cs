using System.IO;
using System;
using UnityEngine;
using System.Collections.Generic;

namespace WADinator.Structures.Textmap
{
    internal static class TextMapFormatParseTools
    {
        internal static string StripComments(string input)
        {
            var output = input;

            while (output.Contains("/*"))
            {
                var index = output.IndexOf("/*");
                var index2 = output.IndexOf("*/",index);

                output = output.Remove(index, index2 - index + 2);
            }

            while (output.Contains("//"))
            {
                var index = output.IndexOf("//");
                var index2 = output.IndexOf("\n", index);

                output = output.Remove(index, index2 - index);
            }

            return output;
        }

        internal static List<Block> GetBlocks(string input)
        {
            var output = new List<Block>();

            var chunks = input.Split('}');

            for(var i = 0; i< chunks.Length; i++)
            {
                if (!chunks[i].Contains("{"))
                {
                    continue;
                }

                var blockStr = chunks[i];

                var block = new Block();

                var blockSplit = blockStr.Split('{');

                block.type = Trim(blockSplit[0]);

                var propArr = blockSplit[1].Split(';');

                for(var j = 0; j < propArr.Length; j++)
                {
                    if (!propArr[j].Contains("="))
                    {
                        continue;
                    }

                    var propSplit = propArr[j].Split('=');
                    
                    var prop = Trim(propSplit[0]);
                    var value = Trim(propSplit[1]);

                    block.properties.Add(new Property
                    {
                        property = prop,
                        value = value
                    });
                }

                output.Add(block);
            }

            return output;
        }

        private static string Trim(string s)
        {
            if (s.Contains(";"))
            {
                var split = s.Split(';');
                s = split[split.Length - 1];
            }
            return s.Trim(new[] { ' ', '\n', '\t', '"' }).Trim();
        }
    }

    public class Block
    {
        internal string type;
        internal List<Property> properties = new List<Property>();
    }

    public class Property
    {
        internal string property;
        internal string value;
    }
}
