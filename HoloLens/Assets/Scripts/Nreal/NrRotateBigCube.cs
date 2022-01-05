using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NRKernal;

public class NrRotateBigCube : MonoBehaviour
{
    /// <summary>
    /// Preverarios rotation state of object
    /// </summary>
    Quaternion prevCubeRot;

    // Start is called before the first frame update
    void Start()
    {
        prevCubeRot = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (NRInput.GetButton(ControllerButton.HOME))
        {
            var relative = NRInput.GetRotation();
            transform.rotation = relative;
        }
    }
}
