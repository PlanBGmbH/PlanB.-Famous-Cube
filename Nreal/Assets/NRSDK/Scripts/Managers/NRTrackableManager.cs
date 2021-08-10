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
    using System.Collections.Generic;

    /// <summary>
    /// Manages AR system state and handles the session lifecycle. this class, application can create
    /// a session, configure it, start/pause or stop it. </summary>
    public class NRTrackableManager
    {
        /// <summary> Dictionary of trackables. </summary>
        private Dictionary<UInt64, NRTrackable> m_TrackableDict = new Dictionary<UInt64, NRTrackable>();

        /// <summary> Dictionary of trackable types. </summary>
        private Dictionary<TrackableType, Dictionary<UInt64, NRTrackable>> m_TrackableTypeDict = new Dictionary<TrackableType, Dictionary<ulong, NRTrackable>>();

        /// <summary> all trackables. </summary>
        private List<NRTrackable> m_AllTrackables = new List<NRTrackable>();
        /// <summary> The new trackables. </summary>
        private List<NRTrackable> m_NewTrackables = new List<NRTrackable>();

        /// <summary> The old trackables. </summary>
        private HashSet<NRTrackable> m_OldTrackables = new HashSet<NRTrackable>();

        /// <summary> Temp trackable handle list. </summary>
        private List<UInt64> m_TempTrackableHandles = new List<UInt64>();

        /// <summary> The native interface. </summary>
        private NativeInterface m_NativeInterface = null;

        /// <summary> Constructor. </summary>
        /// <param name="nativeInterface"> The native interface.</param>
        public NRTrackableManager(NativeInterface nativeInterface)
        {
            m_NativeInterface = nativeInterface;
        }

        /// <summary> Creates a new NRTrackable. </summary>
        /// <param name="trackable_handle"> Handle of the trackable.</param>
        /// <param name="nativeInterface">  The native interface.</param>
        /// <returns> A NRTrackable. </returns>
        private NRTrackable Create(UInt64 trackable_handle, NativeInterface nativeInterface)
        {
            if (trackable_handle == 0)
            {
                return null;
            }

            NRTrackable result;
            if (m_TrackableDict.TryGetValue(trackable_handle, out result))
            {
                return result;
            }

            if (nativeInterface == null)
            {
                return null;
            }
            TrackableType trackableType = nativeInterface.NativeTrackable.GetTrackableType(trackable_handle);
            if (trackableType == TrackableType.TRACKABLE_PLANE)
            {
                result = new NRTrackablePlane(trackable_handle, nativeInterface);
            }
            else if (trackableType == TrackableType.TRACKABLE_IMAGE)
            {
                result = new NRTrackableImage(trackable_handle, nativeInterface);
            }
            else
            {
                throw new NotImplementedException(
                    "TrackableFactory::No constructor for requested trackable type.");
            }
            if (result != null)
            {
                m_TrackableDict.Add(trackable_handle, result);
                if (!m_TrackableTypeDict.ContainsKey(trackableType))
                {
                    m_TrackableTypeDict.Add(trackableType, new Dictionary<ulong, NRTrackable>());
                }
                Dictionary<ulong, NRTrackable> trackbletype_dict = null;
                m_TrackableTypeDict.TryGetValue(trackableType, out trackbletype_dict);
                if (!trackbletype_dict.ContainsKey(trackable_handle))
                {
                    trackbletype_dict.Add(trackable_handle, result);
                    m_AllTrackables.Add(result);
                }
            }

            return result;
        }

        /// <summary> Updates the trackables described by trackable_type. </summary>
        /// <param name="trackable_type"> Type of the trackable.</param>
        private void UpdateTrackables(TrackableType trackable_type)
        {
            if (m_NativeInterface == null || m_NativeInterface.TrackingHandle == 0)
            {
                return;
            }

            if (m_NativeInterface.NativeTrackable.UpdateTrackables(trackable_type, m_TempTrackableHandles))
            {
                for (int i = 0; i < m_TempTrackableHandles.Count; i++)
                {
                    Create(m_TempTrackableHandles[i], m_NativeInterface);
                }
            }
        }

        /// <summary> Get the list of trackables with specified filter. </summary>
        /// <typeparam name="T"> Generic type parameter.</typeparam>
        /// <param name="trackables"> trackableList A list where the returned trackable stored. The
        ///                           previous values will be cleared.</param>
        /// <param name="filter">     Query filter.</param>
        public void GetTrackables<T>(List<T> trackables, NRTrackableQueryFilter filter) where T : NRTrackable
        {
            TrackableType t_type = GetTrackableType<T>();

            // Update trackable by type
            UpdateTrackables(t_type);

            // Find the new trackable in this frame
            m_NewTrackables.Clear();
            for (int i = 0; i < m_AllTrackables.Count; i++)
            {
                NRTrackable trackable = m_AllTrackables[i];
                if (!m_OldTrackables.Contains(trackable))
                {
                    m_NewTrackables.Add(trackable);
                    m_OldTrackables.Add(trackable);
                }
            }

            trackables.Clear();

            if (filter == NRTrackableQueryFilter.All)
            {
                for (int i = 0; i < m_AllTrackables.Count; i++)
                {
                    SafeAdd<T>(m_AllTrackables[i], trackables);
                }
            }
            else if (filter == NRTrackableQueryFilter.New)
            {
                for (int i = 0; i < m_NewTrackables.Count; i++)
                {
                    SafeAdd<T>(m_NewTrackables[i], trackables);
                }
            }
        }

        /// <summary> Safe add. </summary>
        /// <typeparam name="T"> Generic type parameter.</typeparam>
        /// <param name="trackable">  The trackable.</param>
        /// <param name="trackables"> trackableList A list where the returned trackable stored. The
        ///                           previous values will be cleared.</param>
        private void SafeAdd<T>(NRTrackable trackable, List<T> trackables) where T : NRTrackable
        {
            if (trackable is T)
            {
                trackables.Add(trackable as T);
            }
        }

        /// <summary> Gets trackable type. </summary>
        /// <typeparam name="T"> Generic type parameter.</typeparam>
        /// <returns> The trackable type. </returns>
        private TrackableType GetTrackableType<T>() where T : NRTrackable
        {
            if (typeof(NRTrackablePlane).Equals(typeof(T)))
            {
                return TrackableType.TRACKABLE_PLANE;
            }
            else if (typeof(NRTrackableImage).Equals(typeof(T)))
            {
                return TrackableType.TRACKABLE_IMAGE;
            }
            return TrackableType.TRACKABLE_BASE;
        }
    }
}
