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
    using System;
    using AOT;
    using System.Runtime.InteropServices;

    /// <summary> A video encoder. </summary>
    public class VideoEncoder : IEncoder
    {
#if !UNITY_EDITOR
        private const int STARTENCODEEVENT = 0x1001;
        private const int STOPENCODEEVENT = 0x1002;
        public static NativeEncoder NativeEncoder { get; set; }
        private delegate void RenderEventDelegate(int eventID);
        private static RenderEventDelegate RenderThreadHandle = new RenderEventDelegate(RunOnRenderThread);
        private static IntPtr RenderThreadHandlePtr = Marshal.GetFunctionPointerForDelegate(RenderThreadHandle);
#endif

        private AudioRecordTool m_AudioEncodeTool;
        public NativeEncodeConfig EncodeConfig;
        private IntPtr m_TexPtr = IntPtr.Zero;
        private byte[] m_AudioRawData;
        private bool m_IsStarted = false;

        /// <summary> Default constructor. </summary>
        public VideoEncoder()
        {
#if !UNITY_EDITOR
            NativeEncoder = new NativeEncoder();
            NativeEncoder.Create();
#endif
        }

#if !UNITY_EDITOR
        [MonoPInvokeCallback(typeof(RenderEventDelegate))]
        private static void RunOnRenderThread(int eventID)
        {
            if (eventID == STARTENCODEEVENT)
            {
                NativeEncoder.Start();
            }
            if (eventID == STOPENCODEEVENT)
            {
                NativeEncoder.Stop();
            }
        }
#endif

        /// <summary> Configurations the given parameter. </summary>
        /// <param name="param"> The parameter.</param>
        public void Config(CameraParameters param)
        {
            EncodeConfig = new NativeEncodeConfig(param);
        }

        /// <summary> Starts this object. </summary>
        public void Start()
        {
            if (m_IsStarted)
            {
                return;
            }

            if (EncodeConfig.audioUseExternalData)
            {
                InitAudioEncodeTool();
                m_AudioEncodeTool?.StartRecord();
            }

#if !UNITY_EDITOR
            NativeEncoder.SetConfigration(EncodeConfig);
            GL.IssuePluginEvent(RenderThreadHandlePtr, STARTENCODEEVENT);
#endif
            NRDebugger.Info("[VideoEncoder] Start");
            m_IsStarted = true;
        }

        private void InitAudioEncodeTool()
        {
            AudioListener audioListener = null;
            if (NRSessionManager.Instance.NRHMDPoseTracker != null)
            {
                audioListener = NRSessionManager.Instance.NRHMDPoseTracker.centerCamera.gameObject.GetComponent<AudioListener>();
            }
            else if (GameObject.FindObjectOfType<AudioListener>() != null)
            {
                audioListener = GameObject.FindObjectOfType<AudioListener>();
            }

            if (audioListener != null)
            {
                m_AudioEncodeTool = audioListener.gameObject.GetComponent<AudioRecordTool>();
                if (m_AudioEncodeTool == null)
                {
                    m_AudioEncodeTool = audioListener.gameObject.AddComponent<AudioRecordTool>();
                }
            }
            else
            {
                throw (new MissingComponentException("Can not find a 'AudioListener' in current scene."));
            }
        }

        /// <summary> Commits. </summary>
        /// <param name="rt">        The right.</param>
        /// <param name="timestamp"> The timestamp.</param>
        public void Commit(RenderTexture rt, UInt64 timestamp)
        {
            if (!m_IsStarted)
            {
                return;
            }
            if (m_TexPtr == IntPtr.Zero)
            {
                m_TexPtr = rt.GetNativeTexturePtr();
            }

#if !UNITY_EDITOR
            NativeEncoder.UpdateSurface(m_TexPtr, timestamp);

            if (m_AudioEncodeTool != null)
            {
                bool result = m_AudioEncodeTool.Flush(ref m_AudioRawData);
                if (result)
                {
                    NativeEncoder.UpdateAudioData(m_AudioRawData, m_AudioEncodeTool.SampleRate,2,1);
                }
            }
#endif
        }

        /// <summary> Stops this object. </summary>
        public void Stop()
        {
            if (!m_IsStarted)
            {
                return;
            }

            m_AudioEncodeTool?.StopRecord();
#if !UNITY_EDITOR
            GL.IssuePluginEvent(RenderThreadHandlePtr, STOPENCODEEVENT);
#endif
            NRDebugger.Info("[VideoEncoder] Stop");
            m_IsStarted = false;
        }

        /// <summary> Releases this object. </summary>
        public void Release()
        {
            NRDebugger.Info("[VideoEncoder] Release...");
#if !UNITY_EDITOR
            NativeEncoder.Destroy();
#endif
            if (m_AudioEncodeTool != null)
            {
                GameObject.Destroy(m_AudioEncodeTool);
                m_AudioEncodeTool = null;
            }
        }
    }
}
