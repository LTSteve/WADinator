using System.IO;
using System;
using UnityEngine;
using UnityEditor;
using WADinator.Structures;
using WADinator.Structures.Textmap;
using System.Collections.Generic;

namespace WADinator.Controllers
{
    [ExecuteInEditMode]
    public class WADController : MonoBehaviour
    {
        public string TexturesPath = "WADTextures";

        public string MapPath;

        public WAD Wad;
        public TextMap Textmap;

        public static WADController instance = null;

        public static Material defaultMaterial = null;

        public static Transform objectPrefab = null;

        public void OnEnable()
        {
            Awake();
        }

        public void Awake()
        {
            if(String.IsNullOrEmpty(MapPath) && Wad != null)
            {
                MapPath = Wad.filePath;
            }

            if(defaultMaterial == null)
            {
                var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
                defaultMaterial = quad.GetComponent<MeshRenderer>().sharedMaterial;
                DestroyImmediate(quad);
            }

            if(objectPrefab == null)
            {
                objectPrefab = AssetDatabase.LoadAssetAtPath<Transform>("Assets/Scripts/WADinator/object.prefab");
            }

            instance = this;
        }

        public void Start()
        {
            if(transform.childCount == 0 && !string.IsNullOrEmpty(MapPath))
            {
                try
                {
                    Wad = WADLoad.LoadWad(MapPath, defaultMaterial);
                }
                catch(Exception e)
                {
                    Debug.LogError(e);
                    Debug.LogError("Incorrect format, please use UDMF format WADs with at least one map.");
                    return;
                }

                if(Wad.textmapNames.Count == 1)
                {
                    Create(0);
                }
            }
        }

        public void Create(int selectedTextMap)
        {
            if (string.IsNullOrEmpty(MapPath))
            {
                return;
            }

            EditorUtility.DisplayProgressBar("Loading Map...", "Reading textmap", 0);

            try
            {
                Wad.Create(selectedTextMap, this);
                Textmap = Wad.textmaps[selectedTextMap];
            }
            catch(Exception e)
            {
                Debug.LogError(e);
            }

            EditorUtility.ClearProgressBar();
        }

    }
}
