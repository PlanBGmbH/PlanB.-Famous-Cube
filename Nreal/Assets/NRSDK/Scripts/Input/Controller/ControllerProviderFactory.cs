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

    
    /// <summary> A controller provider factory. </summary>
    internal static class ControllerProviderFactory
    {
        /// <summary> Type of the android controller provider. </summary>
        public static Type androidControllerProviderType = typeof(NRControllerProvider);

        /// <summary> Creates controller provider. </summary>
        /// <param name="states"> The states.</param>
        /// <returns> The new controller provider. </returns>
        public static ControllerProviderBase CreateControllerProvider(ControllerState[] states)
        {
            ControllerProviderBase provider = CreateControllerProvider(androidControllerProviderType, states);
            return provider;
        }

        /// <summary> Creates controller provider. </summary>
        /// <param name="providerType"> Type of the provider.</param>
        /// <param name="states">       The states.</param>
        /// <returns> The new controller provider. </returns>
        private static ControllerProviderBase CreateControllerProvider(Type providerType, ControllerState[] states)
        {
            if (providerType != null)
            {
                object parserObj = Activator.CreateInstance(providerType, new object[] { states });
                if (parserObj is ControllerProviderBase)
                    return parserObj as ControllerProviderBase;
            }
            return null;
        }
    }
    
}