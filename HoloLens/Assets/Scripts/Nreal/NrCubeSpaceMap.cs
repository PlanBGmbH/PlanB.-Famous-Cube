using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents the space map which flyes out of the cube
/// </summary>
public class NrCubeSpaceMap : MonoBehaviour
{
    /// <summary>
    /// Right flylng cube site
    /// </summary>
    public GameObject RightSide;

    /// <summary>
    /// Left flylng cube site
    /// </summary>
    public GameObject LeftSide;

    /// <summary>
    /// Front flylng cube site
    /// </summary>
    public GameObject FrontSide;

    /// <summary>
    /// Back flylng cube site
    /// </summary>
    public GameObject BackSide;

    /// <summary>
    /// Up flylng cube site
    /// </summary>
    public GameObject UpSide;

    /// <summary>
    /// Down flylng cube site
    /// </summary>
    public GameObject DownSide;
    
    /// <summary>
    /// Right target postion of flying
    /// </summary>
    private Vector3 rightTarget;

    /// <summary>
    /// Left target postion of flying
    /// </summary>
    private Vector3 leftTarget;

    /// <summary>
    /// Front target postion of flying
    /// </summary>
    private Vector3 frontTarget;

    /// <summary>
    /// Back target postion of flying
    /// </summary>
    private Vector3 backTarget;

    /// <summary>
    /// Up target postion of flying
    /// </summary>
    private Vector3 upTarget;

    /// <summary>
    /// Down target postion of flying
    /// </summary>
    private Vector3 downTarget;
    
    /// <summary>
    /// Indicates if flying is in progress
    /// </summary>
    private bool flying = false;

    /// <summary>
    /// Speed of flying
    /// </summary>
    public float Speed = 5;

    /// <summary>
    /// Distance of flying in realtion to cube
    /// </summary>
    public float Distance = 10;

    /// <summary>
    /// Spacemap ist visible
    /// </summary>
    public bool visible = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        if (flying)
        {
            FlyInOut();
        }
    }

    /// <summary>
    /// Implements the flying process of the space map
    /// </summary>
    private void FlyInOut()
    {
        float step = Speed * Time.deltaTime;

        RightSide.transform.localPosition = Vector3.MoveTowards(RightSide.transform.localPosition, rightTarget, step);
        LeftSide.transform.localPosition = Vector3.MoveTowards(LeftSide.transform.localPosition, leftTarget, step);
        FrontSide.transform.localPosition = Vector3.MoveTowards(FrontSide.transform.localPosition, frontTarget, step);
        BackSide.transform.localPosition = Vector3.MoveTowards(BackSide.transform.localPosition, backTarget, step);
        UpSide.transform.localPosition = Vector3.MoveTowards(UpSide.transform.localPosition, upTarget, step);
        DownSide.transform.localPosition = Vector3.MoveTowards(DownSide.transform.localPosition, downTarget, step);

        if (RightSide.transform.localPosition == rightTarget)
        {
            flying = false;

            if (!visible)
            {
                visible = true;
            }
            else
            {
                RightSide.SetActive(false);
                LeftSide.SetActive(false);
                FrontSide.SetActive(false);
                BackSide.SetActive(false);
                UpSide.SetActive(false);
                DownSide.SetActive(false);
                visible = false;
            }
        }
    }

    /// <summary>
    /// Toggle view of active/ deactive
    /// </summary>
    public void OnToggleView()
    {
        float directionBasedDistance;
        if (!visible)
        {
            directionBasedDistance = Distance;
            RightSide.SetActive(true);
            LeftSide.SetActive(true);
            FrontSide.SetActive(true);
            BackSide.SetActive(true);
            UpSide.SetActive(true);
            DownSide.SetActive(true);
        }
        else
        {
            directionBasedDistance = Distance*-1;
        }

        rightTarget = new Vector3(RightSide.transform.localPosition.x, RightSide.transform.localPosition.y, RightSide.transform.localPosition.z + directionBasedDistance * -1);
        leftTarget = new Vector3(LeftSide.transform.localPosition.x, LeftSide.transform.localPosition.y, LeftSide.transform.localPosition.z + directionBasedDistance);
        frontTarget = new Vector3(FrontSide.transform.localPosition.x + directionBasedDistance * -1, FrontSide.transform.localPosition.y, FrontSide.transform.localPosition.z);
        backTarget = new Vector3(BackSide.transform.localPosition.x + directionBasedDistance, BackSide.transform.localPosition.y, BackSide.transform.localPosition.z);
        upTarget = new Vector3(UpSide.transform.localPosition.x, UpSide.transform.localPosition.y + directionBasedDistance, UpSide.transform.localPosition.z);
        downTarget = new Vector3(DownSide.transform.localPosition.x, DownSide.transform.localPosition.y + directionBasedDistance * -1, DownSide.transform.localPosition.z);

        flying = true;
    }

}
