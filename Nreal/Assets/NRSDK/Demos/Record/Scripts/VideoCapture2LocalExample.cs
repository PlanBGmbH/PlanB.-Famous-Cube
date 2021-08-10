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
        public BlendMode blendMode;
        public ResolutionLevel resolutionLevel;
        public LayerMask cullingMask;
        public bool useAudio;
        public bool useGreenBackGround;

        public enum ResolutionLevel
        {
            High,
            Middle,
            Low,
        }

        /// <summary> Save the video to Application.persistentDataPath. </summary>
        /// <value> The full pathname of the video save file. </value>
        public string VideoSavePath
        {
            get
            {
                string timeStamp = Time.time.ToString().Replace(".", "").Replace(":", "");
                string filename = string.Format("Nreal_Record_{0}.mp4", timeStamp);
                return Path.Combine(Application.persistentDataPath, filename);
            }
        }

        /// <summary> The video capture. </summary>
        NRVideoCapture m_VideoCapture = null;

        void Start()
        {
            CreateVideoCaptureTest();
        }

        /// <summary> Tests create video capture. </summary>
        void CreateVideoCaptureTest()
        {
            NRVideoCapture.CreateAsync(false, delegate (NRVideoCapture videoCapture)
            {
                NRDebugger.Info("Created VideoCapture Instance!");
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
            if (m_VideoCapture != null)
            {
                CameraParameters cameraParameters = new CameraParameters();
                Resolution cameraResolution = GetResolutionByLevel(resolutionLevel);
                cameraParameters.hologramOpacity = 0.0f;
                cameraParameters.frameRate = cameraResolution.refreshRate;
                cameraParameters.cameraResolutionWidth = cameraResolution.width;
                cameraParameters.cameraResolutionHeight = cameraResolution.height;
                cameraParameters.pixelFormat = CapturePixelFormat.BGRA32;
                // Set the blend mode.
                cameraParameters.blendMode = blendMode;
                // Set audio state, audio record needs the permission of "android.permission.RECORD_AUDIO",
                // Add it to your "AndroidManifest.xml" file in "Assets/Plugin".
                cameraParameters.audioState = useAudio ? NRVideoCapture.AudioState.MicAudio : NRVideoCapture.AudioState.None;

                m_VideoCapture.StartVideoModeAsync(cameraParameters, OnStartedVideoCaptureMode);
            }
        }

        private Resolution GetResolutionByLevel(ResolutionLevel level)
        {
            var resolutions = NRVideoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height);
            Resolution resolution = new Resolution();
            switch (level)
            {
                case ResolutionLevel.High:
                    resolution = resolutions.ElementAt(0);
                    break;
                case ResolutionLevel.Middle:
                    resolution = resolutions.ElementAt(1);
                    break;
                case ResolutionLevel.Low:
                    resolution = resolutions.ElementAt(2);
                    break;
                default:
                    break;
            }
            return resolution;
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
            if (!result.success)
            {
                NRDebugger.Info("Started Video Capture Mode faild!");
                return;
            }

            NRDebugger.Info("Started Video Capture Mode!");
            m_VideoCapture.StartRecordingAsync(VideoSavePath, OnStartedRecordingVideo);
            // Set preview texture.
            Previewer.SetData(m_VideoCapture.PreviewTexture, true);
        }

        /// <summary> Executes the 'started recording video' action. </summary>
        /// <param name="result"> The result.</param>
        void OnStartedRecordingVideo(NRVideoCapture.VideoCaptureResult result)
        {
            if (!result.success)
            {
                NRDebugger.Info("Started Recording Video Faild!");
                return;
            }

            NRDebugger.Info("Started Recording Video!");
            if (useGreenBackGround)
            {
                // Set green background color.
                m_VideoCapture.GetContext().GetBehaviour().SetBackGroundColor(Color.green);
            }
            m_VideoCapture.GetContext().GetBehaviour().CaptureCamera.cullingMask = cullingMask.value;
        }

        /// <summary> Executes the 'stopped recording video' action. </summary>
        /// <param name="result"> The result.</param>
        void OnStoppedRecordingVideo(NRVideoCapture.VideoCaptureResult result)
        {
            if (!result.success)
            {
                NRDebugger.Info("Stopped Recording Video Faild!");
                return;
            }

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
