using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Implements some common UI interaction of this app
/// </summary>
public class HLInteractions : MonoBehaviour
{
    /// <summary>
    /// Flag of the state of the viewing state of the map panel
    /// </summary>
    private bool MapPaneIsActive = true;

    /// <summary>
    /// Game object of the panel which incl. the map
    /// </summary>
    public GameObject MapPanel;

    /// <summary>
    /// Game object of the panel which incl. the imprint
    /// </summary>
    public GameObject ImpressumPanel;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Opens data privacy of the app in a new browser
    /// </summary>
    public void OpenDataPrivay()
    {
        Application.OpenURL("https://www.PlanB.net");
    }

    /// <summary>
    /// Button click methode to toggle the view state of the map
    /// </summary>
    public void OnToggleMapPanel()
    {
        if (MapPaneIsActive)
        {
            MapPanel.SetActive(false);
            MapPaneIsActive = false;
        }
        else
        {
            MapPanel.SetActive(true);
            MapPaneIsActive = true;
        }
    }

    /// <summary>
    /// Button click methode of open the impressum
    /// </summary>
    public void OnPressImpressum()
    {
        ImpressumPanel.SetActive(true);
    }

    /// <summary>
    /// Button click methode of close the app
    /// </summary>
    public void OnPressQuit()
    {
        Application.Quit();
    }
}
