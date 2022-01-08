using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartController : MonoBehaviour
{
    private static DateTime startTime;

    // Start is called before the first frame update
    void Start()
    {
        startTime = DateTime.Now.AddSeconds(5d);
    }

    // Update is called once per frame
    void Update()
    {
        if (startTime < DateTime.Now)
        {
#if UNITY_ANDROID
            SceneManager.LoadSceneAsync(Const.Scene_Nreal_Home);
#else
        SceneManager.LoadSceneAsync(Const.Scene_MRTK_Cube);
#endif
        }
    }
}
