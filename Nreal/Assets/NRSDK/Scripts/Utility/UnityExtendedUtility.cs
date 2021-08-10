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

    public class UnityExtendedUtility
    {
        public static RenderTexture CreateRenderTexture(int width, int height, int depth = 24, RenderTextureFormat format = RenderTextureFormat.ARGB32, bool usequaAnti = true)
        {
            var rt = new RenderTexture(width, height, depth, format);
            rt.wrapMode = TextureWrapMode.Clamp;
            if (QualitySettings.antiAliasing > 0 && usequaAnti)
            {
                rt.antiAliasing = QualitySettings.antiAliasing;
            }
            rt.Create();
            return rt;
        }
    }
}
