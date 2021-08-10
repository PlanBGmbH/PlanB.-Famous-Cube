/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRKernal
{
    using UnityEngine;
    using UnityEngine.UI;

    public class NRTrackingModeChangedTip : MonoBehaviour
    {
        [SerializeField]
        private Camera m_RenderCamera;
        [SerializeField]
        private Text m_Lable;
        [SerializeField]
        private Transform m_LoadingUI;
        public RenderTexture RT { get; private set; }

        private static NativeResolution resolution = new NativeResolution(1920, 1080);

        public static NRTrackingModeChangedTip Create()
        {
            NRTrackingModeChangedTip lostTrackingTip;
            var config = NRSessionManager.Instance.NRSessionBehaviour?.SessionConfig;
            if (config == null || config.TrackingModeChangeTipPrefab == null)
            {
                lostTrackingTip = GameObject.Instantiate(Resources.Load<NRTrackingModeChangedTip>("NRTrackingModeChangedTip"));
            }
            else
            {
                lostTrackingTip = GameObject.Instantiate(config.TrackingModeChangeTipPrefab);
            }
            lostTrackingTip.transform.position = Vector3.one * 8888;
#if !UNITY_EDITOR
            resolution = NRDevice.Instance.NativeHMD.GetEyeResolution((int)NativeEye.LEFT);
#endif
            lostTrackingTip.RT = UnityExtendedUtility.CreateRenderTexture(resolution.width, resolution.height, 24, RenderTextureFormat.Default);
            lostTrackingTip.m_RenderCamera.targetTexture = lostTrackingTip.RT;
            return lostTrackingTip;
        }

        public void SetMessage(string msg)
        {
            m_Lable.text = msg;
        }

        void Update()
        {
            m_LoadingUI.Rotate(-Vector3.forward, 2f, Space.Self);
        }

        void Start()
        {
            m_RenderCamera.aspect = (float)resolution.width / resolution.height;
            m_RenderCamera.targetTexture = RT;
        }

        void OnEnable()
        {
            m_RenderCamera.enabled = true;
        }

        void OnDisable()
        {
            m_RenderCamera.enabled = false;
        }

        void OnDestroy()
        {
            RT?.Release();
        }
    }
}
