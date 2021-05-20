/****************************************************************************
* Copyright 2019 Nreal Techonology Limited.All rights reserved.
*
* This file is part of NRSDK.
*
* https://www.nreal.ai/        
*
*****************************************************************************/

namespace NRKernal
{
    using UnityEditor;
    using UnityEngine;
    using System.IO;
    using UnityEngine.Rendering;

    /// <summary> Form for viewing the project tips. </summary>
    [InitializeOnLoad]
    public class ProjectTipsWindow : EditorWindow
    {
        /// <summary> A check. </summary>
        private abstract class Check
        {
            /// <summary> The key. </summary>
            protected string key;

            /// <summary> Ignores this object. </summary>
            public void Ignore()
            {
                EditorPrefs.SetBool(ignorePrefix + key, true);
            }

            /// <summary> Query if this object is ignored. </summary>
            /// <returns> True if ignored, false if not. </returns>
            public bool IsIgnored()
            {
                return EditorPrefs.HasKey(ignorePrefix + key);
            }

            /// <summary> Deletes the ignore. </summary>
            public void DeleteIgnore()
            {
                EditorPrefs.DeleteKey(ignorePrefix + key);
            }

            /// <summary> Query if this object is valid. </summary>
            /// <returns> True if valid, false if not. </returns>
            public abstract bool IsValid();

            /// <summary> Draw graphical user interface. </summary>
            public abstract void DrawGUI();

            /// <summary> Query if this object is fixable. </summary>
            /// <returns> True if fixable, false if not. </returns>
            public abstract bool IsFixable();

            /// <summary> Fixes this object. </summary>
            public abstract void Fix();
        }

        /// <summary> A ckeck android vsyn. </summary>
        private class CkeckAndroidVsyn : Check
        {
            /// <summary> Default constructor. </summary>
            public CkeckAndroidVsyn()
            {
                key = this.GetType().Name;
            }

            /// <summary> Query if this object is valid. </summary>
            /// <returns> True if valid, false if not. </returns>
            public override bool IsValid()
            {
                return QualitySettings.vSyncCount == 0;
            }

            /// <summary> Draw graphical user interface. </summary>
            public override void DrawGUI()
            {
                EditorGUILayout.HelpBox("vSyn is opened on Mobile Devices", MessageType.Error);

                string message = @"In order to render correct on mobile devices, the vSyn in quality settings must be disabled. 
in dropdown list of Quality Settings > V Sync Count, choose 'Dont't Sync' for all levels.";
                EditorGUILayout.LabelField(message, EditorStyles.textArea);
            }

            /// <summary> Query if this object is fixable. </summary>
            /// <returns> True if fixable, false if not. </returns>
            public override bool IsFixable()
            {
                return true;
            }

            /// <summary> Fixes this object. </summary>
            public override void Fix()
            {
                if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android ||
                    EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS)
                {

                    QualitySettings.vSyncCount = 0;
                }
            }
        }

        /// <summary> Ckeck android SD card permission descriptor. </summary>
        private class CkeckAndroidSDCardPermission : Check
        {
            /// <summary> Default constructor. </summary>
            public CkeckAndroidSDCardPermission()
            {
                key = this.GetType().Name;
            }

            /// <summary> Query if this object is valid. </summary>
            /// <returns> True if valid, false if not. </returns>
            public override bool IsValid()
            {
                if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
                {
                    return PlayerSettings.Android.forceSDCardPermission;
                }
                else
                {
                    return false;
                }
            }

            /// <summary> Draw graphical user interface. </summary>
            public override void DrawGUI()
            {
                EditorGUILayout.HelpBox("Sdcard permission not available", MessageType.Error);

                string message = @"In order to run correct on mobile devices, the sdcard write permission should be set. 
in dropdown list of Player Settings > Other Settings > Write Permission, choose 'External(SDCard)'.";
                EditorGUILayout.LabelField(message, EditorStyles.textArea);
            }

            /// <summary> Query if this object is fixable. </summary>
            /// <returns> True if fixable, false if not. </returns>
            public override bool IsFixable()
            {
                return true;
            }

            /// <summary> Fixes this object. </summary>
            public override void Fix()
            {
                if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
                {
                    PlayerSettings.Android.forceSDCardPermission = true;
                }
            }
        }

        /// <summary> Android minSdkVersion should be higher than 26. </summary>
        private class CkeckAndroidMinAPILevel : Check
        {
            public CkeckAndroidMinAPILevel()
            {
                key = this.GetType().Name;
            }

            public override bool IsValid()
            {
                if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
                {
                    return PlayerSettings.Android.minSdkVersion >= AndroidSdkVersions.AndroidApiLevel26 ||
                        PlayerSettings.Android.minSdkVersion == AndroidSdkVersions.AndroidApiLevelAuto;
                }
                else
                {
                    return false;
                }
            }

            /// <summary> Draw graphical user interface. </summary>
            public override void DrawGUI()
            {
                EditorGUILayout.HelpBox("Android minSdkVersion should be higher than 26.", MessageType.Error);

                string message = @"In order to run correct on mobile devices, Android minSdkVersion should be higher than 26.";
                EditorGUILayout.LabelField(message, EditorStyles.textArea);
            }

            /// <summary> Query if this object is fixable. </summary>
            /// <returns> True if fixable, false if not. </returns>
            public override bool IsFixable()
            {
                return true;
            }

            /// <summary> Fixes this object. </summary>
            public override void Fix()
            {
                if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
                {
                    PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel26;
                }
            }
        }

        /// <summary> A ckeck android orientation. </summary>
        private class CkeckAndroidOrientation : Check
        {
            /// <summary> Default constructor. </summary>
            public CkeckAndroidOrientation()
            {
                key = this.GetType().Name;
            }

            /// <summary> Query if this object is valid. </summary>
            /// <returns> True if valid, false if not. </returns>
            public override bool IsValid()
            {
                return PlayerSettings.defaultInterfaceOrientation == UIOrientation.Portrait;
            }

            /// <summary> Draw graphical user interface. </summary>
            public override void DrawGUI()
            {
                EditorGUILayout.HelpBox("Orientation is not portrait", MessageType.Error);

                string message = @"In order to display correct on mobile devices, the orientation should be set to portrait. 
in dropdown list of Player Settings > Resolution and Presentation > Default Orientation, choose 'Portrait'.";
                EditorGUILayout.LabelField(message, EditorStyles.textArea);
            }

            /// <summary> Query if this object is fixable. </summary>
            /// <returns> True if fixable, false if not. </returns>
            public override bool IsFixable()
            {
                return true;
            }

            /// <summary> Fixes this object. </summary>
            public override void Fix()
            {
                if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
                {
                    PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
                }
            }
        }

        /// <summary> A ckeck android graphics a pi. </summary>
        private class CkeckAndroidGraphicsAPI : Check
        {
            /// <summary> Default constructor. </summary>
            public CkeckAndroidGraphicsAPI()
            {
                key = this.GetType().Name;
            }

            /// <summary> Query if this object is valid. </summary>
            /// <returns> True if valid, false if not. </returns>
            public override bool IsValid()
            {
                if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
                {
                    var graphics = PlayerSettings.GetGraphicsAPIs(BuildTarget.Android);
                    if (graphics != null && graphics.Length == 1 &&
                        graphics[0] == UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3)
                    {
                        return true;
                    }
                    return false;
                }
                else
                {
                    return false;
                }
            }

            /// <summary> Draw graphical user interface. </summary>
            public override void DrawGUI()
            {
                EditorGUILayout.HelpBox("GraphicsAPIs is not OpenGLES3", MessageType.Error);

                string message = @"In order to render correct on mobile devices, the graphicsAPIs should be set to OpenGLES3. 
in dropdown list of Player Settings > Other Settings > Graphics APIs , choose 'OpenGLES3'.";
                EditorGUILayout.LabelField(message, EditorStyles.textArea);
            }

            /// <summary> Query if this object is fixable. </summary>
            /// <returns> True if fixable, false if not. </returns>
            public override bool IsFixable()
            {
                return true;
            }

            /// <summary> Fixes this object. </summary>
            public override void Fix()
            {
                if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
                {
                    PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new GraphicsDeviceType[1] { GraphicsDeviceType.OpenGLES3 });
                }
            }
        }

        /// <summary> A ckeck color space. </summary>
        private class CkeckColorSpace : Check
        {
            /// <summary> Default constructor. </summary>
            public CkeckColorSpace()
            {
                key = this.GetType().Name;
            }

            /// <summary> Query if this object is valid. </summary>
            /// <returns> True if valid, false if not. </returns>
            public override bool IsValid()
            {
                return PlayerSettings.colorSpace == ColorSpace.Linear;
            }

            /// <summary> Draw graphical user interface. </summary>
            public override void DrawGUI()
            {
                EditorGUILayout.HelpBox("ColorSpace is not Linear", MessageType.Warning);

                string message = @"In order to display correct on mobile devices, the colorSpace should be set to linear. 
in dropdown list of Player Settings > Other Settings > Color Space, choose 'Linear'.";
                EditorGUILayout.LabelField(message, EditorStyles.textArea);
            }

            /// <summary> Query if this object is fixable. </summary>
            /// <returns> True if fixable, false if not. </returns>
            public override bool IsFixable()
            {
                return true;
            }

            /// <summary> Fixes this object. </summary>
            public override void Fix()
            {
                if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
                {
                    PlayerSettings.colorSpace = ColorSpace.Linear;
                }
            }
        }

        /// <summary> The checks. </summary>
        private static Check[] checks = new Check[]
        {
            new CkeckAndroidVsyn(),
            new CkeckAndroidMinAPILevel(),
            //new CkeckAndroidSDCardPermission(),
            new CkeckAndroidOrientation(),
            new CkeckAndroidGraphicsAPI(),
            new CkeckColorSpace(),
        };

        /// <summary> The window. </summary>
        private static ProjectTipsWindow m_Window;
        /// <summary> The scroll position. </summary>
        private Vector2 m_ScrollPosition;
        /// <summary> The ignore prefix. </summary>
        private const string ignorePrefix = "NRKernal.ignore";

        //static ProjectTipsWindow()
        //{
        //    EditorApplication.update -= Update;
        //    EditorApplication.update += Update;
        //}

        /// <summary> Shows the window. </summary>
        [MenuItem("NRSDK/Project Tips", false, 50)]
        public static void ShowWindow()
        {
            m_Window = GetWindow<ProjectTipsWindow>(true);
            m_Window.minSize = new Vector2(320, 300);
            m_Window.maxSize = new Vector2(320, 800);
            m_Window.titleContent = new GUIContent("NRSDK | Project Tips");
        }

        /// <summary> Updates this object. </summary>
        private static void Update()
        {
            bool show = false;

            foreach (Check check in checks)
            {
                if (!check.IsIgnored() && !check.IsValid())
                {
                    show = true;
                }
            }

            if (show)
            {
                ShowWindow();
            }

            //EditorApplication.update -= Update;
        }

        /// <summary> Executes the 'graphical user interface' action. </summary>
        public void OnGUI()
        {
            var resourcePath = GetResourcePath();
            var logo = AssetDatabase.LoadAssetAtPath<Texture2D>(resourcePath + "icon.png");
            var rect = GUILayoutUtility.GetRect(position.width, 80, GUI.skin.box);
            GUI.DrawTexture(rect, logo, ScaleMode.ScaleToFit);

            string aboutText = "This window provides tips to help fix common issues with the NRSDK and your project.";
            EditorGUILayout.LabelField(aboutText, EditorStyles.textArea);

            int ignoredCount = 0;
            int fixableCount = 0;
            int invalidNotIgnored = 0;

            for (int i = 0; i < checks.Length; i++)
            {
                Check check = checks[i];

                bool ignored = check.IsIgnored();
                bool valid = check.IsValid();
                bool fixable = check.IsFixable();

                if (!valid && !ignored && fixable)
                {
                    fixableCount++;
                }

                if (!valid && !ignored)
                {
                    invalidNotIgnored++;
                }

                if (ignored)
                {
                    ignoredCount++;
                }
            }

            Rect issuesRect = EditorGUILayout.GetControlRect();
            GUI.Box(new Rect(issuesRect.x - 4, issuesRect.y, issuesRect.width + 8, issuesRect.height), "Tips", EditorStyles.toolbarButton);

            if (invalidNotIgnored > 0)
            {
                m_ScrollPosition = GUILayout.BeginScrollView(m_ScrollPosition);
                {
                    for (int i = 0; i < checks.Length; i++)
                    {
                        Check check = checks[i];

                        if (!check.IsIgnored() && !check.IsValid())
                        {
                            invalidNotIgnored++;

                            GUILayout.BeginVertical("box");
                            {
                                check.DrawGUI();

                                EditorGUILayout.BeginHorizontal();
                                {
                                    // Aligns buttons to the right
                                    GUILayout.FlexibleSpace();

                                    if (check.IsFixable())
                                    {
                                        if (GUILayout.Button("Fix"))
                                            check.Fix();
                                    }

                                    //if (GUILayout.Button("Ignore"))
                                    //    check.Ignore();
                                }
                                EditorGUILayout.EndHorizontal();
                            }
                            GUILayout.EndVertical();
                        }
                    }
                }
                GUILayout.EndScrollView();
            }

            GUILayout.FlexibleSpace();

            if (invalidNotIgnored == 0)
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();

                    GUILayout.BeginVertical();
                    {
                        GUILayout.Label("No issues found");

                        if (GUILayout.Button("Close Window"))
                            Close();
                    }
                    GUILayout.EndVertical();

                    GUILayout.FlexibleSpace();
                }
                GUILayout.EndHorizontal();

                GUILayout.FlexibleSpace();
            }

            EditorGUILayout.BeginHorizontal("box");
            {
                if (fixableCount > 0)
                {
                    if (GUILayout.Button("Accept All"))
                    {
                        if (EditorUtility.DisplayDialog("Accept All", "Are you sure?", "Yes, Accept All", "Cancel"))
                        {
                            for (int i = 0; i < checks.Length; i++)
                            {
                                Check check = checks[i];

                                if (!check.IsIgnored() &&
                                    !check.IsValid())
                                {
                                    if (check.IsFixable())
                                        check.Fix();
                                }
                            }
                        }
                    }
                }

                //if (invalidNotIgnored > 0)
                //{
                //    if (GUILayout.Button("Ignore All"))
                //    {
                //        if (EditorUtility.DisplayDialog("Ignore All", "Are you sure?", "Yes, Ignore All", "Cancel"))
                //        {
                //            for (int i = 0; i < checks.Length; i++)
                //            {
                //                Check check = checks[i];

                //                if (!check.IsIgnored())
                //                    check.Ignore();
                //            }
                //        }
                //    }
                //}

                //if (ignoredCount > 0)
                //{
                //    if (GUILayout.Button("Show Ignored"))
                //    {
                //        foreach (Check check in checks)
                //            check.DeleteIgnore();
                //    }
                //}
            }
            GUILayout.EndHorizontal();
        }

        /// <summary> Gets resource path. </summary>
        /// <returns> The resource path. </returns>
        private string GetResourcePath()
        {
            var ms = MonoScript.FromScriptableObject(this);
            var path = AssetDatabase.GetAssetPath(ms);
            path = Path.GetDirectoryName(path);
            return path.Substring(0, path.Length - "Editor".Length - 1) + "Textures/";
        }
    }
}
