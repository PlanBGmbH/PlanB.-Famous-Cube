using NRKernal;
using NRKernal.Record;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Implements screen recording for nreal glasses and includes also the ui controlling
/// </summary>
public class RecordController : MonoBehaviour
{
    /// <summary>
    /// Gets or the sets the preview image
    /// </summary>
    public Image RecordingIndicator;

    /// <summary>
    /// Represents the state of recording
    /// </summary>
    private bool recordingInProgress = false;

    /// <summary> Save the video to Application.persistentDataPath. </summary>
    /// <value> The full pathname of the video save file. </value>
    public string VideoSavePath
    {
        get
        {
            string timeStamp = Time.time.ToString().Replace(".", "").Replace(":", "");
            string filename = string.Format("RC_Record_{0}.mp4", timeStamp);
            string filepath = Path.Combine(Application.persistentDataPath, filename);
            //filepath = filepath.Replace("/", @"\");
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

    /// <summary>
    /// Toggles the recording
    /// </summary>
    public void ToggleRecording()
    {
        if (!recordingInProgress)
        {
            recordingInProgress = true;
            this.StartVideoCapture();
        }
        else
        {
            recordingInProgress = false;
            this.StopVideoCapture();
        }
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

            RecordingIndicator.color = Color.red;
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
        m_VideoCapture.Dispose();
        RecordingIndicator.color = new Color(1f, 1f, 1f, 0.3921569f);
        NRDebugger.Info("Stopped Video Capture Mode!");
    }
}
