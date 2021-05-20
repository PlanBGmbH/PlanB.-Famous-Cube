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

    /// <summary>
    /// HMDPoseTracker update the infomations of pose tracker. This component is used to initialize
    /// the camera parameter, update the device posture, In addition, application can change
    /// TrackingType through this component. </summary>
    [HelpURL("https://developer.nreal.ai/develop/discover/introduction-nrsdk")]
    public class NRHMDPoseTracker : MonoBehaviour
    {
        /// <summary> Hmd pose tracker event. </summary>
        public delegate void HMDPoseTrackerEvent();
        /// <summary> Event queue for all listeners interested in OnHMDPoseReady events. </summary>
        public static event HMDPoseTrackerEvent OnHMDPoseReady;
        /// <summary> Event queue for all listeners interested in OnHMDLostTracking events. </summary>
        public static event HMDPoseTrackerEvent OnHMDLostTracking;

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
        /// <summary> The right camera. </summary>
        public Camera rightCamera;

        /// <summary> Awakes this object. </summary>
        void Awake()
        {
#if UNITY_EDITOR
            leftCamera.enabled = false;
            rightCamera.enabled = false;
            centerCamera.depth = 1;
            centerCamera.enabled = true;
#else
            centerCamera.enabled = false;
#endif
            StartCoroutine(Initialize());
        }

        /// <summary> Executes the 'enable' action. </summary>
        void OnEnable()
        {
            NRKernalUpdater.OnUpdate += OnUpdate;
        }

        /// <summary> Executes the 'disable' action. </summary>
        void OnDisable()
        {
            NRKernalUpdater.OnUpdate -= OnUpdate;
        }

        /// <summary> Executes the 'update' action. </summary>
        void OnUpdate()
        {
            CheckHMDPoseState();
            UpdatePoseByTrackingType();
        }

        private bool _isChangeModeLocked = false;
        /// <summary> Change mode. </summary>
        /// <param name="trackingtype">        The trackingtype.</param>
        /// <param name="modeChangedCallBack"> The mode changed call back and return the result.</param>
        private void ChangeMode(TrackingType trackingtype, Action<bool> modeChangedCallBack = null)
        {
            if (NRFrame.SessionStatus != SessionState.Running ||
                trackingtype == m_TrackingType || _isChangeModeLocked)
            {
                modeChangedCallBack?.Invoke(false);
                return;
            }

#if !UNITY_EDITOR
            _isChangeModeLocked = true;
            AsyncTaskExecuter.Instance.RunAction(() =>
            {
                var result = NRSessionManager.Instance.NativeAPI.NativeTracking.SwitchTrackingMode((TrackingMode)trackingtype);

                if (result)
                {
                    NRFrame.ClearPose();
                    m_TrackingType = trackingtype;
                }
                _isChangeModeLocked = false;

                modeChangedCallBack?.Invoke(result);
            });
#else
            m_TrackingType = trackingtype;
            modeChangedCallBack?.Invoke(true);
#endif
        }

        /// <summary> Change to 6 degree of freedom. </summary>
        public void ChangeTo6Dof() { ChangeTo6Dof(null); }
        public void ChangeTo6Dof(Action<bool> modeChangedCallBack) { ChangeMode(TrackingType.Tracking6Dof, modeChangedCallBack); }

        /// <summary> Change to 3 degree of freedom. </summary>
        public void ChangeTo3Dof() { ChangeTo3Dof(null); }
        public void ChangeTo3Dof(Action<bool> modeChangedCallBack) { ChangeMode(TrackingType.Tracking3Dof, modeChangedCallBack); }

        /// <summary> Change to 0 degree of freedom. </summary>
        public void ChangeTo0Dof() { ChangeTo0Dof(null); }
        public void ChangeTo0Dof(Action<bool> modeChangedCallBack) { ChangeMode(TrackingType.Tracking0Dof, modeChangedCallBack); }

        /// <summary> Initializes this object. </summary>
        /// <returns> An IEnumerator. </returns>
        private IEnumerator Initialize()
        {
            leftCamera.enabled = false;
            rightCamera.enabled = false;
            while (NRFrame.SessionStatus != SessionState.Running)
            {
                NRDebugger.Debug("[NRHMDPoseTracker] Waitting to initialize.");
                yield return new WaitForEndOfFrame();
            }

#if !UNITY_EDITOR
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
                centerCamera.transform.localPosition = (leftCamera.transform.localPosition + rightCamera.transform.localPosition) * 0.5f;
                centerCamera.transform.localRotation = Quaternion.Lerp(leftCamera.transform.localRotation, rightCamera.transform.localRotation, 0.5f);
            }
#endif
            NRDebugger.Info("[NRHMDPoseTracker] Initialized success.");
        }

        /// <summary> Updates the pose by tracking type. </summary>
        private void UpdatePoseByTrackingType()
        {
            Pose pose = NRFrame.HeadPose;
            switch (m_TrackingType)
            {
                case TrackingType.Tracking6Dof:
                    if (UseRelative)
                    {
                        transform.localRotation = pose.rotation;
                        transform.localPosition = pose.position;
                    }
                    else
                    {
                        transform.rotation = pose.rotation;
                        transform.position = pose.position;
                    }
                    break;
                case TrackingType.Tracking3Dof:
                    if (UseRelative)
                    {
                        transform.localRotation = pose.rotation;
                        transform.localPosition = Vector3.zero;
                    }
                    else
                    {
                        transform.rotation = pose.rotation;
                        transform.position = Vector3.zero;
                    }
                    break;
                case TrackingType.Tracking0Dof:

                    break;
                default:
                    break;
            }

            centerCamera.transform.localPosition = (leftCamera.transform.localPosition + rightCamera.transform.localPosition) * 0.5f;
            centerCamera.transform.localRotation = Quaternion.Lerp(leftCamera.transform.localRotation, rightCamera.transform.localRotation, 0.5f);
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
                if (currentReason != LostTrackingReason.NONE)
                {
                    OnHMDLostTracking?.Invoke();
                }
                else if (currentReason == LostTrackingReason.NONE)
                {
                    OnHMDPoseReady?.Invoke();
                }
                m_LastReason = currentReason;
            }
        }
    }
}
