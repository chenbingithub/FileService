using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Aspose.Cells;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            

            WorkbookDesigner designer = new WorkbookDesigner(new Workbook(@"F:\VS2015\FileService\FileService\WebFile\Upload\Template\test.xlsx"));
            MemoryStream ms=new MemoryStream();
            designer.Workbook.Save(ms, SaveFormat.Pdf);
            ms.Position = 0;

            var content=new MultipartFormDataContent();
            content.Add(new StringContent("test.pdf"),"fileName");
            content.Add(new ByteArrayContent(ms.GetBuffer()),"content");
            var r=new HttpClient().PostAsync("http://localhost:52375/file/StreamToPdf", content).Result.Content.ReadAsStringAsync().Result;
            Console.ReadKey();
        }

        private void TextStrikeType()
        {
            string StrFileName = @"E:\迅雷下载\MongoDB2.rar"; //根据实际情况设置
            string StrUrl = "http://localhost:52375/Upload/MongoDB.rar"; //根据实际情况设置

            //打开上次下载的文件或新建文件
            long lStartPos = 0;
            System.IO.FileStream fs;
            if (System.IO.File.Exists(StrFileName))
            {
                fs = System.IO.File.OpenWrite(StrFileName);
                lStartPos = fs.Length;
                fs.Seek(lStartPos, System.IO.SeekOrigin.Current); //移动文件流中的当前指针
            }
            else
            {
                fs = new System.IO.FileStream(StrFileName, System.IO.FileMode.Create);
                lStartPos = 0;
            }

            //打开网络连接
            try
            {
                System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(StrUrl);
                if (lStartPos > 0)
                    request.AddRange((int)lStartPos); //设置Range值

                //向服务器请求，获得服务器回应数据流
                System.IO.Stream ns = request.GetResponse().GetResponseStream();

                byte[] nbytes = new byte[1024];
                if (ns != null)
                {
                    var nReadSize = ns.Read(nbytes, 0, 1024);
                    while (nReadSize > 0)
                    {
                        fs.Write(nbytes, 0, nReadSize);
                        nReadSize = ns.Read(nbytes, 0, 1024);
                    }
                    fs.Close();
                    ns.Close();
                }
                Console.WriteLine("下载完成");
            }
            catch (Exception ex)
            {
                fs.Close();
                Console.WriteLine("下载过程中出现错误:" + ex.ToString());
            }
        }
    }
}
