/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRKernal
{
    using UnityEngine;

    /// <summary>
    /// The class is to emulate controller with mouse and keyboard input in unity editor mode. </summary>
    public class EditorControllerProvider : ControllerProviderBase
    {
        /// <summary> The mouse delta. </summary>
        private Vector2 mouseDelta = new Vector2();

        /// <summary> The axis horizontal. </summary>
        private const string AXIS_HORIZONTAL = "Horizontal";
        /// <summary> The axis vertical. </summary>
        private const string AXIS_VERTICAL = "Vertical";
        /// <summary> The axis mouse x coordinate. </summary>
        private const string AXIS_MOUSE_X = "Mouse X";
        /// <summary> The axis mouse y coordinate. </summary>
        private const string AXIS_MOUSE_Y = "Mouse Y";
        /// <summary> The rotate sensitivity. </summary>
        private const float ROTATE_SENSITIVITY = 4;

        /// <summary> Gets the number of controllers. </summary>
        /// <value> The number of controllers. </value>
        public override int ControllerCount
        {
            get
            {
                return 1;
            }
        }

        /// <summary> Constructor. </summary>
        /// <param name="states"> The states.</param>
        public EditorControllerProvider(ControllerState[] states) : base(states)
        {
            Inited = true;
        }

        /// <summary> Executes the 'pause' action. </summary>
        public override void OnPause()
        {

        }

        /// <summary> Executes the 'resume' action. </summary>
        public override void OnResume()
        {

        }

        /// <summary> Updates this object. </summary>
        public override void Update()
        {
            if (!Inited)
                return;
            UpdateControllerState(0);
        }

        /// <summary> Executes the 'destroy' action. </summary>
        public override void OnDestroy()
        {

        }

        /// <summary> Updates the controller state described by index. </summary>
        /// <param name="index"> Zero-based index of the.</param>
        private void UpdateControllerState(int index)
        {
            states[index].controllerType = ControllerType.CONTROLLER_TYPE_EDITOR;
            states[index].availableFeature = ControllerAvailableFeature.CONTROLLER_AVAILABLE_FEATURE_ROTATION;
            states[index].connectionState = ControllerConnectionState.CONTROLLER_CONNECTION_STATE_CONNECTED;
            UpdateRotation(index);

            states[index].touchPos = new Vector2(Input.GetAxis(AXIS_HORIZONTAL), Input.GetAxis(AXIS_VERTICAL));
            states[index].isTouching = !states[index].touchPos.Equals(Vector2.zero);
            states[index].recentered = false;

            states[index].buttonsState =
                (Input.GetKey(KeyCode.Mouse0) ? ControllerButton.TRIGGER : 0)
                | (Input.GetKey(KeyCode.Mouse1) ? ControllerButton.HOME : 0)
                | (Input.GetKey(KeyCode.Mouse2) ? ControllerButton.APP : 0);
            states[index].buttonsDown = 
                (Input.GetKeyDown(KeyCode.Mouse0) ? ControllerButton.TRIGGER : 0)
                | (Input.GetKeyDown(KeyCode.Mouse1) ? ControllerButton.HOME : 0)
                | (Input.GetKeyDown(KeyCode.Mouse2) ? ControllerButton.APP : 0);
            states[index].buttonsUp = 
                (Input.GetKeyUp(KeyCode.Mouse0) ? ControllerButton.TRIGGER : 0)
                | (Input.GetKeyUp(KeyCode.Mouse1) ? ControllerButton.HOME : 0)
                | (Input.GetKeyUp(KeyCode.Mouse2) ? ControllerButton.APP : 0);
            states[index].isCharging = false;
            states[index].batteryLevel = 100;
            CheckRecenter(index);
        }

        /// <summary> Updates the rotation described by index. </summary>
        /// <param name="index"> Zero-based index of the.</param>
        private void UpdateRotation(int index)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                mouseDelta.Set(
                    Input.GetAxis(AXIS_MOUSE_X),
                    -Input.GetAxis(AXIS_MOUSE_Y));
                Vector3 deltaDegrees = mouseDelta * ROTATE_SENSITIVITY;
                Quaternion yaw = Quaternion.AngleAxis(deltaDegrees.x, Vector3.up);
                Quaternion pitch = Quaternion.AngleAxis(deltaDegrees.y, Vector3.right);
                states[index].rotation = states[index].rotation * yaw * pitch;
            }
        }

        /// <summary> Check recenter. </summary>
        /// <param name="index"> Zero-based index of the.</param>
        private void CheckRecenter(int index)
        {
            if (states[index].GetButtonDown(ControllerButton.APP))
            {
                states[index].recentered = true;
                states[index].rotation = Quaternion.identity;
            }
        }
    }
}
