using Assets.Scripts;
using NRKernal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeController : MonoBehaviour
{
    public GameObject MRTKButton;

    // Start is called before the first frame update
    void Start()
    {
        if (!NRInput.SetInputSource(InputSourceEnum.Hands))
        {
            MRTKButton.SetActive(false);
        }

        NRInput.SetInputSource(InputSourceEnum.Controller);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    /// <summary>
    /// Handles the interaction of the Nreal/Laser Button
    /// </summary>
    public void OnClickNreal()
    {
        SceneManager.UnloadSceneAsync(Const.Scene_Nreal_Home);
        SceneManager.LoadSceneAsync(Const.Scene_Nreal_Cube, LoadSceneMode.Additive);
    }

    /// <summary>
    /// Handles the interaction of the MRTK/ Hand Interaction Button
    /// </summary>
    public void OnClickMRTK()
    {
        SceneManager.UnloadSceneAsync(Const.Scene_Nreal_Home);
        NRInput.SetInputSource(InputSourceEnum.Hands);
        StartController.EnableMRTK();
        SceneManager.LoadSceneAsync(Const.Scene_MRTK_Cube, LoadSceneMode.Additive);
    }
}
