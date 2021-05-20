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

    public class NRDefaultPhoneScreenProvider : NRPhoneScreenProviderBase
    {
        public class AndroidSystemButtonDataProxy : AndroidJavaProxy, ISystemButtonDataProxy
        {
            private NRPhoneScreenProviderBase m_Provider;

            public AndroidSystemButtonDataProxy(NRPhoneScreenProviderBase provider) : base("ai.nreal.virtualcontroller.ISystemButtonDataReceiver")
            {
                this.m_Provider = provider;
            }

            public void OnUpdate(AndroidJavaObject data)
            {
                SystemButtonState state = new SystemButtonState();
                byte[] sbuffer = data.Call<byte[]>("getRawData");
                //byte[] Bytes = new byte[sbuffer.Length];
                //Buffer.BlockCopy(sbuffer, 0, Bytes, 0, Bytes.Length);
                state.DeSerialize(sbuffer);
                m_Provider.OnSystemButtonDataChanged(state);
            }
        }

        public override void RegistFragment(AndroidJavaObject unityActivity, ISystemButtonDataProxy proxy)
        {
            AndroidJavaClass VirtualDisplayFragment = new AndroidJavaClass("ai.nreal.virtualcontroller.VirtualControllerFragment");
            VirtualDisplayFragment.CallStatic<AndroidJavaObject>("RegistFragment", unityActivity, proxy);
        }

        public override ISystemButtonDataProxy CreateAndroidDataProxy()
        {
            return new AndroidSystemButtonDataProxy(this);
        }
    }
}
