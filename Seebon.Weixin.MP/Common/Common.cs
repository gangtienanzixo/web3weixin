using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Seebon.Weixin.MP.Common
{
    public class Common
    {
        private const string Key64 = "SEEBONWX"; //注意了，是8个字符，64位

        private const string Iv64 = "SEEBONWX";


        public static string Encode(string data)
        {
            byte[] byKey = System.Text.Encoding.ASCII.GetBytes(Key64);
            byte[] byIv = System.Text.Encoding.ASCII.GetBytes(Iv64);

            var cryptoProvider = new DESCryptoServiceProvider();
            int i = cryptoProvider.KeySize;
            var ms = new MemoryStream();
            var cst = new CryptoStream(ms, cryptoProvider.CreateEncryptor(byKey, byIv), CryptoStreamMode.Write);

            var sw = new StreamWriter(cst);
            sw.Write(data);
            sw.Flush();
            cst.FlushFinalBlock();
            sw.Flush();
            return Convert.ToBase64String(ms.GetBuffer(), 0, (int) ms.Length).Replace("+", "%2$%");

        }

        public static string Decode(string data)
        {
            byte[] byKey = System.Text.Encoding.ASCII.GetBytes(Key64);
            byte[] byIv = System.Text.Encoding.ASCII.GetBytes(Iv64);

            byte[] byEnc;
            try
            {
                byEnc = Convert.FromBase64String(data.Replace("%2$%", "+"));
            }
            catch
            {
                return null;
            }

            var cryptoProvider = new DESCryptoServiceProvider();
            var ms = new MemoryStream(byEnc);
            var cst = new CryptoStream(ms, cryptoProvider.CreateDecryptor(byKey, byIv), CryptoStreamMode.Read);
            var sr = new StreamReader(cst);
            return sr.ReadToEnd();
        }

        public static string GetMd5Str(string convertString)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            string t2 = BitConverter.ToString(md5.ComputeHash(UTF8Encoding.Default.GetBytes(convertString)), 4, 8);
            t2 = t2.Replace("-", "");
            return t2;
        }

        public static string GetAppId
        {
            get
            {
                return "wxaeda291fbad95f96";
            }
        }

        public static string GetAppSecret
        {
            get
            {
                return "48c559b2b35fdc936e894771e0837c4d";
            }
        }

        /// <summary>
        /// Application格式为"token,time"
        /// </summary>
        public static string GetAccessToken
        {
            get
            {
                if (HttpContext.Current.Application["accesstoken"] == null)
                {
                    HttpContext.Current.Application["accesstoken"] =
                        CommonAPIs.CommonApi.GetToken(GetAppId, GetAppSecret)
                            .access_token + "," + DateTime.Now.ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    DateTime time =
                        Convert.ToDateTime(
                            HttpContext.Current.Application["accesstoken"].ToString().Split(new[]{','})[1]);
                    if (time.AddSeconds(7200) < DateTime.Now)
                    {
                        HttpContext.Current.Application["accesstoken"] =
                            CommonAPIs.CommonApi.GetToken(GetAppId, GetAppSecret)
                                .access_token + "," + DateTime.Now.ToString(CultureInfo.InvariantCulture);
                    }
                }
                return HttpContext.Current.Application["accesstoken"].ToString().Split(new[]{','})[0];
            }
        }

        /// <summary>
        /// 把\\n转换为换行
        /// </summary>
        /// <param name="strSource"></param>
        /// <returns></returns>
        public static string ConvertoWordWrap(string strSource)
        {
            return strSource.Replace("\\n", ((char) 10).ToString(CultureInfo.InvariantCulture));
        }
    }

    /// <summary>
    /// WriteInLog 的摘要说明。
    /// </summary>
    public class WriteInLog
    {
        private string logFileName;
        private int logFileSizes;

        /// <summary>
        /// 写入日志文件
        /// </summary>
        public WriteInLog()
        {
            logFileName = @"d:\seebon\WeixinService\app_data\log.txt";
        }

        /// <summary>
        /// 自动删除日志文件大小,此方法已经重载.
        /// </summary>
        /// <param name="fileSize">日志文件大小,单位KB</param>
        public WriteInLog(int fileSize)
            : this()
        {
            if (fileSize != 0)
            {
                this.logFileSizes = fileSize * 1024;
            }
            else
            {
                this.logFileSizes = 1024;
            }
        }
        /// <summary>
        /// 日志文件完全名,如:@"d:\seebon\WeixinService\app_data\log.txt"
        /// </summary>
        public string LogFileName
        {
            set
            {
                this.logFileName = value;
            }
        }

        /// <summary>
        /// 写入日志信息
        /// </summary>
        /// <param name="msg">日志内容</param>
        public void writeInLog(string msg)
        {
            if (logFileSizes != 0)
            {
                writeInLog(msg, true);
            }
            else
            {
                writeInLog(msg, false);
            }
        }

        /// <summary>
        /// 写入日志信息
        /// </summary>
        /// <param name="msg">日志内容</param>
        /// <param name="IsAutoDelete">是否自动删除日志</param>
        private void writeInLog(string msg, bool IsAutoDelete)
        {
            try
            {
                FileInfo fileinfo = new FileInfo(logFileName);
                if (IsAutoDelete)
                {
                    if (fileinfo.Exists && fileinfo.Length >= logFileSizes)
                    {
                        fileinfo.Delete();
                    }
                }
                using (FileStream fs = fileinfo.OpenWrite())
                {
                    StreamWriter sw = new StreamWriter(fs);
                    sw.BaseStream.Seek(0, SeekOrigin.End);
                    sw.WriteLine("=====================================");
                    sw.Write("添加日期为:" + DateTime.Now.ToString() + "\r\n");
                    sw.Write("日志内容为:" + msg + "\r\n");
                    sw.WriteLine("=====================================");
                    sw.Flush();
                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

    }

}