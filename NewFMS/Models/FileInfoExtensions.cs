using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
///using Trinet.Core.IO.Ntfs;
using System.Runtime.InteropServices.ComTypes;
using System.Text.RegularExpressions;
using System.Drawing.Imaging;
using System.Text;
using System.Runtime.InteropServices;

namespace NewFMS.Models
{
    public static class FileInfoExtensions
    {
        private const string ZoneIdentifierStreamName = "Zone.Identifier";
        // need to cast because we can't directly implement the interface in C# code
        //var persistFile = (IPersistFile) new PersistentZoneIdentifier();
        //persistFile.Load(filename, (int) (STGM.READWRITE | STGM.SHARE_EXCLUSIVE));

        //// need to cast because we can't directly implement the interface in C# code
        //var zoneId = (IZoneIdentifier) persistFile;
        //var removeResult = zoneId.Remove();

        //persistFile.Save(filename, true);

        //Marshal.ReleaseComObject(persistFile);
        //Marshal.ReleaseComObject(zoneId);
        public enum ImgFormat
        {
            bmp,
            jpeg,
            gif,
            tiff,
            png,
            unknown
        }
        private enum Extensions
        {
            Unknown = 0,
            DocOrXls,
            DocxOrXlsx
        }

        private static readonly Dictionary<Extensions, string> ExtensionSignature = new Dictionary<Extensions, string>
        {
            {Extensions.DocOrXls, "D0-CF-11-E0-A1-B1-1A-E1"},
            //{Extensions.Jpg, "FF-D8-FF-E"},
            //{Extensions.Png, "89-50-4E-47-0D-0A-1A-0A"},
            {Extensions.DocxOrXlsx, "50-4B-03-04-14-00-06-00"}
        };

        public static bool IsExcel(HttpPostedFileBase postedFile)
        {
            //-------------------------------------------
            //  Check the file mime types
            //-------------------------------------------
            if (!string.Equals(postedFile.ContentType, "application/vnd.ms-excel", StringComparison.OrdinalIgnoreCase)
               && !string.Equals(postedFile.ContentType, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            var postedFileExtension = Path.GetExtension(postedFile.FileName);
            //-------------------------------------------
            //  Check the Block extension
            //-------------------------------------------
            //MedicalExaminationsService service = new MedicalExaminationsService();
            //SrvClasses.TOL.OBJ_Param param = new SrvClasses.TOL.OBJ_Param();
            //string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
            //UserInfo user = (UserInfo)(System.Web.HttpContext.Current.Session["User"]);
            //param.FieldName = "fldType";
            //param.Value = postedFileExtension;
            //param.Top = 1;
            //var BlockList = service.SelectFileBlockList(param, user.UserKey, IP).FileBlockList.FirstOrDefault();
            //if (BlockList != null)
            //{
            //    return false;
            //}
            //-------------------------------------------
            //  Check the file extension
            //-------------------------------------------
            if (!string.Equals(postedFileExtension, ".xls", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(postedFileExtension, ".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            try
            {
                var binaryReader = new BinaryReader(postedFile.InputStream);
                var fileData = binaryReader.ReadBytes(postedFile.ContentLength);
                if (fileData.Length < 8)
                    return false;

                var signatureBytes = new byte[8];
                Array.Copy(fileData, signatureBytes, signatureBytes.Length);
                string signature = BitConverter.ToString(signatureBytes);
                Extensions extension = ExtensionSignature.FirstOrDefault(pair => signature.Contains(pair.Value)).Key;

                switch (extension)
                {
                    case Extensions.Unknown:
                        return false;
                    case Extensions.DocOrXls:
                        if (fileData.Length < 512)
                            break;
                        signatureBytes = new byte[4];
                        Array.Copy(fileData, 512, signatureBytes, 0, signatureBytes.Length);
                        signature = BitConverter.ToString(signatureBytes);
                        if (signature == "EC-A5-C1-00")
                            return false;//".doc";
                        return true;//".xls";
                    case Extensions.DocxOrXlsx:
                        string fileBody = Encoding.UTF8.GetString(fileData);
                        //if (fileBody.Contains("word"))
                        //    return false;//".docx";
                        if (fileBody.Contains("xl"))
                            return true;//".xlsx";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                postedFile.InputStream.Position = 0;
            }
        }
        public static bool IsExcel2(HttpPostedFileBase postedFile)
        {
            //-------------------------------------------
            //  Check the file mime types
            //-------------------------------------------
            if (!string.Equals(postedFile.ContentType, "application/vnd.ms-excel", StringComparison.OrdinalIgnoreCase)
               && !string.Equals(postedFile.ContentType, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            var postedFileExtension = Path.GetExtension(postedFile.FileName);
            //-------------------------------------------
            //  Check the Block extension
            //-------------------------------------------
            //MedicalExaminationsService service = new MedicalExaminationsService();
            //SrvClasses.TOL.OBJ_Param param = new SrvClasses.TOL.OBJ_Param();
            //string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
            //UserInfo user = (UserInfo)(System.Web.HttpContext.Current.Session["User"]);
            //param.FieldName = "fldType";
            //param.Value = postedFileExtension;
            //param.Top = 1;
            //var BlockList = service.SelectFileBlockList(param, user.UserKey, IP).FileBlockList.FirstOrDefault();
            //if (BlockList != null)
            //{
            //    return false;
            //}
            //-------------------------------------------
            //  Check the file extension
            //-------------------------------------------
            if (!string.Equals(postedFileExtension, ".xls", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(postedFileExtension, ".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            try
            {
                var binaryReader = new BinaryReader(postedFile.InputStream);
                var fileData = binaryReader.ReadBytes(postedFile.ContentLength);
                if (fileData.Length < 8)
                    return false;

                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                postedFile.InputStream.Position = 0;
            }
        }
        public static bool IsWord(HttpPostedFileBase postedFile)
        {
            //-------------------------------------------
            //  Check the file mime types
            //-------------------------------------------
            if (!string.Equals(postedFile.ContentType, "application/msword", StringComparison.OrdinalIgnoreCase)
               && !string.Equals(postedFile.ContentType, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            var postedFileExtension = Path.GetExtension(postedFile.FileName);
            //-------------------------------------------
            //  Check the Block extension
            //-------------------------------------------
            //MedicalExaminationsService service = new MedicalExaminationsService();
            //SrvClasses.TOL.OBJ_Param param = new SrvClasses.TOL.OBJ_Param();
            //string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
            //UserInfo user = (UserInfo)(System.Web.HttpContext.Current.Session["User"]);
            //param.FieldName = "fldType";
            //param.Value = postedFileExtension;
            //param.Top = 1;
            //var BlockList = service.SelectFileBlockList(param, user.UserKey, IP).FileBlockList.FirstOrDefault();
            //if (BlockList != null)
            //{
            //    return false;
            //}
            //-------------------------------------------
            //  Check the file extension
            //-------------------------------------------
            if (!string.Equals(postedFileExtension, ".doc", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(postedFileExtension, ".docx", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            try
            {
                var binaryReader = new BinaryReader(postedFile.InputStream);
                var fileData = binaryReader.ReadBytes(postedFile.ContentLength);
                if (fileData.Length < 8)
                    return false;

                var signatureBytes = new byte[8];
                Array.Copy(fileData, signatureBytes, signatureBytes.Length);
                string signature = BitConverter.ToString(signatureBytes);
                Extensions extension = ExtensionSignature.FirstOrDefault(pair => signature.Contains(pair.Value)).Key;

                switch (extension)
                {
                    case Extensions.Unknown:
                        return false;
                    case Extensions.DocOrXls:
                        if (fileData.Length < 512)
                            break;
                        signatureBytes = new byte[4];
                        Array.Copy(fileData, 512, signatureBytes, 0, signatureBytes.Length);
                        signature = BitConverter.ToString(signatureBytes);
                        if (signature == "EC-A5-C1-00")
                            return true;//".doc";
                        return false;//".xls";
                    case Extensions.DocxOrXlsx:
                        string fileBody = Encoding.UTF8.GetString(fileData);
                        if (fileBody.Contains("word"))
                            return true;//".docx";
                        //if (fileBody.Contains("xl"))
                        //    return false;//".xlsx";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                postedFile.InputStream.Position = 0;
            }
        }
        public static bool IsPowerPoint(HttpPostedFileBase postedFile)
        {
            //-------------------------------------------
            //  Check the file mime types
            //-------------------------------------------
            if (!string.Equals(postedFile.ContentType, "application/vnd.ms-powerpoint", StringComparison.OrdinalIgnoreCase)
               && !string.Equals(postedFile.ContentType, "application/vnd.openxmlformats-officedocument.presentationml.presentation", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(postedFile.ContentType, "vnd.openxmlformats-officedocument.presentationml.slideshow", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            var postedFileExtension = Path.GetExtension(postedFile.FileName);
            //-------------------------------------------
            //  Check the Block extension
            //-------------------------------------------
            //MedicalExaminationsService service = new MedicalExaminationsService();
            //SrvClasses.TOL.OBJ_Param param = new SrvClasses.TOL.OBJ_Param();
            //string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
            //UserInfo user = (UserInfo)(System.Web.HttpContext.Current.Session["User"]);
            //param.FieldName = "fldType";
            //param.Value = postedFileExtension;
            //param.Top = 1;
            //var BlockList = service.SelectFileBlockList(param, user.UserKey, IP).FileBlockList.FirstOrDefault();
            //if (BlockList != null)
            //{
            //    return false;
            //}
            //-------------------------------------------
            //  Check the file extension
            //-------------------------------------------
            if (!string.Equals(postedFileExtension, ".pps", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(postedFileExtension, ".ppt", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(postedFileExtension, ".pptx", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(postedFileExtension, ".ppsx", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            try
            {
                var binaryReader = new BinaryReader(postedFile.InputStream);
                var fileData = binaryReader.ReadBytes(postedFile.ContentLength);
                if (fileData.Length < 8)
                    return false;

                var signatureBytes = new byte[8];
                Array.Copy(fileData, signatureBytes, signatureBytes.Length);
                string signature = BitConverter.ToString(signatureBytes);
                if (signature != "50-4B-03-04-14-00-06-00"&&signature!="D0-CF-11-E0-A1-B1-1A-E1" )
                {
                    return false;
                }

               
               
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                postedFile.InputStream.Position = 0;
            }
            return true;
        }
        public static ImgFormat GetImageFormat(byte[] bytes)
        {
            var bmp = new byte[] { 66, 77 };//Encoding.ASCII.GetBytes("BM");     // BMP
            var gif87a = new byte[] { 71, 73, 70, 56, 55, 97 }; //Encoding.ASCII.GetBytes("GIF");    // GIF
            var gif89a = new byte[] { 71, 73, 70, 56, 57, 97 }; //Encoding.ASCII.GetBytes("GIF");    // GIF
            var png = new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 };//new byte[] { 137, 80, 78, 71 };    // PNG
            var tiff = new byte[] { 73, 73, 42, 0 };//new byte[] { 73, 73, 42 };         // TIFF
            var tiff2 = new byte[] { 77, 77, 0, 42 };//new byte[] { 77, 77, 42};         // TIFF
            var jpeg = new byte[] { 255, 216, 255, 224 }; // jpeg
            var jpeg2 = new byte[] { 255, 216, 255, 225 }; // jpeg canon

            if (bmp.SequenceEqual(bytes.Take(bmp.Length)))
                return ImgFormat.bmp;

            if (gif87a.SequenceEqual(bytes.Take(gif87a.Length)))
                return ImgFormat.gif;

            if (gif89a.SequenceEqual(bytes.Take(gif89a.Length)))
                return ImgFormat.gif;

            if (png.SequenceEqual(bytes.Take(png.Length)))
                return ImgFormat.png;

            if (tiff.SequenceEqual(bytes.Take(tiff.Length)))
                return ImgFormat.tiff;

            if (tiff2.SequenceEqual(bytes.Take(tiff2.Length)))
                return ImgFormat.tiff;

            if (jpeg.SequenceEqual(bytes.Take(jpeg.Length)))
                return ImgFormat.jpeg;

            if (jpeg2.SequenceEqual(bytes.Take(jpeg2.Length)))
                return ImgFormat.jpeg;

            return ImgFormat.unknown;
        }
        public static void Unblock(this FileInfo file)
        {
            if (file == null)
            {
                throw new ArgumentNullException("file");
            }

            if (!file.Exists)
            {
                throw new FileNotFoundException("Unable to find the specified file.", file.FullName);
            }

            //if (file.Exists && file.AlternateDataStreamExists(ZoneIdentifierStreamName))
            //{
            //    file.DeleteAlternateDataStream(ZoneIdentifierStreamName);
            //}
        }
        public static bool IsPDF(HttpPostedFileBase postedFile)
        {
            //-------------------------------------------
            //  Check the image mime types
            //-------------------------------------------
            if (!string.Equals(postedFile.ContentType, "application/pdf", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            var postedFileExtension = Path.GetExtension(postedFile.FileName);
            //-------------------------------------------
            //  Check the Block extension
            //-------------------------------------------
            //MedicalExaminationsService service = new MedicalExaminationsService();
            //SrvClasses.TOL.OBJ_Param param = new SrvClasses.TOL.OBJ_Param();
            //string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
            //UserInfo user = (UserInfo)(System.Web.HttpContext.Current.Session["User"]);
            //param.FieldName = "fldType";
            //param.Value = postedFileExtension;
            //param.Top = 1;
            //var BlockList = service.SelectFileBlockList(param, user.UserKey, IP).FileBlockList.FirstOrDefault();
            //if (BlockList != null)
            //{
            //    return false;
            //}

            //-------------------------------------------
            //  Check the file extension
            //-------------------------------------------
            if (!string.Equals(postedFileExtension, ".pdf", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            //-------------------------------------------
            //  Check Content of First Bytes
            //-------------------------------------------
            try
            {
                byte[] buffer = null;
                BinaryReader br = new BinaryReader(postedFile.InputStream);
                buffer = br.ReadBytes(5);

                var enc = new ASCIIEncoding();
                var header = enc.GetString(buffer);

                //%PDF−1.0
                // If you are loading it into a long, this is (0x04034b50).
                if (buffer[0] == 0x25 && buffer[1] == 0x50
                    && buffer[2] == 0x44 && buffer[3] == 0x46)
                {
                    return header.StartsWith("%PDF-");
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                postedFile.InputStream.Position = 0;
            }
        }

        public const int ImageMinimumBytes = 512;
        public static bool IsImage(HttpPostedFileBase postedFile)
        {
            //-------------------------------------------
            //  Check the image mime types
            //-------------------------------------------
            if (!string.Equals(postedFile.ContentType, "image/jpg", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(postedFile.ContentType, "image/jpeg", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(postedFile.ContentType, "image/pjpeg", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(postedFile.ContentType, "image/gif", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(postedFile.ContentType, "image/x-png", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(postedFile.ContentType, "image/png", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(postedFile.ContentType, "image/bmp", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(postedFile.ContentType, "image/tiff", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            var postedFileExtension = Path.GetExtension(postedFile.FileName);
            //-------------------------------------------
            //  Check the Block extension
            //-------------------------------------------
            //MedicalExaminationsService service = new MedicalExaminationsService();
            //SrvClasses.TOL.OBJ_Param param = new SrvClasses.TOL.OBJ_Param();
            //string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
            //UserInfo user = (UserInfo)(System.Web.HttpContext.Current.Session["User"]);
            //param.FieldName = "fldType";
            //param.Value = postedFileExtension;
            //param.Top = 1;
            //var BlockList = service.SelectFileBlockList(param, user.UserKey, IP).FileBlockList.FirstOrDefault();
            //if (BlockList != null)
            //{
            //    return false;
            //}

            //-------------------------------------------
            //  Check the image extension
            //-------------------------------------------
            if (!string.Equals(postedFileExtension, ".jpg", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(postedFileExtension, ".png", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(postedFileExtension, ".gif", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(postedFileExtension, ".jpeg", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(postedFileExtension, ".bmp", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(postedFileExtension, ".tiff", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(postedFileExtension, ".tif", StringComparison.OrdinalIgnoreCase)
                )
            {
                return false;
            }

            //-------------------------------------------
            //  Attempt to read the file and check the first bytes
            //-------------------------------------------
            try
            {
                if (!postedFile.InputStream.CanRead)
                {
                    return false;
                }

                //------------------------------------------
                //   Check whether the image size exceeding the limit or not
                //------------------------------------------ 
                if (postedFile.ContentLength < ImageMinimumBytes)
                {
                    return false;
                }

                byte[] buffer = new byte[ImageMinimumBytes];
                postedFile.InputStream.Read(buffer, 0, ImageMinimumBytes);
                string content = System.Text.Encoding.UTF8.GetString(buffer);
                if (Regex.IsMatch(content, @"<script|<html|<head|<title|<body|<pre|<table|<a\s+href|<img|<plaintext|<cross\-domain\-policy",
                    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline))
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                postedFile.InputStream.Position = 0;
            }
            //-------------------------------------------
            //  Try to instantiate new Bitmap, if .NET will throw exception
            //  we can assume that it's not a valid image
            //-------------------------------------------

            try
            {
                using (var bitmap = new System.Drawing.Bitmap(postedFile.InputStream))
                {
                    if (!bitmap.RawFormat.Equals(ImageFormat.Bmp) &&
                       !bitmap.RawFormat.Equals(ImageFormat.Gif) &&
                       !bitmap.RawFormat.Equals(ImageFormat.Jpeg) &&
                       !bitmap.RawFormat.Equals(ImageFormat.Png) &&
                        !bitmap.RawFormat.Equals(ImageFormat.Tiff))
                        return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                postedFile.InputStream.Position = 0;
            }

            //-------------------------------------------
            //  Check Content of First Bytes
            //-------------------------------------------
            try
            {
                var binaryReader = new BinaryReader(postedFile.InputStream);
                var fileData = binaryReader.ReadBytes(postedFile.ContentLength);
                var Format = GetImageFormat(fileData);
                //var aaa = IsValidImageFile(fileData);
                if (Format == ImgFormat.unknown)
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                postedFile.InputStream.Position = 0;
            }
            return true;
        }
        public static bool IsMP4(HttpPostedFileBase postedFile)
        {
            //-------------------------------------------
            //  Check the image mime types
            //-------------------------------------------
            if (!string.Equals(postedFile.ContentType, "video/mp4", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            var postedFileExtension = Path.GetExtension(postedFile.FileName);
            //-------------------------------------------
            //  Check the Block extension
            //-------------------------------------------
            //MedicalExaminationsService service = new MedicalExaminationsService();
            //SrvClasses.TOL.OBJ_Param param = new SrvClasses.TOL.OBJ_Param();
            //string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
            //UserInfo user = (UserInfo)(System.Web.HttpContext.Current.Session["User"]);
            //param.FieldName = "fldType";
            //param.Value = postedFileExtension;
            //param.Top = 1;
            //var BlockList = service.SelectFileBlockList(param, user.UserKey, IP).FileBlockList.FirstOrDefault();
            //if (BlockList != null)
            //{
            //    return false;
            //}

            //-------------------------------------------
            //  Check the file extension
            //-------------------------------------------
            if (!string.Equals(postedFileExtension, ".mp4", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            //-------------------------------------------
            //  Attempt to read the file
            //-------------------------------------------
            try
            {
                if (!postedFile.InputStream.CanRead)
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }

            //-------------------------------------------
            //  Check Content of First Bytes
            //-------------------------------------------
            try
            {
                var binaryReader = new BinaryReader(postedFile.InputStream);
                var fileData = binaryReader.ReadBytes(postedFile.ContentLength);
                var signatureBytes = new byte[8];
                Array.Copy(fileData, 4, signatureBytes, 0, signatureBytes.Length);
                string signature = BitConverter.ToString(signatureBytes);

                if (signature != "66-74-79-70-69-73-6F-6D" && signature != "66-74-79-70-6D-70-34-32" && signature != "66-74-79-70-4D-53-4E-56")
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                postedFile.InputStream.Position = 0;
            }
            return true;
        }

        //public static bool blockList(HttpPostedFileBase postedFile)
        //{
        //    //-------------------------------------------
        //    //  Check the image mime types
        //    //-------------------------------------------

        //    var postedFileExtension = Path.GetExtension(postedFile.FileName);
        //    //-------------------------------------------
        //    //  Check the Block extension
        //    //-------------------------------------------
        //    MedicalExaminationsService service = new MedicalExaminationsService();
        //    SrvClasses.TOL.OBJ_Param param = new SrvClasses.TOL.OBJ_Param();
        //    string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        //    UserInfo user = (UserInfo)(System.Web.HttpContext.Current.Session["User"]);
        //    param.FieldName = "fldType";
        //    param.Value = postedFileExtension;
        //    param.Top = 1;
        //    var BlockList = service.SelectFileBlockList(param, user.UserKey, IP).FileBlockList.FirstOrDefault();
        //    if (BlockList != null)
        //    {
        //        return false;
        //    }
        //    else
        //        return true;
        //}
    }
}