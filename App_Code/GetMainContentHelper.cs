using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace WawaSoft.Search
{
    public class GetMainContentHelper
    {
        /// <summary>
        ///compare string length
        ///for soft
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static int CompareDinosByChineseLength(string x, string y)
        {
            if (x == null)
            {
                if (y == null)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                if (y == null)
                {
                    return 1;
                }
                else
                {
                    Regex r = new Regex("[\u0600-\u06ff]");
                    float xCount = (float)(r.Matches(x).Count) / (float)x.Length;
                    float yCount = (float)(r.Matches(y).Count) / (float)y.Length;

                    int retval = xCount.CompareTo(yCount);

                    if (retval != 0)
                    {
                        return retval;
                    }
                    else
                    {
                        return x.CompareTo(y);
                    }
                }
            }
        }

        /// <summary>
        /// get tag from html
        /// </summary>
        /// <param name="input">html string</param>
        /// <param name="tag">tag name</param>
        /// <returns></returns>
        public static List<string> GetTags(string input, string tag)
        {
            StringReader strReader = new StringReader(input);
            int lowerThanCharCounter = 0;
            int lowerThanCharPos = 0;
            Stack<int> tagPos = new Stack<int>();
            List<string> taglist = new List<string>();
            int i = 0;
            while (true)
            {
                try
                {
                    int intCharacter = strReader.Read();
                    if (intCharacter == -1) break;

                    char convertedCharacter = Convert.ToChar(intCharacter);

                    if (lowerThanCharCounter > 0)
                    {
                        if (convertedCharacter == '>')
                        {
                            lowerThanCharCounter--;

                            string biaoqian = input.Substring(lowerThanCharPos, i - lowerThanCharPos + 1);
                            if (biaoqian.StartsWith(string.Format("<{0}", tag)))
                            {
                                tagPos.Push(lowerThanCharPos);
                            }
                            if (biaoqian.StartsWith(string.Format("</{0}", tag)))
                            {
                                if (tagPos.Count < 1)
                                    continue;
                                int tempTagPos = tagPos.Pop();
                                string strdiv = input.Substring(tempTagPos, i - tempTagPos + 1);
                                taglist.Add(strdiv);
                            }
                        }
                    }

                    if (convertedCharacter == '<')
                    {
                        lowerThanCharCounter++;
                        lowerThanCharPos = i;
                    }
                }
                finally
                {
                    i++;
                }
            }
            return taglist;
        }

        /// <summary>
        /// get string data from url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string getDataFromUrl(string url)
        {
            string str = string.Empty;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);

            //����httpͷ
            request.AllowAutoRedirect = true;
            request.AllowWriteStreamBuffering = true;
            request.Referer = "";
            request.Timeout = 10 * 1000;
            request.UserAgent = "";

            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    //����httpӦ���httpͷ���жϱ���
                    string characterSet = response.CharacterSet;
                    Encoding encode;
                    if (characterSet != "")
                    {
                        if (characterSet == "ISO-8859-1")
                        {
                            characterSet = "gb2312";
                        }
                        encode = Encoding.GetEncoding(characterSet);
                    }
                    else
                    {
                        encode = Encoding.UTF8;
                    }

                    //����һ���ڴ���������httpӦ����
                    Stream receiveStream = response.GetResponseStream();
                    MemoryStream mStream = new MemoryStream();

                    byte[] bf = new byte[255];
                    int count = receiveStream.Read(bf, 0, 255);
                    while (count > 0)
                    {
                        mStream.Write(bf, 0, count);
                        count = receiveStream.Read(bf, 0, 255);
                    }
                    receiveStream.Close();

                    mStream.Seek(0, SeekOrigin.Begin);

                    //���ڴ������ȡ�ַ���
                    StreamReader reader = new StreamReader(mStream, encode);
                    char[] buffer = new char[1024];
                    count = reader.Read(buffer, 0, 1024);
                    while (count > 0)
                    {
                        str += new String(buffer, 0, count);
                        count = reader.Read(buffer, 0, 1024);
                    }

                    //�ӽ��������ַ������ж�charset�������httpӦ��ı��벻һֱ
                    //��ô��ҳ��������Ϊ׼���ٴδ��ڴ��������¶�ȡ�ı�
                    Regex reg =
                        new Regex(@"<meta[\s\S]+?charset=(.*)""[\s\S]+?>",
                                  RegexOptions.Multiline | RegexOptions.IgnoreCase);
                    MatchCollection mc = reg.Matches(str);
                    if (mc.Count > 0)
                    {
                        string tempCharSet = mc[0].Result("$1");
                        if (string.Compare(tempCharSet, characterSet, true) != 0)
                        {
                            encode = Encoding.GetEncoding(tempCharSet);
                            str = string.Empty;
                            mStream.Seek(0, SeekOrigin.Begin);
                            reader = new StreamReader(mStream, encode);
                            buffer = new char[255];
                            count = reader.Read(buffer, 0, 255);
                            while (count > 0)
                            {
                                str += new String(buffer, 0, count);
                                count = reader.Read(buffer, 0, 255);
                            }
                        }
                    }
                    reader.Close();
                    mStream.Close();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
            }
            finally
            {
                if (response != null)
                    response.Close();
            }
            return str;
        }

        /// <summary>
        /// get content
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetMainContent(string input)
        {
            string reg1 = @"<(p|br)[^<]*>";
            string reg2 =
                @"(\[([^=]*)(=[^\]]*)?\][\s\S]*?\[/\1\])|(?<lj>(?=[^\u0600-\u06FF\uFB50-\uFDFF,."");])<a\s+[^>]*>[^<]{2,}</a>(?=[^\u4E00-\u9FA5\uFE30-\uFFA0,."");]))|(?<Style><style[\s\S]+?/style>)|(?<select><select[\s\S]+?/select>)|(?<Script><script[\s\S]*?/script>)|(?<Explein><\!\-\-[\s\S]*?\-\->)|(?<li><li(\s+[^>]+)?>[\s\S]*?/li>)|(?<Html></?\s*[^> ]+(\s*[^=>]+?=['""]?[^""']+?['""]?)*?[^\[<]*>)|(?<Other>&[a-zA-Z]+;)|(?<Other2>\#[a-z0-9]{6})|(?<Space>\s+)|(\&\#\d+\;)";
            string reg3 = @"<[^>]*>";
            string newline = @"[\r\n]+";
            //1. Div lists
            List<string> list = GetTags(input, "div");

            //2.need to remove texts
            List<string> needToRemove = new List<string>();
            foreach (string s in list)
            {
                Regex r = new Regex("[\u0600-\u06FF]");
                if (r.Matches(s).Count < 300)
                {
                    needToRemove.Add(s);
                }
            }
            foreach (string s in needToRemove)
            {
                list.Remove(s);
            }

            //3.sort by length
            list.Sort(CompareDinosByChineseLength);
            if (list.Count < 1)
            {
                return "";
            }
            input = list[list.Count - 1];//longst text

            //4. new line to [p][br] 
            input = new Regex(reg1, RegexOptions.Multiline | RegexOptions.IgnoreCase).Replace(input, "[$1]");

            //5. no uyghur, no english chars convert to space
            input = new Regex(reg2, RegexOptions.Multiline | RegexOptions.IgnoreCase).Replace(input, " ");

            //remove doblicated lines
            //input = new Regex(newline).Replace(input,"\r\n");

            //6.[p][br] to new line
            input = new Regex("\\[p]", RegexOptions.Multiline | RegexOptions.IgnoreCase).Replace(input, "\r\n");
            input = new Regex("\\[br]", RegexOptions.Multiline | RegexOptions.IgnoreCase).Replace(input, "\r\n");
            return input;
        }


    }
}
