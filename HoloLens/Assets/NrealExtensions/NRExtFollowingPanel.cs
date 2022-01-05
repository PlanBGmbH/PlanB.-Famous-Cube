using NRKernal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NRExtFollowingPanel : MonoBehaviour
{
    /// <summary> The instance. </summary>
    private GameObject m_Instance;
    /// <summary> True if is showing, false if not. </summary>
    public bool IsVisible = true;

    private Transform m_CenterCamera;
    private Transform CenterCamera
    {
        get
        {
            if (m_CenterCamera == null)
            {
                m_CenterCamera = NRSessionManager.Instance.CenterCameraAnchor;
            }
            return m_CenterCamera;
        }
    }

    /// <summary> Starts this object. </summary>
    void Start()
    {
        m_Instance = this.gameObject;

        if (IsVisible)
        {
            this.Show();
        }
        else
        {
            this.Hide();
        }
    }

    /// <summary> Updates this object. </summary>
    void Update()
    {
        if (IsVisible && NRInput.RaycastMode == RaycastModeEnum.Laser)
            FollowCamera();
    }

    /// <summary> Executes the 'cancel button click' action. </summary>
    private void OnCancelButtonClick()
    {
        Hide();
    }

    /// <summary> Follow camera. </summary>
    private void FollowCamera()
    {
        if (m_Instance && CenterCamera)
        {
            m_Instance.transform.position = CenterCamera.transform.position;
            m_Instance.transform.rotation = new Quaternion(0f, CenterCamera.transform.rotation.y, 0f, 0f);

            float yRotation = CenterCamera.transform.eulerAngles.y;
            m_Instance.transform.eulerAngles = new Vector3(m_Instance.transform.eulerAngles.x, yRotation, m_Instance.transform.eulerAngles.z);
        }
    }

    /// <summary> Toggles this object. </summary>
    public void ToggleViewState()
    {
        if (IsVisible)
            Hide();
        else
            Show();
    }

    /// <summary> Shows this object. </summary>
    public void Show()
    {
        if (m_Instance)
        {
            m_Instance.gameObject.SetActive(true);
            IsVisible = true;
            if (NRInput.RaycastMode == RaycastModeEnum.Gaze)
                FollowCamera();
        }
    }

    /// <summary> Hides this object. </summary>
    public void Hide()
    {
        if (m_Instance)
        {
            m_Instance.gameObject.SetActive(false);
            IsVisible = false;
        }
    }
}
