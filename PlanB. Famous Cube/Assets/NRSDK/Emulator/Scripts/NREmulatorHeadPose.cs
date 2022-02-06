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
#if UNITY_EDITOR
    using UnityEngine;
    public class NREmulatorHeadPose : MonoBehaviour
    {

        private GameObject m_CameraTarget;
        /// <summary> regular speed. </summary>
        public float HeadMoveSpeed = 1.0f;
        /// <summary> How sensitive it with mouse. </summary>
        public float HeadRotateSpeed = 1.0f;

        public Pose headPose { get; private set; }

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

        private void Update()
        {
            if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
            {
                UpdateHeadPosByInput();
            }
        }

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

            headPose = new Pose(pos, q);
        }

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
#endif
}