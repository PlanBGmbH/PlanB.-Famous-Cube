using NrealExt.Functions.Record;
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
    /// Gets or the sets the recorder
    /// </summary>
    public VideoCapture2Local Recoder;

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


    /// <summary> Starts this object. </summary>
    void Start()
    {
        RecordController.RequestPermission();
    }


    /// <summary>
    /// Toggles the recording
    /// </summary>
    public void ToggleRecording()
    {
        if (Recoder == null)
        {
            Recoder = new VideoCapture2Local();
        }

        if (!recordingInProgress)
        {
            recordingInProgress = true;
            this.Recoder.StartVideoCapture();
            RecordingIndicator.color = Color.red;
        }
        else
        {
            recordingInProgress = false;
            this.Recoder.StopVideoCapture();
            RecordingIndicator.color = new Color(1f, 1f, 1f, 0.3921569f);
        }
    }
}
