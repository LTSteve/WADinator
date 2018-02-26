using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using WADinator.Controllers;

namespace WADinator.Emulation
{
    public class Billboard : MonoBehaviour
    {
        void Update()
        {
            transform.LookAt(Camera.main.transform.position, -Vector3.up);
        }
    }
}
