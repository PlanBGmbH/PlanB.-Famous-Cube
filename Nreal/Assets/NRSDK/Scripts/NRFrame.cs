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
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Holds information about NR Device's pose in the world coordinate, trackables, etc.. Through
    /// this class, application can get the information of current frame. It contains session status,
    /// lost tracking reason, device pose, trackables, etc. </summary>
    public class NRFrame
    {
        /// <summary> Get the tracking state of HMD. </summary>
        /// <value> The session status. </value>
        public static SessionState SessionStatus
        {
            get
            {
                return NRSessionManager.Instance.SessionState;
            }
        }

        /// <summary> Get the lost tracking reason of HMD. </summary>
        /// <value> The lost tracking reason. </value>
        public static LostTrackingReason LostTrackingReason
        {
            get
            {
                return NRSessionManager.Instance.LostTrackingReason;
            }
        }

        /// <summary> The head pose. </summary>
        private static Pose m_HeadPose;

        /// <summary> Get the pose of device in unity world coordinate. </summary>
        /// <value> Pose of device. </value>
        public static Pose HeadPose
        {
            get
            {
                return m_HeadPose;
            }
        }

        public static bool isHeadPoseReady { get; private set; }

        /// <summary> Gets head pose by recommond timestamp. </summary>
        /// <param name="pose">      [in,out] The pose.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public static bool GetHeadPoseByTime(ref Pose pose)
        {
            if (SessionStatus == SessionState.Running)
            {
                isHeadPoseReady = NRSessionManager.Instance.NativeAPI.NativeHeadTracking.GetHeadPoseRecommend(ref pose);
                return isHeadPoseReady;
            }
            return false;
        }

        /// <summary> Gets head pose by timestamp. </summary>
        /// <param name="pose">      [in,out] The pose.</param>
        /// <param name="timestamp"> The timestamp.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public static bool GetHeadPoseByTime(ref Pose pose, UInt64 timestamp)
        {
            if (SessionStatus == SessionState.Running)
            {
                isHeadPoseReady = NRSessionManager.Instance.NativeAPI.NativeHeadTracking.GetHeadPose(ref pose, timestamp);
                return isHeadPoseReady;
            }
            return false;
        }

        /// <summary>
        /// Get the pose information when the current frame display on the screen.
        /// </summary>
        /// <param name="pose"></param>
        /// <param name="timestamp">current timestamp to the pose.</param>
        /// <returns></returns>
        public static bool GetFramePresentHeadPose(ref Pose pose, ref UInt64 timestamp)
        {
            if (SessionStatus == SessionState.Running)
            {
                isHeadPoseReady = NRSessionManager.Instance.NativeAPI.NativeHeadTracking.GetFramePresentHeadPose(ref pose, ref timestamp);
                return isHeadPoseReady;
            }
            return false;
        }

        /// <summary> Get the pose of center camera between left eye and right eye. </summary>
        /// <value> The center eye pose. </value>
        public static Pose CenterEyePose
        {
            get
            {
                Transform leftCamera = NRSessionManager.Instance.NRHMDPoseTracker.leftCamera.transform;
                Transform rightCamera = NRSessionManager.Instance.NRHMDPoseTracker.rightCamera.transform;

                Vector3 centerEye_pos = (leftCamera.position + rightCamera.position) * 0.5f;
                Quaternion centerEye_rot = Quaternion.Lerp(leftCamera.rotation, rightCamera.rotation, 0.5f);

                return new Pose(centerEye_pos, centerEye_rot);
            }
        }

        /// <summary> The eye position from head. </summary>
        private static EyePoseData m_EyePosFromHead;

        /// <summary> Get the offset position between eye and head. </summary>
        /// <value> The eye pose from head. </value>
        public static EyePoseData EyePoseFromHead
        {
            get
            {
                if (SessionStatus == SessionState.Running)
                {
                    m_EyePosFromHead.LEyePose = NRDevice.Instance.NativeHMD.GetEyePoseFromHead((int)NativeEye.LEFT);
                    m_EyePosFromHead.REyePose = NRDevice.Instance.NativeHMD.GetEyePoseFromHead((int)NativeEye.RIGHT);
                    m_EyePosFromHead.RGBEyePose = NRDevice.Instance.NativeHMD.GetEyePoseFromHead((int)NativeEye.RGB);
                }
                return m_EyePosFromHead;
            }
        }

        /// <summary> Get the project matrix of camera in unity. </summary>
        /// <param name="result"> [out] True to result.</param>
        /// <param name="znear">  The znear.</param>
        /// <param name="zfar">   The zfar.</param>
        /// <returns> project matrix of camera. </returns>
        public static EyeProjectMatrixData GetEyeProjectMatrix(out bool result, float znear, float zfar)
        {
            result = false;
            EyeProjectMatrixData m_EyeProjectMatrix = new EyeProjectMatrixData();
            result = NRDevice.Instance.NativeHMD.GetProjectionMatrix(ref m_EyeProjectMatrix, znear, zfar);
            return m_EyeProjectMatrix;
        }

        /// <summary> Get the intrinsic matrix of rgb camera. </summary>
        /// <returns> The RGB camera intrinsic matrix. </returns>
        public static NativeMat3f GetRGBCameraIntrinsicMatrix()
        {
            NativeMat3f result = new NativeMat3f();
            NRDevice.Instance.NativeHMD.GetCameraIntrinsicMatrix((int)NativeEye.RGB, ref result);
            return result;
        }

        /// <summary> Get the Distortion of rgbcamera. </summary>
        /// <returns> The RGB camera distortion. </returns>
        public static NRDistortionParams GetRGBCameraDistortion()
        {
            NRDistortionParams result = new NRDistortionParams();
            NRDevice.Instance.NativeHMD.GetCameraDistortion((int)NativeEye.RGB, ref result);
            return result;
        }

        private static UInt64 m_CurrentPoseTimeStamp = 0;
        public static UInt64 CurrentPoseTimeStamp
        {
            get
            {
                return m_CurrentPoseTimeStamp;
            }
        }
        /// <summary> Executes the 'update' action. </summary>
        internal static void OnUpdate()
        {
            // Update head pos
            Pose pose = Pose.identity;
            if (GetFramePresentHeadPose(ref pose, ref m_CurrentPoseTimeStamp) && LostTrackingReason != LostTrackingReason.INITIALIZING)
            {
                m_HeadPose = pose;
            }
        }

        /// <summary> Get the list of trackables with specified filter. </summary>
        /// <typeparam name="T"> Generic type parameter.</typeparam>
        /// <param name="trackables"> A list where the returned trackable is stored. The previous values
        ///                           will be cleared.</param>
        /// <param name="filter">     Query filter.</param>
        public static void GetTrackables<T>(List<T> trackables, NRTrackableQueryFilter filter) where T : NRTrackable
        {
            if (SessionStatus != SessionState.Running)
            {
                return;
            }
            trackables.Clear();
            NRSessionManager.Instance.NativeAPI.TrackableFactory.GetTrackables<T>(trackables, filter);
        }
    }
}
