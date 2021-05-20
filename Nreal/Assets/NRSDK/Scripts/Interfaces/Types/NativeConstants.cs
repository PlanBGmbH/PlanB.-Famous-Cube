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

    /// <summary> A native constants. </summary>
    public class NativeConstants
    {
        /// <summary> The nrhandlenull. </summary>
        public const UInt64 NRHANDLENULL = 0;

        /// <summary> The illegal int. </summary>
        public const UInt32 IllegalInt = 0;

#if  UNITY_EDITOR
        /// <summary> The nr native library. </summary>
        public const string NRNativeLibrary = "libnr_api_editor";
#elif UNITY_STANDALONE_WIN
        public const string NRNativeLibrary = "libnr_api";
#else
        public const string NRNativeLibrary = "nr_api";
#endif

#if !UNITY_EDITOR && UNITY_ANDROID
        public const string NRNativeAnchorPointLib = "nr_tracking";
#else
        public const string NRNativeAnchorPointLib = "libnr_tracking";
#endif

#if UNITY_EDITOR_OSX
        public static string TrackingImageCliBinary = "trackableImageTools_osx";
#elif UNITY_EDITOR_WIN
        /// <summary> The tracking image CLI binary. </summary>
        public static string TrackingImageCliBinary = "trackableImageTools_win";
#elif UNITY_EDITOR_LINUX
        public static string TrackingImageCliBinary = "trackableImageTools_linux";
#endif

        /// <summary> The read external storage permission. </summary>
        public const string READ_EXTERNAL_STORAGE_PERMISSION = "android.permission.READ_EXTERNAL_STORAGE";

        /// <summary> The zip key. </summary>
        public static string ZipKey = "89f55314-6d41-416c-b4d9-4bdbc155e576";
        /// <summary> The glasses disconnect error tip. </summary>
        public static string GlassesDisconnectErrorTip = "Please connect your Nreal Light Glasses.";
        /// <summary> The sdk version mismatch error tip. </summary>
        public static string SdkVersionMismatchErrorTip = "Please update to the latest version of NRSDK.";
        /// <summary> The sdcard permission deny error tip. </summary>
        public static string SdcardPermissionDenyErrorTip = "There is no read permission for sdcard. Please go to the authorization management page of the device to authorize.";
        /// <summary> The unknow error tip. </summary>
        public static string UnknowErrorTip = "Unkown error! \nPlease contact Nreal's customer service.";

        /// <summary> The notification level warning. </summary>
        public static string NotificationLevelWarning = "Warning";
    }
}
