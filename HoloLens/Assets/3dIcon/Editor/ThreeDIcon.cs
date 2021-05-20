using System.IO;
using System.Xml;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

/// <summary>
/// Represents the settings of the 3d HoloLens icon
/// </summary>
[CreateAssetMenu(fileName = "ThreeDIcon", menuName = "HL 3D Icon")]
public class ThreeDIcon : ScriptableObject
{
    public string Model;
    public bool OverrideBoundingBox = false;
    public Vector3 Center;
    public Vector3 Extents = Vector3.one;

}