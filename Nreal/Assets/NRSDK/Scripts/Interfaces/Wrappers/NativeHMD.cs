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

    /// <summary> HMD Eye offset Native API . </summary>
    public partial class NativeHMD
    {
        /// <summary> Handle of the hmd. </summary>
        private UInt64 m_HmdHandle;
        /// <summary> Gets the handle of the hmd. </summary>
        /// <value> The hmd handle. </value>
        public UInt64 HmdHandle
        {
            get
            {
                return m_HmdHandle;
            }
        }

        /// <summary> Creates a new bool. </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool Create()
        {
            NativeResult result = NativeApi.NRHMDCreate(ref m_HmdHandle);
            NativeErrorListener.Check(result, this, "Create");
            return result == NativeResult.Success;
        }

        /// <summary> Pauses this object. </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool Pause()
        {
            NativeResult result = NativeApi.NRHMDPause(m_HmdHandle);
            NativeErrorListener.Check(result, this, "Pause");
            return result == NativeResult.Success;
        }

        /// <summary> Resumes this object. </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool Resume()
        {
            NativeResult result = NativeApi.NRHMDResume(m_HmdHandle);
            NativeErrorListener.Check(result, this, "Resume");
            return result == NativeResult.Success;
        }

        /// <summary> Gets eye pose from head. </summary>
        /// <param name="eye"> The eye.</param>
        /// <returns> The eye pose from head. </returns>
        public Pose GetEyePoseFromHead(int eye)
        {
            Pose outEyePoseFromHead = Pose.identity;
            NativeMat4f mat4f = new NativeMat4f(Matrix4x4.identity);
            NativeResult result = NativeApi.NRHMDGetEyePoseFromHead(m_HmdHandle, (int)eye, ref mat4f);
            if (result == NativeResult.Success)
            {
                ConversionUtility.ApiPoseToUnityPose(mat4f, out outEyePoseFromHead);
            }
            return outEyePoseFromHead;
        }

        /// <summary> Gets projection matrix. </summary>
        /// <param name="outEyesProjectionMatrix"> [in,out] The out eyes projection matrix.</param>
        /// <param name="znear">                   The znear.</param>
        /// <param name="zfar">                    The zfar.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool GetProjectionMatrix(ref EyeProjectMatrixData outEyesProjectionMatrix, float znear, float zfar)
        {
            NativeFov4f fov = new NativeFov4f();
            NativeResult result_left = NativeApi.NRHMDGetEyeFov(m_HmdHandle, (int)NativeEye.LEFT, ref fov);
            outEyesProjectionMatrix.LEyeMatrix = ConversionUtility.GetProjectionMatrixFromFov(fov, znear, zfar).ToUnityMat4f();
            NativeResult result_right = NativeApi.NRHMDGetEyeFov(m_HmdHandle, (int)NativeEye.RIGHT, ref fov);
            outEyesProjectionMatrix.REyeMatrix = ConversionUtility.GetProjectionMatrixFromFov(fov, znear, zfar).ToUnityMat4f();
            NativeResult result_RGB = NativeApi.NRHMDGetEyeFov(m_HmdHandle, (int)NativeEye.RGB, ref fov);
            outEyesProjectionMatrix.RGBEyeMatrix = ConversionUtility.GetProjectionMatrixFromFov(fov, znear, zfar).ToUnityMat4f();
            return (result_left == NativeResult.Success && result_right == NativeResult.Success && result_RGB == NativeResult.Success);
        }

        /// <summary> Gets camera intrinsic matrix. </summary>
        /// <param name="eye">                  The eye.</param>
        /// <param name="CameraIntrinsicMatix"> [in,out] The camera intrinsic matix.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool GetCameraIntrinsicMatrix(int eye, ref NativeMat3f CameraIntrinsicMatix)
        {
            //if (eye != NativeEye.RGB)
            //{
            //    NRDebugger.Error("[NativeHMD] Only for rgb camera now. Not support this camera:" + eye.ToString());
            //    return false;
            //}
            var result = NativeApi.NRHMDGetCameraIntrinsicMatrix(m_HmdHandle, (int)eye, ref CameraIntrinsicMatix);
            return true;
        }

        /// <summary> Gets camera distortion. </summary>
        /// <param name="eye">        The eye.</param>
        /// <param name="distortion"> A variable-length parameters list containing distortion.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool GetCameraDistortion(int eye, ref NRDistortionParams distortion)
        {
            //if (eye != NativeEye.RGB)
            //{
            //    NRDebugger.Error("[NativeHMD] Only for rgb camera now. Not support this camera:" + eye.ToString());
            //    return false;
            //}
            var result = NativeApi.NRHMDGetCameraDistortionParams(m_HmdHandle, eye, ref distortion);
            return true;
        }

        /// <summary> Gets eye resolution. </summary>
        /// <param name="eye"> The eye.</param>
        /// <returns> The eye resolution. </returns>
        public NativeResolution GetEyeResolution(int eye)
        {
            NativeResolution resolution = new NativeResolution(1920, 1080);
#if UNITY_EDITOR
            return resolution;
#else
            var result = NativeApi.NRHMDGetEyeResolution(m_HmdHandle, eye, ref resolution);
            NativeErrorListener.Check(result, this, "GetEyeResolution");
            return resolution;
#endif
        }

        /// <summary> Destroys this object. </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool Destroy()
        {
            NativeResult result = NativeApi.NRHMDDestroy(m_HmdHandle);
            NativeErrorListener.Check(result, this, "Destroy");
            return result == NativeResult.Success;
        }

        /// <summary> A native api. </summary>
        private struct NativeApi
        {
            /// <summary> Nrhmd create. </summary>
            /// <param name="out_hmd_handle"> [in,out] Handle of the out hmd.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRHMDCreate(ref UInt64 out_hmd_handle);

            /// <summary> Nrhmd pause. </summary>
            /// <param name="hmd_handle"> Handle of the hmd.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRHMDPause(UInt64 hmd_handle);

            /// <summary> Nrhmd resume. </summary>
            /// <param name="hmd_handle"> Handle of the hmd.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRHMDResume(UInt64 hmd_handle);

            /// <summary> Nrhmd get eye pose from head. </summary>
            /// <param name="hmd_handle">         Handle of the hmd.</param>
            /// <param name="eye">                The eye.</param>
            /// <param name="outEyePoseFromHead"> [in,out] The out eye pose from head.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRHMDGetEyePoseFromHead(UInt64 hmd_handle, int eye, ref NativeMat4f outEyePoseFromHead);

            /// <summary> Nrhmd get eye fov. </summary>
            /// <param name="hmd_handle">  Handle of the hmd.</param>
            /// <param name="eye">         The eye.</param>
            /// <param name="out_eye_fov"> [in,out] The out eye fov.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRHMDGetEyeFov(UInt64 hmd_handle, int eye, ref NativeFov4f out_eye_fov);

            /// <summary> Nrhmd get camera intrinsic matrix. </summary>
            /// <param name="hmd_handle">           Handle of the hmd.</param>
            /// <param name="eye">                  The eye.</param>
            /// <param name="out_intrinsic_matrix"> [in,out] The out intrinsic matrix.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRHMDGetCameraIntrinsicMatrix(
                    UInt64 hmd_handle, int eye, ref NativeMat3f out_intrinsic_matrix);

            /// <summary> Nrhmd get camera distortion parameters. </summary>
            /// <param name="hmd_handle"> Handle of the hmd.</param>
            /// <param name="eye">        The eye.</param>
            /// <param name="out_params"> A variable-length parameters list containing out parameters.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRHMDGetCameraDistortionParams(
                    UInt64 hmd_handle, int eye, ref NRDistortionParams out_params);

            /// <summary> Nrhmd get eye resolution. </summary>
            /// <param name="hmd_handle">         Handle of the hmd.</param>
            /// <param name="eye">                The eye.</param>
            /// <param name="out_eye_resolution"> [in,out] The out eye resolution.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRHMDGetEyeResolution(UInt64 hmd_handle, int eye, ref NativeResolution out_eye_resolution);

            /// <summary> Nrhmd destroy. </summary>
            /// <param name="hmd_handle"> Handle of the hmd.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRHMDDestroy(UInt64 hmd_handle);
        };
    }
}
