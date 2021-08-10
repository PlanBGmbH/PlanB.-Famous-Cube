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
#if UNITY_ANDROID && !UNITY_EDITOR
    using System.Runtime.InteropServices;
    using System.Threading;
    using UnityEngine;
#endif


    /// <summary> Glasses event. </summary>
    /// <param name="eventtype"> The eventtype.</param>
    public delegate void GlassesEvent(NRDevice.GlassesEventType eventtype);
    /// <summary> Glasses disconnect event. </summary>
    /// <param name="reason"> The reason.</param>
    public delegate void GlassesDisconnectEvent(GlassesDisconnectReason reason);
    /// <summary> Glassed temporary level changed. </summary>
    /// <param name="level"> The level.</param>
    public delegate void GlassedTempLevelChanged(GlassesTemperatureLevel level);

    /// <summary> Manage the HMD device and quit. </summary>
    public class NRDevice : SingleTon<NRDevice>
    {
        /// <summary> Event queue for all listeners interested in OnGlassesStateChanged events. </summary>
        public static event GlassesEvent OnGlassesStateChanged;
        /// <summary> Event queue for all listeners interested in OnGlassesDisconnect events. </summary>
        public static event GlassesDisconnectEvent OnGlassesDisconnect;

        /// <summary> Values that represent glasses event types. </summary>
        public enum GlassesEventType
        {
            /// <summary> An enum constant representing the put on option. </summary>
            PutOn,
            /// <summary> An enum constant representing the put off option. </summary>
            PutOff
        }

        private const float SDK_RELEASE_TIMEOUT = 2f;
        /// <summary> The native hmd. </summary>
        private NativeHMD m_NativeHMD;
        /// <summary> Gets the native hmd. </summary>
        /// <value> The native hmd. </value>
        public NativeHMD NativeHMD
        {
            get
            {
                if (m_IsGlassesPlugOut)
                {
                    return null;
                }
                return m_NativeHMD;
            }
        }

        /// <summary> The lock. </summary>
        private readonly object m_Lock = new object();
        /// <summary> The native glasses controller. </summary>
        private NativeGlassesController m_NativeGlassesController;
        /// <summary> Gets the native glasses controller. </summary>
        /// <value> The native glasses controller. </value>
        public NativeGlassesController NativeGlassesController
        {
            get
            {
                if (m_IsGlassesPlugOut)
                {
                    return null;
                }
                return m_NativeGlassesController;
            }
        }

        /// <summary> True if is initialize, false if not. </summary>
        private bool m_IsInit = false;
        /// <summary> True if is glasses plug out, false if not. </summary>
        private static bool m_IsGlassesPlugOut = false;
        /// <summary> Gets a value indicating whether this object is glasses plug out. </summary>
        /// <value> True if this object is glasses plug out, false if not. </value>
        public bool IsGlassesPlugOut
        {
            get
            {
                return m_IsGlassesPlugOut;
            }
        }

        private Exception m_InitException = null;


#if UNITY_ANDROID && !UNITY_EDITOR
        private static AndroidJavaObject m_UnityActivity;
#endif

        /// <summary> Init HMD device. </summary>
        public void Init()
        {
            // Keep the exception state.
            if (m_InitException != null)
            {
                throw m_InitException;
            }

            if (m_IsInit || m_IsGlassesPlugOut)
            {
                return;
            }
            NRTools.Init();
            MainThreadDispather.Initialize();
#if UNITY_ANDROID && !UNITY_EDITOR
            // Init before all actions.
            AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            m_UnityActivity = cls_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            NativeApi.NRSDKInitSetAndroidActivity(m_UnityActivity.GetRawObject()); 
#endif
            try
            {
                CreateGlassesController();
                CreateHMD();
            }
            catch (Exception e)
            {
                m_InitException = e;
                throw e;
            }

            m_IsInit = true;
        }

        /// <summary> Pauses this object. </summary>
        public void Pause()
        {
            PauseGlassesController();
            PauseHMD();
        }

        /// <summary> Resumes this object. </summary>
        public void Resume()
        {
            ResumeGlassesController();
            ResumeHMD();
        }

        #region HMD
        /// <summary> Creates the hmd. </summary>
        private void CreateHMD()
        {
            if (m_IsGlassesPlugOut)
            {
                return;
            }
#if !UNITY_EDITOR
            lock (m_Lock)
            {
                m_NativeHMD = new NativeHMD();
                m_NativeHMD.Create();
            }
#endif
        }

        /// <summary> Pause hmd. </summary>
        private void PauseHMD()
        {
#if !UNITY_EDITOR
            lock (m_Lock)
            {
                m_NativeHMD?.Pause();
            }
#endif
        }

        /// <summary> Resume hmd. </summary>
        private void ResumeHMD()
        {
#if !UNITY_EDITOR
            lock (m_Lock)
            {
                m_NativeHMD?.Resume();
            }
#endif
        }

        /// <summary> Destroys the hmd. </summary>
        private void DestroyHMD()
        {
#if !UNITY_EDITOR
            lock (m_Lock)
            {
                m_NativeHMD?.Destroy();
                m_NativeHMD = null;
            }
#endif
        }

        /// <summary> Gets RGB camera resolution. </summary>
        /// <exception cref="NRGlassesConnectError"> Raised when a NR Glasses Connect error condition
        ///                                          occurs.</exception>
        /// <returns> The RGB camera resolution. </returns>
        public NativeResolution GetRGBCameraResolution()
        {
#if !UNITY_EDITOR
            if (NativeHMD == null)
            {
                throw new NRGlassesConnectError("Init hmd device faild.");
            }
            return NativeHMD.GetEyeResolution((int)NativeEye.LEFT);
#else
            return new NativeResolution(1280, 720);
#endif
        }
        #endregion

        #region Glasses Controller
        /// <summary> Gets the temperature level. </summary>
        /// <value> The temperature level. </value>
        public GlassesTemperatureLevel TemperatureLevel
        {
            get
            {
                if (m_IsGlassesPlugOut)
                {
                    return GlassesTemperatureLevel.TEMPERATURE_LEVEL_NORMAL;
                }
                this.Init();
#if !UNITY_EDITOR
                return this.NativeGlassesController.GetTempratureLevel();
#else
                return GlassesTemperatureLevel.TEMPERATURE_LEVEL_NORMAL;
#endif
            }
        }

        /// <summary> Creates glasses controller. </summary>
        private void CreateGlassesController()
        {
            if (m_IsGlassesPlugOut)
            {
                return;
            }
#if !UNITY_EDITOR
            try
            {
                lock (m_Lock)
                {
                    m_NativeGlassesController = new NativeGlassesController();
                    m_NativeGlassesController.Create();
                    m_NativeGlassesController.RegisGlassesWearCallBack(OnGlassesWear, 1);
                    m_NativeGlassesController.RegistGlassesEventCallBack(OnGlassesDisconnectEvent);
                    m_NativeGlassesController.Start();
                }
            }
            catch (Exception)
            {
                throw;
            }
#endif
        }

        /// <summary> Pause glasses controller. </summary>
        private void PauseGlassesController()
        {
#if !UNITY_EDITOR
            lock (m_Lock)
            {
                m_NativeGlassesController?.Pause();
            }
#endif
        }

        /// <summary> Resume glasses controller. </summary>
        private void ResumeGlassesController()
        {
#if !UNITY_EDITOR
            lock (m_Lock)
            {
                m_NativeGlassesController?.Resume();
            }
#endif
        }

        /// <summary> Destroys the glasses controller. </summary>
        private void DestroyGlassesController()
        {
#if !UNITY_EDITOR
            lock (m_Lock)
            {
                m_NativeGlassesController?.Stop();
                m_NativeGlassesController?.Destroy();
                m_NativeGlassesController = null;
            }
#endif
        }

        /// <summary> Executes the 'glasses wear' action. </summary>
        /// <param name="glasses_control_handle"> Handle of the glasses control.</param>
        /// <param name="wearing_status">         The wearing status.</param>
        /// <param name="user_data">              Information describing the user.</param>
        [MonoPInvokeCallback(typeof(NRGlassesControlWearCallback))]
        private static void OnGlassesWear(UInt64 glasses_control_handle, int wearing_status, UInt64 user_data)
        {
            NRDebugger.Info("[NRDevice] " + (wearing_status == 1 ? "Glasses put on" : "Glasses put off"));
            MainThreadDispather.QueueOnMainThread(() =>
            {
                OnGlassesStateChanged?.Invoke(wearing_status == 1 ? GlassesEventType.PutOn : GlassesEventType.PutOff);
                NRSessionManager.OnGlassesStateChanged?.Invoke(wearing_status == 1 ? GlassesEventType.PutOn : GlassesEventType.PutOff);
            });
        }

        /// <summary> Executes the 'glasses disconnect event' action. </summary>
        /// <exception cref="Exception"> Thrown when an exception error condition occurs.</exception>
        /// <param name="glasses_control_handle"> Handle of the glasses control.</param>
        /// <param name="user_data">              Information describing the user.</param>
        /// <param name="reason">                 The reason.</param>
        [MonoPInvokeCallback(typeof(NRGlassesControlNotifyQuitAppCallback))]
        private static void OnGlassesDisconnectEvent(UInt64 glasses_control_handle, IntPtr user_data, GlassesDisconnectReason reason)
        {
            if (m_IsGlassesPlugOut)
            {
                return;
            }
            m_IsGlassesPlugOut = true;

            NRDebugger.Info("[NRDevice] OnGlassesDisconnectEvent:" + reason.ToString());
            // If NRSDK release time out in 2 seconds, FoceKill the process.
            AsyncTaskExecuter.Instance.RunAction(() =>
            {
                try
                {
                    OnGlassesDisconnect?.Invoke(reason);
                    NRSessionManager.OnGlassesDisconnect?.Invoke(reason);
                }
                catch (Exception e)
                {
                    NRDebugger.Info("[NRDevice] Operate OnGlassesDisconnect event error:" + e.ToString());
                    throw e;
                }
                finally
                {
                    ForceKill(true);
                }
            }, () =>
            {
                NRDebugger.Error("[NRDevice] Release sdk timeout, force kill the process!!!");
                ForceKill(false);
            }, SDK_RELEASE_TIMEOUT);
        }
        #endregion

        #region Quit
        /// <summary> Quit the app. </summary>
        public static void QuitApp()
        {
            NRDebugger.Info("[NRDevice] Start To Quit Application.");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            ForceKill();
#endif
        }

        /// <summary> Force kill the app. Avoid timeout to pause UnityEngine. </summary>
        /// <exception cref="Exception"> Thrown when an exception error condition occurs.</exception>
        public static void ForceKill(bool needrelease = true)
        {
            try
            {
                ReleaseSDK();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                AndroidJNI.AttachCurrentThread();
                m_UnityActivity?.Call("finish");

                AndroidJavaClass processClass = new AndroidJavaClass("android.os.Process");
                int myPid = processClass.CallStatic<int>("myPid");
                processClass.CallStatic("killProcess", myPid);
#endif
            }
        }

        public static void ReleaseSDK()
        {
            NRDebugger.Info("[NRDevice] Start to release sdk");
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            NRSessionManager.Instance.DestroySession();
            NRDebugger.Info("[NRDevice] Release sdk end, cost:{0} ms", stopwatch.ElapsedMilliseconds);
        }

        /// <summary> Destory HMD resource. </summary>
        public void Destroy()
        {
            DestroyGlassesController();
            DestroyHMD();
        }
        #endregion

#if UNITY_ANDROID && !UNITY_EDITOR
        private struct NativeApi
        {
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRSDKInitSetAndroidActivity(IntPtr android_activity);
    }
#endif
    }
}
