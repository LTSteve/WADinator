using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayerController : MonoBehaviour {
    
    void FixedUpdate()
    {
        var x = Input.GetAxisRaw("Horizontal") * Time.deltaTime * 150.0f;
        var z = Input.GetAxisRaw("Vertical") * Time.deltaTime * 3.0f;
        
        transform.Rotate(0, x, 0);
        transform.Translate(0, 0, z);
    }
}
