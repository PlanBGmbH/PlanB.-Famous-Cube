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
        public const string NRNativeEncodeLibrary = "media_enc";
        public UInt64 EncodeHandle;

        public bool Create()
        {
            var result = NativeApi.HWEncoderCreate(ref EncodeHandle);
            return result == 0;
        }

        public bool Start()
        {
            var result = NativeApi.HWEncoderStart(EncodeHandle);
            NativeErrorListener.Check(result, this, "Start");
            return result == 0;
        }

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
            NativeApi.HWEncoderUpdateSurface(EncodeHandle, texture_id, time_stamp);
        }

        public void UpdateAudioData(byte[] audioData, int samplerate, int bytePerSample, int channel)
        {
            //NRDebugger.Info("[NativeEncode] UpdateAudioData, audioData len:{0} samplerate:{1} bytePerSample:{2} channel:{3}", audioData.Length, samplerate, bytePerSample, channel);
            NativeApi.HWEncoderNotifyAudioData(EncodeHandle, audioData, audioData.Length / bytePerSample, bytePerSample, channel, samplerate, 1);
        }

        public bool Stop()
        {
            var result = NativeApi.HWEncoderStop(EncodeHandle);
            NativeErrorListener.Check(result, this, "Stop");
            return result == 0;
        }

        public void Destroy()
        {
            var result = NativeApi.HWEncoderDestroy(EncodeHandle);
            NativeErrorListener.Check(result, this, "Destroy");
        }

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

            [DllImport(NRNativeEncodeLibrary)]
            public static extern NativeResult HWEncoderNotifyAudioData(UInt64 encoder, byte[] audioSamples, int nSamples,
                             int nBytesPerSample, int nChannels, int samples_per_sec, int sample_fmt); //sample_fmt :0:s16, 8 float

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