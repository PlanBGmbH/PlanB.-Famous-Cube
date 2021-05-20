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

    // Start is called before the first frame update
    void Start()
    {
        readCube = FindObjectOfType<ReadCube>();
        cubeState = FindObjectOfType<CubeState>();
        //m_MeshRender = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Event reciver of start a side grab/rotation interaction
    /// </summary>
    /// <param name="inputFace"></param>
    public void OnGrabStarted(GameObject inputFace)
    {

        readCube.ReadState();

        GameObject face = inputFace;

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
            foreach (var partSide in cubeSide)
            {
                if (partSide.transform.parent.gameObject == face)
                {
                    cubeState.PickUp(cubeSide);
                    cubeSide[4].transform.parent.GetComponent<PivotRotation>().Rotate(cubeSide);
                    break;
                }
            }
        }
    }
}
