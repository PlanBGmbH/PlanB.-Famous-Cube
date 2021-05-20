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
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using UnityEngine;

    /// <summary> A main thread dispather. </summary>
    [ExecuteInEditMode]
    public class MainThreadDispather : MonoBehaviour
    {
        /// <summary> A delayed queue item. </summary>
        public class DelayedQueueItem
        {
            /// <summary> The time. </summary>
            public float time;

            /// <summary> The action. </summary>
            public Action action;
        }

        /// <summary> The current. </summary>
        private static MainThreadDispather _current;

        /// <summary> Number of. </summary>
        private int _count;

        /// <summary> True once initialization is complete. </summary>
        private static bool _initialized;

        /// <summary> Identifier for the thread. </summary>
        private static int _threadId = -1;

        /// <summary> The actions. </summary>
        private List<Action> _actions = new List<Action>();

        /// <summary> The delayed. </summary>
        private List<MainThreadDispather.DelayedQueueItem> _delayed = new List<MainThreadDispather.DelayedQueueItem>();

        /// <summary> Gets the current. </summary>
        /// <value> The current. </value>
        public static MainThreadDispather Current
        {
            get
            {
                if (!MainThreadDispather._initialized)
                {
                    MainThreadDispather.Initialize();
                }
                return MainThreadDispather._current;
            }
        }

        /// <summary> Initializes this object. </summary>
        public static void Initialize()
        {
            bool flag = !MainThreadDispather._initialized;
            if (flag && MainThreadDispather._threadId != -1 && MainThreadDispather._threadId != Thread.CurrentThread.ManagedThreadId)
            {
                return;
            }
            if (flag)
            {
                GameObject gameObject = new GameObject("MainThreadDispather");
                gameObject.hideFlags = HideFlags.DontSave;
                UnityEngine.Object.DontDestroyOnLoad(gameObject);
                if (MainThreadDispather._current)
                {
                    if (Application.isPlaying)
                    {
                        UnityEngine.Object.Destroy(MainThreadDispather._current.gameObject);
                    }
                    else
                    {
                        UnityEngine.Object.DestroyImmediate(MainThreadDispather._current.gameObject);
                    }
                }
                MainThreadDispather._current = gameObject.AddComponent<MainThreadDispather>();
                UnityEngine.Object.DontDestroyOnLoad(MainThreadDispather._current);
                MainThreadDispather._initialized = true;
                MainThreadDispather._threadId = Thread.CurrentThread.ManagedThreadId;
            }
        }

        /// <summary> Executes the 'destroy' action. </summary>
        private void OnDestroy()
        {
            MainThreadDispather._initialized = false;
        }

        /// <summary> Queue on main thread. </summary>
        /// <param name="action"> The action.</param>
        public static void QueueOnMainThread(Action action)
        {
            MainThreadDispather.QueueOnMainThread(action, 0f);
        }

        /// <summary> Queue on main thread. </summary>
        /// <param name="action"> The action.</param>
        /// <param name="time">   The time.</param>
        public static void QueueOnMainThread(Action action, float time)
        {
            if (time != 0f)
            {
                List<MainThreadDispather.DelayedQueueItem> delayed = MainThreadDispather.Current._delayed;
                lock (delayed)
                {
                    MainThreadDispather.Current._delayed.Add(new MainThreadDispather.DelayedQueueItem
                    {
                        time = Time.time + time,
                        action = action
                    });
                }
            }
            else
            {
                List<Action> actions = MainThreadDispather.Current._actions;
                lock (actions)
                {
                    MainThreadDispather.Current._actions.Add(action);
                }
            }
        }

        /// <summary> Executes the 'asynchronous' operation. </summary>
        /// <param name="action"> The action.</param>
        public static void RunAsync(Action action)
        {
            new Thread(new ParameterizedThreadStart(MainThreadDispather.RunAction))
            {
                Priority = System.Threading.ThreadPriority.Normal
            }.Start(action);
        }

        /// <summary> Executes the action. </summary>
        /// <param name="action"> The action.</param>
        private static void RunAction(object action)
        {
            ((Action)action)?.Invoke();
        }

        /// <summary> Updates this object. </summary>
        private void Update()
        {
            List<Action> actions = this._actions;
            if (actions.Count > 0)
            {
                lock (actions)
                {
                    for (int i = 0; i < this._actions.Count; i++)
                    {
                        this._actions[i]();
                    }
                    this._actions.Clear();
                }
            }

            List<MainThreadDispather.DelayedQueueItem> delayed = this._delayed;
            if (delayed.Count > 0)
            {
                lock (delayed)
                {
                    for (int j = 0; j < this._delayed.Count; j++)
                    {
                        MainThreadDispather.DelayedQueueItem delayedQueueItem = this._delayed[j];
                        if (delayedQueueItem.time <= Time.time)
                        {
                            delayedQueueItem.action();
                            this._delayed.RemoveAt(j);
                            j--;
                        }
                    }
                }
            }
        }
    }
}
