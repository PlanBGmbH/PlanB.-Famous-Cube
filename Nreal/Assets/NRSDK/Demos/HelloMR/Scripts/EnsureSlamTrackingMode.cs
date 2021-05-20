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
    /// <summary> An ensure slam tracking mode. </summary>
    public class EnsureSlamTrackingMode : MonoBehaviour
    {
        /// <summary> Type of the tracking. </summary>
        [SerializeField]
        private NRHMDPoseTracker.TrackingType m_TrackingType = NRHMDPoseTracker.TrackingType.Tracking6Dof;

        /// <summary> Starts this object. </summary>
        void Start()
        {
            switch (m_TrackingType)
            {
                case NRHMDPoseTracker.TrackingType.Tracking6Dof:
                    NRSessionManager.Instance.NRHMDPoseTracker.ChangeTo6Dof();
                    break;
                case NRHMDPoseTracker.TrackingType.Tracking3Dof:
                    NRSessionManager.Instance.NRHMDPoseTracker.ChangeTo3Dof();
                    break;
                case NRHMDPoseTracker.TrackingType.Tracking0Dof:
                    NRSessionManager.Instance.NRHMDPoseTracker.ChangeTo0Dof();
                    break;
                default:
                    break;
            }
        }
    }
}