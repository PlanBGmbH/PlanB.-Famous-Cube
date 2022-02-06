using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartController : MonoBehaviour
{
    private static bool started = false;
    private static DateTime startTime;

    private static GameObject MRTK_ToolkitInstance;
    private static GameObject MRTK_PlayspaceInstance;
    private static GameObject MRTK_SceneContentInstance;

    public GameObject MRTK_Toolkit;
    public GameObject MRTK_Playspace;
    public GameObject MRTK_SceneContent;

    public static void EnableMRTK()
    {
        MRTK_ToolkitInstance.SetActive(true);
        MRTK_PlayspaceInstance.SetActive(true);
        MRTK_SceneContentInstance.SetActive(true);
    }

    public static void DisableMRTK()
    {
        MRTK_ToolkitInstance.SetActive(false);
        MRTK_PlayspaceInstance.SetActive(false);
        MRTK_SceneContentInstance.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        startTime = DateTime.Now.AddSeconds(5d);

        MRTK_ToolkitInstance = MRTK_Toolkit;
        MRTK_PlayspaceInstance = MRTK_Playspace;
        MRTK_SceneContentInstance = MRTK_SceneContent;
    }

    // Update is called once per frame
    void Update()
    {
        if (startTime < DateTime.Now && !started)
        {
            started = true;
#if UNITY_ANDROID
            StartController.DisableMRTK();
            SceneManager.LoadSceneAsync(Const.Scene_Nreal_Home, LoadSceneMode.Additive);
#else
        SceneManager.LoadSceneAsync(Const.Scene_MRTK_Cube, LoadSceneMode.Additive);
#endif
        }
    }
}
