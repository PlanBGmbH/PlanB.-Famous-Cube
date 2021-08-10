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
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary> A plane in the real world detected by NRInternel. </summary>
    public class NRTrackablePlane : NRTrackable
    {
        /// <summary> Constructor. </summary>
        /// <param name="nativeHandle">    Handle of the native.</param>
        /// <param name="nativeInterface"> The native interface.</param>
        internal NRTrackablePlane(UInt64 nativeHandle, NativeInterface nativeInterface)
          : base(nativeHandle, nativeInterface)
        {
        }

        /// <summary> Type of the trackable plane. </summary>
        TrackablePlaneType trackablePlaneType;
        /// <summary> Get the plane type. </summary>
        /// <returns> Plane type. </returns>
        public TrackablePlaneType GetPlaneType()
        {
            if (NRFrame.SessionStatus != SessionState.Running)
            {
                return trackablePlaneType;
            }
            trackablePlaneType = NativeInterface.NativePlane.GetPlaneType(TrackableNativeHandle);
            return trackablePlaneType;
        }

        /// <summary> The center pose. </summary>
        Pose centerPose;
        /// <summary>
        /// Gets the position and orientation of the plane's center in Unity world space. </summary>
        /// <returns> The center pose. </returns>
        public override Pose GetCenterPose()
        {
            if (NRFrame.SessionStatus != SessionState.Running)
            {
                return centerPose;
            }
            centerPose = NativeInterface.NativePlane.GetCenterPose(TrackableNativeHandle);
            return centerPose;
        }

        /// <summary>
        /// Gets the extent of plane in the X dimension, centered on the plane position. </summary>
        /// <value> The extent x coordinate. </value>
        public float ExtentX
        {
            get
            {
                if (NRFrame.SessionStatus != SessionState.Running)
                {
                    return 0;
                }
                return NativeInterface.NativePlane.GetExtentX(TrackableNativeHandle);
            }
        }

        /// <summary>
        /// Gets the extent of plane in the Z dimension, centered on the plane position. </summary>
        /// <value> The extent z coordinate. </value>
        public float ExtentZ
        {
            get
            {
                if (NRFrame.SessionStatus != SessionState.Running)
                {
                    return 0;
                }
                return NativeInterface.NativePlane.GetExtentZ(TrackableNativeHandle);
            }
        }

        /// <summary>
        /// Gets a list of points(in clockwise order) in plane coordinate representing a boundary polygon
        /// for the plane. </summary>
        /// <param name="polygonList"> polygonList A list used to be filled with polygon points.</param>
        public void GetBoundaryPolygon(List<Vector3> polygonList)
        {
            if (NRFrame.SessionStatus != SessionState.Running)
            {
                return;
            }
            var planetype = GetPlaneType();
            if (planetype == TrackablePlaneType.INVALID)
            {
                NRDebugger.Error("Invalid plane type.");
                return;
            }

            NativeInterface.NativePlane.GetBoundaryPolygon(TrackableNativeHandle, polygonList);
        }
    }
}
