using NRKernal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Exstensions functions for nreal input controller
/// </summary>
public class NrealInputExt
{
    /// <summary>
    /// Tries to return the laser end postion of the laser input
    /// </summary>
    /// <param name="point">Returns the world postions</param>
    /// <returns>Indicates the success</returns>
    public static bool GetLaserEndWorldPosition(out Vector3 point)
    {
        RaycastHit hitResult;
        return GetLaserEndWorldPosition(out point, out hitResult);
    }

    /// <summary>
    /// Tries to return the laser end postion of the laser input
    /// </summary>
    /// <param name="point">Returns the world postions</param>
    /// <param name="hitInfo">Returns the hit information of the raycast</param>
    /// <returns>Indicates the success</returns>
    public static bool GetLaserEndWorldPosition(out Vector3 point, out RaycastHit hitInfo)
    {
        Transform laserAnchor = NRInput.AnchorsHelper.GetAnchor(NRInput.RaycastMode == RaycastModeEnum.Gaze ? ControllerAnchorEnum.GazePoseTrackerAnchor : ControllerAnchorEnum.RightLaserAnchor);

        RaycastHit hitResult;
        if (Physics.Raycast(new Ray(laserAnchor.transform.position, laserAnchor.transform.forward), out hitResult, 10))
        {
            hitInfo = hitResult;
            point = Camera.main.WorldToScreenPoint(hitResult.point);
            return true;
        }

        hitInfo = new RaycastHit();
        point = new Vector3();
        return false;
    }
}
