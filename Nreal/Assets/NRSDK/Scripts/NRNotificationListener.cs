/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

using System.Collections.Generic;
using UnityEngine;

namespace NRKernal
{
    /// <summary> A nr notification listener. </summary>
    public class NRNotificationListener : MonoBehaviour
    {
        /// <summary> Values that represent levels. </summary>
        public enum Level
        {
            /// <summary> An enum constant representing the high option. </summary>
            High,
            /// <summary> An enum constant representing the middle option. </summary>
            Middle,
            /// <summary> An enum constant representing the low option. </summary>
            Low
        }

        /// <summary> A notification. </summary>
        public class Notification
        {
            /// <summary> The notification listener. </summary>
            protected NRNotificationListener NotificationListener;

            /// <summary> Constructor. </summary>
            /// <param name="listener"> The listener.</param>
            public Notification(NRNotificationListener listener)
            {
                this.NotificationListener = listener;
            }

            /// <summary> Updates the state. </summary>
            public virtual void UpdateState() { }

            /// <summary> Executes the 'state changed' action. </summary>
            /// <param name="level"> The level.</param>
            public virtual void OnStateChanged(Level level)
            {
                NotificationListener.Dispath(this, level);
            }
        }

        /// <summary> A low power notification. </summary>
        public class LowPowerNotification : Notification
        {
            /// <summary> Values that represent power states. </summary>
            public enum PowerState
            {
                /// <summary> An enum constant representing the full option. </summary>
                Full,
                /// <summary> An enum constant representing the middle option. </summary>
                Middle,
                /// <summary> An enum constant representing the low option. </summary>
                Low
            }
            /// <summary> The current state. </summary>
            private PowerState currentState = PowerState.Full;

            /// <summary> Constructor. </summary>
            /// <param name="listener"> The listener.</param>
            public LowPowerNotification(NRNotificationListener listener) : base(listener)
            {
            }

            /// <summary> Gets state by value. </summary>
            /// <param name="val"> The value.</param>
            /// <returns> The state by value. </returns>
            private PowerState GetStateByValue(float val)
            {
                if (val < 0.3f)
                {
                    return PowerState.Low;
                }
                else if (val < 0.4f)
                {
                    return PowerState.Middle;
                }
                return PowerState.Full;
            }

            /// <summary> Updates the state. </summary>
            public override void UpdateState()
            {
#if !UNITY_EDITOR
                var state = GetStateByValue(SystemInfo.batteryLevel);
#else
                var state = GetStateByValue(1f);
#endif

                if (state != currentState)
                {
                    if (state == PowerState.Low)
                    {
                        this.OnStateChanged(Level.High);
                    }
                    else if (state == PowerState.Middle)
                    {
                        this.OnStateChanged(Level.Middle);
                    }
                    this.currentState = state;
                }
            }
        }

        /// <summary> A slam state notification. </summary>
        public class SlamStateNotification : Notification
        {
            /// <summary> Values that represent slam states. </summary>
            private enum SlamState
            {
                /// <summary> An enum constant representing the none option. </summary>
                None,
                /// <summary> An enum constant representing the lost tracking option. </summary>
                LostTracking,
                /// <summary> An enum constant representing the tracking ready option. </summary>
                TrackingReady
            }
            /// <summary> The current state. </summary>
            private SlamState m_CurrentState = SlamState.None;

            /// <summary> Constructor. </summary>
            /// <param name="listener"> The listener.</param>
            public SlamStateNotification(NRNotificationListener listener) : base(listener)
            {
                NRHMDPoseTracker.OnHMDLostTracking += OnHMDLostTracking;
                NRHMDPoseTracker.OnHMDPoseReady += OnHMDPoseReady;
            }

            /// <summary> Executes the 'hmd pose ready' action. </summary>
            private void OnHMDPoseReady()
            {
                NRDebugger.Info("[SlamStateNotification] OnHMDPoseReady.");
                m_CurrentState = SlamState.TrackingReady;
            }

            /// <summary> Executes the 'hmd lost tracking' action. </summary>
            private void OnHMDLostTracking()
            {
                NRDebugger.Info("[SlamStateNotification] OnHMDLostTracking.");
                if (m_CurrentState != SlamState.LostTracking)
                {
                    this.OnStateChanged(Level.Middle);
                    m_CurrentState = SlamState.LostTracking;
                }
            }
        }

        /// <summary> A temperature level notification. </summary>
        public class TemperatureLevelNotification : Notification
        {
            /// <summary> The current state. </summary>
            private GlassesTemperatureLevel currentState = GlassesTemperatureLevel.TEMPERATURE_LEVEL_NORMAL;

            /// <summary> Constructor. </summary>
            /// <param name="listener"> The listener.</param>
            public TemperatureLevelNotification(NRNotificationListener listener) : base(listener)
            {
            }

            /// <summary> Updates the state. </summary>
            public override void UpdateState()
            {
                base.UpdateState();

                var level = NRDevice.Instance.TemperatureLevel;
                if (currentState != level)
                {
                    if (level != GlassesTemperatureLevel.TEMPERATURE_LEVEL_NORMAL)
                    {
                        this.OnStateChanged(level == GlassesTemperatureLevel.TEMPERATURE_LEVEL_HOT
                            ? Level.High : Level.Middle);
                    }

                    this.currentState = level;
                }
            }
        }

        /// <summary> True to enable, false to disable the low power tips. </summary>
        [Header("Whether to open the low power prompt")]
        public bool EnableLowPowerTips;
        /// <summary> The low power notification prefab. </summary>
        public NRNotificationWindow LowPowerNotificationPrefab;
        /// <summary> True to enable, false to disable the slam state tips. </summary>
        [Header("Whether to open the slam state prompt")]
        public bool EnableSlamStateTips;
        /// <summary> The slam state notification prefab. </summary>
        public NRNotificationWindow SlamStateNotificationPrefab;
        /// <summary> True to enable, false to disable the high temporary tips. </summary>
        [Header("Whether to open the over temperature prompt")]
        public bool EnableHighTempTips;
        /// <summary> The high temporary notification prefab. </summary>
        public NRNotificationWindow HighTempNotificationPrefab;

        /// <summary> List of notifications. </summary>
        protected List<Notification> NotificationList = new List<Notification>();
        /// <summary> The tips last time. </summary>
        private Dictionary<Level, float> TipsLastTime = new Dictionary<Level, float>() {
            { Level.High,3.5f},
            { Level.Middle,2.5f},
            { Level.Low,1.5f}
        };

        /// <summary> A notification message. </summary>
        public struct NotificationMsg
        {
            /// <summary> The notification. </summary>
            public Notification notification;
            /// <summary> The level. </summary>
            public Level level;
        }
        /// <summary> Queue of notifications. </summary>
        private Queue<NotificationMsg> NotificationQueue = new Queue<NotificationMsg>();
        /// <summary> The lock time. </summary>
        private float m_LockTime = 0f;
        private const float updateInterval = 1f;
        private float m_TimeLast = 0f;

        /// <summary> Awakes this object. </summary>
        void Awake()
        {
            LowPowerNotificationPrefab.gameObject.SetActive(false);
            SlamStateNotificationPrefab.gameObject.SetActive(false);
            HighTempNotificationPrefab.gameObject.SetActive(false);
        }

        /// <summary> Starts this object. </summary>
        public void Start()
        {
            DontDestroyOnLoad(gameObject);
            RegistNotification();
        }

        /// <summary> Regist notification. </summary>
        protected virtual void RegistNotification()
        {
            if (EnableLowPowerTips) NotificationList.Add(new LowPowerNotification(this));
            if (EnableSlamStateTips) NotificationList.Add(new SlamStateNotification(this));
            if (EnableHighTempTips) NotificationList.Add(new TemperatureLevelNotification(this));

            NRKernalUpdater.OnUpdate += OnUpdate;
        }

        /// <summary> Executes the 'update' action. </summary>
        private void OnUpdate()
        {
            m_TimeLast += Time.deltaTime;
            if (m_TimeLast < updateInterval)
            {
                return;
            }
            m_TimeLast = 0;

            for (int i = 0; i < NotificationList.Count; i++)
            {
                NotificationList[i].UpdateState();
            }

            if (m_LockTime < float.Epsilon)
            {
                if (NotificationQueue.Count != 0)
                {
                    var msg = NotificationQueue.Dequeue();
                    this.OprateNotificationMsg(msg);
                    m_LockTime = TipsLastTime[msg.level];
                }
            }
            else
            {
                m_LockTime -= updateInterval;
            }
        }

        /// <summary> Dispaths. </summary>
        /// <param name="notification"> The notification.</param>
        /// <param name="lev">          The level.</param>
        public void Dispath(Notification notification, Level lev)
        {
            NotificationQueue.Enqueue(new NotificationMsg()
            {
                notification = notification,
                level = lev
            });
        }

        /// <summary> Oprate notification message. </summary>
        /// <param name="msg"> The message.</param>
        protected virtual void OprateNotificationMsg(NotificationMsg msg)
        {
            NRNotificationWindow prefab = null;
            Notification notification_obj = msg.notification;
            Level notification_level = msg.level;
            float duration = TipsLastTime[notification_level];

            // Notification window will not be destroyed automatic when lowpower and high level warning
            // Set it's duration to -1
            if (notification_obj is LowPowerNotification)
            {
                prefab = LowPowerNotificationPrefab;
                if (notification_level == Level.High)
                {
                    duration = -1f;
                }
            }
            else if (notification_obj is SlamStateNotification)
            {
                prefab = SlamStateNotificationPrefab;
            }
            else if (notification_obj is TemperatureLevelNotification)
            {
                prefab = HighTempNotificationPrefab;
            }

            if (prefab != null)
            {
                NRDebugger.Info("[NRNotificationListener] Dispath:" + notification_obj.GetType().ToString());
                NRNotificationWindow notification = Instantiate(prefab);
                notification.gameObject.SetActive(true);
                notification.transform.SetParent(transform);
                notification.FillData(notification_level, duration);
            }
        }
    }
}