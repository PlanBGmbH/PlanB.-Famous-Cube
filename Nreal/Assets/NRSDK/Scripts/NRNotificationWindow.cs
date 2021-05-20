/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

using UnityEngine;
using UnityEngine.UI;
using System;

namespace NRKernal
{
    /// <summary> Form for viewing the nr notification. </summary>
    public class NRNotificationWindow : MonoBehaviour
    {
        /// <summary> (Serializable) information about the notification. </summary>
        [Serializable]
        public struct NotificationInfo
        {
            /// <summary> The sprite. </summary>
            public Sprite sprite;
            /// <summary> The title. </summary>
            public string title;
            /// <summary> The message. </summary>
            public string message;
        }

        /// <summary> Information describing the high level. </summary>
        public NotificationInfo m_HighLevelInfo;
        /// <summary> Information describing the middle level. </summary>
        public NotificationInfo m_MiddleLevelInfo;

        /// <summary> The icon. </summary>
        [SerializeField]
        private Image m_Icon;
        /// <summary> The title. </summary>
        [SerializeField]
        private Text m_Title;
        /// <summary> The message. </summary>
        [SerializeField]
        private Text m_Message;
        /// <summary> The confirm control. </summary>
        [SerializeField]
        private Button m_ConfirmBtn;

        /// <summary> The duration. </summary>
        private float m_Duration = 2f;

        /// <summary> Fill data. </summary>
        /// <param name="level">    The level.</param>
        /// <param name="duration"> (Optional) The duration.</param>
        public virtual void FillData(NRNotificationListener.Level level, float duration = 2f)
        {
            NotificationInfo info;

            switch (level)
            {
                case NRNotificationListener.Level.High:
                    info = m_HighLevelInfo;
                    break;
                case NRNotificationListener.Level.Middle:
                    info = m_MiddleLevelInfo;
                    m_ConfirmBtn?.gameObject.SetActive(false);
                    break;
                case NRNotificationListener.Level.Low:
                default:
                    GameObject.Destroy(gameObject);
                    return;
            }

            m_Title.text = info.title;
            m_Message.text = info.message;
            m_Duration = duration;
            m_Icon.sprite = info.sprite;

            m_ConfirmBtn?.onClick.AddListener(() =>
            {
                NRDevice.QuitApp();
            });

            if (m_Duration > 0)
            {
                Invoke("AutoDestroy", m_Duration);
            }
        }

        /// <summary> Automatic destroy. </summary>
        private void AutoDestroy()
        {
            GameObject.Destroy(gameObject);
        }
    }
}
