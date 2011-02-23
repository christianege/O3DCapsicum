// 
// O3DCapsicum is a C# Implementation of the ifm electronic gmbh
// O3D2xx distance camera xml-rpc API.
// Copyright (C) 2011  Christian Ege <chege (at) cybertux.org>
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CookComputing.XmlRpc;
using System.Net;
using System.Net.Sockets;
using System.Drawing;

namespace o3dcapsicum
{

    /// <summary>
    /// Represents O3D image data.
    /// </summary>
    public class O3DImage
    {
        /// <summary>
        /// Available image types
        /// </summary>
        public enum ImageType
        {
            /// <summary>
            /// Invalid image
            /// </summary>
            InvalidImage = 0,
            /// <summary>
            /// Radial distance image
            /// </summary>
            DistanceImage = 1,
            /// <summary>
            /// Amplitude / Intensity image
            /// </summary>
            AmplitudeImage = 3,
            /// <summary>
            /// X Component of the normal vector. (Camera model)
            /// </summary>
            XNormalVector = 5,
            /// <summary>
            /// Y Component of the normal vector. (Camera model)
            /// </summary>
            YNormalVector = 6,
            /// <summary>
            /// Z Component of the normal vector. (Camera model)
            /// </summary>
            ZNormalVector = 7,
            /// <summary>
            /// X Component of cartesian (world) coordinates.
            /// </summary>
            XComponent = 8,
            /// <summary>
            /// Y Component of cartesian (world) coordinates.
            /// </summary>
            YComponent = 9,
            /// <summary>
            /// Z Component of cartesian (world) coordinates.
            /// </summary>
            ZComponent = 10
        };

        /// <summary>
        /// Represents the type of current image
        /// </summary>
        public ImageType imageType;
        /// <summary>
        /// Meta data size;
        /// </summary>
        public const int metaSize = 94;
        /// <summary>
        /// Image width
        /// </summary>
        public const int width = 64;
        /// <summary>
        /// Image height 
        /// </summary>
        public const int height = 50;

        /// <summary>
        /// Constructor
        /// </summary>
        public O3DImage(ImageType type)
        {
            imageType = type;
            metaData = new float[metaSize];
            imageData = new float[height, width];
        }
        private O3DImage()
        {
            
        }
        /// <summary>
        /// Meta or header data 
        /// </summary>
        public float[] metaData;

        /// <summary>
        /// The image data 
        /// </summary>
        public float[,] imageData;
        public float min;
        public float max;

        public enum BitmapType { 
            GrayScale = 0,
            PseudoColor = 1
        };

        public Bitmap toBitmap(BitmapType type)
        {
            Bitmap bmp = new Bitmap(height, width);
            if (BitmapType.GrayScale == type)
            {
                for (int x = 0; x < O3DImage.height; x++)
                {
                    for (int y = 0; y < O3DImage.width; y++)
                    {
                        float dum = ((imageData[x, y] - min) / (max - min)) * 255;
                        int gray = (int)dum;
                        bmp.SetPixel(x, y, Color.FromArgb(gray, gray, gray));
                    }
                }
            }
            return bmp;
        }

    }

    /// <summary>
    /// This Interface is for wrapping the xml-rpc calls.
    /// </summary>
    [XmlRpcUrl("http://192.168.0.69:8080")]
    public interface IO3DXmlRpcProxy : IXmlRpcProxy
    {
    
        /// <summary>
        /// Establishes a connection to the device.
        /// </summary>
        /// <param name="ip">The local client IP-Adress, usally the IP of the PC</param>
        /// <param name="heartBeat">This parameter controls the heart beat mechanism</param>
        /// <returns>An array of return values. The first element cotains the return code.</returns>
        [XmlRpcMethod]
        System.Object[] MDAXMLConnectCP(string ip, int heartBeat);
        
        /// <summary>
        /// Disconnects from the device. This removes the "Onli" from the device display.
        /// </summary>
        /// <param name="ip">The local client IP-Adress, usally the IP of the PC</param>
        /// <returns>An array of return values. The first element cotains the return code.</returns>
        [XmlRpcMethod]
        System.Object[] MDAXMLDisconnectCP(string ip);
        
        /// <summary>
        /// Retrieves the imager settings of the O3D
        /// </summary>
        /// <returns>An array of return values. The first element cotains the return code.</returns>
        [XmlRpcMethod]
        System.Object[] MDAXMLGetFrontendData();

        /// <summary>
        /// Controls the imager settings of the O3D.
        /// </summary>
        /// <param name="zero1">Always zero "0", used for compatibility.</param>
        /// <param name="modFreq">Modulation frequency index.</param>
        /// <param name="samplingMode">Sampling mode index.</param>
        /// <param name="zero2">Always zero "0", used for compatibility.</param>
        /// <param name="intTime1">Integration time of the first illumination cycle</param>
        /// <param name="intTime2">Integration time of the second illumination cycle</param>
        /// <param name="resetTime">Reset time between cycles, always 20 us.</param>
        /// <param name="interFrameMute">Delay time between frames. This controlls the frame rate.</param>
        /// <returns>An array of return values. The first element cotains the return code.</returns>
        [XmlRpcMethod]
        System.Object[] MDAXMLSetFrontendData(int zero1, int modFreq, int samplingMode, int zero2,
                                              int intTime1, int intTime2, int resetTime,
                                              int interFrameMute
                        );

        /// <summary>
        /// Controls the image server on the device.
        /// </summary>
        /// <param name="mode">0 - live image server is turned off, 1 - live image server is turned on.</param>
        /// <returns>An array of return values. The first element cotains the return code.</returns>
        [XmlRpcMethod]
        System.Object[] MDAXMLSetWorkingMode(int mode);

        /// <summary>
        /// Gets current working mode
        /// </summary>
        /// <returns>An array of return values. The first element cotains the return code.</returns>
        [XmlRpcMethod]
        System.Object[] MDAXMLGetWorkingMode();

        /// <summary>
        /// Controls which program is executed on the sensor.
        /// </summary>
        /// <param name="zero1">Always zero "0", used for compatibility.</param>
        /// <param name="zero2">Always zero "0", used for compatibility.</param>
        /// <param name="program">Program used. 7 - Live image only. </param>
        /// <returns>An array of return values. The first element cotains the return code.</returns>
        [XmlRpcMethod]
        System.Object[] MDAXMLSetProgram(int zero1, int zero2, int program);

        /// <summary>
        /// Get TCP Port
        /// </summary>
        /// <returns>An array of return values. The first element cotains the return code.</returns>
        [XmlRpcMethod]
        System.Object[] MDAXMLGetTCPPortCP();

        /// <summary>
        /// Get Current Program
        /// </summary>
        /// <returns>An array of return values. The first element cotains the return code</returns>
        [XmlRpcMethod]
        System.Object[] MDAXMLGetProgram();
    }
   
	
    /// <summary>
    /// O3D control class
    /// </summary>
    public class O3D
    {


        public Dictionary<O3DImage.ImageType, string> commands;

		public enum SamplingMode { Standard = 0, HighDynamic = 1 };
        public enum ModulationFrequency { 
            SingleFirst  = 0,
            SingleSecond = 1,
            SingleThird  = 2
        };

		public class ImagerData
        {
            public ModulationFrequency freq;
            public SamplingMode samplingMode;
            public int integrationTimeShort;
            public int integrationTimeLong;
            public int delayTime;			 
        };


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="deviceIPAddr">IP-Adress of the sensor</param>
        /// <param name="localIpAddr">The local IP-Adress </param>
        public O3D(string deviceIPAddr, string localIpAddr, int port)
        {
            this.deviceIPAdress = deviceIPAddr;
            this.localIpAdress = localIpAddr;
            commands = new Dictionary<O3DImage.ImageType, string>();
            commands.Add(O3DImage.ImageType.AmplitudeImage, "i");
            commands.Add(O3DImage.ImageType.DistanceImage, "d");
            commands.Add(O3DImage.ImageType.XComponent, "x");
            commands.Add(O3DImage.ImageType.YComponent, "y");
            commands.Add(O3DImage.ImageType.ZComponent, "z");
            commands.Add(O3DImage.ImageType.XNormalVector, "e");
            commands.Add(O3DImage.ImageType.YNormalVector, "f");
            commands.Add(O3DImage.ImageType.ZNormalVector, "g");
            oldProgram = -1;
            proxy = XmlRpcProxyGen.Create<IO3DXmlRpcProxy>();
            proxy.Url = "http://" + deviceIPAddr + ":" + port;
        }
        
        public static int getLocalIPAddress(out string localIPAddress)
        {
            int ret = 2;
            localIPAddress = ""; 
            // Get Own IP 
            IPAddress[] a = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
            if (1 == a.Length)
            {
                localIPAddress = a[0].ToString();
                ret = 0;
            }
            else
            {
                ret = 1;
            }
            return ret;
        }

        /// <summary>
        /// xml-rpc Wrapper
        /// </summary>
        private IO3DXmlRpcProxy proxy;
        /// <summary>
        /// Sensor IP Adress
        /// </summary>
        private string deviceIPAdress;
        /// <summary>
        /// The local communication IP-Adress
        /// </summary>
        private string localIpAdress;
        /// <summary>
        /// Live image server socket
        /// </summary>
        private Socket sock;
        /// <summary>
        /// Live image server communication endpoint
        /// </summary>
        private IPEndPoint ipEndPoint;
        /// <summary>
        /// Image meta data index 
        /// </summary>
        public enum ImageMetaDataIndex
        {
            /// <summary>
            /// Image data size 
            /// </summary>
            DataSize = 0,
            /// <summary>
            /// Meta data size
            /// </summary>
            HeaderSize,
            /// <summary>
            /// Image type see enum <seealso cref="O3DImage.ImageType">ImageType</seealso>
            /// </summary>
            ImageType,
            /// <summary>
            /// meta data version
            /// </summary>
            Version,
            /// <summary>
            /// Imager sampling mode 
            /// </summary>
            SamplingMode,
            /// <summary>
            /// Illumination mode
            /// </summary>
            IlluMode,
            /// <summary>
            /// Frequency mode
            /// </summary>
            FrequencyMode,
            /// <summary>
            /// Unambigiuos range
            /// </summary>
            UnambiguousRange,
            /// <summary>
            /// Evaluation time for current frame
            /// </summary>
            EvaluationTime,
            /// <summary>
            /// First integration time of current frame
            /// </summary>
            IntegrationTimeExp0,
            /// <summary>
            /// Second integration time of current frame
            /// </summary>
            IntegrationTimeExp1,
            /// <summary>
            /// Time stamp seconds
            /// </summary>
            TimeStampSeconds,
            /// <summary>
            /// Timestamp microseconds
            /// </summary>
            TimeStampUSeconds,
            /// <summary>
            /// Median Filter status
            /// </summary>
            MedianFilter,
            /// <summary>
            /// Mean filter status
            /// </summary>
            MeanFilter,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalA1,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalA2,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalA3,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalA4,
            /// <summary>
            /// Error code
            /// </summary>
            ErrorCode,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalB1,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalB2,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalB3,
            CurrentTriggerMode,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalC1,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalC2,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalC3,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalC4,
            IfmTime,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalD1,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalD2,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalD3,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalD4,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalD5,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalD6,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalD7,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalD8,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalD9,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalD10,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalD11,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalD12,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalD13,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalD14,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalD15,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalD16,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalD17,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalD18,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalD19,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalD20,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalD21,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalD22,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalD23,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalD24,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalD25,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalD26,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalD27,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalD28,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalD29,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalD30,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalD31,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalD32,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalD33,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalD34,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalD35,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalD36,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalD37,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalD38,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalD39,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalD40,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalD41,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalD42,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalD43,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalD44,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalD45,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalD46,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalD47,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalD48,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalD49,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalD50,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalD51,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalD52,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalD53,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalD54,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalD55,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalD56,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalD57,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalD58,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalD59,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalD60,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalD61,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalD62,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalD63,
            /// <summary>
            /// Internal used only, do not rely on this.
            /// </summary>
            InternalD64
        };


        private int oldProgram;

        public int connect(out string fwVersion, out string deviceType)
        {
            System.Object[] ret = proxy.MDAXMLConnectCP(localIpAdress, 1);
            if (0 == (int)ret[0])
            {
                fwVersion = (string)ret[1];
                deviceType = (string)ret[2];
            }
            else
            {
                fwVersion = "";
                deviceType = "";
            }
            return (int)ret[0];
        }

        public int disconnect()
        {
            System.Object[] ret;
            ret = proxy.MDAXMLDisconnectCP(localIpAdress);
            return (int)ret[0];
        }
		
        public int getFrontendSettings(ref ImagerData settings)
        {
            System.Object[] result;
            result = proxy.MDAXMLGetFrontendData();
			if (0 == (int)result[0])
            {
				settings.freq = (ModulationFrequency)result[2];
    			settings.samplingMode = (SamplingMode) result[3];
				settings.integrationTimeShort = (int)result[5];
				settings.integrationTimeLong = (int)result[6];
				settings.delayTime = (int)result[8];

			}
            return (int)result[0];
        }

        public int setFrontendSettings(ImagerData settings)
        {
            System.Object[] ret;
            ret = proxy.MDAXMLSetFrontendData(0, 
			                                  (int)settings.freq, 
			                                  (int)settings.samplingMode, 
			                                  0, 
			                                  settings.integrationTimeLong,
			                                  settings.integrationTimeShort, 20, 
			                                  settings.delayTime
			                                  );
            return (int)ret[0];
        }
		
		/// <summary>
		/// Sets the working mode in the camera 
		/// </summary>
		/// <param name="mode">
		/// A <see cref="System.Int32"/>
		/// The working mode to be applied
		/// </param>
		/// <param name="port">
		/// A <see cref="System.Int32"/>
		/// Returns the current TCP/IP image transfer port
		/// </param>
		/// <returns>
		/// A <see cref="System.Int32"/>
		/// Returns 0 on success and an error code on failure
		/// </returns>
        public int setWorkingMode( int mode,out int port)
        {
            
            System.Object[] ret = proxy.MDAXMLSetWorkingMode(mode);
            port = - 1;
            if (0 == (int)ret[0])
            {
                port = (int)ret[1];
            }
            return (int)ret[0];
        }

		/// <summary>
		/// Retrieves the current working mode of the camera 
		/// </summary>
		/// <param name="mode">
		/// A <see cref="System.Int32"/>
		/// Returns the current working mode
		/// </param>
		/// <returns>
		/// A <see cref="System.Int32"/>
		/// Returns 0 on success and an error code on failure
		/// </returns>
        public int getWorkingMode(out int mode)
        {
            System.Object[] ret = proxy.MDAXMLGetWorkingMode();
            mode = -1;
            if (0 == (int)ret[0])
            {
                mode = (int)ret[1];
            }
            return (int)ret[0];
        }
		
		/// <summary>
		/// Retrieves the TCP/IP Port for image transfer 
		/// </summary>
		/// <param name="port">
		/// A <see cref="System.Int32"/>
		/// A reference to the TCP/IP port of the camera
		/// </param>
		/// <returns>
		/// A <see cref="System.Int32"/>
		/// Returns 0 on success and an error code on failure
		/// </returns>
        public int getTCPPort(out int port)
        {
            System.Object[] ret = proxy.MDAXMLGetTCPPortCP();
            port = -1;
            if (0 == (int)ret[0])
            {
                port = (int)ret[1];
            }
            return (int)ret[0];
        }
		
		/// <summary>
		/// Set the program of the camera. 
		/// </summary>
		/// <param name="prog">
		/// A <see cref="System.Int32"/>
		/// The program to switch 
		/// </param>
		/// <returns>
		/// A <see cref="System.Int32"/>
		/// Returns 0 on success and an error code on failure
		/// </returns>
        public int setProgram(int prog)
        {
            System.Object[] ret;
            ret = proxy.MDAXMLSetProgram(0, 0, prog);
            return (int)ret[0];
        }
		
		/// <summary>
		/// Retrieves the current program set in camera
		/// for the camera this should alwyas return 7 
		/// </summary>
		/// <param name="prog">
		/// A <see cref="System.Int32"/>
		/// A reference to the program variable
		/// </param>
		/// <returns>
		/// A <see cref="System.Int32"/>
		/// Returns 0 on success and an error code on failure
		/// </returns>
        public int getProgram(out int prog)
        {
            System.Object[] ret;
            ret = proxy.MDAXMLGetProgram();
            prog = -1;
            if (0 == (int)ret[0])
            {
                prog = (int)ret[1];
            }
            return (int)ret[0];
        }
        
		/// <summary>
		/// Starts the live image server  
		/// </summary>
		/// <returns>
		/// A <see cref="System.Int32"/>
		/// Returns 0 on success and an error code on failure
		/// </returns>
        public int startLiveImageServer()
        {
            int port = 0;
            int ret = 2;
            if (0 == getTCPPort(out port))
            {
                int workingModePort = 0;
                // we can not check return type
                // because of an bug in firmware version 4041
                ret = setWorkingMode(1, out workingModePort);
                if (-1 == oldProgram)
                {
                    if (0 == getProgram(out oldProgram))
                    {
                        ret = 0;
                    }
                }
                if (0 == ret)
                {
                    ret = setProgram(7);
                    ret = 0;
                    sock = new Socket(AddressFamily.InterNetwork,
                            SocketType.Stream,
                            ProtocolType.Tcp);
                    ipEndPoint = new IPEndPoint(IPAddress.Parse(deviceIPAdress), port);
                    sock.Connect(ipEndPoint);
                }
            }
            else
            {
                ret = 1;
            }
            return ret;
        }
		/// <summary>
		/// Stops the live image server 
		/// </summary>
		/// <returns>
		/// A <see cref="System.Int32"/>
		/// Returns 0 on success and an error code on failure
		/// </returns>
        public int stopLiveImageServer()
        {
            int ret = 0;
            if (-1 != oldProgram)
            {
                ret = setProgram(oldProgram);
            }
            if (sock.Connected)
            {
                Byte[] toSend = Encoding.ASCII.GetBytes("q");
                sock.Send(toSend, toSend.Length, SocketFlags.None);
                sock.Shutdown(SocketShutdown.Both);
                sock.Close();
            }
            return ret;
        }
        
		/// <summary>
		/// Retrieve images from the camera you have to provide a arry with 
		/// image references where which define the image type 
		/// </summary>
		/// <param name="images">
		/// A <see cref="O3DImage[]"/>
		/// the array which containes the images to retrieve.
		/// </param>
		/// <param name="synced">
		/// A <see cref="System.Boolean"/>
		/// This parameter defines if the transfer is synchron which means upper case
		/// letters or asynchronous lowercase letters please refer to the "o3d2xx Programmers Guide"
		/// </param>
		/// <returns>
		/// A <see cref="System.Int32"/>
		/// Return 0 on success otherwise an error code is returned
		/// </returns>
        public int getImageData(ref O3DImage[] images, bool synced)
        {
            int ret = 0;
            try
            {
                if (sock.Connected)
                {
                    string command = "";
                    for (int idx = 0; idx < images.Length; idx++ )
                    {
                        command += commands[images[idx].imageType];
                    }
                    if (synced)
                    {
                        string start = command.Substring(0, 1);
                        string rest = command.Remove(0,1);
                        command = start.ToUpper() + rest;
                    }

                    int size = (O3DImage.metaSize + (O3DImage.height * O3DImage.width)) * 4;
                    Byte[] chunk = new Byte[size];

                    float[] floatChunk = new float[Convert.ToInt32(chunk.Length / 4)];
                    Byte[] toSend = Encoding.ASCII.GetBytes(command);

                    sock.Send(toSend, toSend.Length, SocketFlags.None);
                    for (int idx = 0; idx < images.Length; idx++)
                    {
                        int received = 0;
                        do
                        {
                            received += sock.Receive(chunk, received, size - received, SocketFlags.None);
                        } while (received < size);

                        for (int i = 0; i < floatChunk.Length; i++)
                        {
                            if (BitConverter.IsLittleEndian)
                            {
                                Array.Reverse(chunk, i * 4, 4);
                            }
                            floatChunk[i] = BitConverter.ToSingle(chunk, i * 4);
                        }


                        System.Console.WriteLine("{0}", floatChunk[(int)O3D.ImageMetaDataIndex.UnambiguousRange]);
                        Array.Copy(floatChunk, 0, images[idx].metaData, 0, O3DImage.metaSize);

                        for (int y = 0; y < O3DImage.height; y++)
                        {
                            for (int x = 0; x < O3DImage.width; x++)
                            {
                                images[idx].imageData[y, x] = floatChunk[O3DImage.metaSize + (y * O3DImage.width) + x];
                                images[idx].min = Math.Min(images[idx].imageData[y, x], images[idx].min);
                                images[idx].max = Math.Max(images[idx].imageData[y, x], images[idx].max);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return ret;
        }
    }
}
