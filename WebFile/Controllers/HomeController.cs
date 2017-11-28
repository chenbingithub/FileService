using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MongoDB.Driver;
using WebFile.Models;

namespace WebFile.Controllers
{
    public class HomeController : Controller
    {
        private MongoCollection<FileView> _fileInfo;

        public HomeController()
        {
            this._fileInfo = new MongoDbHelper().Database.GetCollection<FileView>("FileInfo");
        }
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        
    }
}