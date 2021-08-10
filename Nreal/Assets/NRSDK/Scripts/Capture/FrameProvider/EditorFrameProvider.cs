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
    using NRKernal;
    using UnityEngine;
    using System.Collections;

    /// <summary> An editor frame provider. </summary>
    public class EditorFrameProvider : AbstractFrameProvider
    {
        /// <summary> The default texture. </summary>
        private Texture2D m_DefaultTexture;
        /// <summary> The default frame. </summary>
        private UniversalTextureFrame m_DefaultFrame;
        /// <summary> True if is play, false if not. </summary>
        private bool m_IsPlay = false;

        /// <summary> Default constructor. </summary>
        public EditorFrameProvider()
        {
            m_DefaultTexture = Resources.Load<Texture2D>("Record/Textures/captureDefault");
            m_DefaultFrame = new UniversalTextureFrame();
            m_DefaultFrame.textures = new Texture[1];
            m_DefaultFrame.textureType = TextureType.RGB;
            m_DefaultFrame.textures[0] = m_DefaultTexture;

            NRKernalUpdater.Instance.StartCoroutine(UpdateFrame());
        }

        /// <summary> Updates the frame. </summary>
        /// <returns> An IEnumerator. </returns>
        public IEnumerator UpdateFrame()
        {
            while (true)
            {
                if (m_IsPlay)
                {
                    m_DefaultFrame.timeStamp = NRTools.GetTimeStamp();
                    OnUpdate?.Invoke(m_DefaultFrame);
                    m_IsFrameReady = true;
                }
                yield return new WaitForEndOfFrame();
            }
        }

        /// <summary> Gets frame information. </summary>
        /// <returns> The frame information. </returns>
        public override Resolution GetFrameInfo()
        {
            Resolution resolution = new Resolution();
            resolution.width = m_DefaultTexture.width;
            resolution.height = m_DefaultTexture.height;
            return resolution;
        }

        /// <summary> Plays this object. </summary>
        public override void Play()
        {
            m_IsPlay = true;
        }

        /// <summary> Stops this object. </summary>
        public override void Stop()
        {
            m_IsPlay = false;
        }

        /// <summary> Releases this object. </summary>
        public override void Release()
        {
            m_IsPlay = false;
            NRKernalUpdater.Instance?.StopCoroutine(UpdateFrame());
        }
    }
}
