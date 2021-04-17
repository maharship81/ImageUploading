using ImageUploading.EF;
using ImageUploading.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ImageUploading.Controllers
{
    public class DefaultController : Controller
    {
        private DBContext _context = new DBContext();

        // GET: Default
        public ActionResult ImageUpload()
        {
            return View();
        }
        public ActionResult ImageSave(ImageUpload obj)
        {
            try
            {
                //ImageUpload objTaskDetails = new ImageUpload();
                HttpFileCollectionBase files = Request.Files;
                var x = "";
                if (!string.IsNullOrEmpty(obj.ToString()))
                {

                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];
                        byte[] ByteImgArray;
                        ByteImgArray = ConvertToBytes(file);
                        var ImageQuality = ConfigurationManager.AppSettings["ImageQuality"];
                        var reduceIMage = ReduceImageSize(ByteImgArray, ImageQuality);
                        string fileName = file.FileName;
                        x += fileName + ",";
                        string serverMapPath = Server.MapPath("~/Images/");
                        string filePath = serverMapPath + "//" + fileName;
                        SaveFile(reduceIMage, filePath, file.FileName);
                    }
                    obj.Image = x.Remove(x.Length - 1);
                    _context.ImageUploads.Add(obj);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return View("ImageUpload", obj);
        }
        #region ImageResize
        private byte[] ConvertToBytes(HttpPostedFileBase image)
        {
            byte[] CoverImageBytes = null;
            BinaryReader reader = new BinaryReader(image.InputStream);
            CoverImageBytes = reader.ReadBytes((int)image.ContentLength);
            return CoverImageBytes;
        }
        public static byte[] ReduceImageSize(byte[] inputBytes, string ImageQuality)
        {
            Byte[] imageBytes;
            int jpegQuality;

            //string ImageQuality = "";/*ConfigurationManager.AppSettings["ImageQuality"];*/
            if (!string.IsNullOrEmpty(ImageQuality))
            {
                jpegQuality = Convert.ToInt32(ImageQuality);
            }
            else
            {
                jpegQuality = 25;
            }

            System.Drawing.Image image;

            using (var inputStream = new MemoryStream(inputBytes))
            {
                // Create an Encoder object based on the GUID  for the Quality parameter category.  
                System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
                image = System.Drawing.Image.FromStream(inputStream);
                var jpegEncoder = ImageCodecInfo.GetImageDecoders().First(c => c.FormatID == ImageFormat.Jpeg.Guid);
                var encoderParameters = new EncoderParameters(1);
                EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 50L);
                encoderParameters.Param[0] = myEncoderParameter;
                using (var outputStream = new MemoryStream())
                {
                    image.Save(outputStream, jpegEncoder, encoderParameters);
                    imageBytes = outputStream.ToArray();
                }
            }
            return imageBytes;
        }
        public string SaveFile(byte[] file, string filePath, string filename)
        {
            string directoryPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryPath))
                System.IO.Directory.CreateDirectory(directoryPath);

            if (file != null)
            {
                using (System.Drawing.Image image = System.Drawing.Image.FromStream(new System.IO.MemoryStream(file)))
                {
                    //var i = Image.FromFile(filePath + file);
                    //var i2 = new Bitmap(i);
                    if (filename.ToLower().Contains(".jpg") || filename.ToLower().Contains(".jpeg"))
                    {
                        image.Save(filePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                        //i2.Save(filePath, ImageFormat.Jpeg);
                    }
                    else if (filename.ToLower().Contains(".png"))
                    {
                        image.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
                        //i2.Save(filePath, ImageFormat.Png);
                    }
                    else if (filename.ToLower().Contains(".bmp"))
                    {
                        image.Save(filePath, System.Drawing.Imaging.ImageFormat.Bmp);
                        //i2.Save(filePath, ImageFormat.Bmp);
                    }
                    else if (filename.ToLower().Contains(".gif"))
                    {
                        image.Save(filePath, System.Drawing.Imaging.ImageFormat.Gif);
                        //i2.Save(filePath, ImageFormat.Gif);
                    }
                }
            }
            return string.Empty;
        }

        #endregion
    }
}