using NRKernal;
using UnityEngine;
using UnityEngine.UI;

/// <summary> A nr home menu. </summary>
public class NrImpressumController : MonoBehaviour
{
    /// <summary> The cancel control. </summary>
    public Button cancelBtn;

    /// <summary> The instance. </summary>
    public GameObject m_Instance;
    /// <summary> True if is showing, false if not. </summary>
    private static bool m_IsShowing = false;

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
        cancelBtn.onClick.AddListener(OnCancelButtonClick);
    }

    /// <summary> Updates this object. </summary>
    void Update()
    {
        if (m_IsShowing && NRInput.RaycastMode == RaycastModeEnum.Laser)
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
            m_Instance.transform.rotation = CenterCamera.transform.rotation;
        }
    }


    /// <summary> Toggles this object. </summary>
    public void Toggle()
    {
        if (m_IsShowing)
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
            m_IsShowing = true;
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
            m_IsShowing = false;
        }
    }
}

