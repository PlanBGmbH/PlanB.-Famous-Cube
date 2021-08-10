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
        /// <summary> True if is started, false if not. </summary>
        private bool m_IsStarted = false;

        /// <summary> The encode configuration. </summary>
        public NativeEncodeConfig EncodeConfig;

        /// <summary> The tex pointer. </summary>
        private IntPtr m_TexPtr = IntPtr.Zero;

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
            Config(new NativeEncodeConfig(param));
        }

        /// <summary> Configurations the given configuration. </summary>
        /// <param name="config"> The configuration.</param>
        public void Config(NativeEncodeConfig config)
        {
            EncodeConfig = config;
            NRDebugger.Info("[VideoEncoder] Encode record Config：" + config.ToString());
        }

        /// <summary> Starts this object. </summary>
        public void Start()
        {
            if (m_IsStarted)
            {
                return;
            }
#if !UNITY_EDITOR
            NativeEncoder.SetConfigration(EncodeConfig);
            GL.IssuePluginEvent(RenderThreadHandlePtr, STARTENCODEEVENT);
#endif
            NRDebugger.Info("[VideoEncoder] Encode record Start");
            m_IsStarted = true;
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
#endif
        }

        /// <summary> Stops this object. </summary>
        public void Stop()
        {
            if (!m_IsStarted)
            {
                return;
            }
#if !UNITY_EDITOR
            GL.IssuePluginEvent(RenderThreadHandlePtr, STOPENCODEEVENT);
#endif
            NRDebugger.Info("[VideoEncoder] Encode record Stop");
            m_IsStarted = false;
        }

        /// <summary> Releases this object. </summary>
        public void Release()
        {
            NRDebugger.Info("[VideoEncoder] Encode record Release...");
#if !UNITY_EDITOR
            NativeEncoder.Destroy();
#endif
        }
    }
}
