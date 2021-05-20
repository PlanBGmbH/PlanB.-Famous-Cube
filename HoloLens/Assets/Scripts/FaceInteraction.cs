using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FaceInteraction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    MeshRenderer m_MeshRender;
    // Start is called before the first frame update
    void Start()
    {
        m_MeshRender = GetComponent<MeshRenderer>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary> when pointer hover, set the cube color to green. </summary>
    /// <param name="eventData"> Current event data.</param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!PivotRotation.DraggingInProgress)
        {
            m_MeshRender.material.EnableKeyword("_EMISSION");
        }
    }

    /// <summary> when pointer exit hover, set the cube color to white. </summary>
    /// <param name="eventData"> Current event data.</param>
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!PivotRotation.DraggingInProgress)
        {
            m_MeshRender.material.DisableKeyword("_EMISSION");
        }
    }
}
