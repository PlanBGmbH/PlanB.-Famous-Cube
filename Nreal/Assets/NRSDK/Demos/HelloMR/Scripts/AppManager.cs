/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

using UnityEngine;

namespace NRKernal.NRExamples
{
    /// <summary> Manager for applications. </summary>
    [DisallowMultipleComponent]
    [HelpURL("https://developer.nreal.ai/develop/discover/introduction-nrsdk")]
    public class AppManager : MonoBehaviour
    {
        /// <summary>
        /// If enable this, quick click app button for three times, a profiler bar would show. </summary>
        public bool enableTriggerProfiler;

        /// <summary> The last click time. </summary>
        private float m_LastClickTime = 0f;
        /// <summary> The cumulative click number. </summary>
        private int m_CumulativeClickNum = 0;
        /// <summary> True if is profiler opened, false if not. </summary>
        private bool m_IsProfilerOpened = false;
        /// <summary> The button press timer. </summary>
        private float m_ButtonPressTimer;

        /// <summary> Number of trigger profiler clicks. </summary>
        private const int TRIGGER_PROFILER_CLICK_COUNT = 3;
        /// <summary> Duration of the button long press. </summary>
        private const float BUTTON_LONG_PRESS_DURATION = 1.2f;

        /// <summary> Executes the 'enable' action. </summary>
        private void OnEnable()
        {
            NRInput.AddClickListener(ControllerHandEnum.Right, ControllerButton.HOME, OnHomeButtonClick);
            NRInput.AddClickListener(ControllerHandEnum.Left, ControllerButton.HOME, OnHomeButtonClick);
            NRInput.AddClickListener(ControllerHandEnum.Right, ControllerButton.APP, OnAppButtonClick);
            NRInput.AddClickListener(ControllerHandEnum.Left, ControllerButton.APP, OnAppButtonClick);
        }

        /// <summary> Executes the 'disable' action. </summary>
        private void OnDisable()
        {
            NRInput.RemoveClickListener(ControllerHandEnum.Right, ControllerButton.HOME, OnHomeButtonClick);
            NRInput.RemoveClickListener(ControllerHandEnum.Left, ControllerButton.HOME, OnHomeButtonClick);
            NRInput.RemoveClickListener(ControllerHandEnum.Right, ControllerButton.APP, OnAppButtonClick);
            NRInput.RemoveClickListener(ControllerHandEnum.Left, ControllerButton.APP, OnAppButtonClick);
        }

        /// <summary> Updates this object. </summary>
        private void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                QuitApplication();
            }
#endif
            CheckButtonLongPress();
        }

        /// <summary> Executes the 'home button click' action. </summary>
        private void OnHomeButtonClick()
        {
            NRHomeMenu.Toggle();
        }

        /// <summary> Executes the 'application button click' action. </summary>
        private void OnAppButtonClick()
        {
            if (enableTriggerProfiler)
            {
                CollectClickEvent();
            }
        }

        /// <summary> Check button long press. </summary>
        private void CheckButtonLongPress()
        {
            if (NRInput.GetButton(ControllerButton.HOME))
            {
                m_ButtonPressTimer += Time.deltaTime;
                if (m_ButtonPressTimer > BUTTON_LONG_PRESS_DURATION)
                {
                    m_ButtonPressTimer = float.MinValue;

                    // Reset layser when long press Home btn.
                    NRInput.RecenterController();
                }
            }
            else
            {
                m_ButtonPressTimer = 0f;
            }
        }

        /// <summary> Collect click event. </summary>
        private void CollectClickEvent()
        {
            if (Time.unscaledTime - m_LastClickTime < 0.2f)
            {
                m_CumulativeClickNum++;
                if (m_CumulativeClickNum == (TRIGGER_PROFILER_CLICK_COUNT - 1))
                {
                    // Show the VisualProfiler
                    NRVisualProfiler.Instance.Switch(!m_IsProfilerOpened);
                    m_IsProfilerOpened = !m_IsProfilerOpened;
                    m_CumulativeClickNum = 0;
                }
            }
            else
            {
                m_CumulativeClickNum = 0;
            }
            m_LastClickTime = Time.unscaledTime;
        }


        /// <summary> Quit application. </summary>
        public static void QuitApplication()
        {
            NRDevice.QuitApp();
        }
    }
}
