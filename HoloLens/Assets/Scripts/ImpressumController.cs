using UnityEngine;
using UnityEngine.UI;

/// <summary> A nr home menu. </summary>
public class ImpressumController : MonoBehaviour
{
    /// <summary> The cancel control. </summary>
    public Button cancelBtn;



    /// <summary> Starts this object. </summary>
    void Start()
    {
        cancelBtn.onClick.AddListener(OnCancelButtonClick);
    }

    /// <summary> Updates this object. </summary>
    void Update()
    {
    }

    /// <summary> Executes the 'cancel button click' action. </summary>
    private void OnCancelButtonClick()
    {
        Hide();
    }


    /// <summary> Toggles this object. </summary>
    public void Toggle()
    {

    }

    /// <summary> Shows this object. </summary>
    public void Show()
    {

    }

    /// <summary> Hides this object. </summary>
    public void Hide()
    {
        
    }
}

