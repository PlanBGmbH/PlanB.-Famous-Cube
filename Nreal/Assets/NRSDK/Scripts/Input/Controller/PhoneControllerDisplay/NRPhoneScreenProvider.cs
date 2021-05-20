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
    using UnityEngine;

    /// <summary> A system button state. </summary>
    public class SystemInputState
    {
        /// <summary> TRIGGER APP HOME. </summary>
        public bool[] buttons = new bool[3];
        /// <summary> The touch. </summary>
        public Vector2 touch;
        /// <summary> The origin touch. </summary>
        public Vector2 originTouch;

        /// <summary> True to pressing. </summary>
        public bool pressing;
        /// <summary> True to press down. </summary>
        public bool pressDown;
        /// <summary> True to press up. </summary>
        public bool pressUp;

        public override string ToString()
        {
            return string.Format("buttons {0} {1} {2} touch x:{3} touch y:{4}", buttons[0], buttons[1], buttons[2], touch.x, touch.y);
        }
    }

    public class SystemButtonState
    {
        /// <summary> APP TRIGGER HOME. </summary>
        public bool[] buttons = new bool[3];
        public float touch_x;
        public float touch_y;

        public SystemInputState TransformTo(SystemInputState unitystate)
        {
            if (unitystate == null)
            {
                unitystate = new SystemInputState();
            }
            unitystate.buttons[0] = this.buttons[1];
            unitystate.buttons[1] = this.buttons[0];
            unitystate.buttons[2] = this.buttons[2];
            unitystate.touch = new Vector2(this.touch_x, this.touch_y);
            return unitystate;
        }

        public SystemButtonState DeSerialize(byte[] data)
        {
            buttons[0] = Convert.ToBoolean(data[0]);
            buttons[1] = Convert.ToBoolean(data[1]);
            buttons[2] = Convert.ToBoolean(data[2]);
            touch_x = ConvertUtility.IntBitsToFloat(BitConverter.ToInt32(data, 3));
            touch_y = ConvertUtility.IntBitsToFloat(BitConverter.ToInt32(data, 7));
            return this;
        }

        public override string ToString()
        {
            return string.Format("buttons {0} {1} {2} touch x:{3} touch y:{4}", buttons[0], buttons[1], buttons[2], touch_x, touch_y);
        }
    }

    public interface ISystemButtonStateProvider
    {
        void BindReceiver(ISystemButtonStateReceiver receiver);
    }

    public interface ISystemButtonStateReceiver
    {
        void OnDataReceived(SystemButtonState state);
    }

    public interface ISystemButtonDataProxy
    {
        void OnUpdate(AndroidJavaObject state);
    }

    public class NRPhoneScreenProviderBase : ISystemButtonStateProvider
    {
        protected ISystemButtonDataProxy m_AndroidSystemButtonDataProxy;
        protected ISystemButtonStateReceiver m_Receiver;
        protected SystemInputState m_SystemButtonState = new SystemInputState();
        private AndroidJavaObject m_UnityActivity;

        public NRPhoneScreenProviderBase()
        {
            AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            m_UnityActivity = cls_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            m_AndroidSystemButtonDataProxy = CreateAndroidDataProxy();
            this.RegistFragment(m_UnityActivity, m_AndroidSystemButtonDataProxy);
        }

        public void BindReceiver(ISystemButtonStateReceiver receiver)
        {
            this.m_Receiver = receiver;
        }

        public virtual void RegistFragment(AndroidJavaObject unityActivity, ISystemButtonDataProxy proxy) { }

        public virtual ISystemButtonDataProxy CreateAndroidDataProxy() { return null; }

        public virtual void OnSystemButtonDataChanged(SystemButtonState state)
        {
            this.m_Receiver?.OnDataReceived(state);
        }

        public virtual void ResizeView(int w, int h)
        {
            this.RegistFragment(m_UnityActivity, m_AndroidSystemButtonDataProxy);
        }
    }
}