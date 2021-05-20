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
    using System.Runtime.InteropServices;

    /// <summary> 6-dof Trackable's Native API . </summary>
    public partial class NativeTrackable
    {
        /// <summary> The native interface. </summary>
        private NativeInterface m_NativeInterface;

        /// <summary> Constructor. </summary>
        /// <param name="nativeInterface"> The native interface.</param>
        public NativeTrackable(NativeInterface nativeInterface)
        {
            m_NativeInterface = nativeInterface;
        }

        /// <summary> Creates trackable list. </summary>
        /// <returns> The new trackable list. </returns>
        public UInt64 CreateTrackableList()
        {
            if (m_NativeInterface.TrackingHandle == 0)
            {
                return 0;
            }
            UInt64 trackable_list_handle = 0;
            NativeApi.NRTrackableListCreate(m_NativeInterface.TrackingHandle, ref trackable_list_handle);
            return trackable_list_handle;
        }

        /// <summary> Destroys the trackable list described by trackable_list_handle. </summary>
        /// <param name="trackable_list_handle"> Handle of the trackable list.</param>
        public void DestroyTrackableList(UInt64 trackable_list_handle)
        {
            if (m_NativeInterface.TrackingHandle == 0)
            {
                return;
            }
            NativeApi.NRTrackableListDestroy(m_NativeInterface.TrackingHandle, trackable_list_handle);
        }

        /// <summary> Gets a size. </summary>
        /// <param name="trackable_list_handle"> Handle of the trackable list.</param>
        /// <returns> The size. </returns>
        public int GetSize(UInt64 trackable_list_handle)
        {
            if (m_NativeInterface.TrackingHandle == 0)
            {
                return 0;
            }
            int list_size = 0;
            NativeApi.NRTrackableListGetSize(m_NativeInterface.TrackingHandle, trackable_list_handle, ref list_size);
            return list_size;
        }

        /// <summary> Acquires the item. </summary>
        /// <param name="trackable_list_handle"> Handle of the trackable list.</param>
        /// <param name="index">                 Zero-based index of the.</param>
        /// <returns> An UInt64. </returns>
        public UInt64 AcquireItem(UInt64 trackable_list_handle, int index)
        {
            if (m_NativeInterface.TrackingHandle == 0)
            {
                return 0;
            }
            UInt64 trackable_handle = 0;
            NativeApi.NRTrackableListAcquireItem(m_NativeInterface.TrackingHandle, trackable_list_handle, index, ref trackable_handle);
            return trackable_handle;
        }

        /// <summary> Gets an identify. </summary>
        /// <param name="trackable_handle"> Handle of the trackable.</param>
        /// <returns> The identify. </returns>
        public UInt32 GetIdentify(UInt64 trackable_handle)
        {
            if (m_NativeInterface.TrackingHandle == 0)
            {
                return 0;
            }
            UInt32 identify = NativeConstants.IllegalInt;
            NativeApi.NRTrackableGetIdentifier(m_NativeInterface.TrackingHandle, trackable_handle, ref identify);
            return identify;
        }

        /// <summary> Gets trackable type. </summary>
        /// <param name="trackable_handle"> Handle of the trackable.</param>
        /// <returns> The trackable type. </returns>
        public TrackableType GetTrackableType(UInt64 trackable_handle)
        {
            if (m_NativeInterface.TrackingHandle == 0)
            {
                return TrackableType.TRACKABLE_BASE;
            }
            TrackableType trackble_type = TrackableType.TRACKABLE_BASE;
            NativeApi.NRTrackableGetType(m_NativeInterface.TrackingHandle, trackable_handle, ref trackble_type);
            return trackble_type;
        }

        /// <summary> Gets tracking state. </summary>
        /// <param name="trackable_handle"> Handle of the trackable.</param>
        /// <returns> The tracking state. </returns>
        public TrackingState GetTrackingState(UInt64 trackable_handle)
        {
            if (m_NativeInterface.TrackingHandle == 0)
            {
                return TrackingState.Stopped;
            }
            TrackingState status = TrackingState.Stopped;
            NativeApi.NRTrackableGetTrackingState(m_NativeInterface.TrackingHandle, trackable_handle, ref status);
            return status;
        }

        private partial struct NativeApi
        {
            /// <summary> Nr trackable list create. </summary>
            /// <param name="session_handle">            Handle of the session.</param>
            /// <param name="out_trackable_list_handle"> [in,out] Handle of the out trackable list.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRTrackableListCreate(UInt64 session_handle,
                ref UInt64 out_trackable_list_handle);

            /// <summary> Nr trackable list destroy. </summary>
            /// <param name="session_handle">            Handle of the session.</param>
            /// <param name="out_trackable_list_handle"> Handle of the out trackable list.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRTrackableListDestroy(UInt64 session_handle,
                UInt64 out_trackable_list_handle);

            /// <summary> Nr trackable list get size. </summary>
            /// <param name="session_handle">        Handle of the session.</param>
            /// <param name="trackable_list_handle"> Handle of the trackable list.</param>
            /// <param name="out_list_size">         [in,out] Size of the out list.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRTrackableListGetSize(UInt64 session_handle,
                UInt64 trackable_list_handle, ref int out_list_size);

            /// <summary> Nr trackable list acquire item. </summary>
            /// <param name="session_handle">        Handle of the session.</param>
            /// <param name="trackable_list_handle"> Handle of the trackable list.</param>
            /// <param name="index">                 Zero-based index of the.</param>
            /// <param name="out_trackable">         [in,out] The out trackable.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRTrackableListAcquireItem(UInt64 session_handle,
                UInt64 trackable_list_handle, int index, ref UInt64 out_trackable);

            /// <summary> Nr trackable get identifier. </summary>
            /// <param name="session_handle">   Handle of the session.</param>
            /// <param name="trackable_handle"> Handle of the trackable.</param>
            /// <param name="out_identifier">   [in,out] Identifier for the out.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRTrackableGetIdentifier(UInt64 session_handle,
                UInt64 trackable_handle, ref UInt32 out_identifier);

            /// <summary> Nr trackable get type. </summary>
            /// <param name="session_handle">     Handle of the session.</param>
            /// <param name="trackable_handle">   Handle of the trackable.</param>
            /// <param name="out_trackable_type"> [in,out] Type of the out trackable.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRTrackableGetType(UInt64 session_handle,
                UInt64 trackable_handle, ref TrackableType out_trackable_type);

            /// <summary> Nr trackable get tracking state. </summary>
            /// <param name="session_handle">     Handle of the session.</param>
            /// <param name="trackable_handle">   Handle of the trackable.</param>
            /// <param name="out_tracking_state"> [in,out] State of the out tracking.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRTrackableGetTrackingState(UInt64 session_handle,
                UInt64 trackable_handle, ref TrackingState out_tracking_state);
        };
    }
}
