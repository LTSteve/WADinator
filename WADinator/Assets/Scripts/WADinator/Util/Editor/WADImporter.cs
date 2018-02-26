using UnityEditor;
using WADinator.Controllers;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;
using System;

namespace WADinator.Util
{

    [ScriptedImporter(2, "wad")]
    public class WADImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            var mapObject = new GameObject();

            var controller = mapObject.AddComponent<WADController>();
            
            controller.MapPath = ctx.assetPath;

            ctx.AddObjectToAsset("Map Root", mapObject);
            ctx.SetMainObject(mapObject);
        }
    }
}
