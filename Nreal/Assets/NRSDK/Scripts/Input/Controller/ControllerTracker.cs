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
    public class ControllerTracker : MonoBehaviour {
        /// <summary> The default hand enum. </summary>
        public ControllerHandEnum defaultHandEnum;
        /// <summary> The raycaster. </summary>
        public NRPointerRaycaster raycaster;
        /// <summary> The model anchor. </summary>
        public Transform modelAnchor;

        /// <summary> The verify y coordinate angle. </summary>
        private float m_VerifyYAngle = 0f;
        /// <summary> True if is enabled, false if not. </summary>
        private bool m_IsEnabled;
        /// <summary> True if is 6dof, false if not. </summary>
        private bool m_Is6dof;
        /// <summary> The default local offset. </summary>
        private Vector3 m_DefaultLocalOffset;
        /// <summary> Target position. </summary>
        private Vector3 m_TargetPos = Vector3.zero;
        /// <summary> True if is moving to target, false if not. </summary>
        private bool m_IsMovingToTarget;
        /// <summary> The laser default local offset. </summary>
        private Vector3 m_LaserDefaultLocalOffset;
        /// <summary> The model default local offset. </summary>
        private Vector3 m_ModelDefaultLocalOffset;

        /// <summary> The track target speed. </summary>
        private const float TrackTargetSpeed = 6f;
        /// <summary> The maximum distance from target. </summary>
        private const float MaxDistanceFromTarget = 0.12f;

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
            m_LaserDefaultLocalOffset = raycaster.transform.localPosition;
            m_ModelDefaultLocalOffset = modelAnchor.localPosition;
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
                TrackPose();
        }

        /// <summary> Track pose. </summary>
        private void TrackPose()
        {
            m_Is6dof = NRInput.GetControllerAvailableFeature(ControllerAvailableFeature.CONTROLLER_AVAILABLE_FEATURE_POSITION)
                && NRInput.GetControllerAvailableFeature(ControllerAvailableFeature.CONTROLLER_AVAILABLE_FEATURE_ROTATION);
            if (m_Is6dof)
            {
                raycaster.transform.localPosition = Vector3.zero;
                modelAnchor.localPosition = Vector3.zero;
                UpdatePosition();
            }
            else
            {
                raycaster.transform.localPosition = m_LaserDefaultLocalOffset;
                modelAnchor.localPosition = m_ModelDefaultLocalOffset;
                SmoothTrackTargetPosition();
            }
            UpdateRotation();
        }

        /// <summary> Updates the position. </summary>
        private void UpdatePosition()
        {
            transform.position = NRInput.GetPosition(defaultHandEnum);
        }

        /// <summary> Updates the rotation. </summary>
        private void UpdateRotation()
        {
            transform.localRotation = NRInput.GetRotation(defaultHandEnum);
            transform.Rotate(Vector3.up * m_VerifyYAngle, Space.World);
        }

        /// <summary> Smooth track target position. </summary>
        private void SmoothTrackTargetPosition()
        {
            int sign = defaultHandEnum == ControllerHandEnum.Right ? 1 : -1;
            m_TargetPos = CameraCenter.position + Vector3.up * m_DefaultLocalOffset.y
                + new Vector3(CameraCenter.right.x, 0f, CameraCenter.right.z).normalized * Mathf.Abs(m_DefaultLocalOffset.x) * sign
                + new Vector3(CameraCenter.forward.x, 0f, CameraCenter.forward.z).normalized * m_DefaultLocalOffset.z;
            if (!m_IsMovingToTarget)
            {
                if (Vector3.Distance(transform.position, m_TargetPos) > MaxDistanceFromTarget)
                    m_IsMovingToTarget = true;
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, m_TargetPos, TrackTargetSpeed * Time.deltaTime);
                if (Vector3.Distance(m_TargetPos, transform.position) < 0.02f)
                    m_IsMovingToTarget = false;
            }
        }

        /// <summary> Executes the 'recentering' action. </summary>
        private void OnRecentering()
        {
            Vector3 horizontalFoward = Vector3.ProjectOnPlane(CameraCenter.forward, Vector3.up);
            m_VerifyYAngle = Mathf.Sign(Vector3.Cross(Vector3.forward, horizontalFoward).y) * Vector3.Angle(horizontalFoward, Vector3.forward);
        }
    }
    
}