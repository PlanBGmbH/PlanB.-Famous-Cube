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
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

#if UNITY_EDITOR
    using UnityEditor;
#endif

    /// <summary> A nr virtual displayer. </summary>
    [HelpURL("https://developer.nreal.ai/develop/unity/customize-phone-controller")]
    [ScriptOrder(NativeConstants.NRVIRTUALDISPLAY_ORDER)]
    public class NRVirtualDisplayer : SingletonBehaviour<NRVirtualDisplayer>, ISystemButtonStateReceiver
    {
        /// <summary>
        /// Event queue for all listeners interested in onDisplayScreenChanged events. </summary>
        public static event Action<Resolution> OnDisplayScreenChangedEvent;
        /// <summary> State of the system button. </summary>
        internal static SystemInputState SystemButtonState = new SystemInputState();

        /// <summary> The camera. </summary>
        [SerializeField]
        private Camera m_UICamera;
        /// <summary> The virtual controller. </summary>
        [SerializeField]
        private MultiScreenController m_VirtualController;
        private ISystemButtonStateProvider m_ISystemButtonStateProvider;
        /// <summary> The screen resolution. </summary>
        private Vector2 m_ScreenResolution;

        /// <summary> Not supported on runtime. </summary>
        private const float ScaleFactor = 1f;
        /// <summary> The virtual display FPS. </summary>
        public static int VirtualDisplayFPS = 24;
        /// <summary> The current time. </summary>
        private float m_CurrentTime;

        public enum DisplayMode
        {
            Unity,
            AndroidNative
        }

        private DisplayMode m_DisplayMode = DisplayMode.AndroidNative;
        public static DisplayMode displayMode
        {
            get
            {
                if (Instance != null)
                {
                    return Instance.m_DisplayMode;
                }
                else
                {
                    return DisplayMode.AndroidNative;
                }
            }
        }

#if !UNITY_EDITOR
        private static IntPtr m_RenderTexturePtr;
        private delegate void RenderEventDelegate(int eventID);
        private static RenderEventDelegate RenderThreadHandle = new RenderEventDelegate(RunOnRenderThread);
        private static IntPtr RenderThreadHandlePtr = Marshal.GetFunctionPointerForDelegate(RenderThreadHandle);
#else
        /// <summary> The controller screen. </summary>
        private static RenderTexture m_ControllerScreen;
#endif
        /// <summary> Gets or sets the native multi display. </summary>
        /// <value> The native multi display. </value>
        internal static NativeMultiDisplay NativeMultiDisplay { get; private set; }
        /// <summary> Event queue for all listeners interested in OnMultiDisplayInited events. </summary>
        public event Action OnMultiDisplayInitialized;

        /// <summary> True to run in background. </summary>
        public static bool RunInBackground = true;
        /// <summary> True if is initialize, false if not. </summary>
        private bool m_IsInitialized = false;
        /// <summary> Gets or sets a value indicating whether this object is playing. </summary>
        /// <value> True if this object is playing, false if not. </value>
        public bool IsPlaying { get; private set; }

        /// <summary> Executes the 'application pause' action. </summary>
        /// <param name="pause"> True to pause.</param>
        private void OnApplicationPause(bool pause)
        {
            if (!m_IsInitialized || isDirty)
            {
                return;
            }

            if (RunInBackground)
            {
                if (pause)
                {
                    this.Pause();
                }
                else
                {
                    this.Resume();
                }
            }
            else
            {
                NRDevice.ForceKill();
            }
        }

        /// <summary> Pauses this object. </summary>
        private void Pause()
        {
            if (!m_IsInitialized || !IsPlaying)
            {
                return;
            }
#if !UNITY_EDITOR
            NativeMultiDisplay.Pause();
#endif
            IsPlaying = false;
        }

        /// <summary> Resumes this object. </summary>
        private void Resume()
        {
            if (!m_IsInitialized || IsPlaying)
            {
                return;
            }
#if !UNITY_EDITOR
            NativeMultiDisplay.Resume();
#endif
            IsPlaying = true;
        }

        /// <summary> Starts this object. </summary>
        void Start()
        {
            if (isDirty) return;

            NRDebugger.Info("[NRVirtualDisplayer] Start");
            this.CreateDisplay();
        }

        /// <summary> Initializes this object. </summary>
        private void CreateDisplay()
        {
            if (m_IsInitialized) return;

            try
            {
                NRDevice.Instance.Init();
            }
            catch (Exception e)
            {
                NRDebugger.Error("[NRVirtualDisplayer] NRDevice init error:" + e.ToString());
                throw;
            }

#if !UNITY_EDITOR && UNITY_ANDROID
            //m_RenderTexturePtr = m_ControllerScreen.GetNativeTexturePtr();
            NativeMultiDisplay = new NativeMultiDisplay();
            NativeMultiDisplay.Create();
            //NativeMultiDisplay.InitColorSpace();
            NativeMultiDisplay.ListenMainScrResolutionChanged(OnDisplayResolutionChanged);
            NativeMultiDisplay.Start();
            // Creat multiview controller.
            //GL.IssuePluginEvent(RenderThreadHandlePtr, 0);
            //LoadPhoneScreen();
            
            if (m_VirtualController == null)
            {
                var phoneScreenReplayceTool = FindObjectOfType<NRPhoneDisplayReplayceTool>();
                if (phoneScreenReplayceTool == null)
                {
                    NRDebugger.Info("[NRMultiDisplayManager] Use default phone sceen provider.");
                    this.BindVirtualDisplayProvider(new NRDefaultPhoneScreenProvider());
                }
                else
                {
                    NRDebugger.Info("[NRMultiDisplayManager] Use replayced phone sceen provider.");
                    this.BindVirtualDisplayProvider(phoneScreenReplayceTool.CreatePhoneScreenProvider());
                }
            }
            else
            {
                this.BindVirtualDisplayProvider(null);
            }
#else
            this.BindVirtualDisplayProvider(null);
#endif

            NRSessionManager.Instance.VirtualDisplayer = this;
            NRDebugger.Info("[NRVirtualDisplayer] Initialize");
            m_IsInitialized = true;
            OnMultiDisplayInitialized?.Invoke();
        }

        /// <summary> Updates this object. </summary>
        private void Update()
        {
            if (!m_IsInitialized) return;

#if UNITY_EDITOR
            UpdateEmulator();
            if (m_VirtualController)
            {
                m_VirtualController.gameObject.SetActive(NRInput.EmulateVirtualDisplayInEditor);
            }
#else
            if (m_DisplayMode == DisplayMode.Unity)
            {
                if (IsPlaying)
                {
                    m_CurrentTime += Time.deltaTime;
                }

                if (IsPlaying && m_CurrentTime > (1f / VirtualDisplayFPS))
                {
                    m_CurrentTime = 0;
                    m_UICamera?.Render();
                }
            }
#endif
        }

        /// <summary> Destories this object. </summary>
        public void Destory()
        {
#if !UNITY_EDITOR
            if (NativeMultiDisplay != null)
            {
                NativeMultiDisplay.Stop();
                NativeMultiDisplay.Destroy();
                NativeMultiDisplay = null;
            }
#else
            m_ControllerScreen?.Release();
            m_ControllerScreen = null;
#endif
            IsPlaying = false;
        }

        /// <summary>
        /// Base OnDestroy method that destroys the Singleton's unique instance. Called by Unity when
        /// destroying a MonoBehaviour. Scripts that extend Singleton should be sure to call
        /// base.OnDestroy() to ensure the underlying static Instance reference is properly cleaned up. </summary>
        new void OnDestroy()
        {
            if (isDirty) return;
            base.OnDestroy();
            this.Destory();
        }

        /// <summary> If m_VirtualController is null, use android native 
        ///           fragment as the virtual controller provider. </summary>
        private void BindVirtualDisplayProvider(NRPhoneScreenProviderBase provider)
        {
            if (m_ISystemButtonStateProvider != null)
            {
                return;
            }

            if (provider != null && m_VirtualController == null && Application.platform == RuntimePlatform.Android)
            {
                m_DisplayMode = DisplayMode.AndroidNative;
                m_ISystemButtonStateProvider = provider;
                NRDebugger.Info("[NRVirtualDisplayer] Bind android native controller");
            }
            else
            {
                m_DisplayMode = DisplayMode.Unity;
                transform.position = Vector3.one * 99999f;
                m_ISystemButtonStateProvider = m_VirtualController;
                NRDebugger.Info("[NRVirtualDisplayer] Bind unity virtual controller");
#if UNITY_EDITOR
                var canvas = transform.GetComponentInChildren<Canvas>();
                var scaler = canvas.transform.GetComponent<CanvasScaler>();
                if (scaler != null)
                {
                    scaler.enabled = false;
                }
                SetVirtualDisplayResolution();
                InitEmulator();
#else
                if (m_UICamera != null)
                {
                    this.m_UICamera.enabled = false;
                }
#endif
            }

            m_ISystemButtonStateProvider?.BindReceiver(this);
            IsPlaying = true;
        }

        public void OnDataReceived(SystemButtonState state)
        {
            lock (SystemButtonState)
            {
                state.TransformTo(SystemButtonState);
            }
        }

        /// <summary> Updates the resolution described by size. </summary>
        /// <param name="size"> The size.</param>
        public void UpdateResolution(Vector2 size)
        {
#if !UNITY_EDITOR
            //m_RenderTexturePtr = m_ControllerScreen.GetNativeTexturePtr();
            //GL.IssuePluginEvent(RenderThreadHandlePtr, 0);
#else
            NRPhoneScreen.Resolution = size;
            this.SetVirtualDisplayResolution();
            this.UpdateEmulatorScreen(size * ScaleFactor);
#endif

            var m_PointRaycaster = gameObject.GetComponentInChildren<NRMultScrPointerRaycaster>();
            if (m_PointRaycaster != null)
            {
                m_PointRaycaster.UpdateScreenSize(size * ScaleFactor);
            }

            if (m_ISystemButtonStateProvider != null && m_ISystemButtonStateProvider is NRPhoneScreenProviderBase)
            {
                ((NRPhoneScreenProviderBase)m_ISystemButtonStateProvider).ResizeView((int)size.x, (int)size.y);
            }
        }

#if UNITY_EDITOR
        /// <summary> Sets virtual display resolution. </summary>
        private void SetVirtualDisplayResolution()
        {
            m_ScreenResolution = NRPhoneScreen.Resolution;
            if (m_ControllerScreen != null)
            {
                m_ControllerScreen.Release();
            }

            m_ControllerScreen = new RenderTexture(
                (int)(m_ScreenResolution.x * ScaleFactor),
                (int)(m_ScreenResolution.y * ScaleFactor),
                24
            );
            m_UICamera.targetTexture = m_ControllerScreen;
            m_UICamera.aspect = m_ScreenResolution.x / m_ScreenResolution.y;
            m_UICamera.orthographicSize = 6;
        }
#endif

        /// <summary> Executes the 'display resolution changed' action. </summary>
        /// <param name="w"> The width.</param>
        /// <param name="h"> The height.</param>
        [MonoPInvokeCallback(typeof(NativeMultiDisplay.NRDisplayResolutionCallback))]
        public static void OnDisplayResolutionChanged(int w, int h)
        {
            NRDebugger.Info("[NRVirtualDisplayer] Display resolution changed width:{0} height:{1}", w, h);
            MainThreadDispather.QueueOnMainThread(delegate ()
            {
                NRVirtualDisplayer.Instance.UpdateResolution(new Vector2(w, h));
                OnDisplayScreenChangedEvent?.Invoke(new Resolution()
                {
                    width = w,
                    height = h
                });
            });
        }

#if !UNITY_EDITOR
        [MonoPInvokeCallback(typeof(RenderEventDelegate))]
        private static void RunOnRenderThread(int eventID)
        {
            //NativeMultiDisplay.UpdateHomeScreenTexture(m_RenderTexturePtr);
        }
#endif

#if UNITY_EDITOR
        /// <summary> The emulator touch. </summary>
        private static Vector2 m_EmulatorTouch = Vector2.zero;
        /// <summary> The emulator phone screen anchor. </summary>
        private Vector2 m_EmulatorPhoneScreenAnchor;
        /// <summary> Width of the emulator raw image. </summary>
        private float m_EmulatorRawImageWidth;
        /// <summary> Height of the emulator raw image. </summary>
        private float m_EmulatorRawImageHeight;
        /// <summary> The emulator phone raw image. </summary>
        private RawImage emulatorPhoneRawImage;

        /// <summary> Gets emulator screen touch. </summary>
        /// <returns> The emulator screen touch. </returns>
        public static Vector2 GetEmulatorScreenTouch()
        {
            return m_EmulatorTouch;
        }

        /// <summary> Initializes the emulator. </summary>
        private void InitEmulator()
        {
            GameObject emulatorVirtualController = new GameObject("NREmulatorVirtualController");
            DontDestroyOnLoad(emulatorVirtualController);
            Canvas controllerCanvas = emulatorVirtualController.AddComponent<Canvas>();
            controllerCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            GameObject rawImageObj = new GameObject("RamImage");
            rawImageObj.transform.parent = controllerCanvas.transform;
            emulatorPhoneRawImage = rawImageObj.AddComponent<RawImage>();
            emulatorPhoneRawImage.raycastTarget = false;
            UpdateEmulatorScreen(m_ScreenResolution * ScaleFactor);
        }

        /// <summary> Updates the emulator screen described by size. </summary>
        /// <param name="size"> The size.</param>
        private void UpdateEmulatorScreen(Vector2 size)
        {
            float scaleRate = 0.18f;
            m_EmulatorRawImageWidth = size.x * scaleRate;
            m_EmulatorRawImageHeight = size.y * scaleRate;
            emulatorPhoneRawImage.rectTransform.pivot = Vector2.right;
            emulatorPhoneRawImage.rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0f, 0f);
            emulatorPhoneRawImage.rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0f, 0f);
            emulatorPhoneRawImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, m_EmulatorRawImageWidth);
            emulatorPhoneRawImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, m_EmulatorRawImageHeight);
            emulatorPhoneRawImage.texture = m_ControllerScreen;

            Vector2 gameViewSize = Handles.GetMainGameViewSize();
            m_EmulatorPhoneScreenAnchor = new Vector2(gameViewSize.x - m_EmulatorRawImageWidth, 0f);
        }

        /// <summary> Updates the emulator. </summary>
        private void UpdateEmulator()
        {
            if (NRInput.EmulateVirtualDisplayInEditor
                && Input.GetMouseButton(0)
                && Input.mousePosition.x > m_EmulatorPhoneScreenAnchor.x
                && Input.mousePosition.y < (m_EmulatorPhoneScreenAnchor.y + m_EmulatorRawImageHeight))
            {
                m_EmulatorTouch.x = (Input.mousePosition.x - m_EmulatorPhoneScreenAnchor.x) / m_EmulatorRawImageWidth * 2f - 1f;
                m_EmulatorTouch.y = (Input.mousePosition.y - m_EmulatorPhoneScreenAnchor.y) / m_EmulatorRawImageHeight * 2f - 1f;
            }
            else
            {
                m_EmulatorTouch = Vector2.zero;
            }
        }
#endif
    }
}
