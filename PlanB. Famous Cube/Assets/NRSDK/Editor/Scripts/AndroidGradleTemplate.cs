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
    using System;
    using System.Text;
    using System.Xml;
    using System.IO;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary> A list of the android. </summary>
    internal class AndroidGradleTemplate
    {
        string m_Path;
        public AndroidGradleTemplate(string path)
        {
            m_Path = path;
        }

        public bool SetGradlePluginVersion()
        {
            string[] lines;
            try
            {
                lines = File.ReadAllLines(m_Path);
                string[] modifyLines = new string[lines.Length];
                bool modify = false;
                string gradleToolToken = "com.android.tools.build:gradle:";
                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i];
                    // var trimedLine = line.Trim();
                    var idx = line.IndexOf(gradleToolToken);
                    if (idx > 0)
                    {
                        string subLine = line.Substring(idx + gradleToolToken.Length);
                        string subVersion = subLine.Substring(0, subLine.IndexOf('\''));
                        Debug.LogFormat("subVersion : {0}", subVersion);

                        string[] versions = subVersion.Split('.');
                        if (versions.Length == 3)
                        {
                            int vMain = 0;
                            int vMiddle = 0;
                            int vMin = 0;
                            int.TryParse(versions[0], out vMain);
                            int.TryParse(versions[1], out vMiddle);
                            int.TryParse(versions[2], out vMin);
                            bool updateVersion = false;
                            /// update version before 3.4.3
                            if (vMain < 4)
                            {
                                if (vMain < 3)
                                    updateVersion = true;
                                else if (vMiddle < 4)
                                    updateVersion = true;
                                else if (vMiddle == 4 && vMin < 3)
                                    updateVersion = true;
                            }

                            if (updateVersion)
                            {
                                modify = true;
                                line = line.Replace(subVersion, "3.4.3");
                                Debug.LogFormat("update gradle setting : {0} --> {1}", subVersion, "3.4.3");
                            }
                        }
                    }

                    modifyLines[i] = line;
                }

                if (modify)
                    File.WriteAllLines(m_Path, modifyLines);
            }
            catch (Exception ex)
            {
                Debug.LogErrorFormat("SetGradlePluginVersion exception : {0}", ex.Message);
                return false;
            }

            return true;
        }
    }

}
