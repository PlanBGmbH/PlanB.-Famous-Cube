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
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary> A frame capture context. </summary>
    public class FrameCaptureContext
    {
        /// <summary> The blender. </summary>
        private FrameBlender m_Blender;
        /// <summary> The encoder. </summary>
        private IEncoder m_Encoder;
        /// <summary> Options for controlling the camera. </summary>
        private CameraParameters m_CameraParameters;
        /// <summary> The frame provider. </summary>
        private AbstractFrameProvider m_FrameProvider;
        /// <summary> The capture behaviour. </summary>
        private CaptureBehaviourBase m_CaptureBehaviour;
        /// <summary> True if is initialize, false if not. </summary>
        private bool m_IsInit = false;

        private List<IFrameConsumer> m_FrameConsumerList;

        /// <summary> Gets the preview texture. </summary>
        /// <value> The preview texture. </value>
        public Texture PreviewTexture
        {
            get
            {
                return m_Blender?.BlendTexture;
            }
        }

        /// <summary> Gets the behaviour. </summary>
        /// <returns> The behaviour. </returns>
        public CaptureBehaviourBase GetBehaviour()
        {
            return m_CaptureBehaviour;
        }

        /// <summary> Gets frame provider. </summary>
        /// <returns> The frame provider. </returns>
        public AbstractFrameProvider GetFrameProvider()
        {
            return m_FrameProvider;
        }

        /// <summary> Gets the blender. </summary>
        /// <returns> The blender. </returns>
        public FrameBlender GetBlender()
        {
            return m_Blender;
        }

        /// <summary> Request camera parameter. </summary>
        /// <returns> The CameraParameters. </returns>
        public CameraParameters RequestCameraParam()
        {
            return m_CameraParameters;
        }

        /// <summary> Gets the encoder. </summary>
        /// <returns> The encoder. </returns>
        public IEncoder GetEncoder()
        {
            return m_Encoder;
        }

        /// <summary> Constructor. </summary>
        /// <param name="provider"> The provider.</param>
        public FrameCaptureContext(AbstractFrameProvider provider)
        {
            this.m_FrameProvider = provider;
        }

        /// <summary> Starts capture mode. </summary>
        /// <param name="param"> The parameter.</param>
        public void StartCaptureMode(CameraParameters param)
        {
            if (m_IsInit) return;

            NRDebugger.Info("[CaptureContext] Create...");
            if (m_CaptureBehaviour == null)
            {
                this.m_CaptureBehaviour = this.GetCaptureBehaviourByMode(param.camMode);
            }

            this.m_CameraParameters = param;
            this.m_Encoder = GetEncoderByMode(param.camMode);
            this.m_Encoder.Config(param);
            this.m_Blender = new FrameBlender();
            this.m_Blender.Init(m_CaptureBehaviour.CaptureCamera, m_Encoder, param);
            this.m_CaptureBehaviour.Init(this);
            this.m_FrameProvider.OnUpdate += UpdateFrame;

            this.m_FrameConsumerList = new List<IFrameConsumer>();
            this.Sequence(m_CaptureBehaviour)
                .Sequence(m_Blender);

            this.m_IsInit = true;
        }

        private FrameCaptureContext Sequence(IFrameConsumer consummer)
        {
            this.m_FrameConsumerList.Add(consummer);
            return this;
        }

        private void UpdateFrame(UniversalTextureFrame frame)
        {
            for (int i = 0; i < m_FrameConsumerList.Count; i++)
            {
                m_FrameConsumerList[i].OnFrame(frame);
            }
        }

        /// <summary> Gets capture behaviour by mode. </summary>
        /// <exception cref="Exception"> Thrown when an exception error condition occurs.</exception>
        /// <param name="mode"> The mode.</param>
        /// <returns> The capture behaviour by mode. </returns>
        private CaptureBehaviourBase GetCaptureBehaviourByMode(CamMode mode)
        {
            if (mode == CamMode.PhotoMode)
            {
                NRCaptureBehaviour capture = GameObject.FindObjectOfType<NRCaptureBehaviour>();
                var headParent = NRSessionManager.Instance.NRSessionBehaviour.transform.parent;
                if (capture == null)
                {
                    capture = GameObject.Instantiate(Resources.Load<NRCaptureBehaviour>("Record/Prefabs/NRCaptureBehaviour"), headParent);
                }
                GameObject.DontDestroyOnLoad(capture.gameObject);
                return capture;
            }
            else if (mode == CamMode.VideoMode)
            {
                NRRecordBehaviour capture = GameObject.FindObjectOfType<NRRecordBehaviour>();
                var headParent = NRSessionManager.Instance.NRSessionBehaviour.transform.parent;
                if (capture == null)
                {
                    capture = GameObject.Instantiate(Resources.Load<NRRecordBehaviour>("Record/Prefabs/NRRecorderBehaviour"), headParent);
                }
                GameObject.DontDestroyOnLoad(capture.gameObject);
                return capture;
            }
            else
            {
                throw new Exception("CamMode need to be set correctly for capture behaviour!");
            }
        }

        /// <summary> Gets encoder by mode. </summary>
        /// <exception cref="Exception"> Thrown when an exception error condition occurs.</exception>
        /// <param name="mode"> The mode.</param>
        /// <returns> The encoder by mode. </returns>
        private IEncoder GetEncoderByMode(CamMode mode)
        {
            if (mode == CamMode.PhotoMode)
            {
                return new ImageEncoder();
            }
            else if (mode == CamMode.VideoMode)
            {
                return new VideoEncoder();
            }
            else
            {
                throw new Exception("CamMode need to be set correctly for encoder!");
            }
        }

        /// <summary> Stops capture mode. </summary>
        public void StopCaptureMode()
        {
            this.Release();
        }

        /// <summary> Starts a capture. </summary>
        public void StartCapture()
        {
            if (!m_IsInit)
            {
                return;
            }
            NRDebugger.Info("[CaptureContext] Start...");

            m_Encoder?.Start();
            m_FrameProvider?.Play();
        }

        /// <summary> Stops a capture. </summary>
        public void StopCapture()
        {
            if (!m_IsInit)
            {
                return;
            }
            NRDebugger.Info("[CaptureContext] Stop...");

            // Need stop encoder firstly.
            m_Encoder?.Stop();
            m_FrameProvider?.Stop();
        }

        /// <summary> Releases this object. </summary>
        public void Release()
        {
            if (!m_IsInit)
            {
                return;
            }
            NRDebugger.Info("[CaptureContext] Release...");
            if (m_FrameProvider != null)
            {
                m_FrameProvider.OnUpdate -= this.m_CaptureBehaviour.OnFrame;
                m_FrameProvider?.Release();
            }

            m_Blender?.Dispose();
            m_Encoder?.Release();

            if (m_CaptureBehaviour != null)
            {
                GameObject.Destroy(m_CaptureBehaviour.gameObject);
                m_CaptureBehaviour = null;
            }

            m_IsInit = false;
        }
    }
}
