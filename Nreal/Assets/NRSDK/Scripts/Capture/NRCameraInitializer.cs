/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRKernal.Record
{
    using NRKernal;
    using UnityEngine;
    using System.Collections;

    /// <summary> A nr camera initializer. </summary>
    [RequireComponent(typeof(Camera))]
    public class NRCameraInitializer : MonoBehaviour
    {
        /// <summary> Target camera. </summary>
        private Camera m_TargetCamera;
        /// <summary> Type of the eye. </summary>
        [SerializeField]
        private NativeEye EyeType = NativeEye.RGB;

#if UNITY_EDITOR
        /// <summary> The matrix. </summary>
        private Matrix4x4 matrix = new Matrix4x4(
                   new Vector4(1.92188f, 0f, 0f, 0f),
                   new Vector4(0f, 3.41598f, 0f, 0f),
                   new Vector4(0.0169f, 0.02256f, -1.00060f, -1f),
                   new Vector4(0, 0f, -0.60018f, 0f)
           );
#endif

        /// <summary> Starts this object. </summary>
        void Start()
        {
            m_TargetCamera = gameObject.GetComponent<Camera>();

#if UNITY_EDITOR
            m_TargetCamera.projectionMatrix = matrix;
#else
            StartCoroutine(Initialize());
#endif
        }

        /// <summary> Initializes this object. </summary>
        /// <returns> An IEnumerator. </returns>
        private IEnumerator Initialize()
        {
            bool result;

            EyeProjectMatrixData matrix_data = NRFrame.GetEyeProjectMatrix(out result, m_TargetCamera.nearClipPlane, m_TargetCamera.farClipPlane);
            while (!result)
            {
                NRDebugger.Info("Waitting to initialize camera param.");
                yield return new WaitForEndOfFrame();
                matrix_data = NRFrame.GetEyeProjectMatrix(out result, m_TargetCamera.nearClipPlane, m_TargetCamera.farClipPlane);
            }

            var eyeposFromHead = NRFrame.EyePoseFromHead;
            switch (EyeType)
            {
                case NativeEye.LEFT:
                    m_TargetCamera.projectionMatrix = matrix_data.LEyeMatrix;
                    NRDebugger.Info("[Matrix] RGB Camera Project Matrix :" + m_TargetCamera.projectionMatrix.ToString());
                    transform.localPosition = eyeposFromHead.LEyePose.position;
                    transform.localRotation = eyeposFromHead.LEyePose.rotation;
                    NRDebugger.Info("RGB Camera pos:{0} rotation:{1}", transform.localPosition.ToString(), transform.localRotation.ToString());
                    break;
                case NativeEye.RIGHT:
                    m_TargetCamera.projectionMatrix = matrix_data.REyeMatrix;
                    NRDebugger.Info("[Matrix] RGB Camera Project Matrix :" + m_TargetCamera.projectionMatrix.ToString());
                    transform.localPosition = eyeposFromHead.REyePose.position;
                    transform.localRotation = eyeposFromHead.REyePose.rotation;
                    NRDebugger.Info("RGB Camera pos:{0} rotation:{1}", transform.localPosition.ToString(), transform.localRotation.ToString());
                    break;
                case NativeEye.RGB:
                    m_TargetCamera.projectionMatrix = matrix_data.RGBEyeMatrix;
                    NRDebugger.Info("[Matrix] RGB Camera Project Matrix :" + m_TargetCamera.projectionMatrix.ToString());
                    transform.localPosition = eyeposFromHead.RGBEyePos.position;
                    transform.localRotation = eyeposFromHead.RGBEyePos.rotation;
                    NRDebugger.Info("RGB Camera pos:{0} rotation:{1}", transform.localPosition.ToString(), transform.localRotation.ToString());
                    break;
                default:
                    break;
            }
        }

        /// <summary> Switch to eye parameter. </summary>
        /// <param name="eye"> The eye.</param>
        public void SwitchToEyeParam(NativeEye eye)
        {
            EyeType = eye;
#if !UNITY_EDITOR
            StartCoroutine(Initialize());
#endif
        }
    }
}
