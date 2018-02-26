using System.IO;
using System;
using UnityEngine;
using WADinator.Structures.Textmap;
using System.Collections.Generic;

namespace WADinator.Structures
{
    public static class WADLoad
    {
        public static readonly int HEADER_SIZE = 12;
        public static readonly int LUMP_HEADER_SIZE = 16;

        /* Grabs the header and creates a WAD using the data */
        public static WAD LoadWad(string filepath, Material def)
        {
            var stream = File.OpenRead(filepath);

            if(stream.Length < 12)
            {
                stream.Close();
                throw new Exception("File too short! This can't be a WAD");
            }
            
            stream.Seek(0, SeekOrigin.Begin);

            var headerData = new byte[HEADER_SIZE];

            stream.Read(headerData, 0, headerData.Length);

            var header = new Header {
                wadType = StringFromBytes(headerData, 0, 4) == "IWAD" ? WADType.IWAD : WADType.PWAD,
                entriesCount = IntFromBytes(headerData, 4),
                lumpDefsLocation = IntFromBytes(headerData, 8)
            };
            
            var textMaps = new List<string>();

            var workingTitle = string.Empty;

            for (var i = 0; i < header.entriesCount; i++)
            {
                stream.Seek(header.lumpDefsLocation + i * LUMP_HEADER_SIZE, SeekOrigin.Begin);

                var lumpHeaderData = new byte[LUMP_HEADER_SIZE];

                stream.Read(lumpHeaderData, 0, lumpHeaderData.Length);

                var lump = new Lump
                {
                    location = IntFromBytes(lumpHeaderData, 0),
                    size = IntFromBytes(lumpHeaderData, 4),
                    name = StringFromBytes(lumpHeaderData, 8, 8)
                };

                //TODO: figure out what to do with the rest of the lumps
                if (lump.name == "TEXTMAP")
                {
                    textMaps.Add(workingTitle);
                }
                else
                {
                    workingTitle = lump.name;
                }
            }

            stream.Close();

            if(textMaps.Count == 0)
            {
                throw new Exception("Couldn't find any textmaps");
            }

            return new WAD(header, textMaps, filepath, def);
        }

        public static string StringFromBytes(byte[] bytes, int start = 0, int length = 0)
        {
            if(length == 0)
            {
                length = bytes.Length;
            }

            var toReturn = string.Empty;

            for(var i = start; i < (start + length);  i++)
            {
                if(bytes[i] == 0)
                {
                    break;
                }
                toReturn += (char)bytes[i];
            }

            return toReturn;
        }

        public static int IntFromBytes(byte[] bytes, int start = 0)
        {
            var subArray = SubArray(bytes, start, 4);

            int toReturn = 0;

            for(var i = 0; i < subArray.Length; i++)
            {
                toReturn += subArray[i] * IPow(256, (int)i);
            }

            return toReturn;
        }

        private static int IPow(int i1, int i2)
        {
            var toRet = (int)1;

            for(var i = 0; i < i2; i++)
            {
                toRet = toRet * i1;
            }

            return toRet;
        }

        private static byte[] SubArray(byte[] bytes, int start = 0 , int length = 0)
        {
            if(length <= 0)
            {
                length = bytes.Length;
            }

            var toReturn = new byte[length];

            for(var i = 0; i < length; i++)
            {
                toReturn[i] = bytes[start + i];
            }

            return toReturn;
        }
    }

    public enum WADType
    {
        IWAD,
        PWAD
    }
}
