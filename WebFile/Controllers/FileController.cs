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
        public ActionResult FileDataGrid(int page = 1, int pagesize = 20)
        {
            var data = _fileInfo.FindAll().ToList();
            var json = data.Skip(pagesize * (page - 1)).Take(pagesize).Select(u=>new {u.Id,u.FileName,u.FileExtName,u.Size});
            return Json(new { Rows = json, Total = data.Count });
        }
        /// <summary>
        /// 上传图片或文件
        /// </summary>
        /// <returns></returns>
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
                        return Json("400",JsonRequestBehavior.AllowGet);
                    }
                    fileView.Size = postFile.ContentLength;
                    
                    if (fileView.Size > 1024*1024)
                    {
                        if (fileView.FileExtName== ".gif" || fileView.FileExtName == ".jpeg" || fileView.FileExtName == ".jpg")
                        {
                            string path = Server.MapPath("/Upload/Image/");
                            if (!Directory.Exists(path))
                            {
                                Directory.CreateDirectory(path);
                            }
                            fileView.TempPath = path + fileView.RealName + fileView.FileExtName;
                            postFile.SaveAs(path+ fileView.RealName + fileView.FileExtName);
                        }
                        else
                        {
                            string path = Server.MapPath("/Upload/File/");
                            if (!Directory.Exists(path))
                            {
                                Directory.CreateDirectory(path);
                            }
                            fileView.TempPath = path + fileView.RealName;
                            postFile.SaveAs(path + fileView.RealName);
                        }
                    }
                    else
                    {
                        fileView.Unit = "B";
                        var stream = postFile.InputStream;
                        fileView.Path = new byte[stream.Length];
                        stream.Read(fileView.Path, 0, fileView.Path.Length);
                        stream.Close();
                    }
                    
                    _fileInfo.Save(fileView);
                    return Json(fileView.Id);
                }
                return Json("404", JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                LogHelper.WriteLog("上传文件出错",e);
                return Json("500", JsonRequestBehavior.AllowGet);
            }
            
        }
        /// <summary>
        /// 获取文件或图片
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public ActionResult GetFile(string key)
        {
            try
            {
                var model = _fileInfo.AsQueryable().FirstOrDefault(u => u.Id == key);
                if (model != null)
                {
                    if (model.Size > 1024 * 1024)
                    {
                        if (model.FileExtName == ".gif" || model.FileExtName == ".jpeg" ||
                            model.FileExtName == ".jpg")
                        {
                            return File(model.TempPath, ExtToMime(model.FileExtName));
                        }
                        else
                        {
                            
                            if (model.Size > 1024 * 1024*30)
                            {
                                System.IO.FileInfo fileInfo = new System.IO.FileInfo(model.TempPath);
                                if (fileInfo.Exists == true)
                                {
                                                     const long chunkSize = 102400;//100K 每次读取文件，只读取100K，这样可以缓解服务器的压力
                                                    byte[] buffer = new byte[chunkSize];
                                    
                                                    Response.Clear();
                                                    System.IO.FileStream iStream = System.IO.File.OpenRead(model.TempPath);
                                                     long dataLengthToRead = iStream.Length;//获取下载的文件总大小
                                                     Response.ContentType = "application/octet-stream";
                                                    Response.AddHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(model.FileName));
                                                     while (dataLengthToRead > 0 && Response.IsClientConnected)
                                                         {
                                                             int lengthRead = iStream.Read(buffer, 0, Convert.ToInt32(chunkSize));//读取的大小
                                                             Response.OutputStream.Write(buffer, 0, lengthRead);
                                                             Response.Flush();
                                                             dataLengthToRead = dataLengthToRead - lengthRead;
                                                         }
                                                     Response.Close();
                                    return Json("ok", JsonRequestBehavior.AllowGet);

                                }
                            }
                            else
                            {
                                return File(model.TempPath, ExtToMime(model.FileExtName), Server.UrlEncode(model.FileName));
                            }  
                        }
                    }
                    else
                    {
                        return File(model.Path, ExtToMime(model.FileExtName));
                    }

                }
                return Json("404", JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {

                LogHelper.WriteLog("上传文件出错", e);
                return Json("500", JsonRequestBehavior.AllowGet);
            }
          
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
                case ".zip": str = "application/zip"; break;
                case ".rar": str = "application/octet-stream"; break;
                default: str = "application/x-msdownload"; break;
            }
            return str;
        }
        /// <summary>
        /// 删除文件或图片
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public ActionResult Delete(string key)
        {
            try
            {
                var model = _fileInfo.AsQueryable().FirstOrDefault(u => u.Id == key);
                if (model != null)
                {
                    if (model.Size > 1024 * 1024)
                    {
                        FileInfo info=new FileInfo(model.TempPath);
                        if (info.Exists)
                        {
                            info.Delete();
                        }
                        var query = MongoDB.Driver.Builders.Query<FileView>.EQ(u => u.Id, key);
                        _fileInfo.Remove(query);
                        return Json("200");
                    }
                    else
                    {
                        var query = MongoDB.Driver.Builders.Query<FileView>.EQ(u => u.Id, key);
                        _fileInfo.Remove(query);
                        return Json("200");
                    }

                }
                return Json("404", JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {

                LogHelper.WriteLog("删除文件出错", e);
                return Json("500", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult StreamToPdf(string fileName)
        {
            try
            {
                var stream = HttpContext.Request.InputStream;
                if (stream != Stream.Null && stream.Length > 0)
                {
                    FileView fileView = new FileView();
                    fileView.FileName = fileName;//文件名称
                    fileView.FileExtName =".pdf";//文件的扩展名
                    fileView.Size = (int)stream.Length;
                    if (fileView.Size > 1024 * 100)
                    {
                        
                            string path = Server.MapPath("/Upload/File/");
                            if (!Directory.Exists(path))
                            {
                                Directory.CreateDirectory(path);
                            }
                            fileView.TempPath = path + fileView.RealName;
                        const long chunkSize = 102400;//100K 每次读取文件，只读取100K，这样可以缓解服务器的压力
                        long dataLengthToRead = stream.Length;//获取文件总大小
                        var buff = new byte[chunkSize];
                        FileStream file = new FileStream(fileView.TempPath, FileMode.CreateNew);
                        BinaryWriter bw = new BinaryWriter(file);
                        while (dataLengthToRead > 0)
                        {
                            int lengthRead = stream.Read(buff, 0, Convert.ToInt32(chunkSize));//读取的大小
                            bw.Write(buff,0, lengthRead);
                            dataLengthToRead = dataLengthToRead - lengthRead;
                        }
                        bw.Close();
                        file.Close();

                    }
                    else
                    {
                        fileView.Unit = "B";
                        fileView.Path = new byte[stream.Length];
                        stream.Read(fileView.Path, 0, fileView.Path.Length);
                        stream.Close();
                    }

                    _fileInfo.Save(fileView);
                    return Json(fileView.Id);
                }
                return Json("404", JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {

                LogHelper.WriteLog("上传文件出错", e);
                return Json("500", JsonRequestBehavior.AllowGet);
            }
        }

        
    }
}