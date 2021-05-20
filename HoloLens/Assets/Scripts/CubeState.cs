using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implements the cube state/ objects
/// </summary>
public class CubeState : MonoBehaviour
{
    /// <summary>
    /// Flag of auto rotatio is active
    /// </summary>
    public static bool autoRotating = false;

    /// <summary>
    /// Flag of start programm
    /// </summary>
    public static bool started = false;

    /// <summary>
    /// List of front mini cube game objects
    /// </summary>
    public List<GameObject> front = new List<GameObject>();

    /// <summary>
    /// List of front mini cube game objects
    /// </summary>
    public List<GameObject> back = new List<GameObject>();

    /// <summary>
    /// List of front mini cube game objects
    /// </summary>
    public List<GameObject> up = new List<GameObject>();

    /// <summary>
    /// List of front mini cube game objects
    /// </summary>
    public List<GameObject> down = new List<GameObject>();

    /// <summary>
    /// List of front mini cube game objects
    /// </summary>
    public List<GameObject> left = new List<GameObject>();

    /// <summary>
    /// List of front mini cube game objects
    /// </summary>
    public List<GameObject> right = new List<GameObject>();
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Putting game objects together/ inside the middle object
    /// </summary>
    /// <param name="cubeSide"></param>
    public void PickUp(List<GameObject> cubeSide)
    {
        foreach (GameObject face in cubeSide)
        {
            if (face != cubeSide[4])
            {
                face.transform.parent.transform.parent = cubeSide[4].transform.parent;
            }
        }        
    }

    /// <summary>
    /// Putting game objects together/ inside the middle object (auto rotation)
    /// </summary>
    /// <param name="littleCubes"></param>
    /// <param name="pivot"></param>
    public void PutDown(List<GameObject> littleCubes, Transform pivot)
    {
        foreach (GameObject littleCube in littleCubes)
        {
            if (littleCube != littleCubes[0])
            {
                littleCube.transform.parent.transform.parent = pivot;
            }
        }
    }

}
