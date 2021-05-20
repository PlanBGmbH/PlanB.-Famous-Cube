using UnityEngine;
using UnityEngine.UI;

namespace NRKernal.NRExamples
{
    /// <summary> A nr home menu. </summary>
    public class NRHomeMenu : MonoBehaviour
    {
        /// <summary> The confirm control. </summary>
        public Button confirmBtn;
        /// <summary> The cancel control. </summary>
        public Button cancelBtn;

        /// <summary> The instance. </summary>
        private static NRHomeMenu m_Instance;
        /// <summary> True if is showing, false if not. </summary>
        private static bool m_IsShowing = false;
        /// <summary> Full pathname of the menu prefab file. </summary>
        private static string m_MenuPrefabPath = "NRUI/NRHomeMenu";

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
            confirmBtn.onClick.AddListener(OnComfirmButtonClick);
            cancelBtn.onClick.AddListener(OnCancelButtonClick);
        }

        /// <summary> Updates this object. </summary>
        void Update()
        {
            if (m_IsShowing && NRInput.RaycastMode == RaycastModeEnum.Laser)
                FollowCamera();
        }

        /// <summary> Executes the 'comfirm button click' action. </summary>
        private void OnComfirmButtonClick()
        {
            Hide();
            AppManager.QuitApplication();
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

        /// <summary> Creates the menu. </summary>
        private static void CreateMenu()
        {
            GameObject menuPrefab = Resources.Load<GameObject>(m_MenuPrefabPath);
            if (menuPrefab == null)
            {
                NRDebugger.Error("Can not find prefab: " + m_MenuPrefabPath);
                return;
            }
            GameObject menuGo = Instantiate(menuPrefab);
            m_Instance = menuGo.GetComponent<NRHomeMenu>();
            if (m_Instance)
                DontDestroyOnLoad(menuGo);
            else
                Destroy(menuGo);
        }

        /// <summary> Toggles this object. </summary>
        public static void Toggle()
        {
            if (m_IsShowing)
                Hide();
            else
                Show();
        }

        /// <summary> Shows this object. </summary>
        public static void Show()
        {
            if (m_Instance == null)
                CreateMenu();
            if (m_Instance)
            {
                m_Instance.gameObject.SetActive(true);
                m_IsShowing = true;
                if (NRInput.RaycastMode == RaycastModeEnum.Gaze)
                    m_Instance.FollowCamera();
            }
        }

        /// <summary> Hides this object. </summary>
        public static void Hide()
        {
            if (m_Instance)
            {
                m_Instance.gameObject.SetActive(false);
                m_IsShowing = false;
            }
        }
    }
}
