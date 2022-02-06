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
    using UnityEngine;

    /// <summary> Manages Android permissions for the Unity application. </summary>
    public class NRAndroidPermissionsManager : AndroidJavaProxy, IAndroidPermissionsCheck
    {
        /// <summary> The instance. </summary>
        private static NRAndroidPermissionsManager _instance;
        /// <summary> The activity. </summary>
        private static AndroidJavaObject _activity;
        /// <summary> The permission service. </summary>
        private static AndroidJavaObject _permissionService;
        /// <summary> The current request. </summary>
        private static AsyncTask<AndroidPermissionsRequestResult> _currentRequest = null;
        /// <summary> The on permissions request finished. </summary>
        private static Action<AndroidPermissionsRequestResult> _onPermissionsRequestFinished;

        /// <summary> Constructs a new AndroidPermissionsManager. </summary>
        public NRAndroidPermissionsManager() : base(
            "ai.nreal.sdk.UnityAndroidPermissions$IPermissionRequestResult")
        {
        }

        /// <summary> Checks if an Android permission is granted to the application. </summary>
        /// <param name="permissionName"> The full name of the Android permission to check (e.g.
        ///                               android.permission.CAMERA).</param>
        /// <returns>
        /// <c>true</c> if <c>permissionName</c> is granted to the application, otherwise
        /// <c>false</c>. </returns>
        public static bool IsPermissionGranted(string permissionName)
        {
            if (Application.platform != RuntimePlatform.Android)
            {
                return true;
            }

            return GetPermissionsService().Call<bool>(
                "IsPermissionGranted", GetUnityActivity(), permissionName);
        }

        /// <summary> Requests an Android permission from the user. </summary>
        /// <param name="permissionName"> The permission to be requested (e.g. android.permission.CAMERA).</param>
        /// <returns>
        /// An asynchronous task that completes when the user has accepted or rejected the requested
        /// permission and yields a <see cref="AndroidPermissionsRequestResult"/> that summarizes the
        /// result. If this method is called when another permissions request is pending, <c>null</c>
        /// will be returned instead. </returns>
        public static AsyncTask<AndroidPermissionsRequestResult> RequestPermission(
            string permissionName)
        {
            if (NRAndroidPermissionsManager.IsPermissionGranted(permissionName))
            {
                return new AsyncTask<AndroidPermissionsRequestResult>(
                    new AndroidPermissionsRequestResult(
                        new string[] { permissionName }, new bool[] { true }));
            }

            if (_currentRequest != null)
            {
                NRDebugger.Error("Attempted to make simultaneous Android permissions requests.");
                return null;
            }

            GetPermissionsService().Call("RequestPermissionAsync", GetUnityActivity(),
                new[] { permissionName }, GetInstance());
            _currentRequest =
                new AsyncTask<AndroidPermissionsRequestResult>(out _onPermissionsRequestFinished);

            return _currentRequest;
        }

        /// <summary> Requests an Android permission from the user. </summary>
        /// <param name="permissionName"> The permission to be requested (e.g. android.permission.CAMERA).</param>
        /// <returns>
        /// An asynchronous task that completes when the user has accepted or rejected the requested
        /// permission and yields a <see cref="AndroidPermissionsRequestResult"/> that summarizes the
        /// result. If this method is called when another permissions request is pending, <c>null</c>
        /// will be returned instead. </returns>
        public AsyncTask<AndroidPermissionsRequestResult> RequestAndroidPermission(
            string permissionName)
        {
            return RequestPermission(permissionName);
        }

        /// <summary> Callback fired when a permission is granted. </summary>
        /// <param name="permissionName"> The name of the permission that was granted.</param>
        public virtual void OnPermissionGranted(string permissionName)
        {
            OnPermissionResult(permissionName, true);
        }

        /// <summary> Callback fired when a permission is denied. </summary>
        /// <param name="permissionName"> The name of the permission that was denied.</param>
        public virtual void OnPermissionDenied(string permissionName)
        {
            OnPermissionResult(permissionName, false);
        }

        /// <summary>
        /// Callback fired on an Android activity result (unused part of UnityAndroidPermissions
        /// interface). </summary>
        public virtual void OnActivityResult()
        {
        }

        /// <summary> Gets the instance. </summary>
        /// <returns> The instance. </returns>
        public static NRAndroidPermissionsManager GetInstance()
        {
            if (_instance == null)
            {
                _instance = new NRAndroidPermissionsManager();
            }

            return _instance;
        }

        /// <summary> Gets unity activity. </summary>
        /// <returns> The unity activity. </returns>
        private static AndroidJavaObject GetUnityActivity()
        {
            if (_activity == null)
            {
                AndroidJavaClass unityPlayer =
                    new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                _activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            }

            return _activity;
        }

        /// <summary> Gets permissions service. </summary>
        /// <returns> The permissions service. </returns>
        private static AndroidJavaObject GetPermissionsService()
        {
            if (_permissionService == null)
            {
                _permissionService =
                    new AndroidJavaObject("ai.nreal.sdk.UnityAndroidPermissions");
            }

            return _permissionService;
        }

        /// <summary> Callback fired on an Android permission result. </summary>
        /// <param name="permissionName"> The name of the permission.</param>
        /// <param name="granted">        If permission is granted or not.</param>
        private void OnPermissionResult(string permissionName, bool granted)
        {
            if (_onPermissionsRequestFinished == null)
            {
                Debug.LogErrorFormat(
                    "AndroidPermissionsManager received an unexpected permissions result {0}",
                    permissionName);
                return;
            }

            // Cache completion method and reset request state.
            var onRequestFinished = _onPermissionsRequestFinished;
            _currentRequest = null;
            _onPermissionsRequestFinished = null;

            onRequestFinished(new AndroidPermissionsRequestResult(new string[] { permissionName },
                new bool[] { granted }));
        }
    }
}
