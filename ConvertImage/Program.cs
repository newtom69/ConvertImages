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
        enum TypeImage
        {
            notAImage,
            WebP,
            Image
        }
        static void Main(string[] args)
        {
            string cheminImage = @".\";
            string nomImage;
            string fileExtension;
            bool webpToSave = false;

            long myCompression = (long)EncoderValue.CompressionNone;
            Encoder myEncoder = Encoder.Compression;
            EncoderParameters myEncoderParameters = new EncoderParameters(1);
            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, myCompression);
            myEncoderParameters.Param[0] = myEncoderParameter;

            if (args.Length == 0)
            {
                Console.WriteLine("usuge : Convert jpg/tiff [no/lzw/nb]");
                return;
            }

            ImageCodecInfo myImageCodecInfo;
            string pathImageSave;
            switch (args[0])
            {
                case "jpeg":
                case "jpg":
                    myImageCodecInfo = GetEncoder(ImageFormat.Jpeg);
                    pathImageSave = @".\Save_jpg\";
                    fileExtension = ".jpg";
                    myEncoder = Encoder.Quality;
                    int qualityTemp = 100;
                    if (args.Length > 1 && !Int32.TryParse(args[1], out qualityTemp)) qualityTemp = 100;
                    long qualityJpg = qualityTemp;
                    myEncoderParameter = new EncoderParameter(myEncoder, qualityJpg);
                    myEncoderParameters.Param[0] = myEncoderParameter;
                    break;

                case "webp":
                    webpToSave = true;
                    pathImageSave = @".\Save_webp\";
                    fileExtension = ".webp";
                    myImageCodecInfo = GetEncoder(ImageFormat.Jpeg); //init only for compil check ok 
                    break;

                case "tiff":
                case "tif":
                    myImageCodecInfo = GetEncoder(ImageFormat.Tiff);
                    pathImageSave = @".\Save_tiff\";
                    myEncoder = Encoder.Compression;
                    myEncoderParameters = new EncoderParameters(1);
                    fileExtension = ".tif";
                    myCompression = (long)EncoderValue.CompressionLZW;
                    if (args.Length > 1)
                    {
                        switch (args[1])
                        {
                            case "lzw":
                                myCompression = (long)EncoderValue.CompressionLZW;
                                break;

                            case "no":
                                myCompression = (long)EncoderValue.CompressionNone;
                                break;

                            default:
                                myCompression = (long)EncoderValue.CompressionNone;
                                break;
                        }
                    }
                    myEncoderParameter = new EncoderParameter(myEncoder, myCompression);
                    myEncoderParameters.Param[0] = myEncoderParameter;
                    break;

                default:
                    Console.WriteLine("Enter a good format argument please (jpg/tif/...");
                    return;
            }


            Directory.CreateDirectory(pathImageSave);
            IEnumerable<string> allFiles = Directory.EnumerateFiles(cheminImage, "*.*");
            foreach (var file in allFiles)
            {
                string cheminEntierImage = file;
                nomImage = file.Substring(cheminImage.Length);
                Console.Write("loading file : " + nomImage + "   ......  ");

                Image theImage = new Bitmap(1, 1); //size of image 1x1
                TypeImage theTypeImage;
                try
                {
                    theImage = new Bitmap(cheminEntierImage);
                    theTypeImage = TypeImage.Image;
                }
                catch (Exception e)
                {
                    try
                    {
                        FileStream theStream = new FileStream(cheminEntierImage, FileMode.Open);
                        WebPFormat imageWebp = new WebPFormat();
                        theImage = imageWebp.Load(theStream);
                        theTypeImage = TypeImage.WebP;
                    }
                    catch (Exception e2)
                    {
                        theTypeImage = TypeImage.notAImage;
                    }
                }

                if (theTypeImage != TypeImage.notAImage)
                {
                    Console.Write("saving image : " + nomImage + fileExtension + "   ......  ");
                    if (!webpToSave)
                    {
                        theImage.Save(pathImageSave + nomImage + fileExtension, myImageCodecInfo, myEncoderParameters);
                    }
                    else if (webpToSave)
                    {
                        WebPFormat imageWebp = new WebPFormat();
                        imageWebp.Save(pathImageSave + nomImage + fileExtension, theImage, 24);
                    }
                    theImage.Dispose();
                    Console.WriteLine("ok");
                }
                else //not a image
                {
                    Console.WriteLine("error with this file");
                }
            }
        }

        static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
    }
}

