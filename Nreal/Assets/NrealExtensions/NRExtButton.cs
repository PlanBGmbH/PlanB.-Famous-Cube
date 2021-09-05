using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using NRKernal;

/// <summary>
/// Extent the button inside the "Laser-Mode" with userfeedback 
/// </summary>
[RequireComponent(typeof(Button))]
public class NRExtButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{

    /// <summary>
    /// Text element of the button
    /// </summary>
    Text txt;

    /// <summary>
    /// Base color of button
    /// </summary>
    Color baseColor;

    /// <summary>
    /// Button gameobject
    /// </summary>
    Button btn;

    /// <summary>
    /// Delay to remove wrong events
    /// </summary>
    bool interactableDelay;

    /// <summary>
    /// Start methode of unity gameobject
    /// </summary>
    void Start()
    {
        txt = GetComponentInChildren<Text>();
        baseColor = txt.color;
        btn = gameObject.GetComponent<Button>();
        interactableDelay = btn.interactable;
    }

    /// <summary>
    /// Update methode of unity gameobject
    /// </summary>
    void Update()
    {
        if (btn.interactable != interactableDelay)
        {
            if (btn.interactable)
            {
                txt.color = baseColor * btn.colors.normalColor * btn.colors.colorMultiplier;
            }
            else
            {
                txt.color = baseColor * btn.colors.disabledColor * btn.colors.colorMultiplier;
            }
        }
        interactableDelay = btn.interactable;
    }

    /// <summary>
    /// Event reciver methode which triggered if the laser cross/ enters the button
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (btn.interactable)
        {            
            txt.color = baseColor * btn.colors.highlightedColor * btn.colors.colorMultiplier;
        }
        else
        {
            txt.color = baseColor * btn.colors.disabledColor * btn.colors.colorMultiplier;
        }
    }

    /// <summary>
    /// Event reciver methode which triggered if a button of the controller is pressed
    /// </summary>
    public void OnPointerDown(PointerEventData eventData)
    {
        if (btn.interactable)
        {
            NRInput.TriggerHapticVibration();
            txt.color = baseColor * btn.colors.pressedColor * btn.colors.colorMultiplier;
        }
        else
        {
            txt.color = baseColor * btn.colors.disabledColor * btn.colors.colorMultiplier;
        }
    }

    /// <summary>
    /// Event reciver methode which triggered if leaves the pressmode
    /// </summary>
    public void OnPointerUp(PointerEventData eventData)
    {
        if (btn.interactable)
        {
            txt.color = baseColor * btn.colors.highlightedColor * btn.colors.colorMultiplier;
        }
        else
        {
            txt.color = baseColor * btn.colors.disabledColor * btn.colors.colorMultiplier;
        }
    }

    /// <summary>
    /// Event reciver methode which triggered if the laser leaves the button
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        if (btn.interactable)
        {
            txt.color = baseColor * btn.colors.normalColor * btn.colors.colorMultiplier;
        }
        else
        {
            txt.color = baseColor * btn.colors.disabledColor * btn.colors.colorMultiplier;
        }
    }
}