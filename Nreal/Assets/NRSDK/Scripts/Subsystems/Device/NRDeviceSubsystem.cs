/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

using AOT;
using System;
using UnityEngine;
using static NRKernal.NRDevice;

namespace NRKernal
{
    /// <summary> Glasses event. </summary>
    /// <param name="eventtype"> The eventtype.</param>
    public delegate void GlassesEvent(NRDevice.GlassesEventType eventtype);
    /// <summary> Glasses disconnect event. </summary>
    /// <param name="reason"> The reason.</param>
    public delegate void GlassesDisconnectEvent(GlassesDisconnectReason reason);
    /// <summary> Glassed temporary level changed. </summary>
    /// <param name="level"> The level.</param>
    public delegate void GlassedTempLevelChanged(GlassesTemperatureLevel level);

    public class NRDeviceSubsystemDescriptor : IntegratedSubsystemDescriptor<NRDeviceSubsystem>
    {
        public const string Name = "Subsystem.HMD";
        public override string id => Name;
    }

    public class NRDeviceSubsystem : IntegratedSubsystem<NRDeviceSubsystemDescriptor>
    {
        public static event GlassesEvent OnGlassesStateChanged;
        public static event GlassesDisconnectEvent OnGlassesDisconnect;
        private NativeHMD m_NativeHMD = null;
        private NativeGlassesController m_NativeGlassesController = null;
        private Exception m_InitException = null;
        private static bool m_IsGlassesPlugOut = false;

        public UInt64 NativeGlassesHandler => m_NativeGlassesController.GlassesControllerHandle;
        public UInt64 NativeHMDHandler => m_NativeHMD.HmdHandle;
        public bool IsAvailable => !m_IsGlassesPlugOut && running && m_InitException == null;

        public NRDeviceSubsystem(NRDeviceSubsystemDescriptor descriptor) : base(descriptor)
        {
            m_NativeGlassesController = new NativeGlassesController();
            m_NativeHMD = new NativeHMD();
        }

        #region LifeCycle
        public override void Start()
        {
            if (running)
            {
                return;
            }

            base.Start();

#if !UNITY_EDITOR
            try
            {
                m_NativeGlassesController.Create();
                m_NativeGlassesController.RegisGlassesWearCallBack(OnGlassesWear, 1);
                m_NativeGlassesController.RegistGlassesEventCallBack(OnGlassesDisconnectEvent);
                m_NativeGlassesController.Start();
                m_NativeHMD.Create();
            }
            catch (Exception e)
            {
                m_InitException = e;
                throw e;
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
            MainThreadDispather.QueueOnMainThread(() =>
            {
                OnGlassesStateChanged?.Invoke(wearing_status == 1 ? GlassesEventType.PutOn : GlassesEventType.PutOff);
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
            OnGlassesDisconnect?.Invoke(reason);
        }

        public void RegestEvents(GlassesEvent onGlassesWear, GlassesDisconnectEvent onGlassesDisconnectEvent)
        {
            OnGlassesStateChanged += onGlassesWear;
            OnGlassesDisconnect += onGlassesDisconnectEvent;
        }

        public override void Pause()
        {
            if (!running)
            {
                return;
            }

            base.Pause();

#if !UNITY_EDITOR
           m_NativeGlassesController?.Pause();
           m_NativeHMD?.Pause();
#endif
        }

        public override void Resume()
        {
            if (running)
            {
                return;
            }

            base.Resume();
#if !UNITY_EDITOR
            m_NativeGlassesController?.Resume();
            m_NativeHMD?.Resume();
#endif
        }

        public override void Stop()
        {
            if (!running)
            {
                return;
            }

            base.Stop();

#if !UNITY_EDITOR
            m_NativeGlassesController?.Stop();
            m_NativeGlassesController?.Destroy();
#endif
        }
        #endregion

        #region Glasses
        /// <summary> Gets the temperature level. </summary>
        /// <value> The temperature level. </value>
        public GlassesTemperatureLevel TemperatureLevel
        {
            get
            {
                if (!IsAvailable)
                {
                    throw new NRGlassesConnectError("Device is not available.");
                }
#if !UNITY_EDITOR
                return m_NativeGlassesController.GetTempratureLevel();
#else
                return GlassesTemperatureLevel.TEMPERATURE_LEVEL_NORMAL;
#endif
            }
        }
        #endregion

        #region HMD
        public bool IsFeatureSupported(NRSupportedFeature feature)
        {
            if (!IsAvailable)
            {
                throw new NRGlassesConnectError("Device is not available.");
            }

#if !UNITY_EDITOR
            return m_NativeHMD.IsFeatureSupported(feature);
#else
            return true;
#endif
        }

        public NativeResolution GetDeviceResolution(NativeDevice device)
        {
            if (!IsAvailable)
            {
                throw new NRGlassesConnectError("Device is not available.");
            }
#if !UNITY_EDITOR
            return m_NativeHMD.GetEyeResolution((int)device);
#else
            return new NativeResolution(1920, 1080);
#endif
        }

        public NRDistortionParams GetDeviceDistortion(NativeDevice device)
        {
            if (!IsAvailable)
            {
                throw new NRGlassesConnectError("Device is not available.");
            }
            NRDistortionParams result = new NRDistortionParams();
#if !UNITY_EDITOR
            m_NativeHMD.GetCameraDistortion((int)device, ref result);
#endif
            return result;
        }

        public NativeMat3f GetDeviceIntrinsicMatrix(NativeDevice device)
        {
            if (!IsAvailable)
            {
                throw new NRGlassesConnectError("Device is not available.");
            }
            NativeMat3f result = new NativeMat3f();
#if !UNITY_EDITOR
             m_NativeHMD.GetCameraIntrinsicMatrix((int)device, ref result);
#endif
            return result;
        }

        public EyeProjectMatrixData GetEyeProjectMatrix(out bool result, float znear, float zfar)
        {
            if (!IsAvailable)
            {
                throw new NRGlassesConnectError("Device is not available.");
            }
            result = false;
            EyeProjectMatrixData m_EyeProjectMatrix = new EyeProjectMatrixData();
#if !UNITY_EDITOR
            result = m_NativeHMD.GetProjectionMatrix(ref m_EyeProjectMatrix, znear, zfar);
#endif
            return m_EyeProjectMatrix;
        }

        public Pose GetDevicePoseFromHead(NativeDevice device)
        {
            if (!IsAvailable)
            {
                throw new NRGlassesConnectError("Device is not available.");
            }
#if !UNITY_EDITOR
            return m_NativeHMD.GetDevicePoseFromHead(device);
#else
            return Pose.identity;
#endif

        }
        #endregion
    }
}