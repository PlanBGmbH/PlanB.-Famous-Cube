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
    /// <summary> A nr emulator head pose. </summary>
    public class NREmulatorHeadPose : MonoBehaviour
    {
        /// <summary> The camera target. </summary>
        private GameObject m_CameraTarget;

        /// <summary> regular speed. </summary>
        public float HeadMoveSpeed = 1.0f;
        /// <summary> How sensitive it with mouse. </summary>
        public float HeadRotateSpeed = 1.0f;

#if UNITY_EDITOR
        /// <summary> Starts this object. </summary>
        private void Start()
        {
            DontDestroyOnLoad(this);
            if (m_CameraTarget == null)
            {
                m_CameraTarget = new GameObject("NREmulatorCameraTarget");
                m_CameraTarget.transform.position = Vector3.zero;
                m_CameraTarget.transform.rotation = Quaternion.identity;
                DontDestroyOnLoad(m_CameraTarget);
            }
        }
#endif

#if UNITY_EDITOR
        /// <summary> Updates this object. </summary>
        private void Update()
        {
            if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
            {
                UpdateHeadPosByInput();
            }
        }
#endif

        /// <summary> Updates the head position by input. </summary>
        void UpdateHeadPosByInput()
        {
            Quaternion q = m_CameraTarget.transform.rotation;
            if (Input.GetKey(KeyCode.Space))
            {
                float mouse_x = Input.GetAxis("Mouse X") * HeadRotateSpeed;
                float mouse_y = Input.GetAxis("Mouse Y") * HeadRotateSpeed;
                Vector3 mouseMove = new Vector3(m_CameraTarget.transform.eulerAngles.x - mouse_y, m_CameraTarget.transform.eulerAngles.y + mouse_x, 0);
                q = Quaternion.Euler(mouseMove);
                m_CameraTarget.transform.rotation = q;
            }

            Vector3 p = GetBaseInput();
            p = p * HeadMoveSpeed * Time.deltaTime;
            Vector3 pos = p + m_CameraTarget.transform.position;
            m_CameraTarget.transform.position = pos;

            NREmulatorManager.Instance?.NativeEmulatorApi?.SetHeadTrackingPose(pos, q);
        }

        /// <summary> Gets base input. </summary>
        /// <returns> The base input. </returns>
        private Vector3 GetBaseInput()
        {
            Vector3 p_Velocity = new Vector3();
            if (Input.GetKey(KeyCode.W))
            {
                p_Velocity += m_CameraTarget.transform.forward.normalized;
            }
            if (Input.GetKey(KeyCode.S))
            {
                p_Velocity += -m_CameraTarget.transform.forward.normalized;
            }
            if (Input.GetKey(KeyCode.A))
            {
                p_Velocity += -m_CameraTarget.transform.right.normalized;
            }
            if (Input.GetKey(KeyCode.D))
            {
                p_Velocity += m_CameraTarget.transform.right.normalized;
            }
            return p_Velocity;
        }
    }
}