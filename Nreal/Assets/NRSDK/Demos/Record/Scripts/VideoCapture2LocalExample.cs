/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

using NRKernal.Record;
using System.IO;
using System.Linq;
using UnityEngine;

namespace NRKernal.NRExamples
{
    /// <summary> A video capture 2 local example. </summary>
    [HelpURL("https://developer.nreal.ai/develop/unity/video-capture")]
    public class VideoCapture2LocalExample : MonoBehaviour
    {
        /// <summary> The previewer. </summary>
        public NRPreviewer Previewer;

        /// <summary> Save the video to Application.persistentDataPath. </summary>
        /// <value> The full pathname of the video save file. </value>
        public string VideoSavePath
        {
            get
            {
                string timeStamp = Time.time.ToString().Replace(".", "").Replace(":", "");
                string filename = string.Format("Nreal_Record_{0}.mp4", timeStamp);
                string filepath = Path.Combine(Application.persistentDataPath, filename);
                return filepath;
            }
        }

        /// <summary> The video capture. </summary>
        NRVideoCapture m_VideoCapture = null;

        /// <summary> Starts this object. </summary>
        void Start()
        {
            CreateVideoCaptureTest();
        }

        /// <summary> Tests create video capture. </summary>
        void CreateVideoCaptureTest()
        {
            NRVideoCapture.CreateAsync(false, delegate (NRVideoCapture videoCapture)
            {
                if (videoCapture != null)
                {
                    m_VideoCapture = videoCapture;
                }
                else
                {
                    NRDebugger.Error("Failed to create VideoCapture Instance!");
                }
            });
        }

        /// <summary> Starts video capture. </summary>
        public void StartVideoCapture()
        {
            Resolution cameraResolution = NRVideoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
            NRDebugger.Info(cameraResolution);

            int cameraFramerate = NRVideoCapture.GetSupportedFrameRatesForResolution(cameraResolution).OrderByDescending((fps) => fps).First();
            NRDebugger.Info(cameraFramerate);

            if (m_VideoCapture != null)
            {
                NRDebugger.Info("Created VideoCapture Instance!");
                CameraParameters cameraParameters = new CameraParameters();
                cameraParameters.hologramOpacity = 0.0f;
                cameraParameters.frameRate = cameraFramerate;
                cameraParameters.cameraResolutionWidth = cameraResolution.width;
                cameraParameters.cameraResolutionHeight = cameraResolution.height;
                cameraParameters.pixelFormat = CapturePixelFormat.BGRA32;
                cameraParameters.blendMode = BlendMode.Blend;

                m_VideoCapture.StartVideoModeAsync(cameraParameters, OnStartedVideoCaptureMode);

                Previewer.SetData(m_VideoCapture.PreviewTexture, true);
            }
        }

        /// <summary> Stops video capture. </summary>
        public void StopVideoCapture()
        {
            if (m_VideoCapture == null)
            {
                return;
            }
            NRDebugger.Info("Stop Video Capture!");
            m_VideoCapture.StopRecordingAsync(OnStoppedRecordingVideo);
            Previewer.SetData(m_VideoCapture.PreviewTexture, false);
        }

        /// <summary> Executes the 'started video capture mode' action. </summary>
        /// <param name="result"> The result.</param>
        void OnStartedVideoCaptureMode(NRVideoCapture.VideoCaptureResult result)
        {
            NRDebugger.Info("Started Video Capture Mode!");
            m_VideoCapture.StartRecordingAsync(VideoSavePath, OnStartedRecordingVideo);
        }

        /// <summary> Executes the 'started recording video' action. </summary>
        /// <param name="result"> The result.</param>
        void OnStartedRecordingVideo(NRVideoCapture.VideoCaptureResult result)
        {
            NRDebugger.Info("Started Recording Video!");
        }

        /// <summary> Executes the 'stopped recording video' action. </summary>
        /// <param name="result"> The result.</param>
        void OnStoppedRecordingVideo(NRVideoCapture.VideoCaptureResult result)
        {
            NRDebugger.Info("Stopped Recording Video!");
            m_VideoCapture.StopVideoModeAsync(OnStoppedVideoCaptureMode);
        }

        /// <summary> Executes the 'stopped video capture mode' action. </summary>
        /// <param name="result"> The result.</param>
        void OnStoppedVideoCaptureMode(NRVideoCapture.VideoCaptureResult result)
        {
            NRDebugger.Info("Stopped Video Capture Mode!");
        }
    }
}
