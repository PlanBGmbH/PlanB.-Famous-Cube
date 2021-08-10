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
        private Matrix4x4 RGBCameraMatrix = new Matrix4x4(
                   new Vector4(1.92188f, 0f, 0f, 0f),
                   new Vector4(0f, 3.41598f, 0f, 0f),
                   new Vector4(0.0169f, 0.02256f, -1.00060f, -1f),
                   new Vector4(0, 0f, -0.60018f, 0f)
           );

        private Pose RGBCameraPoseFromHead = new Pose(Vector3.zero, new Quaternion(0.1f, 0.0f, 0.0f, 1.0f));
#endif

        /// <summary> Starts this object. </summary>
        void Start()
        {
            m_TargetCamera = gameObject.GetComponent<Camera>();

#if UNITY_EDITOR
            m_TargetCamera.projectionMatrix = RGBCameraMatrix;
            transform.localPosition = RGBCameraPoseFromHead.position;
            transform.localRotation = RGBCameraPoseFromHead.rotation;
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
                NRDebugger.Info("[NRCameraInitializer] Waitting to initialize camera param.");
                yield return new WaitForEndOfFrame();
                matrix_data = NRFrame.GetEyeProjectMatrix(out result, m_TargetCamera.nearClipPlane, m_TargetCamera.farClipPlane);
            }

            var eyeposFromHead = NRFrame.EyePoseFromHead;
            switch (EyeType)
            {
                case NativeEye.LEFT:
                    m_TargetCamera.projectionMatrix = matrix_data.LEyeMatrix;
                    NRDebugger.Info("[NRCameraInitializer] Left Camera Project Matrix :" + m_TargetCamera.projectionMatrix.ToString());
                    transform.localPosition = eyeposFromHead.LEyePose.position;
                    transform.localRotation = eyeposFromHead.LEyePose.rotation;
                    NRDebugger.Info("[NRCameraInitializer] Left Camera pos:{0} rotation:{1}", transform.localPosition.ToString(), transform.localRotation.ToString());
                    break;
                case NativeEye.RIGHT:
                    m_TargetCamera.projectionMatrix = matrix_data.REyeMatrix;
                    NRDebugger.Info("[NRCameraInitializer] Right Camera Project Matrix :" + m_TargetCamera.projectionMatrix.ToString());
                    transform.localPosition = eyeposFromHead.REyePose.position;
                    transform.localRotation = eyeposFromHead.REyePose.rotation;
                    NRDebugger.Info("[NRCameraInitializer] Right Camera pos:{0} rotation:{1}", transform.localPosition.ToString(), transform.localRotation.ToString());
                    break;
                case NativeEye.RGB:
                    m_TargetCamera.projectionMatrix = matrix_data.RGBEyeMatrix;
                    NRDebugger.Info("[NRCameraInitializer] RGB Camera Project Matrix :" + m_TargetCamera.projectionMatrix.ToString());
                    transform.localPosition = eyeposFromHead.RGBEyePose.position;
                    transform.localRotation = eyeposFromHead.RGBEyePose.rotation;
                    NRDebugger.Info("[NRCameraInitializer] RGB Camera pos:{0} rotation:{1}", transform.localPosition.ToString(), transform.localRotation.ToString());
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
