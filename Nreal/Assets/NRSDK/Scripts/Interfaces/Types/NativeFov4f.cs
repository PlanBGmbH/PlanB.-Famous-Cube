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
    using System.Runtime.InteropServices;

    /// <summary> A native fov 4f. </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct NativeFov4f
    {
        /// <summary>
        /// The tangent of the angle between the viewing vector and the left edge of the field of view.
        /// The value is positive. </summary>
        [MarshalAs(UnmanagedType.R4)]
        public float left_tan;

        /// <summary>
        /// The tangent of the angle between the viewing vector and the right edge of the field of view.
        /// The value is positive. </summary>
        [MarshalAs(UnmanagedType.R4)]
        public float right_tan;

        /// <summary>
        /// The tangent of the angle between the viewing vector and the top edge of the field of view.
        /// The value is positive. </summary>
        [MarshalAs(UnmanagedType.R4)]
        public float top_tan;

        /// <summary>
        /// The tangent of the angle between the viewing vector and the bottom edge of the field of view.
        /// The value is positive. </summary>
        [MarshalAs(UnmanagedType.R4)]
        public float bottom_tan;
    }
}