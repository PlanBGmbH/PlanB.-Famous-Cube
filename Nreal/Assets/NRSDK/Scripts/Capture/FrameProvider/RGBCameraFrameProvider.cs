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
    using UnityEngine;

    /// <summary> A RGB camera frame provider. </summary>
    public class RGBCameraFrameProvider : AbstractFrameProvider
    {
        /// <summary> The RGB tex. </summary>
        private NRRGBCamTexture m_RGBTex;

        /// <summary> Default constructor. </summary>
        public RGBCameraFrameProvider()
        {
            m_RGBTex = new NRRGBCamTexture();
            m_RGBTex.OnUpdate += UpdateFrame;
        }

        /// <summary> Updates the frame described by frame. </summary>
        /// <param name="frame"> The frame.</param>
        private void UpdateFrame(CameraTextureFrame frame)
        {
            OnUpdate?.Invoke(frame);
            m_IsFrameReady = true;
        }

        /// <summary> Gets frame information. </summary>
        /// <returns> The frame information. </returns>
        public override Resolution GetFrameInfo()
        {
            Resolution resolution = new Resolution();
            resolution.width = m_RGBTex.Width;
            resolution.height = m_RGBTex.Height;
            return resolution;
        }

        /// <summary> Plays this object. </summary>
        public override void Play()
        {
            m_RGBTex.Play();
        }

        /// <summary> Stops this object. </summary>
        public override void Stop()
        {
            m_RGBTex.Pause();
        }

        /// <summary> Releases this object. </summary>
        public override void Release()
        {
            m_RGBTex.Stop();
        }
    }
}
