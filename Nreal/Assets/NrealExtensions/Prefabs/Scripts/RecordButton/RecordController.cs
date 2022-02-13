using NRKernal;
using NRKernal.Record;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Android;
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

    /// <summary>
    /// Quality of recording
    /// </summary>
    public enum ResolutionLevel
    {
        High,
        Middle,
        Low,
    }

    public static void RequestPermission()
    {
        #if PLATFORM_ANDROID
            if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
            {
                Permission.RequestUserPermission(Permission.ExternalStorageRead);
            }

            if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
            {
                Permission.RequestUserPermission(Permission.ExternalStorageWrite);
            }

            if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
            {
                Permission.RequestUserPermission(Permission.Microphone);
            }

            if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
            {
                Permission.RequestUserPermission(Permission.Camera);
            }
    #endif
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

    /// <summary> Starts this object. </summary>
    void Start()
    {
        RecordController.RequestPermission();
    }

    void CreateVideoCapture(Action callback)
    {
        NRVideoCapture.CreateAsync(false, delegate (NRVideoCapture videoCapture)
        {
            NRDebugger.Info("Created VideoCapture Instance!");
            if (videoCapture != null)
            {
                m_VideoCapture = videoCapture;
                callback?.Invoke();
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
        if (m_VideoCapture == null)
        {
            CreateVideoCapture(() =>
            {
                StartVideoCapture();
            });
        }

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
        if (m_VideoCapture == null || m_VideoCapture.IsRecording)
        {
            NRDebugger.Warning("Can not start video capture!");
            return;
        }

        CameraParameters cameraParameters = new CameraParameters();
        Resolution cameraResolution = GetResolutionByLevel(ResolutionLevel.High);
        cameraParameters.hologramOpacity = 0.0f;
        cameraParameters.frameRate = cameraResolution.refreshRate;
        cameraParameters.cameraResolutionWidth = cameraResolution.width;
        cameraParameters.cameraResolutionHeight = cameraResolution.height;
        cameraParameters.pixelFormat = CapturePixelFormat.BGRA32;
        cameraParameters.blendMode = BlendMode.Blend;
        cameraParameters.audioState = NRVideoCapture.AudioState.MicAudio;

        m_VideoCapture.StartVideoModeAsync(cameraParameters, OnStartedVideoCaptureMode, true);

        RecordingIndicator.color = Color.red;
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
        if (m_VideoCapture == null || !m_VideoCapture.IsRecording)
        {
            NRDebugger.Warning("Can not stop video capture!");
            return;
        }

        NRDebugger.Info("Stop Video Capture!");
        m_VideoCapture.StopRecordingAsync(OnStoppedRecordingVideo);
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

        // Release video capture resource.
        m_VideoCapture.Dispose();
        m_VideoCapture = null;
        RecordingIndicator.color = new Color(1f, 1f, 1f, 0.3921569f);
    }

    void OnDestroy()
    {
        // Release video capture resource.
        m_VideoCapture?.Dispose();
        m_VideoCapture = null;
    }
}
