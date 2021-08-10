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
    using System;
    using System.Runtime.InteropServices;

    /// <summary> A native encoder. </summary>
    public class NativeEncoder
    {
        /// <summary> The nr native encode library. </summary>
        public const String NRNativeEncodeLibrary = "media_enc";
        /// <summary> Handle of the encode. </summary>
        public UInt64 EncodeHandle;

        /// <summary> Creates a new bool. </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool Create()
        {
            var result = NativeApi.HWEncoderCreate(ref EncodeHandle);
            return result == 0;
        }

        /// <summary> Starts this object. </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool Start()
        {
            var result = NativeApi.HWEncoderStart(EncodeHandle);
            NativeErrorListener.Check(result, this, "Start");
            return result == 0;
        }

        /// <summary> Sets a configration. </summary>
        /// <param name="config"> The configuration.</param>
        public void SetConfigration(NativeEncodeConfig config)
        {
            var result = NativeApi.HWEncoderSetConfigration(EncodeHandle, config.ToString());
            NativeErrorListener.Check(result, this, "SetConfigration");
        }

        /// <summary> Updates the surface. </summary>
        /// <param name="texture_id"> Identifier for the texture.</param>
        /// <param name="time_stamp"> The time stamp.</param>
        public void UpdateSurface(IntPtr texture_id, UInt64 time_stamp)
        {
            var result = NativeApi.HWEncoderUpdateSurface(EncodeHandle, texture_id, time_stamp);
            NativeErrorListener.Check(result, this, "UpdateSurface");
        }

        /// <summary> Stops this object. </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool Stop()
        {
            var result = NativeApi.HWEncoderStop(EncodeHandle);
            NativeErrorListener.Check(result, this, "Stop");
            return result == 0;
        }

        /// <summary> Destroys this object. </summary>
        public void Destroy()
        {
            var result = NativeApi.HWEncoderDestroy(EncodeHandle);
            NativeErrorListener.Check(result, this, "Destroy");
        }

        /// <summary> A native api. </summary>
        private struct NativeApi
        {
            /// <summary> Hardware encoder create. </summary>
            /// <param name="out_encoder_handle"> [in,out] Handle of the out encoder.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NRNativeEncodeLibrary)]
            public static extern NativeResult HWEncoderCreate(ref UInt64 out_encoder_handle);

            /// <summary> Hardware encoder start. </summary>
            /// <param name="encoder_handle"> Handle of the encoder.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NRNativeEncodeLibrary)]
            public static extern NativeResult HWEncoderStart(UInt64 encoder_handle);

            /// <summary> Hardware encoder set configration. </summary>
            /// <param name="encoder_handle"> Handle of the encoder.</param>
            /// <param name="config">         The configuration.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NRNativeEncodeLibrary)]
            public static extern NativeResult HWEncoderSetConfigration(UInt64 encoder_handle, string config);

            /// <summary> Hardware encoder update surface. </summary>
            /// <param name="encoder_handle"> Handle of the encoder.</param>
            /// <param name="texture_id">     Identifier for the texture.</param>
            /// <param name="time_stamp">     The time stamp.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NRNativeEncodeLibrary)]
            public static extern NativeResult HWEncoderUpdateSurface(UInt64 encoder_handle, IntPtr texture_id, UInt64 time_stamp);

            /// <summary> Hardware encoder stop. </summary>
            /// <param name="encoder_handle"> Handle of the encoder.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NRNativeEncodeLibrary)]
            public static extern NativeResult HWEncoderStop(UInt64 encoder_handle);

            /// <summary> Hardware encoder destroy. </summary>
            /// <param name="encoder_handle"> Handle of the encoder.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NRNativeEncodeLibrary)]
            public static extern NativeResult HWEncoderDestroy(UInt64 encoder_handle);
        }
    }
}