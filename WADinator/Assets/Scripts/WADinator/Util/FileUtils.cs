using System.IO;
using System;
using UnityEngine;
using UnityEditor;
using WADinator.Structures.Textmap;
using WADinator.Controllers;
using System.Collections.Generic;

namespace WADinator.Util
{
    public static class FileUtils
    {
        public static Material GetMaterial(string textureName)
        {
            if (String.IsNullOrEmpty(textureName) || textureName == "-")
            {
                return null;
            }

            textureName += ".png";

            var path = Path.Combine(Application.dataPath, Path.Combine(WADController.instance.TexturesPath, textureName));

            byte[] materialData;
            try
            {
                materialData = File.ReadAllBytes(path);
            }
            catch(Exception e)
            {
                Debug.LogError(e);
                return null;
            }

            var defaultMaterial = WADController.defaultMaterial;

            var mat = new Material(defaultMaterial);

            var tex = new Texture2D(1,1);

            tex.LoadImage(materialData);

            tex.wrapMode = TextureWrapMode.Repeat;

            mat.mainTexture = tex;

            return mat;
        }
    }
}
