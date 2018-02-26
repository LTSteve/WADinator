using System.IO;
using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using WADinator.Controllers;
using System.Collections.Generic;

namespace WADinator.Util
{
    [CustomEditor(typeof(WADController))]
    [CanEditMultipleObjects]
    public class WADControllerEditor : Editor
    {

        private int selectedTextmap = -1;

        private void OnEnable()
        {
            selectedTextmap = -1;
        }

        public override void OnInspectorGUI()
        {
            var controller = (WADController)target;

            DrawSetupPaths(controller);
        }

        private void DrawSetupPaths(WADController controller)
        {
            EditorGUILayout.LabelField("Map Path");
            EditorGUILayout.LabelField(controller.MapPath);
            EditorGUILayout.LabelField("");

            if(GUILayout.Button("Generate Map"))
            {
                if (controller.Wad.textmapNames.Count > 1)
                {
                    var options = new GUIContent[controller.Wad.textmapNames.Count];
                    for (var i = 0; i < controller.Wad.textmapNames.Count; i++)
                    {
                        var name = controller.Wad.textmapNames[i];
                        options[i] = new GUIContent(name);
                    }

                    EditorUtility.DisplayCustomMenu(new Rect(0,0,100,100), options, -1, MapSelectionCallback, controller);
                }
                else
                {
                    FinishCreation(0);
                }
            }

            if(selectedTextmap != -1)
            {
                var textmapId = selectedTextmap;
                selectedTextmap = -1;

                if (controller.transform.childCount == 0 ||
                    EditorUtility.DisplayDialog("This will clear everything under " + controller.name,
                        "Is that OK?", "Clear old map"))
                {
                    var children = new List<GameObject>();
                    foreach (Transform child in controller.transform) children.Add(child.gameObject);
                    children.ForEach(child => DestroyImmediate(child));

                    controller.Create(textmapId);
                }
            }
        }

        private void MapSelectionCallback(object controller, string[] options, int selection)
        {
            FinishCreation(selection);
        }

        private void FinishCreation(int textmapId)
        {
            selectedTextmap = textmapId;
        }
    }
}
