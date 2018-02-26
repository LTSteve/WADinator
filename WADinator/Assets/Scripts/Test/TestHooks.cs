using System.IO;
using System;
using UnityEngine;
using UnityEditor;
using WADinator.Structures;
using WADinator.Structures.Textmap;
using System.Collections.Generic;

public class TestHooks : WADHooks
{
    public override void CreateThing(Thing thing, GameObject creation)
    {
        //remove placeholder sprite
        //GameObject.Destroy(creation.GetComponent<SpriteRenderer>());

        creation.AddComponent<SphereCollider>();
        var body = creation.AddComponent<Rigidbody>();

        if(thing.type == 1) //it's the player
        {
            creation.AddComponent<TestPlayerController>();

            body.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

            if(Camera.main == null)
            {
                var go = new GameObject("Player Camera");
                var cam = go.AddComponent<Camera>();
                go.AddComponent<AudioListener>();
                cam.tag = "MainCamera";
            }

            Camera.main.transform.SetParent(creation.transform);
            Camera.main.transform.localPosition = Vector3.zero;
        }
    }
}
