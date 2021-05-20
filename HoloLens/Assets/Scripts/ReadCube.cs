using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implements the reading of a cube side
/// </summary>
public class ReadCube : MonoBehaviour
{
    /// <summary>
    /// Transform data of the middle cube of up side
    /// </summary>
    public Transform tUp;

    /// <summary>
    /// Transform data of the middle cube of down side
    /// </summary>
    public Transform tDown;

    /// <summary>
    /// Transform data of the middle cube of left side
    /// </summary>
    public Transform tLeft;

    /// <summary>
    /// Transform data of the middle cube of right side
    /// </summary>
    public Transform tRight;

    /// <summary>
    /// Transform data of the middle cube of front side
    /// </summary>
    public Transform tFront;

    /// <summary>
    /// Transform data of the middle cube of back side
    /// </summary>
    public Transform tBack;

    /// <summary>
    /// Empty gameobject for create new one on specfied transform data
    /// </summary>
    public GameObject emptyGo;

    /// <summary>
    /// Layer of the mini cube collider
    /// </summary>
    private int layerMask = 1 << 8;

    /// <summary>
    /// Array of rays for identifing the the side of a mini cube
    /// </summary>
    private List<GameObject> frontRays = new List<GameObject>();

    /// <summary>
    /// Array of rays for identifing the the side of a mini cube
    /// </summary>
    private List<GameObject> backRays = new List<GameObject>();

    /// <summary>
    /// Array of rays for identifing the the side of a mini cube
    /// </summary>
    private List<GameObject> leftRays = new List<GameObject>();

    /// <summary>
    /// Array of rays for identifing the the side of a mini cube
    /// </summary>
    private List<GameObject> rightRays = new List<GameObject>();

    /// <summary>
    /// Array of rays for identifing the the side of a mini cube
    /// </summary>
    private List<GameObject> upRays = new List<GameObject>();

    /// <summary>
    /// Array of rays for identifing the the side of a mini cube
    /// </summary>
    private List<GameObject> downRays = new List<GameObject>();

    /// <summary>
    /// Cube state object of the program
    /// </summary>
    CubeState cubeState;

    /// <summary>
    /// Cube map object of the program
    /// </summary>
    CubeMap cubeMap;
    
    // Start is called before the first frame update
    void Start()
    {
        SetRaysTransForms();
        cubeState = FindObjectOfType<CubeState>();
        cubeMap = FindObjectOfType<CubeMap>();
        ReadState();
        CubeState.started = true;

        DisableEmissions(cubeState.front);
        DisableEmissions(cubeState.back);
        DisableEmissions(cubeState.left);
        DisableEmissions(cubeState.right);
        DisableEmissions(cubeState.up);
        DisableEmissions(cubeState.down);
    }

    /// <summary>
    /// Disable init glow/ emissions -> we need it active because on a deactivated state it will not complied
    /// </summary>
    /// <param name="sides"></param>
    public void DisableEmissions(List<GameObject> sides)
    {
        foreach (var oneSide in sides)
        {
            foreach(var oneMesh in  oneSide.GetComponentsInChildren<MeshRenderer>())
            {
                oneMesh.material.DisableKeyword("_EMISSION");
            }
        }
    }

 

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Read the state of a side in a ways that we are triggering ryas for identifing the colider of the side of a mini cube
    /// </summary>
    public void ReadState()
    {
        cubeState = FindObjectOfType<CubeState>();
        cubeMap = FindObjectOfType<CubeMap>();

        cubeState.up = ReadFace(upRays, tUp);
        cubeState.down = ReadFace(downRays, tDown);
        cubeState.left = ReadFace(leftRays, tLeft);
        cubeState.right = ReadFace(rightRays, tRight);
        cubeState.front = ReadFace(frontRays, tFront);
        cubeState.back = ReadFace(backRays, tBack);

        if (cubeMap != null)
        {
            cubeMap.Set();
        }
    }

    /// <summary>
    /// Create rays around the cube to trigger ryas to identify the side/ face of a mini cube via colider
    /// </summary>
    void SetRaysTransForms()
    {
        upRays = BuildRays(tUp, new Vector3(90, 90, 0));
        downRays = BuildRays(tDown, new Vector3(270, 90, 0));
        leftRays = BuildRays(tLeft, new Vector3(0, 180, 0));
        rightRays = BuildRays(tRight, new Vector3(0, 0, 0));
        frontRays = BuildRays(tFront, new Vector3(0, 90, 0));
        backRays = BuildRays(tBack, new Vector3(0, 270, 0));
    }

    /// <summary>
    /// Create the rays
    /// </summary>
    /// <param name="rayTransform">Center transform object of a side</param>
    /// <param name="direction">View direction of a side</param>
    /// <returns></returns>
    List<GameObject> BuildRays(Transform rayTransform, Vector3 direction)
    {
        int rayCount = 0;
        List<GameObject> rays = new List<GameObject>();

        for (int y = 1; y > -2; y--)
        {
            for (int x = -1; x < 2; x++)
            {
                Vector3 stratPos = new Vector3(rayTransform.position.x + (x * 0.075f),
                                               rayTransform.position.y - (y * 0.075f),
                                               rayTransform.position.z);
                GameObject rayStart = Instantiate(emptyGo, stratPos, Quaternion.identity, rayTransform);
                rayStart.name = rayCount.ToString();
                rays.Add(rayStart);
                rayCount++;
            }
        }

        rayTransform.localRotation = Quaternion.Euler(direction);
        return rays;
    }

    /// <summary>
    /// "Triggers" rays and identitfy the gameboject based on the hited collider
    /// </summary>
    /// <param name="rayStarts"></param>
    /// <param name="rayTransform"></param>
    /// <returns></returns>
    public List<GameObject> ReadFace(List<GameObject> rayStarts, Transform rayTransform)
    {
        List<GameObject> faceHit = new List<GameObject>();

        foreach (GameObject rayStart in rayStarts)
        {
            Vector3 ray = rayStart.transform.position;
            RaycastHit hit;

            if (Physics.Raycast(ray, rayTransform.forward, out hit, Mathf.Infinity, layerMask))
            {
                Debug.DrawRay(ray, rayTransform.forward * hit.distance, Color.yellow);
                faceHit.Add(hit.collider.gameObject);
                //print(hit.collider.gameObject.name);
            }
            else
            {
                Debug.DrawRay(ray, rayTransform.forward * 1000, Color.green);
            }
        }

        return faceHit;
    }
}
