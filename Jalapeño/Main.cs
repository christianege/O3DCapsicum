using System;
using o3dcapsicum;
using System.Drawing;

namespace jalapeno
{
	class MainClass
	{
		public static void Main (string[] args)
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

            if( 0 == ret )
			{
				ret = o3d.startLiveImageServer();
			}
			if(0 == ret )
			{
			
				O3D.ImagerData imager_settings = new O3D.ImagerData();
				ret = o3d.getFrontendSettings(ref imager_settings);
				imager_settings.delayTime = 100;
				ret = o3d.setFrontendSettings(imager_settings);
			}
			if( 0 == ret )
			{
				O3DImage[] images = new O3DImage[2];
	            images[0] = new O3DImage(O3DImage.ImageType.AmplitudeImage);
	            images[1] = new O3DImage(O3DImage.ImageType.DistanceImage);
				ret = o3d.getImageData(ref images,true );
	            Bitmap bmp = images[0].toBitmap(O3DImage.BitmapType.GrayScale);
    	        bmp.Save("mypic_intensity.png",System.Drawing.Imaging.ImageFormat.Png);
         		bmp = images[1].toBitmap(O3DImage.BitmapType.GrayScale);
            	bmp.Save("mypic_distance.png",System.Drawing.Imaging.ImageFormat.Png);
			}
            ret = o3d.stopLiveImageServer();
            ret = o3d.disconnect();
		}
	}
}
