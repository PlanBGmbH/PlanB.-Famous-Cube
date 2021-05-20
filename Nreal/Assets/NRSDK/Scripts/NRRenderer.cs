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
    using AOT;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using UnityEngine;

    /// <summary>
    /// NRNativeRender operate rendering-related things, provides the feature of optimized rendering
    /// and low latency. </summary>
    [ScriptOrder(-300)]
    public class NRRenderer : MonoBehaviour
    {
        /// <summary> Renders the event delegate described by eventID. </summary>
        /// <param name="eventID"> Identifier for the event.</param>
        private delegate void RenderEventDelegate(int eventID);
        /// <summary> Handle of the render thread. </summary>
        private static RenderEventDelegate RenderThreadHandle = new RenderEventDelegate(RunOnRenderThread);
        /// <summary> The render thread handle pointer. </summary>
        private static IntPtr RenderThreadHandlePtr = Marshal.GetFunctionPointerForDelegate(RenderThreadHandle);

        private const int SETRENDERTEXTUREEVENT = 0x0001;
        private const int STARTNATIVERENDEREVENT = 0x0002;
        private const int RESUMENATIVERENDEREVENT = 0x0003;
        private const int PAUSENATIVERENDEREVENT = 0x0004;
        private const int STOPNATIVERENDEREVENT = 0x0005;

        /// <summary> Values that represent eyes. </summary>
        public enum Eyes
        {
            /// <summary> An enum constant representing the left option. </summary>
            Left = 0,
            /// <summary> An enum constant representing the right option. </summary>
            Right = 1,
            /// <summary> An enum constant representing the count option. </summary>
            Count = 2
        }

        /// <summary> The left camera. </summary>
        public Camera leftCamera;
        /// <summary> The right camera. </summary>
        public Camera rightCamera;
        /// <summary> Gets or sets the native renderring. </summary>
        /// <value> The m native renderring. </value>
        static NativeRenderring m_NativeRenderring { get; set; }

        /// <summary> The scale factor. </summary>
        public static float ScaleFactor = 1f;
        private const float m_DefaultFocusDistance = 1.4f;
        private float m_FocusDistance = 1.4f;
        /// <summary> Number of eye textures. </summary>
        private const int EyeTextureCount = 3 * (int)Eyes.Count;
        /// <summary> The eye textures. </summary>
        private readonly RenderTexture[] eyeTextures = new RenderTexture[EyeTextureCount];
        /// <summary> Dictionary of rights. </summary>
        private Dictionary<RenderTexture, IntPtr> m_RTDict = new Dictionary<RenderTexture, IntPtr>();

        /// <summary> Values that represent renderer states. </summary>
        public enum RendererState
        {
            UnInitialized,
            Initialized,
            Running,
            Paused,
            Destroyed
        }

        /// <summary> The current state. </summary>
        private RendererState m_CurrentState = RendererState.UnInitialized;
        /// <summary> Gets the current state. </summary>
        /// <value> The current state. </value>
        public RendererState currentState
        {
            get
            {
                return m_CurrentState;
            }
        }

#if !UNITY_EDITOR
        private int currentEyeTextureIdx = 0;
        private int nextEyeTextureIdx = 0;
#endif

        /// <summary> Gets a value indicating whether this object is linear color space. </summary>
        /// <value> True if this object is linear color space, false if not. </value>
        public static bool isLinearColorSpace
        {
            get
            {
                return QualitySettings.activeColorSpace == ColorSpace.Linear;
            }
        }

        /// <summary> Initialize the render pipleline. </summary>
        /// <param name="leftcamera">  Left Eye.</param>
        /// <param name="rightcamera"> Right Eye.</param>
        ///
        /// ### <param name="poseprovider"> provide the pose of camera every frame.</param>
        public void Initialize(Camera leftcamera, Camera rightcamera)
        {
            NRDebugger.Info("[NRRender] Initialize");
            if (m_CurrentState != RendererState.UnInitialized)
            {
                return;
            }

            leftCamera = leftcamera;
            rightCamera = rightcamera;

#if !UNITY_EDITOR
            leftCamera.depthTextureMode = DepthTextureMode.None;
            rightCamera.depthTextureMode = DepthTextureMode.None;
            leftCamera.rect = new Rect(0, 0, 1, 1);
            rightCamera.rect = new Rect(0, 0, 1, 1);
            leftCamera.enabled = false;
            rightCamera.enabled = false;
            m_CurrentState = RendererState.Initialized;
            StartCoroutine(StartUp());
#endif
        }

        /// <summary> Prepares this object for use. </summary>
        /// <returns> An IEnumerator. </returns>
        private IEnumerator StartUp()
        {
            var virtualDisplay = GameObject.FindObjectOfType<NRVirtualDisplayer>();
            while (virtualDisplay == null || !virtualDisplay.IsPlaying)
            {
                NRDebugger.Info("[NRRender] Wait virtual display ready...");
                yield return new WaitForEndOfFrame();
                if (virtualDisplay == null)
                {
                    virtualDisplay = GameObject.FindObjectOfType<NRVirtualDisplayer>();
                }
            }

            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            NRDebugger.Info("[NRRender] StartUp");
            CreateRenderTextures();
#if !UNITY_EDITOR
            m_NativeRenderring = new NativeRenderring();
            m_NativeRenderring.Create();
#if !UNITY_STANDALONE_WIN
            m_NativeRenderring.InitColorSpace();
#endif
            StartCoroutine(RenderCoroutine());
#endif
            m_CurrentState = RendererState.Running;
            GL.IssuePluginEvent(RenderThreadHandlePtr, STARTNATIVERENDEREVENT);
        }

        /// <summary> Pause render. </summary>
        public void Pause()
        {
            NRDebugger.Info("[NRRender] Pause");
            if (m_CurrentState != RendererState.Running)
            {
                return;
            }
            m_CurrentState = RendererState.Paused;
            GL.IssuePluginEvent(RenderThreadHandlePtr, PAUSENATIVERENDEREVENT);
        }

        /// <summary> Resume render. </summary>
        public void Resume()
        {
            Invoke("DelayResume", 0.3f);
        }

        /// <summary> Delay resume. </summary>
        private void DelayResume()
        {
            NRDebugger.Info("[NRRender] Resume");
            if (m_CurrentState != RendererState.Paused)
            {
                return;
            }
            m_CurrentState = RendererState.Running;
            GL.IssuePluginEvent(RenderThreadHandlePtr, RESUMENATIVERENDEREVENT);
        }

#if !UNITY_EDITOR
        void Update()
        {
            if (m_CurrentState == RendererState.Running)
            {
                leftCamera.targetTexture = eyeTextures[currentEyeTextureIdx];
                rightCamera.targetTexture = eyeTextures[currentEyeTextureIdx + 1];
                currentEyeTextureIdx = nextEyeTextureIdx;
                nextEyeTextureIdx = (nextEyeTextureIdx + 2) % EyeTextureCount;
                leftCamera.enabled = true;
                rightCamera.enabled = true;
            }
        }
#endif

        /// <summary> Generates a render texture. </summary>
        /// <param name="width">  The width.</param>
        /// <param name="height"> The height.</param>
        /// <returns> The render texture. </returns>
        private RenderTexture GenRenderTexture(int width, int height)
        {
            RenderTexture renderTexture = new RenderTexture((int)(width * ScaleFactor), (int)(height * ScaleFactor), 24, RenderTextureFormat.Default);
            renderTexture.wrapMode = TextureWrapMode.Clamp;
            if (QualitySettings.antiAliasing > 0)
            {
                renderTexture.antiAliasing = QualitySettings.antiAliasing;
            }
            renderTexture.Create();

            return renderTexture;
        }

        /// <summary> Creates render textures. </summary>
        private void CreateRenderTextures()
        {
            var resolution = NRDevice.Instance.NativeHMD.GetEyeResolution((int)NativeEye.LEFT);
            NRDebugger.Info("[CreateRenderTextures]  resolution :" + resolution.ToString());

            for (int i = 0; i < EyeTextureCount; i++)
            {
                eyeTextures[i] = GenRenderTexture(resolution.width, resolution.height);
                m_RTDict.Add(eyeTextures[i], eyeTextures[i].GetNativeTexturePtr());
            }
        }

        /// <summary> Renders the coroutine. </summary>
        /// <returns> An IEnumerator. </returns>
        private IEnumerator RenderCoroutine()
        {
            WaitForEndOfFrame delay = new WaitForEndOfFrame();
            yield return delay;

            while (true)
            {
                yield return delay;

                if (m_CurrentState != RendererState.Running)
                {
                    continue;
                }

                NativeMat4f apiPose;
                Pose unityPose = NRFrame.HeadPose;
                ConversionUtility.UnityPoseToApiPose(unityPose, out apiPose);
                IntPtr left_target, right_target;
                if (!m_RTDict.TryGetValue(leftCamera.targetTexture, out left_target)) continue;
                if (!m_RTDict.TryGetValue(rightCamera.targetTexture, out right_target)) continue;
                FrameInfo info = new FrameInfo(left_target, right_target, apiPose, new Vector3(0, 0, -m_FocusDistance), Vector3.forward);
                SetRenderFrameInfo(info);
                // reset focuse distance to default value every frame.
                m_FocusDistance = m_DefaultFocusDistance;
            }
        }


        /// <summary> Sets the focus plane for render thread. </summary>
        /// <param name="distance"> The distance from plane to center camera.</param>
        [Conditional("NRSDK_BETA")]
        public void SetFocusDistance(float distance)
        {
            m_FocusDistance = distance;
        }

        /// <summary> Executes the 'on render thread' operation. </summary>
        /// <param name="eventID"> Identifier for the event.</param>
        [MonoPInvokeCallback(typeof(RenderEventDelegate))]
        private static void RunOnRenderThread(int eventID)
        {
            if (eventID == STARTNATIVERENDEREVENT)
            {
                m_NativeRenderring?.Start();
            }
            else if (eventID == RESUMENATIVERENDEREVENT)
            {
                m_NativeRenderring?.Resume();
            }
            else if (eventID == PAUSENATIVERENDEREVENT)
            {
                m_NativeRenderring?.Pause();
            }
            else if (eventID == STOPNATIVERENDEREVENT)
            {
                m_NativeRenderring?.Destroy();
                m_NativeRenderring = null;
                NRDevice.Instance.Destroy();
            }
            else if (eventID == SETRENDERTEXTUREEVENT)
            {
                FrameInfo framinfo = (FrameInfo)Marshal.PtrToStructure(m_NativeRenderring.FrameInfoPtr, typeof(FrameInfo));
                m_NativeRenderring?.DoRender(framinfo.leftTex, framinfo.rightTex, ref framinfo.pose, ref framinfo.focusPosition, ref framinfo.normalPosition);
            }
        }

        /// <summary> Sets render frame information. </summary>
        /// <param name="frame"> The frame.</param>
        private static void SetRenderFrameInfo(FrameInfo frame)
        {
            Marshal.StructureToPtr(frame, m_NativeRenderring.FrameInfoPtr, true);
            GL.IssuePluginEvent(RenderThreadHandlePtr, SETRENDERTEXTUREEVENT);
        }

        public void Destroy()
        {
            if (m_CurrentState == RendererState.Destroyed)
            {
                return;
            }
            m_CurrentState = RendererState.Destroyed;
            //GL.IssuePluginEvent(RenderThreadHandlePtr, STOPNATIVERENDEREVENT);

            m_NativeRenderring?.Destroy();
            m_NativeRenderring = null;
        }

        private void OnDestroy()
        {
            this.Destroy();
        }
    }
}
