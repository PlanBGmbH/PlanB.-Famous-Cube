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
    public class ChangeModeController : MonoBehaviour
    {
        public void ChangeTo0Dof()
        {
            NRSessionManager.Instance.NRHMDPoseTracker.ChangeTo0Dof((result) =>
            {
                NRDebugger.Info("[ChangeModeController] ChangeTo0Dof result:" + result.success);
            });
        }


        public void ChangeTo3Dof()
        {
            var hmdPoseTracker = NRSessionManager.Instance.NRHMDPoseTracker;
            NRSessionManager.Instance.NRHMDPoseTracker.ChangeTo3Dof((result) =>
            {
                NRDebugger.Info("[ChangeModeController] ChangeTo3Dof result:" + result.success);
                if (result.success)
                {
                    hmdPoseTracker.CacheWorldMatrix();
                }
            });
        }

        public void ChangeTo6Dof()
        {
            var hmdPoseTracker = NRSessionManager.Instance.NRHMDPoseTracker;
            NRSessionManager.Instance.NRHMDPoseTracker.ChangeTo6Dof((result) =>
            {
                NRDebugger.Info("[ChangeModeController] ChangeTo6Dof result:" + result.success);
                if (result.success)
                {
                    hmdPoseTracker.CacheWorldMatrix();
                }
            });
        }
    }
}
