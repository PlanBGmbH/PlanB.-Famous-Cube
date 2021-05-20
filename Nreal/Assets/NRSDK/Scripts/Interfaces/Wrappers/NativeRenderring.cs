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
    using System;
    using UnityEngine;
    using System.Runtime.InteropServices;

    /// <summary>
    /// HMD Eye offset Native API .
    /// </summary>
    internal partial class NativeRenderring
    {
        private UInt64 m_RenderingHandle = 0;
        public IntPtr FrameInfoPtr;

        public NativeRenderring()
        {
            int sizeOfPerson = Marshal.SizeOf(typeof(FrameInfo));
            FrameInfoPtr = Marshal.AllocHGlobal(sizeOfPerson);
        }

        ~NativeRenderring()
        {
            Marshal.FreeHGlobal(FrameInfoPtr);
        }

        public bool Create()
        {
            var result = NativeApi.NRRenderingCreate(ref m_RenderingHandle);
            //AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            //AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            //AndroidJavaObject unityPlayerObj = activity.Get<AndroidJavaObject>("mUnityPlayer");
            //AndroidJavaObject surfaceViewObj = unityPlayerObj.Call<AndroidJavaObject>("getChildAt", 0);
            //AndroidJavaObject surfaceHolderObj = surfaceViewObj.Call<AndroidJavaObject>("getHolder");
            //AndroidJavaObject surfaceObj = surfaceHolderObj.Call<AndroidJavaObject>("getSurface");
            //var result = NativeApi.NRRenderingInitSetAndroidSurface(m_RenderingHandle, surfaceObj.GetRawObject());
            NativeErrorListener.Check(result, this, "Create");
            return result == NativeResult.Success;
        }

        public void InitColorSpace()
        {
            NativeColorSpace colorspace = QualitySettings.activeColorSpace == ColorSpace.Gamma ?
               NativeColorSpace.COLOR_SPACE_GAMMA : NativeColorSpace.COLOR_SPACE_LINEAR;
            NativeResult result = NativeApi.NRRenderingInitSetTextureColorSpace(m_RenderingHandle, colorspace);
            NativeErrorListener.Check(result, this, "InitColorSpace");
        }

        public bool Start()
        {
            var result = NativeApi.NRRenderingStart(m_RenderingHandle);
            NativeErrorListener.Check(result, this, "Start");
            return result == NativeResult.Success;
        }

        public bool Pause()
        {
            var result = NativeApi.NRRenderingPause(m_RenderingHandle);
            NativeErrorListener.Check(result, this, "Pause");
            return result == NativeResult.Success;
        }

        public bool Resume()
        {
            var result = NativeApi.NRRenderingResume(m_RenderingHandle);
            NativeErrorListener.Check(result, this, "Resume");
            return result == NativeResult.Success;
        }

        public bool DoRender(IntPtr left_eye_texture, IntPtr right_eye_texture, ref NativeMat4f head_pose, ref NativeVector3f position, ref NativeVector3f normal)
        {
#if !UNITY_EDITOR
#if NRSDK_BETA
            var result = NativeApi.NRRenderingDoRenderWithFocusPoint(m_RenderingHandle, left_eye_texture, right_eye_texture, ref head_pose,ref position,ref normal);
#else
            var result = NativeApi.NRRenderingDoRender(m_RenderingHandle, left_eye_texture, right_eye_texture, ref head_pose);
#endif
            return result == NativeResult.Success;
#else
            return true;
#endif
        }

        public bool Destroy()
        {
            NativeResult result = NativeApi.NRRenderingDestroy(m_RenderingHandle);
            NativeErrorListener.Check(result, this, "Destroy");
            return result == NativeResult.Success;
        }

        private partial struct NativeApi
        {
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRRenderingCreate(ref UInt64 out_rendering_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRRenderingInitSetTextureColorSpace(UInt64 rendering_handle, NativeColorSpace color_space);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRRenderingStart(UInt64 rendering_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRRenderingDestroy(UInt64 rendering_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRRenderingPause(UInt64 rendering_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRRenderingResume(UInt64 rendering_handle);

#if !UNITY_EDITOR
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRRenderingInitSetAndroidSurface(
                UInt64 rendering_handle, IntPtr android_surface);
#endif

#if NRSDK_BETA
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRRenderingDoRenderWithFocusPoint(UInt64 rendering_handle,
                IntPtr left_eye_texture, IntPtr right_eye_texture, ref NativeMat4f head_pose, ref NativeVector3f position, ref NativeVector3f normal);
#endif

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRRenderingDoRender(UInt64 rendering_handle,
                IntPtr left_eye_texture, IntPtr right_eye_texture, ref NativeMat4f head_pose);
        };
    }
}