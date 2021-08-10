/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRKernal.Record
{
    /// <summary> Values that represent codec types. </summary>
    public enum CodecType
    {
        /// <summary> An enum constant representing the local option. </summary>
        Local = 0,
        /// <summary> An enum constant representing the rtmp option. </summary>
        Rtmp = 1,
        /// <summary> An enum constant representing the rtp option. </summary>
        Rtp = 2,
    }

    /// <summary> Values that represent blend modes. </summary>
    public enum BlendMode
    {
        /// <summary> Blend the virtual image and rgb camera image. </summary>
        Blend,
        /// <summary> Only rgb camera image. </summary>
        RGBOnly,
        /// <summary> Only virtual image. </summary>
        VirtualOnly,
        /// <summary> Arrange virtual image and rgb camera image from left to right. </summary>
        WidescreenBlend
    }

    /// <summary> Callback, called when the capture task. </summary>
    /// <param name="task"> The task.</param>
    /// <param name="data"> The data.</param>
    public delegate void CaptureTaskCallback(CaptureTask task, byte[] data);

    /// <summary> A capture task. </summary>
    public struct CaptureTask
    {
        /// <summary> The width of capture image task. </summary>
        public int Width;
        /// <summary> The height of capture image task. </summary>
        public int Height;
        /// <summary> The capture format. </summary>
        public PhotoCaptureFileOutputFormat CaptureFormat;
        /// <summary> The on receive callback. </summary>
        public CaptureTaskCallback OnReceive;

        /// <summary> Constructor. </summary>
        /// <param name="w">        The width.</param>
        /// <param name="h">        The height.</param>
        /// <param name="format">   Describes the format to use.</param>
        /// <param name="callback"> The callback.</param>
        public CaptureTask(int w, int h, PhotoCaptureFileOutputFormat format, CaptureTaskCallback callback)
        {
            this.Width = w;
            this.Height = h;
            this.CaptureFormat = format;
            this.OnReceive = callback;
        }
    }

    /// <summary> A native encode configuration. </summary>
    public struct NativeEncodeConfig
    {
        /// <summary> Gets or sets the width. </summary>
        /// <value> The width. </value>
        public int width { get; private set; }
        /// <summary> Gets or sets the height. </summary>
        /// <value> The height. </value>
        public int height { get; private set; }
        /// <summary> Gets or sets the bit rate. </summary>
        /// <value> The bit rate. </value>
        public int bitRate { get; private set; }
        /// <summary> Gets or sets the FPS. </summary>
        /// <value> The FPS. </value>
        public int fps { get; private set; }
        /// <summary> Gets or sets the type of the codec. </summary>
        /// <value> The type of the codec. </value>
        public int codecType { get; private set; }    // 0 local; 1 rtmp ; 2 rtp
        /// <summary> Gets or sets the full pathname of the out put file. </summary>
        /// <value> The full pathname of the out put file. </value>
        public string outPutPath { get; private set; }
        /// <summary> Gets or sets the use step time. </summary>
        /// <value> The use step time. </value>
        public int useStepTime { get; private set; }
        /// <summary> Gets or sets a value indicating whether this object use alpha. </summary>
        /// <value> True if use alpha, false if not. </value>
        public bool useAlpha { get; private set; }
        /// <summary>
        /// Gets or sets a value indicating whether this object use linner texture. </summary>
        /// <value> True if use linner texture, false if not. </value>
        public bool useLinnerTexture { get; private set; }

        public bool addMicphoneAudio { get; private set; }

        /// <summary> Constructor. </summary>
        /// <param name="w">         The width.</param>
        /// <param name="h">         The height.</param>
        /// <param name="bitrate">   The bitrate.</param>
        /// <param name="f">         An int to process.</param>
        /// <param name="codectype"> The codectype.</param>
        /// <param name="path">      Full pathname of the file.</param>
        /// <param name="usealpha">  (Optional) True to usealpha.</param>
        public NativeEncodeConfig(int w, int h, int bitrate, int f, CodecType codectype, string path, bool useaudio = true, bool usealpha = false)
        {
            this.width = w;
            this.height = h;
            this.bitRate = bitrate;
            this.fps = 30;
            this.codecType = (int)codectype;
            this.outPutPath = path;
            this.useStepTime = 0;
            this.useAlpha = usealpha;
            this.addMicphoneAudio = useaudio;
            this.useLinnerTexture = NRRenderer.isLinearColorSpace;
        }

        /// <summary> Constructor. </summary>
        /// <param name="cameraparam"> The cameraparam.</param>
        /// <param name="path">        (Optional) Full pathname of the file.</param>
        public NativeEncodeConfig(CameraParameters cameraparam, string path = "")
        {
            this.width = cameraparam.blendMode == BlendMode.WidescreenBlend ? 2 * cameraparam.cameraResolutionWidth : cameraparam.cameraResolutionWidth;
            this.height = cameraparam.cameraResolutionHeight;
            this.bitRate = 10240000;
            this.fps = cameraparam.frameRate;
            this.codecType = GetCodecTypeByPath(path);
            this.outPutPath = path;
            this.useStepTime = 0;
            this.addMicphoneAudio = cameraparam.audioState == NRVideoCapture.AudioState.MicAudio ? true : false;
            this.useAlpha = cameraparam.hologramOpacity < float.Epsilon;
            this.useLinnerTexture = NRRenderer.isLinearColorSpace;
        }

        /// <summary> Sets out put path. </summary>
        /// <param name="path"> Full pathname of the file.</param>
        public void SetOutPutPath(string path)
        {
            this.codecType = GetCodecTypeByPath(path);
            this.outPutPath = path;
        }

        /// <summary> Gets codec type by path. </summary>
        /// <param name="path"> Full pathname of the file.</param>
        /// <returns> The codec type by path. </returns>
        private static int GetCodecTypeByPath(string path)
        {
            if (path.StartsWith("rtmp://"))
            {
                return 1;
            }
            else if (path.StartsWith("rtp://"))
            {
                return 2;
            }
            else
            {
                return 0;
            }
        }

        /// <summary> Constructor. </summary>
        /// <param name="config"> The configuration.</param>
        public NativeEncodeConfig(NativeEncodeConfig config)
            : this(config.width, config.height, config.bitRate, config.fps, (CodecType)config.codecType, config.outPutPath, config.addMicphoneAudio, config.useAlpha)
        {
        }

        /// <summary> Convert this object into a string representation. </summary>
        /// <returns> A string that represents this object. </returns>
        public override string ToString()
        {
            return LitJson.JsonMapper.ToJson(this);
        }
    }
}
