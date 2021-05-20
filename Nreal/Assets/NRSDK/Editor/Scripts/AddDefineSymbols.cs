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
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;

    public class AddDefineSymbolsForBeta : Editor
    {
        public static readonly string Symbols_Beta = "NRSDK_BETA";

        /// <summary> Sets environment for release version. </summary>
        [MenuItem("NRSDK/SetEnvironment/Release", false, 99)]
        public static void SetEnvironmentRelease()
        {
            string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            List<string> allDefines = definesString.Split(';').ToList();
            allDefines.Remove(Symbols_Beta);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(
                EditorUserBuildSettings.selectedBuildTargetGroup,
                string.Join(";", allDefines.ToArray()));
        }

        /// <summary> Add define symbols for beta version. </summary>
        [MenuItem("NRSDK/SetEnvironment/Beta", false, 100)]
        public static void SetEnvironmentBeta()
        {
            string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            List<string> allDefines = definesString.Split(';').ToList();
            List<string> Symbols = new List<string>() { Symbols_Beta };
            allDefines.AddRange(Symbols.Except(allDefines));
            PlayerSettings.SetScriptingDefineSymbolsForGroup(
                EditorUserBuildSettings.selectedBuildTargetGroup,
                string.Join(";", allDefines.ToArray()));
        }
    }
}