using NRKernal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectFace : MonoBehaviour
{
    /// <summary>
    /// Cube state object of the program
    /// </summary>
    CubeState cubeState;

    /// <summary>
    /// Cube read object of the program
    /// </summary>
    ReadCube readCube;

    /// <summary>
    /// Layer of mini cube colider
    /// </summary>
    int layerMask = 1 << 8;

    // Start is called before the first frame update
    void Start()
    {
        readCube = FindObjectOfType<ReadCube>();
        cubeState = FindObjectOfType<CubeState>();
    }

    // Update is called once per frame
    void Update()
    {
        // If the player doesn't click the trigger button, we are done with this update.
        if (!NRInput.GetButtonDown(ControllerButton.TRIGGER) || CubeState.autoRotating)
        {
            return;
        }

        if (NRInput.GetButtonDown(ControllerButton.TRIGGER))
        {
            readCube.ReadState();

            RaycastHit hit;
            Transform laserAnchor = NRInput.AnchorsHelper.GetAnchor(NRInput.RaycastMode == RaycastModeEnum.Gaze ? ControllerAnchorEnum.GazePoseTrackerAnchor : ControllerAnchorEnum.RightLaserAnchor);
            if (Physics.Raycast(new Ray(laserAnchor.transform.position, laserAnchor.transform.forward), out hit, 100.0f, layerMask))
            {
                GameObject face = hit.collider.gameObject;

                List<List<GameObject>> cubeSides = new List<List<GameObject>>()
                {
                    cubeState.up,
                    cubeState.down,
                    cubeState.left,
                    cubeState.right,
                    cubeState.front,
                    cubeState.back
                };

                foreach(List<GameObject> cubeSide in cubeSides)
                {
                    if (cubeSide.Contains(face))
                    {
                        cubeState.PickUp(cubeSide);
                        cubeSide[4].transform.parent.GetComponent<PivotRotation>().Rotate(cubeSide);
                    }
                }
            }
        }
    }
}
