using System.IO;
using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using WADinator.Controllers;
using System.Collections.Generic;
using WADinator.Structures.Textmap;

namespace WADinator.Util
{
    [CustomEditor(typeof(ThingController))]
    [CanEditMultipleObjects]
    public class ThingControllerEditor : Editor
    {
        AnimBool thingHider;

        public void OnEnable()
        {
            thingHider = new AnimBool(true);
            thingHider.valueChanged.AddListener(Repaint);
        }

        public override void OnInspectorGUI()
        {
            var controller = (ThingController)target;

            DrawThing(controller);
        }

        private void DrawThing(ThingController controller)
        {
            if(GUILayout.Button("Show Thing Data"))
            {
                thingHider.target = !thingHider.target;
            }

            if (EditorGUILayout.BeginFadeGroup(thingHider.faded))
            {
                EditorGUILayout.LabelField("{");
                EditorGUI.indentLevel++;

                foreach(var prop in typeof(Thing).GetProperties())
                {
                    GUILayout.Label(prop.Name + ":");
                    GUILayout.Label(prop.GetValue(controller.thing, null).ToString());
                    GUILayout.Label("");
                }

                EditorGUI.indentLevel--;
                EditorGUILayout.LabelField("}");
            }

            EditorGUILayout.EndFadeGroup();
        }
    }
}
