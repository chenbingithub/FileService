using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using WebFile.Models;

namespace WebFile.Controllers
{
    public class FileController : Controller
    {
        private MongoCollection<FileView> _fileInfo;

        public FileController()
        {
            this._fileInfo=new MongoDbHelper().Database.GetCollection<FileView>("FileInfo");
        }
        // GET: File
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Upload()
        {
            try
            {
                HttpPostedFileBase postFile = Request.Files[0];//接收文件

                if (postFile != null)
                {
                    FileView fileView = new FileView();
                    fileView.FileName = Path.GetFileName(postFile.FileName);//文件名称
                    fileView.FileExtName = Path.GetExtension(fileView.FileName);//文件的扩展名
                    if (string.IsNullOrEmpty(ExtToMime(fileView.FileExtName)))
                    {
                        return Json("2");
                    }
                    fileView.Size = postFile.ContentLength;
                    fileView.Unit = "B";
                    var stream = postFile.InputStream;
                    fileView.Path = new byte[stream.Length];
                    stream.Read(fileView.Path, 0, fileView.Path.Length);
                    stream.Close();
                    _fileInfo.Save(fileView);
                    return Json(fileView.Id);
                }
                return Json("0");
            }
            catch (Exception e)
            {
                LogHelper.WriteLog("上传文件出错",e);
                return Json("1");
            }
            
        }

        public ActionResult GetFile(string key)
        {
           var model= _fileInfo.AsQueryable().FirstOrDefault(u => u.Id == key);
            if (model != null)
            {
    
                return File(model.Path, ExtToMime(model.FileExtName));
            }
            return Json("");
        }

        private string ExtToMime(string ext)
        {
            
            string str = "";
            switch (ext)
            {
                case ".jpeg": str = "image/jpeg"; break;
                case ".jpg": str = "image/jpeg"; break;
                case ".txt": str = "text/plain"; break;
                case ".gif": str = "image/gif"; break;
                case ".html": str = "text/html"; break;
                case ".avi": str = "video/x-msvideo"; break;
                case ".xls": str = "application/msexcel"; break;
                case ".doc": str = "application/msword"; break;
                case ".pdf": str = "application/pdf"; break;
                case ".mp3": str = "audio/mpeg"; break;
                default: str = ""; break;
            }
            return str;
        }
    }
}