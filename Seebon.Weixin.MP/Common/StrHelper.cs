using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Seebon.Weixin.MP.Common
{
   public class StrHelper
    {
        /// <summary>
        /// 转换成 HTML code
        /// </summary>
        /// <param name="str">string</param>
        /// <returns>string</returns>
        public static string Encode(string str)
        {
            str = str.Replace("&", "&amp;");
            str = str.Replace("'", "''");
            str = str.Replace("\"", "&quot;");
            str = str.Replace(" ", "&nbsp;");
            str = str.Replace("<", "&lt;");
            str = str.Replace(">", "&gt;");
            str = str.Replace("\n", "<br>");
            return str;
        }
        /// <summary>
        ///解析html成 普通文本
        /// </summary>
        /// <param name="str">string</param>
        /// <returns>string</returns>
        public static string Decode(string str)
        {
            str = str.Replace("<br>", "\n");
            str = str.Replace("&gt;", ">");
            str = str.Replace("&lt;", "<");
            str = str.Replace("&nbsp;", " ");
            str = str.Replace("&quot;", "\"");
            str = str.Replace("''", "'");
            str = str.Replace("&amp;", "&");
            return str;
        }
        public static string GetRandomString(string allChar, int CodeCount)
        {
            string[] allCharArray = allChar.Split(',');
            string RandomCode = "";
            int temp = -1;
            Random rand = new Random();
            for (int i = 0; i < CodeCount; i++)
            {
                if (temp != -1)
                {
                    rand = new Random(temp * i * ((int)DateTime.Now.Ticks));
                }

                int t = rand.Next(allCharArray.Length - 1);

                while (temp == t)
                {
                    t = rand.Next(allCharArray.Length - 1);
                }

                temp = t;
                RandomCode += allCharArray[t];
            }
            return RandomCode;
        }
        public static List<string> GetStrArray(string str, char speater, bool toLower)
        {
            List<string> list = new List<string>();
            string[] ss = str.Split(speater);
            foreach (string s in ss)
            {
                if (!string.IsNullOrEmpty(s) && s != speater.ToString())
                {
                    string strVal = s;
                    if (toLower)
                    {
                        strVal = s.ToLower();
                    }
                    list.Add(strVal);
                }
            }
            return list;
        }
        public static string[] GetStrArray(string str)
        {
            return str.Split(new char[',']);
        }
        public static string GetArrayStr(List<string> list, string speater)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < list.Count; i++)
            {
                if (i == list.Count - 1)
                {
                    sb.Append(list[i]);
                }
                else
                {
                    sb.Append(list[i]);
                    sb.Append(speater);
                }
            }
            return sb.ToString();
        }


        /// <summary>
        /// 删除最后结尾的指定字符后的字符
        /// </summary>
        public static string DelLastChar(string str, string strchar)
        {
            return str.Substring(0, str.LastIndexOf(strchar));
        }

        /// <summary>
        /// 转全角的函数(SBC case)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToSBC(string input)
        {
            //半角转全角：
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 32)
                {
                    c[i] = (char)12288;
                    continue;
                }
                if (c[i] < 127)
                    c[i] = (char)(c[i] + 65248);
            }
            return new string(c);
        }

        /// <summary>
        ///  转半角的函数(SBC case)
        /// </summary>
        /// <param name="input">输入</param>
        /// <returns></returns>
        public static string ToDBC(string input)
        {
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 12288)
                {
                    c[i] = (char)32;
                    continue;
                }
                if (c[i] > 65280 && c[i] < 65375)
                    c[i] = (char)(c[i] - 65248);
            }
            return new string(c);
        }


        public static string GetCleanStyle(string StrList, string SplitString)
        {
            string RetrunValue = "";
            //如果为空，返回空值
            if (StrList == null)
            {
                RetrunValue = "";
            }
            else
            {
                //返回去掉分隔符
                string NewString = "";
                NewString = StrList.Replace(SplitString, "");
                RetrunValue = NewString;
            }
            return RetrunValue;
        }

        public static string GetNewStyle(string StrList, string NewStyle, string SplitString, out string Error)
        {
            string ReturnValue = "";
            //如果输入空值，返回空，并给出错误提示
            if (StrList == null)
            {
                ReturnValue = "";
                Error = "请输入需要划分格式的字符串";
            }
            else
            {
                //检查传入的字符串长度和样式是否匹配,如果不匹配，则说明使用错误。给出错误信息并返回空值
                int strListLength = StrList.Length;
                int NewStyleLength = GetCleanStyle(NewStyle, SplitString).Length;
                if (strListLength != NewStyleLength)
                {
                    ReturnValue = "";
                    Error = "样式格式的长度与输入的字符长度不符，请重新输入";
                }
                else
                {
                    //检查新样式中分隔符的位置
                    string Lengstr = "";
                    for (int i = 0; i < NewStyle.Length; i++)
                    {
                        if (NewStyle.Substring(i, 1) == SplitString)
                        {
                            Lengstr = Lengstr + "," + i;
                        }
                    }
                    if (Lengstr != "")
                    {
                        Lengstr = Lengstr.Substring(1);
                    }
                    //将分隔符放在新样式中的位置
                    string[] str = Lengstr.Split(',');
                    foreach (string bb in str)
                    {
                        StrList = StrList.Insert(int.Parse(bb), SplitString);
                    }
                    //给出最后的结果
                    ReturnValue = StrList;
                    //因为是正常的输出，没有错误
                    Error = "";
                }
            }
            return ReturnValue;
        }

        /// <summary>
        /// 字符串长度
        /// </summary>
        public static int Len(string str)
        {
            System.Text.ASCIIEncoding n = new System.Text.ASCIIEncoding();
            byte[] b = n.GetBytes(str);
            int length = 0;                          // l 为字符串的实际长度
            for (int i = 0; i <= b.Length - 1; i++)
            {
                if (b[i] == 63)             //判断是否为汉字或全脚符号
                {
                    length++;
                }
                length++;
            }
            return length;
        }
        public static string getFirstPic(string pic)
        {
            string returnValue = "default.jpg";
            if (pic.Equals(string.Empty))
                return returnValue;
            if (pic.IndexOf(",") == -1) return pic;
            string[] array = pic.Split(',');
            return array[0];
        }

        /// <summary>
        /// wap网站去除HTML标记
        /// </summary>
        /// <param name="fHtmlString">包括HTML的源码</param>
        /// <returns>已经去除后的文字</returns>
        public static string WapNoHTML(string fHtmlString)
        {
            fHtmlString = Regex.Replace(fHtmlString, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);

            // 去除 <P></P> 及 </br>，<br /> 的以外的 HTML
            fHtmlString = Regex.Replace(fHtmlString, @"<(?!p|\/p|/br|br \/)[^>]+>", "", RegexOptions.IgnoreCase);
            fHtmlString = Regex.Replace(fHtmlString, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            fHtmlString = Regex.Replace(fHtmlString, @"-->", "", RegexOptions.IgnoreCase);
            fHtmlString = Regex.Replace(fHtmlString, @"<!--.*", "", RegexOptions.IgnoreCase);
            //fHtmlString = Regex.Replace(fHtmlString, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            //fHtmlString = Regex.Replace(fHtmlString, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            //fHtmlString = Regex.Replace(fHtmlString, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            //fHtmlString = Regex.Replace(fHtmlString, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            fHtmlString = Regex.Replace(fHtmlString, @"&nbsp;&nbsp;", "　", RegexOptions.IgnoreCase);
            fHtmlString = Regex.Replace(fHtmlString, @"&(nbsp|#160);", " ", RegexOptions.IgnoreCase);
            fHtmlString = Regex.Replace(fHtmlString, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            fHtmlString = Regex.Replace(fHtmlString, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            fHtmlString = Regex.Replace(fHtmlString, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            //fHtmlString = Regex.Replace(fHtmlString, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
            //fHtmlString = Regex.Replace(fHtmlString, @"&#(\d+);", "", RegexOptions.IgnoreCase);

            fHtmlString.Replace("<", "");
            fHtmlString.Replace(">", "");
            fHtmlString.Replace("\r\n", "");

            return fHtmlString;
        }


        /// <summary>
        /// 去除HTML标记
        /// </summary>
        /// <param name="NoHTML">包括HTML的源码 </param>
        /// <returns>已经去除后的文字</returns>
        public static string NoHTML(string Htmlstring)
        {

            //删除脚本
            Htmlstring = Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);

            //删除HTML
            Htmlstring = Regex.Replace(Htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"-->", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(ldquo|#34);", "\"", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(rdquo|#34);", "\"", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(nbsp|#160);", " ", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);
            //Htmlstring = Regex.Replace(Htmlstring, @"&ldquo;", "&quot;", RegexOptions.IgnoreCase);
            //Htmlstring = Regex.Replace(Htmlstring, @"&rdquo;", "&quot;", RegexOptions.IgnoreCase); 
            Htmlstring.Replace("\"", "");
            Htmlstring.Replace("<p>", "");
            Htmlstring.Replace("<", "");
            Htmlstring.Replace(">", "");
            Htmlstring.Replace("\r\n", "");
            Htmlstring = HttpContext.Current.Server.HtmlEncode(Htmlstring).Trim();
            Htmlstring = Regex.Replace(Htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(ldquo|#34);", "\"", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(rdquo|#34);", "\"", RegexOptions.IgnoreCase);
            return Htmlstring;

        }

        /// <summary>
        /// 按字符串实际长度截取定长字符串
        /// </summary>
        /// <param name="str">原字符串</param>
        /// <param name="length">要截取的长度</param>
        /// <param name="chars">要添加的字符</param>
        /// <returns>string型字符串</returns>
        public static string GetString(string str, int length, string chars)
        {
            if (str == null || str == "")
            {
                return str;
            }
            int i = 0, j = 0;
            //去掉所有HTML标记	        
            str = NoHTML(str);
            foreach (char chr in str)
            {
                if ((int)chr > 127)
                {
                    i += 2;
                }
                else
                {
                    i++;
                }
                if (i > length)
                {
                    str = str.Substring(0, j) + chars;
                    break;
                }
                j++;
            }

            return str;

        }

    }
}

