using ImageProcessor.Plugins.WebP.Imaging.Formats;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;


namespace Convert
{
    class Program
    {
        static void Main(string[] args)
        {
            string cheminImage = @".\";
            string cheminImageSave = @".\SaveNoCompression\";
            string cheminEntierImage = "";
            string nomImage;
            long compression= (long)EncoderValue.CompressionNone;

            if (args.Length>0)
            {
                switch (args[0])
                {
                    case "lzw":
                        compression = (long)EncoderValue.CompressionLZW;
                        cheminImageSave = @".\SaveLZW\";
                        break;

                    case "no":
                        compression = (long)EncoderValue.CompressionNone;
                        cheminImageSave = @".\SaveNoCompression\";
                        break;

                    default:
                        compression = (long)EncoderValue.CompressionNone;
                        cheminImageSave = @".\SaveNoCompression\";
                        break;
                }
            }

            Directory.CreateDirectory(cheminImageSave);
            ImageCodecInfo myImageCodecInfo;
            myImageCodecInfo = GetEncoderInfo("image/tiff");
            Encoder myEncoder = Encoder.Compression;
            EncoderParameters myEncoderParameters = new EncoderParameters(1);
            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, compression);
            myEncoderParameters.Param[0] = myEncoderParameter;

            IEnumerable<string> allFiles = Directory.EnumerateFiles(cheminImage, "*.*");
            foreach (var file in allFiles)
            {
                cheminEntierImage = file;
                nomImage = file.Substring(cheminImage.Length);

                Console.Write("loading file : " + nomImage + "   ......  ");
                Bitmap leBitmap;
                try
                {
                    leBitmap = new Bitmap(cheminEntierImage);
                    Console.Write("saving image : " + nomImage + ".tif" + "   ......  ");
                    try
                    {
                        leBitmap.Save(cheminImageSave + nomImage + ".tif", myImageCodecInfo, myEncoderParameters);
                        leBitmap.Dispose();
                        Console.Write(" ok\n");
                    }
                    catch (Exception e)
                    {
                        Console.Write(nomImage + "error with saving :" + e.Message + "\n");
                    }
                    
                }
                catch (Exception e)
                {
                    try
                    {
                        FileStream theStream = new FileStream(cheminEntierImage, FileMode.Open);
                        WebPFormat imageWebp = new WebPFormat();
                        Image theImage = imageWebp.Load(theStream);
                        Console.Write("saving image : " + nomImage + ".tif" + "   ......  ");
                        theImage.Save(cheminImageSave + nomImage + ".tif", myImageCodecInfo, myEncoderParameters);
                        Console.Write("ok\n");
                    }
                    catch (Exception e2)
                    {
                        Console.Write("error with this file\n");
                    }

                }
            }
        }

        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (int i = 0; i < encoders.Length; ++i)
            {
                if (encoders[i].MimeType == mimeType)
                    return encoders[i];
            }
            return null;
        }
    }
}

