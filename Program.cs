using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace DownForIE
{
    class Program
    {
        static void Main(string[] args)
        {
            //适用于该httpUrl地址
            //string Json = "['http://192.171.20.2/imagePool1/MC530/529464/548605/1880057/20200504154951-50530852048760.990027590184.dcm']";
            //string Suffix = ".cer";
            //string DirPath = "C:\\Users\\admin\\UnProject\\DownDir";
            //int count = 1;
            //string Json = args[0];
            //int count = int.Parse(args[1]);
            //---------------------------------------
            //string JsonPath = "C:\\Users\\admin\\UnProject\\20200220齐齐哈尔市人民医院\\DownForIE\\bin\\Release\\1.txt";
            //string Suffix = ".dcm";
            //string DirPath = "./11";
            //1.json内容参数地址 2.后缀 3.下载文件地址
            string JsonPath = args[0];
            string Suffix = args[1];
            string DirPath = args[2];
            
            string Json = FileStreamRead(JsonPath);
            dynamic dyn = Newtonsoft.Json.JsonConvert.DeserializeObject(Json);
            dynamic arr = JValue.Parse(Json);
            JArray json1 = (JArray)JsonConvert.DeserializeObject(Json);
            //JArray array = (JArray)json1["Rows"];
            int count = json1.Count;
           
            Console.WriteLine("dicom文件个数为----->"+ count);
            //Console.WriteLine(Json);
            try
            {
                if (!Directory.Exists(DirPath))
                {

                    Task.Delay(1000);
                    Directory.CreateDirectory(DirPath);
                }
                else
                {
                    DeleteDirectory(DirPath, "");
                    Task.Delay(1000);
                    Directory.CreateDirectory(DirPath);
                }
                int num = 0;
                int errnum = 0;
                var ErrStr = new List<string>();

                for (int i = 0; i < count; i++)
                {
                    if (dyn[i] != null)
                    {
                        string HttpPath = dyn[i];
                        Console.WriteLine(HttpPath);
                        string[] a = HttpPath.Split('.');
                        Array.Reverse(a);
                        string ReverseStr = "";
                        ReverseStr = a[2] + "." + a[1];
                        string[] b = ReverseStr.Split('/');
                        Array.Reverse(b);
                        string ReverseStrName = "";
                        ReverseStrName = b[1] + b[0];
                        Console.WriteLine(ReverseStrName);
                        string Checkid = ReverseStrName;
                        //Console.WriteLine(Checkid);
                        //Console.WriteLine(DirPath);
                        //Console.WriteLine(HttpPath);
                        //Console.WriteLine(Suffix);
                        Console.WriteLine("HttpFileUrl==================>" + HttpPath+ "\n");
                        Console.WriteLine("FilePath==================>" + DirPath + "//" + Checkid + Suffix+ "\n");
                        Console.WriteLine("Number Is ==================>" + i+"\n");

                        int status = button_Click(Checkid, DirPath, HttpPath, Suffix);
                        if (status != 1)
                        {
                            errnum++;
                            ErrStr.Add(HttpPath);
                            Console.WriteLine("警告:---地址错误或目标服务器连接未成功!---⚠⚠⚠⚠\n");
                        }
                        else
                        {
                            num++;
                            Console.WriteLine("😊😊😊😊😊😊😊😊---下载成功!---😊😊😊😊\n");
                        }
                        
                    }
                    else
                    {
                        return;
                    }

                }
                Console.WriteLine("=======================================dicom文件个数为" + "★★" + count + "个★★ =======================================\n\n\n");
                Console.WriteLine("=======================================Success：" + ">>>>>>" + num + "个<<<<<< =======================================\n\n\n");
                Console.WriteLine("=======================================Failed：" + "<<<<<<" + errnum + "个>>>>>> =======================================\n\n\n");
                Console.WriteLine("=======================================错误列表：FailedList=======================================\n");
                int numC = ErrStr.Count;
                for (int k = 0; k < numC; k++)
                {
                    Console.WriteLine("=="+ErrStr[k]+"★"+"No."+k);
                }
                Console.WriteLine("=======================================程序结束：TheExeOver=======================================");
                System.Diagnostics.Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start(); //  开始监视代码运行时间
                                   //  you code ....
                stopwatch.Stop(); //  停止监视
                TimeSpan timespan = stopwatch.Elapsed; //  获取当前实例测量得出的总时间
                double hours = timespan.TotalHours; // 总小时
                double minutes = timespan.TotalMinutes;  // 总分钟
                double seconds = timespan.TotalSeconds;  //  总秒数
                double milliseconds = timespan.TotalMilliseconds;  //  总毫秒数
                Console.WriteLine("程序执行时间++++++++++++++++++++++" + minutes+"min"+ seconds+"s"+ milliseconds + "ms");
            }
            catch (Exception)
            {

                throw;
            }



            return;
        }
        static int button_Click(string FileName,string DirPath, string HttpPath,string Suffix)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(HttpPath);

                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                Console.WriteLine(response);
                Console.WriteLine(response.StatusCode);
                Stream responseStream = response.GetResponseStream();
                Stream stream = new FileStream(DirPath + "//" + FileName + Suffix, FileMode.Create);
                byte[] bArr = new byte[1024];
                int size = responseStream.Read(bArr, 0, bArr.Length);
                while (size > 0)
                {
                    stream.Write(bArr, 0, size);
                    size = responseStream.Read(bArr, 0, bArr.Length);
                }
                stream.Close();
                responseStream.Close();
                return 1;
            }
            catch (Exception)
            {
             
                return 0;
            }          
            
        }

        //删除 文件夹及文件
        public static void DeleteDirectory(string directoryPath, string fileName)
        {

            //删除文件
            for (int i = 0; i < Directory.GetFiles(directoryPath).ToList().Count; i++)
            {
                if (Directory.GetFiles(directoryPath)[i] == fileName)
                {
                    File.Delete(fileName);
                }
            }

            //删除文件夹
            for (int i = 0; i < Directory.GetDirectories(directoryPath).ToList().Count; i++)
            {
                if (Directory.GetDirectories(directoryPath)[i] == fileName)
                {
                    Directory.Delete(fileName, true);
                }
            }
        }

        /// <summary>
        /// 使用FileStream读取文件
        /// </summary>
        /// <param name="path">要读取的文件路径</param>
        /// <param name="str"></param>
        static string FileStreamRead(string path)
        {
            using (FileStream fsRead = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read))
            {
                byte[] buffer = new byte[1024 * 1024 * 5];//每次读取5M长度
                int r = fsRead.Read(buffer, 0, buffer.Length);//返回本次实际读取到的有效字节数
                string str = Encoding.UTF8.GetString(buffer, 0, r);//使用utf-8编码格式
                return str;
            }

        }
    }
}
