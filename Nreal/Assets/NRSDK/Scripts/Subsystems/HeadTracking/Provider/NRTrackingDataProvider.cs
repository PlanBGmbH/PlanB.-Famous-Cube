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

    public class NRTrackingDataProvider : ITrackingDataProvider
    {
        NativeHeadTracking m_NativeHeadTracking;
        NativeTracking m_NativeTracking;

        public NRTrackingDataProvider()
        {
            if (NRSessionManager.Instance.NativeAPI == null)
            {
                NRSessionManager.Instance.NativeAPI = new NativeInterface();
            }
            m_NativeHeadTracking = NRSessionManager.Instance.NativeAPI.NativeHeadTracking;
            m_NativeTracking = NRSessionManager.Instance.NativeAPI.NativeTracking;

            m_NativeTracking.Create();
        }

        public void Start()
        {
            m_NativeTracking.Start();
            m_NativeHeadTracking.Start();
        }

        public void Pause()
        {
            m_NativeTracking.Pause();
        }

        public void Recenter()
        {
            m_NativeTracking.Recenter();
        }

        public void Resume()
        {
            m_NativeTracking.Resume();
        }

        public void Stop()
        {
            m_NativeHeadTracking.Destroy();
            m_NativeTracking.Destroy();
        }

        public bool SwitchTrackingMode(TrackingMode mode)
        {
            return m_NativeTracking.SwitchTrackingMode(mode);
        }

        public bool GetFramePresentHeadPose(ref Pose pose, ref ulong timestamp)
        {
            return m_NativeHeadTracking.GetFramePresentHeadPose(ref pose, ref timestamp);
        }

        public bool GetHeadPose(ref Pose pose, ulong timestamp)
        {
            return m_NativeHeadTracking.GetHeadPose(ref pose, timestamp);
        }

        public LostTrackingReason GetTrackingLostReason()
        {
            return m_NativeHeadTracking.GetTrackingLostReason();
        }

        public bool InitTrackingMode(TrackingMode mode)
        {
            return m_NativeTracking.InitTrackingMode(mode);
        }

        public ulong GetHMDTimeNanos()
        {
            return m_NativeHeadTracking.GetHMDTimeNanos();
        }
    }
}
