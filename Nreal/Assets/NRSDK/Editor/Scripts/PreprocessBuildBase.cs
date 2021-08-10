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
    using System.Diagnostics.CodeAnalysis;
    using System.Xml;
    using UnityEditor;
    using UnityEditor.Build;
    using System.IO;
#if UNITY_2018_1_OR_NEWER
    using UnityEditor.Build.Reporting;
    using UnityEngine;
    using UnityEditor.Android;
    using System.Text;
    using System.Text.RegularExpressions;
#endif

#if UNITY_2018_1_OR_NEWER
    /// <summary> The preprocess build base. </summary>
    internal class PreprocessBuildBase : IPreprocessBuildWithReport
#else
    internal class PreprocessBuildBase : IPreprocessBuild
#endif
    {
        /// <summary>
        /// <para>Returns the relative callback order for callbacks.  Callbacks with lower values are
        /// called before ones with higher values.</para> </summary>
        /// <value> The callback order. </value>
        public int callbackOrder
        {
            get
            {
                return 0;
            }
        }

#if UNITY_2018_1_OR_NEWER
        /// <summary>
        /// <para>Implement this function to receive a callback before the build is started.</para> </summary>
        /// <param name="report"> A report containing information about the build, such as its target
        ///                       platform and output path.</param>
        public void OnPreprocessBuild(BuildReport report)
        {
            OnPreprocessBuild(report.summary.platform, report.summary.outputPath);
        }
#endif

        /// <summary> Executes the 'preprocess build' action. </summary>
        /// <param name="target"> Target for the.</param>
        /// <param name="path">   Full pathname of the file.</param>
        public virtual void OnPreprocessBuild(BuildTarget target, string path)
        {
            if (target == BuildTarget.Android)
            {
                OnPreprocessBuildForAndroid();
            }
        }

        private const string DefaultXML = @"<?xml version='1.0' encoding='utf-8'?>
<manifest
    xmlns:android='http://schemas.android.com/apk/res/android'
    package='com.unity3d.player'
    xmlns:tools='http://schemas.android.com/tools'
    android:installLocation='preferExternal'>
  <uses-sdk tools:overrideLibrary='com.nreal.glasses_sdk'/>
  <supports-screens
      android:smallScreens='true'
      android:normalScreens='true'
      android:largeScreens='true'
      android:xlargeScreens='true'
      android:anyDensity='true'/>

  <application
      android:theme='@style/UnityThemeSelector'
      android:icon='@mipmap/app_icon'
      android:label='@string/app_name'>
    <activity android:name='com.unity3d.player.UnityPlayerActivity'>
      <intent-filter>
        <action android:name='android.intent.action.MAIN' />
        <category android:name='android.intent.category.INFO' tools:node='replace'/>
      </intent-filter>
    </activity>
    <meta-data android:name='nreal_sdk' android:value='true'/>
  </application>
  <uses-permission android:name='android.permission.CAMERA'/>
  <uses-permission android:name='android.permission.BLUETOOTH'/>
  <uses-permission android:name='android.permission.BLUETOOTH_ADMIN' />
  <uses-permission android:name='android.permission.QUERY_ALL_PACKAGES' />
</manifest>
        ";

        /// <summary> True if is show on desktop, false if not. </summary>
        private const bool isShowOnDesktop = true;
        /// <summary> Executes the 'preprocess build for android' action. </summary>
        [MenuItem("NRSDK/PreprocessBuildForAndroid")]
        public static void OnPreprocessBuildForAndroid()
        {
            string basePath = Application.dataPath + "/Plugins/Android";
            if (!Directory.Exists(Application.dataPath + "/Plugins"))
            {
                Directory.CreateDirectory(Application.dataPath + "/Plugins");
            }
            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }
            string xmlPath = Application.dataPath + "/Plugins/Android/AndroidManifest.xml";

            if (!File.Exists(xmlPath))
            {
                string xml = DefaultXML.Replace("\'", "\"");
                if (isShowOnDesktop)
                {
                    xml = xml.Replace("<category android:name=\"android.intent.category.INFO\" tools:node=\"replace\"/>",
                       "<category android:name=\"android.intent.category.LAUNCHER\" />");
                }
                File.WriteAllText(xmlPath, xml);
            }

            AutoGenerateAndroidManifest(xmlPath);
            AssetDatabase.Refresh();

            ApplySettingsToConfig();
        }

        private static void ApplySettingsToConfig()
        {
            var sessionConfigGuids = AssetDatabase.FindAssets("t:NRSessionConfig");
            foreach (var item in sessionConfigGuids)
            {
                var config = AssetDatabase.LoadAssetAtPath<NRSessionConfig>(
                    AssetDatabase.GUIDToAssetPath(item));
                config.UseMultiThread = PlayerSettings.GetMobileMTRendering(BuildTargetGroup.Android);
                EditorUtility.SetDirty(config);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [MenuItem("NRSDK/AddAudioPermission")]
        public static void AddAudioPermission()
        {
            string xmlPath = Application.dataPath + "/Plugins/Android/AndroidManifest.xml";
            var androidManifest = new AndroidManifest(xmlPath);
            androidManifest.SetAudioRecordPermission();
            androidManifest.Save();
        }

        /// <summary> Automatic generate android manifest. </summary>
        /// <param name="path"> Full pathname of the file.</param>
        public static void AutoGenerateAndroidManifest(string path)
        {
            var androidManifest = new AndroidManifest(path);
            //bool needrequestLegacyExternalStorage = (GetAndroidTargetApiLevel() >= 29);
            //androidManifest.SetExternalStorage(needrequestLegacyExternalStorage);
            androidManifest.SetPackageReadPermission();
            androidManifest.SetCameraPermission();
            androidManifest.SetBlueToothPermission();
            androidManifest.SetSDKMetaData();
            androidManifest.SetAPKDisplayedOnLauncher(isShowOnDesktop);

            androidManifest.Save();
        }

        public static int GetAndroidTargetApiLevel()
        {
            if (PlayerSettings.Android.targetSdkVersion == AndroidSdkVersions.AndroidApiLevelAuto)
            {
                string androidsdkRoot = EditorPrefs.GetString("AndroidSdkRoot");
                string[] androidVersions = Directory.GetDirectories(Path.Combine(androidsdkRoot, "platforms"));
                string regex = "android-([0-9]*)";
                int maxVersion = (int)AndroidSdkVersions.AndroidApiLevelAuto;
                foreach (var item in androidVersions)
                {
                    MatchCollection coll = Regex.Matches(item, regex);
                    if (coll != null && coll[0] != null && coll[0].Groups[1] != null)
                    {
                        string result = coll[0].Groups[1].Value;
                        int versionCode = int.Parse(result);
                        if (versionCode > maxVersion)
                        {
                            maxVersion = versionCode;
                        }
                    }
                }
                return maxVersion;
            }
            else
            {
                return (int)PlayerSettings.Android.targetSdkVersion;
            }
        }

        //public void OnPostGenerateGradleAndroidProject(string basePath)
        //{
        //    var pathBuilder = new StringBuilder(basePath);
        //    pathBuilder.Append(Path.DirectorySeparatorChar).Append("src");
        //    pathBuilder.Append(Path.DirectorySeparatorChar).Append("main");
        //    pathBuilder.Append(Path.DirectorySeparatorChar).Append("AndroidManifest.xml");
        //    NRDebugger.Error("OnPostGenerateGradleAndroidProject:" + pathBuilder.ToString());
        //    AutoGenerateAndroidManifest(pathBuilder.ToString());
        //}
    }
}