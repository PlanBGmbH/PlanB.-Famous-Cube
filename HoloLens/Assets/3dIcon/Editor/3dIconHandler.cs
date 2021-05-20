using System.IO;
using System.Xml;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

/// <summary>
/// Handle the post build process to attach the 3d icon for HoloLens
/// </summary>
public class ThreeDIconHandler
{

    [PostProcessBuild(1)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        if (target == BuildTarget.WSAPlayer)
        {
            string[] IconThreeDAssets = AssetDatabase.FindAssets("t:ThreeDIcon");
            if (IconThreeDAssets.Length > 0)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(IconThreeDAssets[0]);
                ThreeDIcon appLauncher = AssetDatabase.LoadAssetAtPath<ThreeDIcon>(assetPath);

                AddThreeDIcon(pathToBuiltProject, appLauncher);
            }
        }
    }

    private static void AddThreeDIcon(string buildPath, ThreeDIcon settings)
    {
        string pathToProjectFiles = Path.Combine(buildPath, Application.productName);

        AddToPackageManifest(pathToProjectFiles, settings);
        AddToProject(pathToProjectFiles);
        CopyGlb(pathToProjectFiles, settings);
    }

    private static void CopyGlb(string buildPath, ThreeDIcon settings)
    {
        string launcherFileSourcePath = Application.dataPath + settings.Model;
        string launcherFileTargetPath = Path.Combine(buildPath, "Assets\\CubeIconThreeD.glb");

        FileUtil.ReplaceFile(launcherFileSourcePath, launcherFileTargetPath);
    }

    private static void AddToProject(string buildPath)
    {
        ScriptingImplementation scriptingImplementation = PlayerSettings.GetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup);

        string projFilename = Path.Combine(buildPath, PlayerSettings.productName + (scriptingImplementation == ScriptingImplementation.IL2CPP ? ".vcxproj" : ".csproj"));
        XmlDocument document = new XmlDocument();
        document.Load(projFilename);

        if (scriptingImplementation == ScriptingImplementation.IL2CPP)
        {
            bool alreadyAdded = false;
            XmlNodeList nones = document.GetElementsByTagName("None");
            foreach (var none in nones)
            {
                XmlElement element = none as XmlElement;
                if (element.GetAttribute("Include") == "Assets\\CubeIconThreeD.glb")
                {
                    alreadyAdded = true;
                }
            }

            if (!alreadyAdded)
            {
                XmlElement newItemGroup = document.CreateElement("ItemGroup", document.DocumentElement.NamespaceURI);
                XmlElement newNoneElement = document.CreateElement("None", document.DocumentElement.NamespaceURI);
                XmlNode deploymentContentNode = document.CreateElement("DeploymentContent", document.DocumentElement.NamespaceURI);
                newNoneElement.AppendChild(deploymentContentNode);
                deploymentContentNode.AppendChild(document.CreateTextNode("true"));
                newNoneElement.SetAttribute("Include", "Assets\\CubeIconThreeD.glb");
                newItemGroup.AppendChild(newNoneElement);
                document.DocumentElement.AppendChild(newItemGroup);
            }
        }
        else
        {
            bool alreadyAdded = false;
            XmlNodeList contents = document.GetElementsByTagName("Content");
            foreach (var content in contents)
            {
                XmlElement element = content as XmlElement;
                if (element.GetAttribute("Include") == "Assets\\CubeIconThreeD.glb")
                {
                    alreadyAdded = true;
                }
            }

            if (!alreadyAdded)
            {
                XmlElement itemGroup = document.CreateElement("ItemGroup", document.DocumentElement.NamespaceURI);
                XmlElement content = document.CreateElement("Content", document.DocumentElement.NamespaceURI);
                content.SetAttribute("Include", "Assets\\CubeIconThreeD.glb");
                itemGroup.AppendChild(content);
                document.DocumentElement.AppendChild(itemGroup);
            }
        }

        document.Save(projFilename);
    }

    private static void AddToPackageManifest(string buildPath, ThreeDIcon settings)
    {
        string packageManifestPath = Path.Combine(buildPath, "Package.appxmanifest");
        XmlDocument document = new XmlDocument();
        document.Load(packageManifestPath);

        XmlNodeList packages = document.GetElementsByTagName("Package");
        XmlElement package = packages.Item(0) as XmlElement;

        package.SetAttribute("xmlns:uap5", "http://schemas.microsoft.com/appx/manifest/uap/windows10/5");
        package.SetAttribute("xmlns:uap6", "http://schemas.microsoft.com/appx/manifest/uap/windows10/6");
        package.SetAttribute("IgnorableNamespaces", "uap uap2 uap5 uap6 mp");

        XmlNodeList mixedRealityModels = document.GetElementsByTagName("uap5:MixedRealityModel");
        XmlElement mixedRealityModel = null;
        if (mixedRealityModels.Count == 0)
        {
            XmlNodeList defaultTiles = document.GetElementsByTagName("uap:DefaultTile");
            XmlNode defaultTile = defaultTiles.Item(0);
            mixedRealityModel = document.CreateElement("uap5", "MixedRealityModel", "http://schemas.microsoft.com/appx/manifest/uap/windows10/5");
            defaultTile.AppendChild(mixedRealityModel);
        }
        else
        {
            mixedRealityModel = mixedRealityModels.Item(0) as XmlElement;
        }

        mixedRealityModel.SetAttribute("Path", "Assets\\CubeIconThreeD.glb");

        XmlNodeList boundingBoxes = document.GetElementsByTagName("uap6:SpatialBoundingBox");
        if (boundingBoxes.Count == 1)
        {
            mixedRealityModel.RemoveChild(boundingBoxes.Item(0));
        }

        if (settings.OverrideBoundingBox)
        {
            XmlElement boundingBox = document.CreateElement("uap6", "SpatialBoundingBox", "http://schemas.microsoft.com/appx/manifest/uap/windows10/6");
            string center = settings.Center.x + "," + settings.Center.y + "," + settings.Center.z;
            string extents = settings.Extents.x + "," + settings.Extents.y + "," + settings.Extents.z;
            boundingBox.SetAttribute("Center", center);
            boundingBox.SetAttribute("Extents", extents);
            mixedRealityModel.AppendChild(boundingBox);
        }

        document.Save(packageManifestPath);
    }
}
