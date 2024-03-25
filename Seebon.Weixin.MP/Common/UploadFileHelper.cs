using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.IO;

namespace Seebon.Weixin.MP.Common
{
   public class UploadFileHelper
    {
        private string[] AllowFileType; //所允许的文件类型 
        private double FileLength; //所允许的文件大小(KB) 
        private string SavePath; //文件的存储路径
        private string _ShowPath; //文件的相对路径
        private string SaveFile; //上传后的文件名 
        private string Error; //存储错误信息 
        private string FileExtesion; //上传文件的扩展名 

        /// <summary> 
        /// 构造函数 
        /// </summary> 
        /// <param name="allFileType">允许的文件类型，多个以","分开</param> 
        /// <param name="fileLength">文件大小(M 为单位)</param> 
        /// <param name="savePath">保存路径</param> 
        public UploadFileHelper(string allFileType, double fileLength, string savePath)
        {
            char[] sp = { ',' };
            AllowFileType = allFileType.Split(sp);
            FileLength = fileLength * 1024 * 1024;
            SavePath = HttpContext.Current.Server.MapPath(savePath);
            _ShowPath = savePath;
        }

        /// <summary> 
        /// 返回生成的文件名 
        /// </summary> 
        public string FileName
        {
            get
            {
                return SaveFile;
            }
        }

        /// <summary> 
        /// 返回生成的文件相对路径 
        /// </summary> 
        public string ShowPath
        {
            get
            {
                return _ShowPath;
            }
        }

        /// <summary> 
        /// 返回出错信息 
        /// </summary> 
        public string ErrorMessage
        {
            get
            {
                return Error;
            }
        }

        /// <summary> 
        /// 根据SavePath,生成文件名 
        /// </summary> 
        /// <returns></returns> 
        private string MakeFileName(string fileType, string fileName)
        {
            
            string file = this.SavePath + DateTime.Now.ToString("yyyyMMddHHmmss");

            Random orandom = new Random();
            string oStringRandom = orandom.Next(9999).ToString();//生成4位随机数字

            file = file + oStringRandom + this.FileExtesion;
            
            return file;
        }

        /// <summary> 
        /// 检查文件类型 
        /// </summary> 
        /// <param name="fileEx">MIME内容</param> 
        /// <returns></returns> 
        private bool CheckFileExt(string fileEx)
        {
            bool result = false;
            for (int i = 0; i < this.AllowFileType.Length; i++)
            {
                if (fileEx.IndexOf(this.AllowFileType[i].ToLower()) > -1)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        public bool UpLoad(HttpPostedFileBase file)
        {
            bool result = true;
            try
            {
                //查看文件长度 
                if (file.ContentLength > (this.FileLength))
                {
                    this.Error = "文件大小超出范允许的范围！";
                    return false;
                }

                string fileName = Path.GetFileName(file.FileName);
                this.FileExtesion = Path.GetExtension(fileName);

                if (!CheckFileExt(this.FileExtesion.ToLower()))
                {
                    this.Error = "文件类型" + this.FileExtesion + "不允许！";
                    return false;
                }

                //取得要保存的文件名 
                string UpFile = this.MakeFileName(this.FileExtesion, fileName);

                //保存文件 
                file.SaveAs(UpFile);

                //返回文件名 
                this.SaveFile = Path.GetFileName(UpFile);

                //返回文件相对路径
                this._ShowPath += this.SaveFile;

            }
            catch (Exception e)
            {
                result = false;
                this.Error = e.Message;
            }
            return result;
        }

    }
}
