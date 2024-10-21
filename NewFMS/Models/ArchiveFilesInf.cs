using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace NewFMS.Models
{
    //public class FilesInfo
    //{
    //    public string FileName { get; set; }
    //    public string Url { get; set; }
    //}
    public class ArchiveFilesInf
    {
        public IEnumerable<FilesInfo> Files { get; set; }
    }
    public class FilesInfo
    {
        private string fldUrl;
        public int fldId { get; set; }
        public byte[] fldImage { get; set; }
        public string fldPasvand { get; set; }
        public int? fldArchiveTreeId { get; set; }
        public bool fldIsBookMark { get; set; }
        public string Url {
            get
            {
                return fldUrl;
            }
            set
            {
                fldUrl = value;
                if(!value.Contains("Uploaded"))
                {
                    fldImage = Convert.FromBase64String(value.Substring(23));
                }
                else
                {
                    fldImage = new MemoryStream(System.IO.File.ReadAllBytes(System.Web.HttpContext.Current.Server.MapPath(value))).ToArray();
                }
            }
        }
    }
    public class FilesInfo_DataBase
    {
        public int fldId { get; set; }
        public byte[] fldImage { get; set; }
        public string fldPasvand { get; set; }
        public bool fldIsBookMark { get; set; }
        public string Url{ get; set; }
        public int? fldArchiveTreeId { get; set; }
    }
    public class FilesInfo_DataBaseGroup
    {
        public List<FilesInfo_DataBase> Items { get; set; }
        public string fldTitle { get; set; }
    }
}