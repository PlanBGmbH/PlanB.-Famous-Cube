using NRKernal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implements the rotation of a cube side
/// </summary>
public class PivotRotation : MonoBehaviour
{
    /// <summary>
    /// Indicates globaly whether a drag of a side is in progress
    /// </summary>
    public static bool DraggingInProgress = false;

    /// <summary>
    /// Represents the active side for this instance
    /// </summary>
    private List<GameObject> activeSide;

    /// <summary>
    /// Represents the mouse postion from the last frame
    /// </summary>
    private Vector3 mouseRef;

    /// <summary>
    /// Represents the angle of dragging from the last frame
    /// </summary>
    private float angelPrev;

    /// <summary>
    /// Indicates whether a drag of a side is in progress
    /// </summary>
    private bool dragging = false;
    
    /// <summary>
    /// Represents the speed of the auto rotation
    /// </summary>
    private float speed = 300f;

    /// <summary>
    /// Represents the actuel rotation
    /// </summary>
    private Vector3 rotation;

    /// <summary>
    /// Indicates whether the auto rotation is in progress
    /// </summary>
    private bool autoRotating = false;

    /// <summary>
    /// Target rotation of a side
    /// </summary>
    private Quaternion targetQuaternion;

    /// <summary>
    /// Cube read object of the program
    /// </summary>
    private ReadCube readCube;

    /// <summary>
    /// Cube state object of the program
    /// </summary>
    private CubeState cubeState;

    // Start is called before the first frame update
    void Start()
    {
        readCube = FindObjectOfType<ReadCube>();
        cubeState = FindObjectOfType<CubeState>();
    }

    // Update is called once per frame
    void Update()
    {
        if (dragging)
        {
            SpinSide(activeSide);
            if (NRInput.GetButtonUp(ControllerButton.TRIGGER))
            {
                dragging = false;
                DraggingInProgress = false;
                RotatToRightAngle();
            }
        }

        if (autoRotating)
        {
            AutoRotate();
        }
    }

    private void SpinSide(List <GameObject> side)
    {
        rotation = Vector3.zero;
        Vector3 laserEndPoint;
        if (NrealInputExt.GetLaserEndWorldPosition(out laserEndPoint))
        {
            Vector3 mouseOffset = laserEndPoint - mouseRef;


            Vector2 positionOnScreen;

            Vector2 mouseRefOnScreen;
            float angleRef = 0f;
            Vector2 mouseOnScreen;
            float angle = 0f;

            var mouse_pos = laserEndPoint;
            var object_pos = Camera.main.WorldToScreenPoint(transform.position);
            mouse_pos.x = mouse_pos.x - object_pos.x;
            mouse_pos.y = mouse_pos.y - object_pos.y;
            angle = (Mathf.Atan2(mouse_pos.y, mouse_pos.x) * Mathf.Rad2Deg);

            mouse_pos = mouseRef;
            mouse_pos.x = mouse_pos.x - object_pos.x;
            mouse_pos.y = mouse_pos.y - object_pos.y;
            angleRef = (Mathf.Atan2(mouse_pos.y, mouse_pos.x) * Mathf.Rad2Deg);


            if (side == cubeState.up)
            {
                angle = angle * -1;
                angleRef = angleRef * -1;
                transform.Rotate(new Vector3(0f, (angle - angleRef - angelPrev), 0f), Space.Self);             
            }

            if (side == cubeState.down)
            {
                angle = angle;
                angleRef = angleRef;
                transform.Rotate(new Vector3(0f, (angle - angleRef - angelPrev), 0f), Space.Self);
            }

            if (side == cubeState.left)
            {
                angle = angle*-1;
                angleRef = angleRef*-1;
                transform.Rotate(new Vector3(0f, 0f, (angle - angleRef - angelPrev)), Space.Self);
            }

            if (side == cubeState.right)
            {
                angle = angle;
                angleRef = angleRef;
                transform.Rotate(new Vector3(0f, 0f, (angle - angleRef - angelPrev)), Space.Self);
            }

            if (side == cubeState.front)
            {
                angle = angle;
                angleRef = angleRef;
                transform.Rotate(new Vector3((angle - angleRef - angelPrev), 0f, 0f), Space.Self);
            }

            if (side == cubeState.back)
            {
                angle = angle * -1;
                angleRef = angleRef * -1;
                transform.Rotate(new Vector3((angle - angleRef - angelPrev), 0f, 0f), Space.Self);
            }

            angelPrev = angle - angleRef;
        }
    }

    float AngleBetweenTwoPoints(Vector3 a, Vector3 b)
    {
        return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
    }

    public void StartAutoRotate(List<GameObject> side, float angle)
    {
        cubeState.PickUp(side);
        Vector3 localForward = Vector3.zero - side[4].transform.parent.transform.localPosition;
        targetQuaternion = Quaternion.AngleAxis(angle, localForward) * transform.localRotation;
        activeSide = side;
        autoRotating = true;
    }

    public void Rotate(List<GameObject> side)
    {
        NRInput.TriggerHapticVibration();
        activeSide = side;
        ActivateGlowing();
        Vector3 laserEndPoint;
        NrealInputExt.GetLaserEndWorldPosition(out laserEndPoint);
        mouseRef = laserEndPoint;
        dragging = true;
        DraggingInProgress = true;
        angelPrev = 0f;
    }

    /// <summary>
    /// Rotates a side to the near side so that the cube snaps to a full cube and we have no side which are in the middle
    /// </summary>
    public void RotatToRightAngle()
    {
        Vector3 vec = transform.localEulerAngles;
        vec.x = Mathf.Round(vec.x / 90) * 90;
        vec.y = Mathf.Round(vec.y / 90) * 90;
        vec.z = Mathf.Round(vec.z / 90) * 90;

        targetQuaternion.eulerAngles = vec;
        autoRotating = true;
    }

    /// <summary>
    /// Activates the glowing/ emissions of a side to show that a side is selected
    /// </summary>
    private void ActivateGlowing()
    {
        foreach (var oneObject in activeSide)
        {
            var meshs = oneObject.GetComponentsInChildren<MeshRenderer>();
            foreach (var oneMesh in meshs)
            {
                oneMesh.material.EnableKeyword("_EMISSION");
            }
        }
    }

    /// <summary>
    /// Deactivates the glowing/ emissions of a side to show that a side is deselected
    /// </summary>
    private void DeactivateGlowing()
    {
        RaycastHit hitResult;
        Vector3 worldPos;
        var success = NrealInputExt.GetLaserEndWorldPosition(out worldPos, out hitResult);

        foreach (var oneObject in activeSide)
        {
            if (hitResult.collider == null || hitResult.collider.gameObject == null || hitResult.collider.gameObject != oneObject)
            {
                var meshs = oneObject.GetComponentsInChildren<MeshRenderer>();
                foreach (var oneMesh in meshs)
                {
                    oneMesh.material.DisableKeyword("_EMISSION");
                }
            }
        }
    }

    /// <summary>
    /// Rotates a side of a cube by programm
    /// </summary>
    private void AutoRotate()
    {
        dragging = false;
        DraggingInProgress = false;
        var step = speed * Time.deltaTime;
        transform.localRotation = Quaternion.RotateTowards(transform.localRotation, targetQuaternion, step);

        if (Quaternion.Angle(transform.localRotation, targetQuaternion) <= 1)
        {
            DeactivateGlowing();
            transform.localRotation = targetQuaternion;
            cubeState.PutDown(activeSide, transform.parent);
            readCube.ReadState();
            CubeState.autoRotating = false;
            autoRotating = false;
            dragging = false;
            DraggingInProgress = false;
        }
    }

}
