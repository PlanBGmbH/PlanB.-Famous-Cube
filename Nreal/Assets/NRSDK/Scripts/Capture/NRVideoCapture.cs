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
    using NRKernal;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Records a video from the MR images directly to disk. MR images comes from rgb camera or rgb
    /// camera image and virtual image blending. The final video recording will be stored on the file
    /// system in the MP4 format. </summary>
    public class NRVideoCapture : IDisposable
    {
        /// <summary> Default constructor. </summary>
        public NRVideoCapture()
        {
            IsRecording = false;
        }

        /// <summary> Finalizer. </summary>
        ~NRVideoCapture()
        {

        }

        /// <summary> The supported resolutions. </summary>
        private static IEnumerable<Resolution> m_SupportedResolutions = null;

        /// <summary> A list of all the supported device resolutions for recording videos. </summary>
        /// <value> The supported resolutions. </value>
        public static IEnumerable<Resolution> SupportedResolutions
        {
            get
            {
                if (m_SupportedResolutions == null)
                {
                    var resolutions = new List<Resolution>();

#if !UNITY_EDITOR
                    NativeResolution rgbResolution = NRDevice.Instance.NativeHMD.GetEyeResolution((int)NativeEye.RGB);
#else
                    NativeResolution rgbResolution = new NativeResolution(1280, 720);
#endif
                    Resolution stand_resolution = new Resolution()
                    {
                        width = rgbResolution.width,
                        height = rgbResolution.height,
                        refreshRate = 30,
                    };
                    Resolution low_resolution = new Resolution()
                    {
                        width = stand_resolution.width / 2,
                        height = stand_resolution.height / 2,
                        refreshRate = 30,
                    };
                    Resolution high_resolution = new Resolution()
                    {
                        width = stand_resolution.width * 3 / 2,
                        height = stand_resolution.height * 3 / 2,
                        refreshRate = 30,
                    };
                    resolutions.Add(stand_resolution);
                    resolutions.Add(high_resolution);
                    resolutions.Add(low_resolution);
                    m_SupportedResolutions = resolutions;
                }

                return m_SupportedResolutions;
            }
        }

        /// <summary>
        /// Indicates whether or not the VideoCapture instance is currently recording video. </summary>
        /// <value> True if this object is recording, false if not. </value>
        public bool IsRecording { get; private set; }

        /// <summary> Context for the capture. </summary>
        private FrameCaptureContext m_CaptureContext;

        /// <summary> Gets the context. </summary>
        /// <returns> The context. </returns>
        public FrameCaptureContext GetContext()
        {
            return m_CaptureContext;
        }

        /// <summary> Gets the preview texture. </summary>
        /// <value> The preview texture. </value>
        public Texture PreviewTexture
        {
            get
            {
                return m_CaptureContext?.PreviewTexture;
            }
        }

        /// <summary> Creates an asynchronous. </summary>
        /// <param name="showHolograms">     True to show, false to hide the holograms.</param>
        /// <param name="onCreatedCallback"> The on created callback.</param>
        public static void CreateAsync(bool showHolograms, OnVideoCaptureResourceCreatedCallback onCreatedCallback)
        {
            NRVideoCapture capture = new NRVideoCapture();
            capture.m_CaptureContext = FrameCaptureContextFactory.Create();
            onCreatedCallback?.Invoke(capture);
        }

        /// <summary>
        /// Returns the supported frame rates at which a video can be recorded given a resolution. </summary>
        /// <param name="resolution"> A recording resolution.</param>
        /// <returns> The frame rates at which the video can be recorded. </returns>
        public static IEnumerable<int> GetSupportedFrameRatesForResolution(Resolution resolution)
        {
            List<int> frameRates = new List<int>();
            frameRates.Add(30);
            return frameRates;
        }

        /// <summary> Dispose must be called to shutdown the PhotoCapture instance. </summary>
        public void Dispose()
        {
            if (m_CaptureContext != null)
            {
                m_CaptureContext.Release();
                m_CaptureContext = null;
            }
        }

        /// <summary> Starts recording asynchronous. </summary>
        /// <param name="filename">                        Filename of the file.</param>
        /// <param name="onStartedRecordingVideoCallback"> The on started recording video callback.</param>
        public void StartRecordingAsync(string filename, OnStartedRecordingVideoCallback onStartedRecordingVideoCallback)
        {
            var result = new VideoCaptureResult();
            if (IsRecording)
            {
                result.resultType = CaptureResultType.UnknownError;
                onStartedRecordingVideoCallback?.Invoke(result);
            }
            else
            {
                try
                {
                    var behaviour = m_CaptureContext.GetBehaviour();
                    ((NRRecordBehaviour)behaviour).SetOutPutPath(filename);
                    m_CaptureContext.StartCapture();
                    IsRecording = true;
                    result.resultType = CaptureResultType.Success;
                    onStartedRecordingVideoCallback?.Invoke(result);
                }
                catch (Exception)
                {
                    result.resultType = CaptureResultType.UnknownError;
                    onStartedRecordingVideoCallback?.Invoke(result);
                    throw;
                }
            }
        }

        /// <summary> Starts video mode asynchronous. </summary>
        /// <param name="setupParams">                Options for controlling the setup.</param>
        /// <param name="onVideoModeStartedCallback"> The on video mode started callback.</param>
        public void StartVideoModeAsync(CameraParameters setupParams, OnVideoModeStartedCallback onVideoModeStartedCallback)
        {
            if (setupParams.audioState == AudioState.MicAudio)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                NRAndroidPermissionsManager.GetInstance().RequestAndroidPermission("android.permission.RECORD_AUDIO").ThenAction((requestResult) =>
                {
                    if (requestResult.IsAllGranted)
                    {
                        StartVideoMode(setupParams, onVideoModeStartedCallback);
                    }
                    else {
                        NRDebugger.Error("Record audio need the permission of 'android.permission.RECORD_AUDIO'.");
                        var result = new VideoCaptureResult();
                        result.resultType = CaptureResultType.UnknownError;
                        onVideoModeStartedCallback?.Invoke(result);
                        NRSessionManager.Instance.OprateInitException(new NRPermissionDenyError(NativeConstants.AudioPermissionDenyErrorTip));
                    }
                });
#else
                StartVideoMode(setupParams, onVideoModeStartedCallback);
#endif
            }
            else
            {
                StartVideoMode(setupParams, onVideoModeStartedCallback);
            }
        }

        private void StartVideoMode(CameraParameters setupParams, OnVideoModeStartedCallback onVideoModeStartedCallback)
        {
            setupParams.camMode = CamMode.VideoMode;
            setupParams.hologramOpacity = 1;
            m_CaptureContext.StartCaptureMode(setupParams);
            var result = new VideoCaptureResult();
            result.resultType = CaptureResultType.Success;
            onVideoModeStartedCallback?.Invoke(result);
        }

        public void StartVideoModeAsync(CameraParameters setupParams, AudioState audioState, OnVideoModeStartedCallback onVideoModeStartedCallback)
        {
            setupParams.audioState = audioState;
            StartVideoModeAsync(setupParams, onVideoModeStartedCallback);
        }

        /// <summary> Stops recording asynchronous. </summary>
        /// <param name="onStoppedRecordingVideoCallback"> The on stopped recording video callback.</param>
        public void StopRecordingAsync(OnStoppedRecordingVideoCallback onStoppedRecordingVideoCallback)
        {
            var result = new VideoCaptureResult();
            if (!IsRecording)
            {
                result.resultType = CaptureResultType.UnknownError;
                onStoppedRecordingVideoCallback?.Invoke(result);
            }
            else
            {
                try
                {
                    m_CaptureContext.StopCapture();
                    IsRecording = false;
                    result.resultType = CaptureResultType.Success;
                    onStoppedRecordingVideoCallback?.Invoke(result);
                }
                catch (Exception)
                {
                    result.resultType = CaptureResultType.UnknownError;
                    onStoppedRecordingVideoCallback?.Invoke(result);
                    throw;
                }
            }
        }

        /// <summary> Stops video mode asynchronous. </summary>
        /// <param name="onVideoModeStoppedCallback"> The on video mode stopped callback.</param>
        public void StopVideoModeAsync(OnVideoModeStoppedCallback onVideoModeStoppedCallback)
        {
            m_CaptureContext.StopCaptureMode();
            var result = new VideoCaptureResult();
            result.resultType = CaptureResultType.Success;
            onVideoModeStoppedCallback?.Invoke(result);
        }

        /// <summary> Contains the result of the capture request. </summary>
        public enum CaptureResultType
        {
            /// <summary>
            /// Specifies that the desired operation was successful.
            /// </summary>
            Success = 0,

            /// <summary>
            /// Specifies that an unknown error occurred.
            /// </summary>
            UnknownError = 1
        }

        /// <summary>
        /// Specifies what audio sources should be recorded while recording the video.
        /// </summary>
        public enum AudioState
        {
            /// <summary>
            /// Only include the mic audio in the video recording.
            /// </summary>
            MicAudio = 0,

            ///// <summary>
            ///// Only include the application audio in the video recording.
            ///// </summary>
            //ApplicationAudio = 1,

            ///// <summary>
            ///// Include both the application audio as well as the mic audio in the video recording.
            ///// </summary>
            //ApplicationAndMicAudio = 2,

            /// <summary>
            /// Do not include any audio in the video recording.
            /// </summary>
            None = 3
        }

        /// <summary>
        /// A data container that contains the result information of a video recording operation. </summary>
        public struct VideoCaptureResult
        {
            /// <summary>
            /// A generic result that indicates whether or not the VideoCapture operation succeeded. </summary>
            public CaptureResultType resultType;

            /// <summary> The specific HResult value. </summary>
            public long hResult;

            /// <summary> Indicates whether or not the operation was successful. </summary>
            /// <value> True if success, false if not. </value>
            public bool success
            {
                get
                {
                    return resultType == CaptureResultType.Success;
                }
            }
        }

        /// <summary> Called when the web camera begins recording the video. </summary>
        /// <param name="result"> Indicates whether or not video recording started successfully.</param>
        public delegate void OnStartedRecordingVideoCallback(VideoCaptureResult result);

        /// <summary> Called when a VideoCapture resource has been created. </summary>
        /// <param name="captureObject"> The VideoCapture instance.</param>
        public delegate void OnVideoCaptureResourceCreatedCallback(NRVideoCapture captureObject);

        /// <summary> Called when video mode has been started. </summary>
        /// <param name="result"> Indicates whether or not video mode was successfully activated.</param>
        public delegate void OnVideoModeStartedCallback(VideoCaptureResult result);

        /// <summary> Called when video mode has been stopped. </summary>
        /// <param name="result"> Indicates whether or not video mode was successfully deactivated.</param>
        public delegate void OnVideoModeStoppedCallback(VideoCaptureResult result);

        /// <summary> Called when the video recording has been saved to the file system. </summary>
        /// <param name="result"> Indicates whether or not video recording was saved successfully to the
        ///                       file system.</param>
        public delegate void OnStoppedRecordingVideoCallback(VideoCaptureResult result);
    }
}
