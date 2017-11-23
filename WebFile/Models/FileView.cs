using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebFile.Models
{
    
    public class FileView
    {
        public FileView()
        {
            this.Id = Guid.NewGuid().ToString("N");
        }
        public string Id { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
        public string  FileName { get; set; }
        /// <summary>
        /// 扩展名
        /// </summary>
        public string  FileExtName { get; set; }
        /// <summary>
        /// 文件大小
        /// </summary>
        public int  Size { get; set; }
        /// <summary>
        /// 文件单位 B KB MB GB
        /// </summary>
        public string Unit { get; set; }
        /// <summary>
        /// 文件以字节流的方式存储
        /// </summary>
        public byte[] Path { get; set; }
    }
}