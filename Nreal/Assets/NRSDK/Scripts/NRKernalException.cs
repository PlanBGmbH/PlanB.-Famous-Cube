using System;

namespace NRKernal
{
    public enum Level
    {
        High,
        Normal,
    }

    /// <summary> A nr kernal error. </summary>
    public class NRKernalError : ApplicationException
    {
        public Level level;
        /// <summary> The error. </summary>
        protected string error;
        /// <summary> The inner exception. </summary>
        protected Exception innerException;

        /// <summary> Constructor. </summary>
        /// <param name="msg">            The message.</param>
        /// <param name="innerException"> (Optional) The inner exception.</param>
        public NRKernalError(string msg, Level level = Level.Normal, Exception innerException = null) : base(msg)
        {
            this.innerException = innerException;
            this.error = msg;
            this.level = level;
        }
        /// <summary> Gets the error. </summary>
        /// <returns> The error. </returns>
        public string GetError()
        {
            return error;
        }
    }

    /// <summary> A nr invalid argument error. </summary>
    public class NRInvalidArgumentError : NRKernalError
    {
        /// <summary> Constructor. </summary>
        /// <param name="msg">            The message.</param>
        /// <param name="innerException"> (Optional) The inner exception.</param>
        public NRInvalidArgumentError(string msg, Exception innerException = null) : base(msg, Level.Normal, innerException)
        {
        }
    }

    /// <summary> A nr not enough memory error. </summary>
    public class NRNotEnoughMemoryError : NRKernalError
    {
        /// <summary> Constructor. </summary>
        /// <param name="msg">            The message.</param>
        /// <param name="innerException"> (Optional) The inner exception.</param>
        public NRNotEnoughMemoryError(string msg, Exception innerException = null) : base(msg, Level.High, innerException)
        {
        }
    }

    /// <summary> A nr sdcard permission deny error. </summary>
    public class NRSdcardPermissionDenyError : NRKernalError
    {
        /// <summary> Constructor. </summary>
        /// <param name="msg">            The message.</param>
        /// <param name="innerException"> (Optional) The inner exception.</param>
        public NRSdcardPermissionDenyError(string msg, Exception innerException = null) : base(msg, Level.High, innerException)
        {
        }
    }

    /// <summary> A nr un supported error. </summary>
    public class NRUnSupportedError : NRKernalError
    {
        /// <summary> Constructor. </summary>
        /// <param name="msg">            The message.</param>
        /// <param name="innerException"> (Optional) The inner exception.</param>
        public NRUnSupportedError(string msg, Exception innerException = null) : base(msg, Level.High, innerException)
        {
        }
    }

    /// <summary> A nr glasses connect error. </summary>
    public class NRGlassesConnectError : NRKernalError
    {
        /// <summary> Constructor. </summary>
        /// <param name="msg">            The message.</param>
        /// <param name="innerException"> (Optional) The inner exception.</param>
        public NRGlassesConnectError(string msg, Exception innerException = null) : base(msg, Level.High, innerException)
        {
        }
    }

    /// <summary> A nr sdk version mismatch error. </summary>
    public class NRSdkVersionMismatchError : NRKernalError
    {
        /// <summary> Constructor. </summary>
        /// <param name="msg">            The message.</param>
        /// <param name="innerException"> (Optional) The inner exception.</param>
        public NRSdkVersionMismatchError(string msg, Exception innerException = null) : base(msg, Level.High, innerException)
        {
        }
    }

    /// <summary> A nrrgb camera device not find error. </summary>
    public class NRRGBCameraDeviceNotFindError : NRKernalError
    {
        /// <summary> Constructor. </summary>
        /// <param name="msg">            The message.</param>
        /// <param name="innerException"> (Optional) The inner exception.</param>
        public NRRGBCameraDeviceNotFindError(string msg, Exception innerException = null) : base(msg, Level.Normal, innerException)
        {
        }
    }

    /// <summary> A nrdp device not find error. </summary>
    public class NRDPDeviceNotFindError : NRKernalError
    {
        /// <summary> Constructor. </summary>
        /// <param name="msg">            The message.</param>
        /// <param name="innerException"> (Optional) The inner exception.</param>
        public NRDPDeviceNotFindError(string msg, Exception innerException = null) : base(msg, Level.High, innerException)
        {
        }
    }

    public class NRMissingKeyComponentError : NRKernalError
    {
        /// <summary> Constructor. </summary>
        /// <param name="msg">            The message.</param>
        /// <param name="innerException"> (Optional) The inner exception.</param>
        public NRMissingKeyComponentError(string msg, Exception innerException = null) : base(msg, Level.High, innerException)
        {
        }
    }

    public class NRPermissionDenyError : NRKernalError
    {
        /// <summary> Constructor. </summary>
        /// <param name="msg">            The message.</param>
        /// <param name="innerException"> (Optional) The inner exception.</param>
        public NRPermissionDenyError(string msg, Exception innerException = null) : base(msg, Level.Normal, innerException)
        {
        }
    }

    /// <summary> A device not support hand tracking calculation error. </summary>
    public class NRUnSupportedHandtrackingCalculationError : NRKernalError
    {
        /// <summary> Constructor. </summary>
        /// <param name="msg">            The message.</param>
        /// <param name="innerException"> (Optional) The inner exception.</param>
        public NRUnSupportedHandtrackingCalculationError(string msg, Exception innerException = null) : base(msg, Level.Normal, innerException)
        {
        }
    }
}
