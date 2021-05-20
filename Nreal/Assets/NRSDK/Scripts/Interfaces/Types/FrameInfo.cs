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
    using System.Runtime.InteropServices;
    using UnityEngine;

    /// @cond EXCLUDE_FROM_DOXYGEN
    [StructLayout(LayoutKind.Sequential)]
    public struct FrameInfo
    {
        [MarshalAs(UnmanagedType.SysInt)]
        public IntPtr leftTex;
        [MarshalAs(UnmanagedType.SysInt)]
        public IntPtr rightTex;
        [MarshalAs(UnmanagedType.Struct)]
        public NativeMat4f pose;
        [MarshalAs(UnmanagedType.Struct)]
        public NativeVector3f focusPosition;
        [MarshalAs(UnmanagedType.Struct)]
        public NativeVector3f normalPosition;

        public FrameInfo(IntPtr left, IntPtr right, NativeMat4f p, Vector3 focuspos,Vector3 normal)
        {
            this.leftTex = left;
            this.rightTex = right;
            this.pose = p;
            this.focusPosition = new NativeVector3f(focuspos);
            this.normalPosition = new NativeVector3f(normal);
        }
    }
    /// @endcond
}
