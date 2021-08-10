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
    using UnityEngine.Rendering;

    [RequireComponent(typeof(Camera))]
    public class NRBackGroundRender : MonoBehaviour
    {
        /// <summary> A material used to render the AR background image. </summary>
        [Tooltip("A material used to render the AR background image.")]
        public Material BackgroundMaterial;
        private Camera m_Camera;
        private CameraClearFlags m_CameraClearFlags = CameraClearFlags.Skybox;
        private CommandBuffer m_CommandBuffer = null;

        private void OnEnable()
        {
            if (BackgroundMaterial == null)
            {
                NRDebugger.Error("[NRBackGroundRender] Material is null...");
                return;
            }
            m_Camera = GetComponent<Camera>();
            EnableARBackgroundRendering();
        }

        private void OnDisable()
        {
            DisableARBackgroundRendering();
        }

        public void SetMaterial(Material mat)
        {
            BackgroundMaterial = mat;
        }

        private void EnableARBackgroundRendering()
        {
            if (BackgroundMaterial == null || m_Camera == null)
            {
                return;
            }

            m_CameraClearFlags = m_Camera.clearFlags;
            m_Camera.clearFlags = CameraClearFlags.Depth;

            m_CommandBuffer = new CommandBuffer();
            m_CommandBuffer.Blit(null, BuiltinRenderTextureType.CameraTarget, BackgroundMaterial);

            m_Camera.AddCommandBuffer(CameraEvent.BeforeForwardOpaque, m_CommandBuffer);
            m_Camera.AddCommandBuffer(CameraEvent.BeforeGBuffer, m_CommandBuffer);
        }

        private void DisableARBackgroundRendering()
        {
            if (m_CommandBuffer == null || m_Camera == null)
            {
                return;
            }

            m_Camera.clearFlags = m_CameraClearFlags;

            m_Camera.RemoveCommandBuffer(CameraEvent.BeforeForwardOpaque, m_CommandBuffer);
            m_Camera.RemoveCommandBuffer(CameraEvent.BeforeGBuffer, m_CommandBuffer);
        }
    }
}
