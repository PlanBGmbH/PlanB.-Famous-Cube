using NRKernal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NrSelectFace : MonoBehaviour
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

    public HandEnum handRight; 
    public HandEnum handLeft;
    public static HandEnum activeHandEnum;

    // Start is called before the first frame update
    void Start()
    {
        readCube = FindObjectOfType<ReadCube>();
        cubeState = FindObjectOfType<CubeState>();
    }

    // Update is called once per frame
    void Update()
    {    
        Ray? laserRay = null;
        if (NrChangeInteractionMode.HandInteractionActive)
        {
            var handState = NRInput.Hands.GetHandState(handRight);

            bool isGrabing = false;

            if (handState.isPinching)
            {
                isGrabing = true;
                activeHandEnum = handRight;
            }
            else
            {
                handState = NRInput.Hands.GetHandState(handLeft);
                if (handState.isPinching)
                {
                    isGrabing = true;
                    activeHandEnum = handLeft;
                }
            }

            // If the player doesn't hand not grapping button, we are done with this update.
            if (!isGrabing || CubeState.autoRotating || PivotRotation.DraggingInProgress)
            {
                return;
            }

            if (handState.isPinching)
            {             
                laserRay = new Ray(handState.pointerPose.position, handState.pointerPose.forward);
            }            
        }
        else
        {
            // If the player doesn't click the trigger button, we are done with this update.
            if (!NRInput.GetButtonDown(ControllerButton.TRIGGER) || CubeState.autoRotating)
            {
                return;
            }

            if (NRInput.GetButtonDown(ControllerButton.TRIGGER))
            {
                var laserAnchor = NRInput.AnchorsHelper.GetAnchor(NRInput.RaycastMode == RaycastModeEnum.Gaze ? ControllerAnchorEnum.GazePoseTrackerAnchor : ControllerAnchorEnum.RightLaserAnchor);
                laserRay = new Ray(laserAnchor.transform.position, laserAnchor.transform.forward);
            }
        }

        if (laserRay != null)
        {
            readCube.ReadState();

            RaycastHit hit;
            
            if (Physics.Raycast(laserRay.Value, out hit, 100.0f, layerMask))
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

                foreach (List<GameObject> cubeSide in cubeSides)
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
