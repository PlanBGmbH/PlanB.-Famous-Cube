using Assets.Scripts;
using NRKernal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NrChangeInteractionMode : MonoBehaviour
{
    public static bool HandInteractionActive = false;
    public GameObject BuLaserInteraction;
    public GameObject BuHandInteraction;
    public GameObject Handmenu;

    // Start is called before the first frame update
    void Start()
    {
        if (!NRInput.SetInputSource(InputSourceEnum.Hands))
        {
            BuHandInteraction.SetActive(false);
            BuHandInteraction.SetActive(false);
        }

        NRInput.SetInputSource(InputSourceEnum.Controller);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Event reciver for on click of the laser button
    /// </summary>
    public void OnClickLaserInteraction()
    {        
        HandInteractionActive = false;
        this.BuHandInteraction.SetActive(true);
        this.BuLaserInteraction.SetActive(false);
        //this.Handmenu.transform.position = new Vector3(this.Handmenu.transform.position.x, this.Handmenu.transform.position.y - 0.1f, this.Handmenu.transform.position.z + 0.1f);
        NRInput.SetInputSource(InputSourceEnum.Controller);
    }

    /// <summary>
    /// Event reciver for on click of the hand button
    /// </summary>
    public void OnClickHandInteraction()
    {
        NRInput.SetInputSource(InputSourceEnum.Hands);
        SceneManager.LoadSceneAsync(Const.Scene_MRTK_Cube);
    }
}
