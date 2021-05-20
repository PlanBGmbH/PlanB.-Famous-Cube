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
    using System.Collections.Generic;

    /// <summary> A native camera proxy. </summary>
    public class NativeCameraProxy
    {
        /// <summary> The identifier. </summary>
        public static string ID = "NativeCameraProxy";
        /// <summary> Gets or sets the camera data provider. </summary>
        /// <value> The camera data provider. </value>
        protected ICameraDataProvider CameraDataProvider { get; private set; }
        /// <summary> The texture pointer. </summary>
        protected IntPtr m_TexturePtr = IntPtr.Zero;
        /// <summary> The current frame. </summary>
        protected FrameRawData m_CurrentFrame;
        /// <summary> Gets the resolution. </summary>
        /// <value> The resolution. </value>
        public virtual NativeResolution Resolution { get; }
        /// <summary> True if is initialized, false if not. </summary>
        private bool m_IsInitialized = false;
        /// <summary> The last frame. </summary>
        protected int m_LastFrame = -1;
        /// <summary> True if is playing, false if not. </summary>
        protected bool m_IsPlaying = false;
        /// <summary> Gets a value indicating whether this object is camera playing. </summary>
        /// <value> True if this object is camera playing, false if not. </value>
        public bool IsCamPlaying
        {
            get
            {
                return m_IsPlaying;
            }
        }

        /// <summary> Queue of fixed sized. </summary>
        public class FixedSizedQueue
        {
            /// <summary> The queue. </summary>
            private Queue<FrameRawData> m_Queue = new Queue<FrameRawData>();
            /// <summary> The lock object. </summary>
            private object m_LockObj = new object();
            /// <summary> The object pool. </summary>
            private ObjectPool m_ObjectPool;

            /// <summary> Constructor. </summary>
            /// <param name="pool"> The pool.</param>
            public FixedSizedQueue(ObjectPool pool)
            {
                m_ObjectPool = pool;
            }

            /// <summary> Gets or sets the limit. </summary>
            /// <value> The limit. </value>
            public int Limit { get; set; }

            /// <summary> Gets the number of. </summary>
            /// <value> The count. </value>
            public int Count
            {
                get
                {
                    lock (m_LockObj)
                    {
                        return m_Queue.Count;
                    }
                }
            }

            /// <summary> Adds an object onto the end of this queue. </summary>
            /// <param name="obj"> The object.</param>
            public void Enqueue(FrameRawData obj)
            {
                lock (m_LockObj)
                {
                    m_Queue.Enqueue(obj);
                    if (m_Queue.Count > Limit)
                    {
                        var frame = m_Queue.Dequeue();
                        m_ObjectPool.Put<FrameRawData>(frame);
                    }
                }

            }

            /// <summary> Removes the head object from this queue. </summary>
            /// <returns> The head object from this queue. </returns>
            public FrameRawData Dequeue()
            {
                lock (m_LockObj)
                {
                    var frame = m_Queue.Dequeue();
                    m_ObjectPool.Put<FrameRawData>(frame);
                    return frame;
                }
            }
        }
        /// <summary> The camera frames. </summary>
        protected FixedSizedQueue m_CameraFrames;
        /// <summary> The frame pool. </summary>
        protected ObjectPool FramePool;
        /// <summary> The active textures. </summary>
        protected List<CameraModelView> m_ActiveTextures;

        /// <summary> Specialized constructor for use only by derived class. </summary>
        /// <param name="provider"> The provider.</param>
        protected NativeCameraProxy(ICameraDataProvider provider)
        {
            CameraDataProvider = provider;
            this.Initialize();
        }

        /// <summary> Initializes this object. </summary>
        public virtual void Initialize()
        {
            if (m_IsInitialized)
            {
                return;
            }
            NRDebugger.Info("[CameraController] Initialize");
            if (FramePool == null)
            {
                FramePool = new ObjectPool();
                FramePool.InitCount = 10;
            }
            if (m_CameraFrames == null)
            {
                m_CameraFrames = new FixedSizedQueue(FramePool);
                m_CameraFrames.Limit = 5;
            }

            m_ActiveTextures = new List<CameraModelView>();
            CameraDataProvider.Create();
            m_IsInitialized = true;
        }

        /// <summary> Callback, called when the regist capture. </summary>
        /// <param name="callback"> The callback.</param>
        protected void RegistCaptureCallback(CameraImageCallback callback)
        {
            CameraDataProvider.SetCaptureCallback(callback);
        }

        /// <summary> Regists the given tex. </summary>
        /// <param name="tex"> The tex to remove.</param>
        public void Regist(CameraModelView tex)
        {
            m_ActiveTextures.Add(tex);
        }

        /// <summary> Removes the given tex. </summary>
        /// <param name="tex"> The tex to remove.</param>
        public void Remove(CameraModelView tex)
        {
            int index = -1;
            for (int i = 0; i < m_ActiveTextures.Count; i++)
            {
                if (tex == m_ActiveTextures[i])
                {
                    index = i;
                }
            }
            if (index != -1)
            {
                m_ActiveTextures.RemoveAt(index);
            }
        }

        /// <summary> Sets image format. </summary>
        /// <param name="format"> Describes the format to use.</param>
        public void SetImageFormat(CameraImageFormat format)
        {
#if !UNITY_EDITOR
            CameraDataProvider.SetImageFormat(format);
#endif
            NRDebugger.Info("[CameraController] SetImageFormat : " + format.ToString());
        }

        /// <summary> Start to play camera. </summary>
        public void Play()
        {
            if (!m_IsInitialized)
            {
                Initialize();
            }
            if (m_IsPlaying)
            {
                return;
            }
            NRDebugger.Info("[CameraController] Start to play");
#if !UNITY_EDITOR
            CameraDataProvider.StartCapture();
#endif
            m_IsPlaying = true;
        }

        /// <summary> Query if this object has frame. </summary>
        /// <returns> True if frame, false if not. </returns>
        public bool HasFrame()
        {
            return m_IsPlaying && (m_CameraFrames.Count > 0 || m_CurrentFrame.data != null);
        }

        /// <summary> Gets the frame. </summary>
        /// <returns> The frame. </returns>
        public FrameRawData GetFrame()
        {
            if (Time.frameCount != m_LastFrame && m_CameraFrames.Count > 0)
            {
                m_CurrentFrame = m_CameraFrames.Dequeue();
                m_LastFrame = Time.frameCount;
            }

            return m_CurrentFrame;
        }

        /// <summary> Updates the frame. </summary>
        /// <param name="camera_handle">       Handle of the camera.</param>
        /// <param name="camera_image_handle"> Handle of the camera image.</param>
        /// <param name="userdata">            The userdata.</param>
        public virtual void UpdateFrame(UInt64 camera_handle, UInt64 camera_image_handle, UInt64 userdata) { }

        /// <summary> Queue frame. </summary>
        /// <param name="textureptr"> The textureptr.</param>
        /// <param name="size">       The size.</param>
        /// <param name="timestamp">  The timestamp.</param>
        protected void QueueFrame(IntPtr textureptr, int size, UInt64 timestamp)
        {
            if (!m_IsPlaying)
            {
                NRDebugger.Error("camera was not stopped properly, it still sending data.");
                return;
            }
            FrameRawData frame = FramePool.Get<FrameRawData>();
            bool result = FrameRawData.MakeSafe(m_TexturePtr, size, timestamp, ref frame);
            if (result)
            {
                m_CameraFrames.Enqueue(frame);
            }
            else
            {
                FramePool.Put<FrameRawData>(frame);
            }
        }

        /// <summary> Stop the camera. </summary>
        public virtual void Stop()
        {
            if (!m_IsPlaying)
            {
                return;
            }
            NRDebugger.Info("[CameraController] Start to Stop");

            // If there is no a active texture, pause and release camera resource.
            if (m_ActiveTextures.Count == 0)
            {
                m_IsPlaying = false;
#if !UNITY_EDITOR
                CameraDataProvider.StopCapture();
#endif
                Release();
            }
        }

        /// <summary> Release the camera. </summary>
        public virtual void Release()
        {
            if (CameraDataProvider == null)
            {
                return;
            }

            NRDebugger.Info("[CameraController] Start to Release");

            //#if !UNITY_EDITOR
            //            CameraDataProvider.Release();
            //            CameraDataProvider = null;
            //#endif
            m_CurrentFrame.data = null;
            m_IsInitialized = false;
            m_IsPlaying = false;
        }
    }
}
