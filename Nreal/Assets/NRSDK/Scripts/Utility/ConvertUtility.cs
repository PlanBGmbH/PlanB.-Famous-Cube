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

    public class ConvertUtility
    {
        public static float IntBitsToFloat(int v)
        {
            byte[] buf = BitConverter.GetBytes(v);
            return BitConverter.ToSingle(buf, 0);
        }

        public static int FloatToRawIntBits(float v)
        {
            byte[] buf = BitConverter.GetBytes(v);
            return BitConverter.ToInt32(buf, 0);
        }
    }
}
