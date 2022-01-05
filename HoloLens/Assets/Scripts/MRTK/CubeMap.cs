using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Implements the state handling of the cube map
/// </summary>
public class CubeMap : MonoBehaviour
{
    /// <summary>
    /// Cube state object of the program
    /// </summary>
    CubeState cubeState;

    /// <summary>
    /// Center object of cube top site
    /// </summary>
    public Transform up;

    /// <summary>
    /// Center object of cube down site
    /// </summary>
    public Transform down;

    /// <summary>
    /// Center object of cube left site
    /// </summary>
    public Transform left;

    /// <summary>
    /// Center object of cube right site
    /// </summary>
    public Transform right;

    /// <summary>
    /// Center object of cube front site
    /// </summary>
    public Transform front;

    /// <summary>
    /// Center object of cube back site
    /// </summary>
    public Transform back;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Sets/ refreshs the map based on the state of the cube
    /// </summary>
    public void Set()
    {
        cubeState = FindObjectOfType<CubeState>();

        UpdateMap(cubeState.front, front);
        UpdateMap(cubeState.back, back);
        UpdateMap(cubeState.left, left);
        UpdateMap(cubeState.right, right);
        UpdateMap(cubeState.up, up);
        UpdateMap(cubeState.down, down);
    }

    /// <summary>
    /// Update a side of the map based one side of cube
    /// </summary>
    /// <param name="face">Side of the cube state</param>
    /// <param name="side">Part of the map</param>
    void UpdateMap(List<GameObject> face, Transform side)
    {
        int i = 0;
        foreach (Transform map in side)
        {
            if (face[i].name.ToUpper()[0] == 'F')
            {
                map.GetComponent<Image>().color = new Color(1, 0.481443f, 0);
            }

            if (face[i].name.ToUpper()[0] == 'D')
            {
                map.GetComponent<Image>().color = new Color(0, 0.07144785f, 1);
                
            }

            if (face[i].name.ToUpper()[0] == 'L')
            {
                map.GetComponent<Image>().color = new Color(0.9743073f, 1, 0);
            }

            if (face[i].name.ToUpper()[0] == 'R')
            {
                map.GetComponent<Image>().color = new Color(1, 0, 0);
            }

            if (face[i].name.ToUpper()[0] == 'T')
            {
                map.GetComponent<Image>().color = new Color(0, 1, 0.07554293f);
            }

            if (face[i].name.ToUpper()[0] == 'B')
            {
                map.GetComponent<Image>().color = new Color(1, 1, 1);
            }

            i++;
        }
    }
}
