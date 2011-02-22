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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CookComputing.XmlRpc;
using System.Net;
using System.Net.Sockets;
using System.Drawing;

namespace o3dcapsicum
{
    class O3DTestApplication
    {
        static void Main(string[] args)
        {
            O3D o3d;
            String localIpAdress = "192.168.0.69";

            if (0 == O3D.getLocalIPAddress(out localIpAdress))
            {
                o3d = new O3D("192.168.1.26", localIpAdress,8080);
            }
            else
            {
                return;
            }

            string fwVersion = "null";
            string deviceType = "null";
            int ret = -1;
            ret = o3d.disconnect();
            ret = o3d.connect(out fwVersion, out deviceType);

            ret = o3d.startLiveImageServer();

            O3DImage[] images = new O3DImage[2];
            images[0] = new O3DImage(O3DImage.ImageType.AmplitudeImage);
            images[1] = new O3DImage(O3DImage.ImageType.DistanceImage);
			
			O3D.ImagerData imager_settings = new O3D.ImagerData();
			ret = o3d.getFrontendSettings(ref imager_settings);
			imager_settings.delayTime = 100;
			ret = o3d.setFrontendSettings(imager_settings);
            ret = o3d.getImageData(ref images,true );
            ret = o3d.stopLiveImageServer();
            ret = o3d.disconnect();

            Bitmap bmp = images[0].toBitmap(O3DImage.BitmapType.GrayScale);
            bmp.Save("C:\\mypic_intensity.png",System.Drawing.Imaging.ImageFormat.Png);
            bmp = images[1].toBitmap(O3DImage.BitmapType.GrayScale);
            bmp.Save("C:\\mypic_grayscale.png",System.Drawing.Imaging.ImageFormat.Png);
            System.Console.WriteLine("Hello World");

        }
 
    }
}
