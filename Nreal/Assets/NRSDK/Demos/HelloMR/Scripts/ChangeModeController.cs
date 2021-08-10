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
            NRSessionManager.Instance.NRHMDPoseTracker.ChangeTo3Dof((result) =>
            {
                NRDebugger.Info("[ChangeModeController] ChangeTo3Dof result:" + result.success);
            });
        }

        public void ChangeTo6Dof()
        {
            NRSessionManager.Instance.NRHMDPoseTracker.ChangeTo6Dof((result) =>
            {
                NRDebugger.Info("[ChangeModeController] ChangeTo6Dof result:" + result.success);
            });
        }
    }
}
