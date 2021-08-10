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
    using UnityEngine.UI;

    /// <summary> A capture behaviour base. </summary>
    public class CaptureBehaviourBase : MonoBehaviour, IFrameConsumer
    {
        /// <summary> The RGB camera rig. </summary>
        [SerializeField]
        private Transform RGBCameraRig;
        /// <summary> The capture camera. </summary>
        public Camera CaptureCamera;
        //[SerializeField]
        //private NRBackGroundRender m_BackGroundRender;
        /// <summary> Context for the frame capture. </summary>
        private FrameCaptureContext m_FrameCaptureContext;
        //private Material m_BackGroundMat;

        /// <summary> Gets the context. </summary>
        /// <returns> The context. </returns>
        public FrameCaptureContext GetContext()
        {
            return m_FrameCaptureContext;
        }

        /// <summary> Initializes this object. </summary>
        /// <param name="context">     The context.</param>
        /// <param name="blendCamera"> The blend camera.</param>
        public virtual void Init(FrameCaptureContext context)
        {
            this.m_FrameCaptureContext = context;
        }

        public void SetBackGroundColor(Color color)
        {
            this.CaptureCamera.backgroundColor = new Color(color.r, color.g, color.b, 0);
        }

        //private void SwitchBackGround(bool flag)
        //{
        //    m_BackGroundRender.enabled = flag;
        //}

        //private void SetBackGroundImage(UniversalTextureFrame frame, TextureType texturetype)
        //{
        //    if (texturetype == TextureType.RGB)
        //    {
        //        m_BackGroundMat.SetTexture("_MainTex", frame.textures[0]);
        //    }
        //    else
        //    {
        //        m_BackGroundMat.SetTexture("_MainTex", frame.textures[0]);
        //        m_BackGroundMat.SetTexture("_UTex", frame.textures[1]);
        //        m_BackGroundMat.SetTexture("_VTex", frame.textures[2]);
        //    }
        //}

        //private Material CreateBackGroundMaterial(TextureType textype)
        //{
        //    string shader_name;
        //    if (textype == TextureType.RGB)
        //    {
        //        shader_name = "Record/Shaders/NRBackground";
        //    }
        //    else
        //    {
        //        shader_name = "Record/Shaders/NRBackgroundYUV";
        //    }

        //    return new Material(Resources.Load<Shader>(shader_name));
        //}

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
        public virtual void OnFrame(UniversalTextureFrame frame)
        {
            // update camera pose
            UpdateHeadPoseByTimestamp(frame.timeStamp);

            //var blendmode = this.GetContext().RequestCameraParam().blendMode;
            //if (blendmode == BlendMode.Blend)
            //{
            //    if (m_BackGroundMat == null)
            //    {
            //        m_BackGroundMat = CreateBackGroundMaterial(frame.textureType);
            //        m_BackGroundRender.SetMaterial(m_BackGroundMat);
            //    }
            //    SwitchBackGround(true);
            //    SetBackGroundImage(frame, frame.textureType);
            //}
            //else
            //{
            //    SwitchBackGround(false);
            //}
        }

        /// <summary> Updates the head pose by timestamp described by timestamp. </summary>
        /// <param name="timestamp"> The timestamp.</param>
        private void UpdateHeadPoseByTimestamp(UInt64 timestamp)
        {
            Pose head_pose = Pose.identity;
            var result = NRFrame.GetHeadPoseByTime(ref head_pose, timestamp);
            if (result)
            {
                RGBCameraRig.transform.localPosition = head_pose.position;
                RGBCameraRig.transform.localRotation = head_pose.rotation;
            }
        }
    }
}
