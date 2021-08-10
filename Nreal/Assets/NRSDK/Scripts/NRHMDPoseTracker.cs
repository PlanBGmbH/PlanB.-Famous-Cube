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
    using System.Collections;
    using System;
    using System.Collections.Generic;

#if USING_XR_SDK
    using UnityEngine.XR;
#endif

    /// <summary> Hmd pose tracker event. </summary>
    public delegate void HMDPoseTrackerEvent();
    public delegate void HMDPoseTrackerModeChangeEvent(NRHMDPoseTracker.TrackingType origin, NRHMDPoseTracker.TrackingType target);
    public delegate void OnTrackingModeChanged(NRHMDPoseTracker.TrackingModeChangedResult result);

    /// <summary>
    /// HMDPoseTracker update the infomations of pose tracker. This component is used to initialize
    /// the camera parameter, update the device posture, In addition, application can change
    /// TrackingType through this component. </summary>
    [HelpURL("https://developer.nreal.ai/develop/discover/introduction-nrsdk")]
    public class NRHMDPoseTracker : MonoBehaviour
    {
        /// <summary> Event queue for all listeners interested in OnHMDPoseReady events. </summary>
        public static event HMDPoseTrackerEvent OnHMDPoseReady;
        /// <summary> Event queue for all listeners interested in OnHMDLostTracking events. </summary>
        public static event HMDPoseTrackerEvent OnHMDLostTracking;
        /// <summary> Event queue for all listeners interested in OnChangeTrackingMode events. </summary>
        public static event HMDPoseTrackerModeChangeEvent OnChangeTrackingMode;

        public struct TrackingModeChangedResult
        {
            public bool success;
            public TrackingType trackingType;
        }

        /// <summary> HMD tracking type. </summary>
        public enum TrackingType
        {
            /// <summary>
            /// Track the position an rotation.
            /// </summary>
            Tracking6Dof = 0,

            /// <summary>
            /// Track the rotation only.
            /// </summary>
            Tracking3Dof = 1,

            /// <summary>
            /// Track nothing.
            /// </summary>
            Tracking0Dof = 2,
        }

        /// <summary> Type of the tracking. </summary>
        [SerializeField]
        private TrackingType m_TrackingType = TrackingType.Tracking6Dof;

        /// <summary> Gets the tracking mode. </summary>
        /// <value> The tracking mode. </value>
        public TrackingType TrackingMode
        {
            get
            {
                return m_TrackingType;
            }
        }

        /// <summary> Use relative coordinates or not. </summary>
        public bool UseRelative = false;
        /// <summary> The last reason. </summary>
        private LostTrackingReason m_LastReason = LostTrackingReason.INITIALIZING;

        /// <summary> The left camera. </summary>
        public Camera leftCamera;
        /// <summary> The center camera. </summary>
        public Camera centerCamera;
        public Transform centerAnchor;
        /// <summary> The right camera. </summary>
        public Camera rightCamera;
        private bool m_ModeChangeLock = false;

#if USING_XR_SDK
        static internal List<XRNodeState> nodeStates = new List<XRNodeState>();
        static internal void GetNodePoseData(XRNode node, out Pose resultPose)
        {
            InputTracking.GetNodeStates(nodeStates);
            for (int i = 0; i < nodeStates.Count; i++)
            {
                var nodeState = nodeStates[i];
                if (nodeState.nodeType == node)
                {
                    nodeState.TryGetPosition(out resultPose.position);
                    nodeState.TryGetRotation(out resultPose.rotation);
                    return;
                }
            }
            resultPose = Pose.identity;
        }
#endif

        /// <summary> Awakes this object. </summary>
        void Awake()
        {
#if UNITY_EDITOR || USING_XR_SDK
            if (leftCamera != null)
                leftCamera.enabled = false;
            if (rightCamera != null)
                rightCamera.enabled = false;
            centerCamera.depth = 1;
            centerCamera.enabled = true;
#else
            if (leftCamera != null)
                leftCamera.enabled = true;
            if (rightCamera != null)
                rightCamera.enabled = true;
            centerCamera.enabled = false;
#endif
            StartCoroutine(Initialize());
        }

        /// <summary> Executes the 'enable' action. </summary>
        void OnEnable()
        {
#if USING_XR_SDK && !UNITY_EDITOR
            Application.onBeforeRender += OnUpdate;
#else
            NRKernalUpdater.OnUpdate += OnUpdate;
#endif
        }

        /// <summary> Executes the 'disable' action. </summary>
        void OnDisable()
        {
#if USING_XR_SDK && !UNITY_EDITOR
            Application.onBeforeRender -= OnUpdate;
#else
            NRKernalUpdater.OnUpdate -= OnUpdate;
#endif
        }

        /// <summary> Executes the 'update' action. </summary>
        void OnUpdate()
        {
            CheckHMDPoseState();
            UpdatePoseByTrackingType();
        }

        /// <summary> Change mode. </summary>
        /// <param name="trackingtype">        The trackingtype.</param>
        /// <param name="OnModeChanged"> The mode changed call back and return the result.</param>
        private bool ChangeMode(TrackingType trackingtype, OnTrackingModeChanged OnModeChanged)
        {
            NRDebugger.Info("[NRHMDPoseTracker] Begin change mode to:" + trackingtype);
            TrackingModeChangedResult result = new TrackingModeChangedResult();
            if (trackingtype == m_TrackingType || m_ModeChangeLock)
            {
                result.success = false;
                result.trackingType = m_TrackingType;
                OnModeChanged?.Invoke(result);
                NRDebugger.Warning("[NRHMDPoseTracker] Change tracking mode faild...");
                return false;
            }

            OnChangeTrackingMode?.Invoke(m_TrackingType, trackingtype);
            NRSessionManager.OnChangeTrackingMode?.Invoke(m_TrackingType, trackingtype);

#if !UNITY_EDITOR
            m_ModeChangeLock = true;
            AsyncTaskExecuter.Instance.RunAction(() =>
            {
                result.success = NRSessionManager.Instance.NativeAPI.NativeTracking.SwitchTrackingMode((TrackingMode)trackingtype);

                if (result.success)
                {
                    m_TrackingType = trackingtype;
                }
                result.trackingType = m_TrackingType;
                OnModeChanged?.Invoke(result);
                m_ModeChangeLock = false;
                NRDebugger.Info("[NRHMDPoseTracker] End Change mode, result:" + result.success);
            });
#else
            m_TrackingType = trackingtype;
            result.success = true;
            result.trackingType = m_TrackingType;
            OnModeChanged?.Invoke(result);
#endif
            return true;
        }

        /// <summary> Change to 6 degree of freedom. </summary>
        public bool ChangeTo6Dof(OnTrackingModeChanged OnModeChanged) { return ChangeMode(TrackingType.Tracking6Dof, OnModeChanged); }

        /// <summary> Change to 3 degree of freedom. </summary>
        public bool ChangeTo3Dof(OnTrackingModeChanged OnModeChanged) { return ChangeMode(TrackingType.Tracking3Dof, OnModeChanged); }

        /// <summary> Change to 0 degree of freedom. </summary>
        public bool ChangeTo0Dof(OnTrackingModeChanged OnModeChanged) { return ChangeMode(TrackingType.Tracking0Dof, OnModeChanged); }

        private Matrix4x4 cachedWorldMatrix = Matrix4x4.identity;
        /// <summary> Cache the world matrix. </summary>
        public void CacheWorldMatrix(Pose pose)
        {
            Plane horizontal_plane = new Plane(Vector3.up, Vector3.zero);
            Vector3 forward_use_gravity = horizontal_plane.ClosestPointOnPlane(pose.forward).normalized;
            Quaternion rotation_use_gravity = Quaternion.LookRotation(forward_use_gravity, Vector3.up);
            cachedWorldMatrix = ConversionUtility.GetTMatrix(pose.position, rotation_use_gravity);
        }

        /// <summary> Reset world matrix ,position:(0,0,0) ,rotation:(x,0,z) </summary>
        public void ResetWorldMatrix()
        {
            Plane horizontal_plane = new Plane(Vector3.up, Vector3.zero);
            Pose pose = new Pose(transform.position, transform.rotation);
            Vector3 forward_use_gravity = horizontal_plane.ClosestPointOnPlane(pose.forward).normalized;
            Quaternion rotation_use_gravity = Quaternion.LookRotation(forward_use_gravity, Vector3.up);
            var matrix = ConversionUtility.GetTMatrix(pose.position, rotation_use_gravity);
            cachedWorldMatrix = Matrix4x4.Inverse(matrix) * cachedWorldMatrix;
        }

        private Pose ApplyWorldMatrix(Pose pose)
        {
            var objectMatrix = ConversionUtility.GetTMatrix(pose.position, pose.rotation);
            var object_in_world = cachedWorldMatrix * objectMatrix;
            return new Pose(ConversionUtility.GetPositionFromTMatrix(object_in_world),
                ConversionUtility.GetRotationFromTMatrix(object_in_world));
        }

        /// <summary> Initializes this object. </summary>
        /// <returns> An IEnumerator. </returns>
        private IEnumerator Initialize()
        {
            while (NRFrame.SessionStatus != SessionState.Running)
            {
                NRDebugger.Debug("[NRHMDPoseTracker] Waitting to initialize.");
                yield return new WaitForEndOfFrame();
            }

#if !UNITY_EDITOR && !USING_XR_SDK
            leftCamera.enabled = false;
            rightCamera.enabled = false;
            bool result;
            var matrix_data = NRFrame.GetEyeProjectMatrix(out result, leftCamera.nearClipPlane, leftCamera.farClipPlane);
            if (result)
            {
                leftCamera.projectionMatrix = matrix_data.LEyeMatrix;
                rightCamera.projectionMatrix = matrix_data.REyeMatrix;

                var eyeposeFromHead = NRFrame.EyePoseFromHead;
                leftCamera.transform.localPosition = eyeposeFromHead.LEyePose.position;
                leftCamera.transform.localRotation = eyeposeFromHead.LEyePose.rotation;
                rightCamera.transform.localPosition = eyeposeFromHead.REyePose.position;
                rightCamera.transform.localRotation = eyeposeFromHead.REyePose.rotation;
                centerAnchor.localPosition = centerCamera.transform.localPosition = (leftCamera.transform.localPosition + rightCamera.transform.localPosition) * 0.5f;
                centerAnchor.localRotation = centerCamera.transform.localRotation = Quaternion.Lerp(leftCamera.transform.localRotation, rightCamera.transform.localRotation, 0.5f);
            }
#endif
            NRDebugger.Info("[NRHMDPoseTracker] Initialized success.");
        }

        /// <summary> Updates the pose by tracking type. </summary>
        private void UpdatePoseByTrackingType()
        {
            Pose headPose;
#if USING_XR_SDK && !UNITY_EDITOR
            Pose centerPose;
            GetNodePoseData(XRNode.Head, out headPose);
            headPose = cachedWorldMatrix.Equals(Matrix4x4.identity) ? headPose : ApplyWorldMatrix(headPose);
            GetNodePoseData(XRNode.CenterEye, out centerPose);
            // NRDebugger.Info("HeadPose:{0} centerAnchor pose:{1}" ,headPose.position.ToString(), centerPose.position.ToString());
            switch (m_TrackingType)
            {
                case TrackingType.Tracking6Dof:
                    if (UseRelative)
                    {
                        transform.localRotation = headPose.rotation;
                        transform.localPosition = headPose.position;
                    }
                    else
                    {
                        transform.rotation = headPose.rotation;
                        transform.position = headPose.position;
                    }
                    break;
                case TrackingType.Tracking3Dof:
                    if (UseRelative)
                    {
                        transform.localRotation = headPose.rotation;
                    }
                    else
                    {
                        transform.rotation = headPose.rotation;
                    }
                    break;
                case TrackingType.Tracking0Dof:
                    break;
                default:
                    break;
            }
            centerCamera.transform.localPosition = Vector3.zero;
            centerCamera.transform.localRotation = Quaternion.identity;
            centerAnchor.localPosition = centerPose.position;
            centerAnchor.localRotation = centerPose.rotation;
#else
            headPose = NRFrame.HeadPose;
            headPose = cachedWorldMatrix.Equals(Matrix4x4.identity) ? headPose : ApplyWorldMatrix(headPose);
            switch (m_TrackingType)
            {
                case TrackingType.Tracking6Dof:
                    if (UseRelative)
                    {
                        transform.localRotation = headPose.rotation;
                        transform.localPosition = headPose.position;
                    }
                    else
                    {
                        transform.rotation = headPose.rotation;
                        transform.position = headPose.position;
                    }
                    break;
                case TrackingType.Tracking3Dof:
                    if (UseRelative)
                    {
                        transform.localRotation = headPose.rotation;
                        transform.localPosition = Vector3.zero;
                    }
                    else
                    {
                        transform.rotation = headPose.rotation;
                        transform.position = Vector3.zero;
                    }
                    break;
                case TrackingType.Tracking0Dof:

                    break;
                default:
                    break;
            }
#endif
        }

        /// <summary> Check hmd pose state. </summary>
        private void CheckHMDPoseState()
        {
            if (NRFrame.SessionStatus != SessionState.Running || TrackingMode != TrackingType.Tracking6Dof)
            {
                return;
            }

            var currentReason = NRFrame.LostTrackingReason;
            // When LostTrackingReason changed.
            if (currentReason != m_LastReason)
            {
                if (currentReason != LostTrackingReason.NONE && m_LastReason == LostTrackingReason.NONE)
                {
                    OnHMDLostTracking?.Invoke();
                    NRSessionManager.OnHMDLostTracking?.Invoke();
                }
                else if (currentReason == LostTrackingReason.NONE && m_LastReason != LostTrackingReason.NONE)
                {
                    OnHMDPoseReady?.Invoke();
                    NRSessionManager.OnHMDPoseReady?.Invoke();
                }
                m_LastReason = currentReason;
            }
        }
    }
}
