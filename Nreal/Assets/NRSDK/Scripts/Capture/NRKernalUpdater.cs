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
    using System;

    /// <summary> A nr kernal updater used to drive the lifecycle. </summary>
    [ScriptOrder(NativeConstants.NRKERNALUPDATER_ORDER)]
    public class NRKernalUpdater : MonoBehaviour
    {
        /// <summary> The instance. </summary>
        private static NRKernalUpdater m_Instance;
        /// <summary> Gets the instance. </summary>
        /// <value> The instance. </value>
        public static NRKernalUpdater Instance
        {
            get
            {
                if (m_Instance == null && !m_IsDestroyed)
                {
                    m_Instance = CreateInstance();
                }
                return m_Instance;
            }
        }

        [RuntimeInitializeOnLoadMethod]
        static void Initialize()
        {
#if !UNITY_EDITOR
            NRDebugger.logLevel = Debug.isDebugBuild ? LogLevel.Debug : LogLevel.Info;
#endif
            if (m_Instance == null)
            {
                m_Instance = CreateInstance();
            }
        }

        /// <summary> Creates the instance. </summary>
        /// <returns> The new instance. </returns>
        private static NRKernalUpdater CreateInstance()
        {
            GameObject updateObj = new GameObject("NRKernalUpdater");
            GameObject.DontDestroyOnLoad(updateObj);
            return updateObj.AddComponent<NRKernalUpdater>();
        }

        /// <summary> Event queue for all listeners interested in OnPreUpdate events. </summary>
        public static event Action OnPreUpdate;
        /// <summary> Event queue for all listeners interested in OnUpdate events. </summary>
        public static event Action OnUpdate;
        /// <summary> Event queue for all listeners interested in OnPostUpdate events. </summary>
        public static event Action OnPostUpdate;

        /// <summary> Updates this object. </summary>
        private void Update()
        {
            OnPreUpdate?.Invoke();
            OnUpdate?.Invoke();
            OnPostUpdate?.Invoke();
        }

        private static bool m_IsDestroyed = false;
        private void OnDestroy()
        {
            m_Instance = null;
            m_IsDestroyed = true;
        }
    }
}
