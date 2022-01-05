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
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    /// <summary> A controller tracker. </summary>
    public class ControllerTracker : MonoBehaviour
    {
        /// <summary> The default hand enum. </summary>
        public ControllerHandEnum defaultHandEnum;
        /// <summary> The raycaster. </summary>
        public NRPointerRaycaster raycaster;
        /// <summary> The model anchor. </summary>
        public Transform modelAnchor;

        /// <summary> The verify y coordinate angle. </summary>
        private float m_VerifyYAngle;
        /// <summary> True if is enabled, false if not. </summary>
        private bool m_IsEnabled;
        /// <summary> True if is 6dof, false if not. </summary>
        private bool m_Is6dof;
        /// <summary> The default local offset. </summary>
        private Vector3 m_DefaultLocalOffset;

        /// <summary> Gets the camera center. </summary>
        /// <value> The camera center. </value>
        private Transform CameraCenter
        {
            get
            {
                return NRInput.CameraCenter;
            }
        }

        /// <summary> Awakes this object. </summary>
        private void Awake()
        {
            m_DefaultLocalOffset = transform.localPosition;
            raycaster.RelatedHand = defaultHandEnum;
        }

        /// <summary> Executes the 'enable' action. </summary>
        private void OnEnable()
        {
            NRInput.OnControllerRecentering += OnRecentering;
            NRInput.OnControllerStatesUpdated += OnControllerStatesUpdated;
        }

        /// <summary> Executes the 'disable' action. </summary>
        private void OnDisable()
        {
            NRInput.OnControllerRecentering -= OnRecentering;
            NRInput.OnControllerStatesUpdated -= OnControllerStatesUpdated;
        }

        /// <summary> Executes the 'controller states updated' action. </summary>
        private void OnControllerStatesUpdated()
        {
            UpdateTracker();
        }

        /// <summary> Updates the tracker. </summary>
        private void UpdateTracker()
        {
            if (CameraCenter == null)
                return;
            m_IsEnabled = NRInput.CheckControllerAvailable(defaultHandEnum) && !NRInput.Hands.IsRunning;
            raycaster.gameObject.SetActive(m_IsEnabled && NRInput.RaycastersActive && NRInput.RaycastMode == RaycastModeEnum.Laser);
            modelAnchor.gameObject.SetActive(m_IsEnabled);
            if (m_IsEnabled)
            {
                TrackPose();
            }
        }

        /// <summary> Track pose. </summary>
        private void TrackPose()
        {
            m_Is6dof = NRInput.GetControllerAvailableFeature(ControllerAvailableFeature.CONTROLLER_AVAILABLE_FEATURE_POSITION)
                && NRInput.GetControllerAvailableFeature(ControllerAvailableFeature.CONTROLLER_AVAILABLE_FEATURE_ROTATION);

            Pose poseInAPIWorld = new Pose(NRInput.GetPosition(defaultHandEnum), NRInput.GetRotation(defaultHandEnum));
            Pose poseInUnityWorld = ConversionUtility.ApiWorldToUnityWorld(poseInAPIWorld);
            Vector3 cameraWorldUp = NRFrame.GetWorldMatrixFromUnityToNative().MultiplyVector(Vector3.up);
            poseInUnityWorld.rotation = Quaternion.AngleAxis(m_VerifyYAngle, cameraWorldUp) * poseInUnityWorld.rotation;

            transform.position = m_Is6dof ? poseInUnityWorld.position : CameraCenter.TransformPoint(m_DefaultLocalOffset);
            transform.rotation = poseInUnityWorld.rotation;
        }

        /// <summary> Executes the 'recentering' action. </summary>
        private void OnRecentering()
        {
            Matrix4x4 cameraWorldMatrix = NRFrame.GetWorldMatrixFromUnityToNative();
            Vector3 cameraWorldUp = cameraWorldMatrix.MultiplyVector(Vector3.up);
            Vector3 cameraWorldFoward = cameraWorldMatrix.MultiplyVector(Vector3.forward);
            Vector3 horizontalFoward = Vector3.ProjectOnPlane(CameraCenter.forward, cameraWorldUp);
            m_VerifyYAngle = Mathf.Sign(Vector3.Cross(cameraWorldFoward, horizontalFoward).y) * Vector3.Angle(horizontalFoward, cameraWorldFoward);
        }
    }
}