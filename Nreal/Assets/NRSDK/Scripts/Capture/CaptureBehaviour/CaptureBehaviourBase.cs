/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRKernal.Record
{
    using System;
    using UnityEngine;

    /// <summary> A capture behaviour base. </summary>
    public class CaptureBehaviourBase : MonoBehaviour
    {
        /// <summary> The RGB camera rig. </summary>
        [SerializeField]
        private Transform RGBCameraRig;
        /// <summary> The capture camera. </summary>
        public Camera CaptureCamera;
        /// <summary> Context for the frame capture. </summary>
        private FrameCaptureContext m_FrameCaptureContext;
        /// <summary> The blender. </summary>
        private FrameBlender m_Blender;

        /// <summary> Gets the context. </summary>
        /// <returns> The context. </returns>
        public FrameCaptureContext GetContext()
        {
            return m_FrameCaptureContext;
        }

        /// <summary> Initializes this object. </summary>
        /// <param name="context">     The context.</param>
        /// <param name="blendCamera"> The blend camera.</param>
        public virtual void Init(FrameCaptureContext context, FrameBlender blendCamera)
        {
            this.m_FrameCaptureContext = context;
            this.m_Blender = blendCamera;
        }

        /// <summary> The predict time. </summary>
        private ulong m_PredictTime;
        /// <summary> Updates the predict time described by value. </summary>
        /// <param name="value"> The value.</param>
        public void UpdatePredictTime(float value)
        {
            m_PredictTime = (ulong)(value * 1000000);
        }

        /// <summary> Executes the 'frame' action. </summary>
        /// <param name="frame"> The frame.</param>
        public virtual void OnFrame(CameraTextureFrame frame)
        {
            // update camera pose
            UpdateHeadPoseByTimestamp(frame.timeStamp);

            // commit a frame
            m_Blender.OnFrame(frame);
        }

        /// <summary> Updates the head pose by timestamp described by timestamp. </summary>
        /// <param name="timestamp"> The timestamp.</param>
        private void UpdateHeadPoseByTimestamp(UInt64 timestamp)
        {
            Pose head_pose = Pose.identity;
            var result = NRFrame.GetHeadPoseByTime(ref head_pose, timestamp, m_PredictTime);
            if (result)
            {
                RGBCameraRig.transform.localPosition = head_pose.position;
                RGBCameraRig.transform.localRotation = head_pose.rotation;
            }
        }
    }
}
