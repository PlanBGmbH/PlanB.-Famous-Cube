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

    /// <summary> Manager for nr multi displays. </summary>
    [HelpURL("https://developer.nreal.ai/develop/unity/customize-phone-controller")]
    public class NRMultiDisplayManager : MonoBehaviour
    {
        /// <summary> The default virtual displayer. </summary>
        [SerializeField]
        private GameObject m_DefaultVirtualDisplayer;
        /// <summary> The virtual displayer. </summary>
        private NRVirtualDisplayer m_VirtualDisplayer;

        /// <summary> Starts this object. </summary>
        private void Start()
        {
            Init();
        }

        /// <summary> Initializes this object. </summary>
        private void Init()
        {
            m_VirtualDisplayer = FindObjectOfType<NRVirtualDisplayer>();
            if (m_VirtualDisplayer != null) return;

#if UNITY_EDITOR
            Instantiate(m_DefaultVirtualDisplayer);
#else
            var virtualDisplayer = new GameObject("NRVirtualDisplayer").AddComponent<NRVirtualDisplayer>();
            GameObject.DontDestroyOnLoad(virtualDisplayer.gameObject);
#endif
        }
    }
}
