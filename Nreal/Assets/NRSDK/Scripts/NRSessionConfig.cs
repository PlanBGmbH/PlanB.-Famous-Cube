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
    using UnityEngine.Serialization;

    /// <summary> A configuration used to track the world. </summary>
    [CreateAssetMenu(fileName = "NRKernalSessionConfig", menuName = "NRSDK/SessionConfig", order = 1)]
    public class NRSessionConfig : ScriptableObject
    {
        /// <summary> Chooses which plane finding mode will be used. </summary>
        [Tooltip("Chooses which plane finding mode will be used.")]
        [FormerlySerializedAs("EnablePlaneFinding")]
        public TrackablePlaneFindingMode PlaneFindingMode = TrackablePlaneFindingMode.DISABLE;

        /// <summary> Chooses which marker finding mode will be used. </summary>
        [Tooltip("Chooses which marker finding mode will be used.")]
        [FormerlySerializedAs("EnableImageTracking")]
        public TrackableImageFindingMode ImageTrackingMode = TrackableImageFindingMode.DISABLE;

        /// <summary>
        /// A scriptable object specifying the NRSDK TrackingImageDatabase configuration. </summary>
        [Tooltip("A scriptable object specifying the NRSDK TrackingImageDatabase configuration.")]
        public NRTrackingImageDatabase TrackingImageDatabase;

        /// <summary> A prefab specifying the NRSDK TrackingImageDatabase configuration. </summary>
        [Tooltip("Chooses whether notification will be used.")]
        public bool EnableNotification;

        /// <summary> A prefab specifying the NRSDK TrackingImageDatabase configuration. </summary>
        [Tooltip("An error prompt will pop up when the device fails to connect.")]
        public NRGlassesInitErrorTip GlassesErrorTipPrefab;

        /// <summary> A prefab specifying the NRSDK TrackingImageDatabase configuration. </summary>
        [Tooltip("An warnning prompt will pop up when the lost tracking.")]
        public NRTrackingModeChangedTip TrackingModeChangeTipPrefab;

        /// <summary> Read it from PlayerdSetting automatically . </summary>
        [Tooltip("It will be read automatically from PlayerdSetting. ")]
        [HideInInspector]
        public bool UseMultiThread = false;


        /// <summary> ValueType check if two NRSessionConfig objects are equal. </summary>
        /// <param name="other"> .</param>
        /// <returns>
        /// True if the two NRSessionConfig objects are value-type equal, otherwise false. </returns>
        public override bool Equals(object other)
        {
            NRSessionConfig otherConfig = other as NRSessionConfig;
            if (other == null)
            {
                return false;
            }

            if (PlaneFindingMode != otherConfig.PlaneFindingMode ||
                ImageTrackingMode != otherConfig.ImageTrackingMode ||
                TrackingImageDatabase != otherConfig.TrackingImageDatabase)
            {
                return false;
            }

            return true;
        }

        /// <summary> Return a hash code for this object. </summary>
        /// <returns> A hash code for this object. </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary> ValueType copy from another SessionConfig object into this one. </summary>
        /// <param name="other"> .</param>
        public void CopyFrom(NRSessionConfig other)
        {
            PlaneFindingMode = other.PlaneFindingMode;
            ImageTrackingMode = other.ImageTrackingMode;
            TrackingImageDatabase = other.TrackingImageDatabase;
            GlassesErrorTipPrefab = other.GlassesErrorTipPrefab;
            TrackingModeChangeTipPrefab = other.TrackingModeChangeTipPrefab;
            UseMultiThread = other.UseMultiThread;
            EnableNotification = other.EnableNotification;
        }
    }
}
