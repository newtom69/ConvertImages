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
            string nomImage;
            string fileExtension;
            bool flagWebp = false;

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
            string cheminImageSave;
            switch (args[0])
            {
                case "jpeg":
                case "jpg":
                    myImageCodecInfo = GetEncoder(ImageFormat.Jpeg);
                    cheminImageSave = @".\Save_jpg\";
                    fileExtension = ".jpg";
                    myEncoder = Encoder.Quality;
                    int qualityTemp = 100;
                    if (args.Length > 1)
                    {
                        if (!Int32.TryParse(args[1], out qualityTemp)) qualityTemp = 100;
                    }
                    long qualityJpg = qualityTemp;
                    myEncoderParameter = new EncoderParameter(myEncoder, qualityJpg);
                    myEncoderParameters.Param[0] = myEncoderParameter;
                    break;

                case "webp":
                    //TODO IMPLEMENT
                    flagWebp = true;
                    cheminImageSave = @".\Save_webp\";
                    fileExtension = ".webp";

                    //TODO TEST
                    myImageCodecInfo = GetEncoder(ImageFormat.Bmp);
                    myEncoder = Encoder.Quality;
                    qualityTemp = 100;
                    if (args.Length > 1)
                    {
                        if (!Int32.TryParse(args[1], out qualityTemp)) qualityTemp = 100;
                    }
                    qualityJpg = qualityTemp;
                    myEncoderParameter = new EncoderParameter(myEncoder, qualityJpg);
                    myEncoderParameters.Param[0] = myEncoderParameter;



                    break;


                case "tiff":
                case "tif":
                    myImageCodecInfo = GetEncoder(ImageFormat.Tiff);
                    cheminImageSave = @".\Save_tiff\";
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



            Directory.CreateDirectory(cheminImageSave);
            IEnumerable<string> allFiles = Directory.EnumerateFiles(cheminImage, "*.*");
            foreach (var file in allFiles)
            {
                string cheminEntierImage = file;
                nomImage = file.Substring(cheminImage.Length);
                Console.Write("loading file : " + nomImage + "   ......  ");
                Bitmap theBitmap;
                try
                {
                    if (!flagWebp)
                    {
                        theBitmap = new Bitmap(cheminEntierImage);
                    }
                    else
                    {
                        theBitmap = new Bitmap(cheminEntierImage);
                    }
                    Console.Write("saving image : " + nomImage + fileExtension + "   ......  ");
                    try
                    {
                        if (flagWebp)
                        {
                            //Bitmap theBitmap2 = new Bitmap(cheminEntierImage);
                            //theBitmap.Dispose();
                            //FileStream theStream = new FileStream(cheminEntierImage, FileMode.Open);
                            //WebPFormat imageWebp = new WebPFormat();
                            
                            //imageWebp.Save(cheminImageSave + nomImage + fileExtension, theImage,24);
                        }
                        else
                        {
                            theBitmap.Save(cheminImageSave + nomImage + fileExtension, myImageCodecInfo, myEncoderParameters);
                            theBitmap.Dispose();
                        }
                        Console.WriteLine(" ok");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(nomImage + "error with saving :" + e.Message);
                    }
                    
                }
                catch (Exception e)
                {
                    try
                    {
                        FileStream theStream = new FileStream(cheminEntierImage, FileMode.Open);
                        WebPFormat imageWebp = new WebPFormat();
                        Image theImage = imageWebp.Load(theStream);
                        Console.Write("saving image : " + nomImage + fileExtension + "   ......  ");
                        theImage.Save(cheminImageSave + nomImage + fileExtension, myImageCodecInfo, myEncoderParameters);
                        Console.WriteLine("ok");
                    }
                    catch (Exception e2)
                    {
                        Console.WriteLine("error with this file" );
                    }

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

