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
    using System;
    using UnityEngine;
    using System.Collections.Generic;

    /// <summary> A nr photo capture. </summary>
    public class NRPhotoCapture : IDisposable
    {
        /// <summary> The supported resolutions. </summary>
        private static IEnumerable<Resolution> m_SupportedResolutions;

        /// <summary> A list of all the supported device resolutions for taking pictures. </summary>
        /// <value> The supported resolutions. </value>
        public static IEnumerable<Resolution> SupportedResolutions
        {
            get
            {
                if (m_SupportedResolutions == null)
                {
                    var resolutions = new List<Resolution>();
                    var resolution = new Resolution();
                    var rgbcamera_resolution = NRDevice.Instance.GetRGBCameraResolution();
                    resolution.width = rgbcamera_resolution.width;
                    resolution.height = rgbcamera_resolution.height;
                    resolutions.Add(resolution);
                    m_SupportedResolutions = resolutions;
                }
                return m_SupportedResolutions;
            }
        }

        /// <summary> Context for the capture. </summary>
        private FrameCaptureContext m_CaptureContext;

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
        public static void CreateAsync(bool showHolograms, OnCaptureResourceCreatedCallback onCreatedCallback)
        {
            NRPhotoCapture photocapture = new NRPhotoCapture();
            photocapture.m_CaptureContext = FrameCaptureContextFactory.Create();
            onCreatedCallback?.Invoke(photocapture);
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

        /// <summary>
        /// Provides a COM pointer to the native IVideoDeviceController. A native COM pointer to the
        /// IVideoDeviceController. </summary>
        /// <returns> The unsafe pointer to video device controller. </returns>
        public IntPtr GetUnsafePointerToVideoDeviceController()
        {
            NRDebugger.Warning("[NRPhotoCapture] Interface not supported...");
            return IntPtr.Zero;
        }

        /// <summary> Starts photo mode asynchronous. </summary>
        /// <param name="setupParams">                Options for controlling the setup.</param>
        /// <param name="onPhotoModeStartedCallback"> The on photo mode started callback.</param>
        public void StartPhotoModeAsync(CameraParameters setupParams, OnPhotoModeStartedCallback onPhotoModeStartedCallback)
        {
            PhotoCaptureResult result = new PhotoCaptureResult();
            try
            {
                setupParams.camMode = CamMode.PhotoMode;
                m_CaptureContext.StartCaptureMode(setupParams);
                m_CaptureContext.StartCapture();

                NRKernalUpdater.Instance.StartCoroutine(OnPhotoModeStartedReady(() =>
                {
                    result.resultType = CaptureResultType.Success;
                    onPhotoModeStartedCallback?.Invoke(result);
                }));
            }
            catch (Exception)
            {
                result.resultType = CaptureResultType.UnknownError;
                onPhotoModeStartedCallback?.Invoke(result);
                throw;
            }
        }

        /// <summary> Executes the 'photo mode started ready' action. </summary>
        /// <param name="callback"> The callback.</param>
        /// <returns> A list of. </returns>
        public System.Collections.IEnumerator OnPhotoModeStartedReady(Action callback)
        {
            while (!this.m_CaptureContext.GetFrameProvider().IsFrameReady())
            {
                NRDebugger.Debug("[PhotoCapture] Wait for the first frame ready...");
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            callback?.Invoke();
        }

        /// <summary> Stops photo mode asynchronous. </summary>
        /// <param name="onPhotoModeStoppedCallback"> The on photo mode stopped callback.</param>
        public void StopPhotoModeAsync(OnPhotoModeStoppedCallback onPhotoModeStoppedCallback)
        {
            PhotoCaptureResult result = new PhotoCaptureResult();
            try
            {
                m_CaptureContext.StopCaptureMode();
                result.resultType = CaptureResultType.Success;
                onPhotoModeStoppedCallback?.Invoke(result);
            }
            catch (Exception)
            {
                result.resultType = CaptureResultType.UnknownError;
                onPhotoModeStoppedCallback?.Invoke(result);
                throw;
            }
        }

        /// <summary> Take photo asynchronous. </summary>
        /// <param name="filename">                      Filename of the file.</param>
        /// <param name="fileOutputFormat">              The file output format.</param>
        /// <param name="onCapturedPhotoToDiskCallback"> The on captured photo disk callback.</param>
        public void TakePhotoAsync(string filename, PhotoCaptureFileOutputFormat fileOutputFormat, OnCapturedToDiskCallback onCapturedPhotoToDiskCallback)
        {
            try
            {
                var capture = m_CaptureContext.GetBehaviour();
                ((NRCaptureBehaviour)capture).Do(filename, fileOutputFormat);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary> Take photo asynchronous. </summary>
        /// <param name="onCapturedPhotoToMemoryCallback"> The on captured photo memory callback.</param>
        public void TakePhotoAsync(OnCapturedToMemoryCallback onCapturedPhotoToMemoryCallback)
        {
            try
            {
                var capture = m_CaptureContext.GetBehaviour();
                ((NRCaptureBehaviour)capture).DoAsyn(onCapturedPhotoToMemoryCallback);
            }
            catch (Exception)
            {
                throw;
            }
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
        /// A data container that contains the result information of a photo capture operation. </summary>
        public struct PhotoCaptureResult
        {
            /// <summary>
            /// A generic result that indicates whether or not the PhotoCapture operation succeeded. </summary>
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

        /// <summary> Called when a PhotoCapture resource has been created. </summary>
        /// <param name="captureObject"> The PhotoCapture instance.</param>
        public delegate void OnCaptureResourceCreatedCallback(NRPhotoCapture captureObject);

        /// <summary> Called when photo mode has been started. </summary>
        /// <param name="result"> Indicates whether or not photo mode was successfully activated.</param>
        public delegate void OnPhotoModeStartedCallback(PhotoCaptureResult result);

        /// <summary> Called when photo mode has been stopped. </summary>
        /// <param name="result"> Indicates whether or not photo mode was successfully deactivated.</param>
        public delegate void OnPhotoModeStoppedCallback(PhotoCaptureResult result);

        /// <summary> Called when a photo has been saved to the file system. </summary>
        /// <param name="result"> Indicates whether or not the photo was successfully saved to the file
        ///                       system.</param>
        public delegate void OnCapturedToDiskCallback(PhotoCaptureResult result);

        /// <summary> Called when a photo has been captured to memory. </summary>
        /// <param name="result">            Indicates whether or not the photo was successfully captured
        ///                                  to memory.</param>
        /// <param name="photoCaptureFrame"> Contains the target texture.If available, the spatial
        ///                                  information will be accessible through this structure as well.</param>
        public delegate void OnCapturedToMemoryCallback(PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame);
    }
}
