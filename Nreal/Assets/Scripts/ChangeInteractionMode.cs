using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeInteractionMode : MonoBehaviour
{
    public static bool HandInteractionActive = false;
    public GameObject BuLaserInteraction;
    public GameObject BuHandInteraction;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickLaserInteraction()
    {
        HandInteractionActive = false;
        this.BuHandInteraction.SetActive(false);
        this.BuLaserInteraction.SetActive(true);
    }

    public void OnClickHandInteraction()
    {
        HandInteractionActive = true;
        this.BuLaserInteraction.SetActive(false);
        this.BuHandInteraction.SetActive(true);        
    }
}
