using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace wintogo
{

    public class SWOnline
    {

        public SWOnline()
        {


        }
        public SWOnline(string releaseUrl, string reportUrl)
        {
            this.ReleaseUrl = releaseUrl;
            this.ReportUrl = reportUrl;
        }
        private string releaseUrl;

        public string ReleaseUrl
        {
            get { return releaseUrl; }
            set { releaseUrl = value; }
        }

        private string reportUrl;

        public string ReportUrl
        {
            get { return reportUrl; }
            set { reportUrl = value; }
        }

        private string[] topicLink;

        public string[] TopicLink
        {
            get { return topicLink; }
            set { topicLink = value; }
        }
        private string[] topicName;

        public string[] TopicName
        {
            get { return topicName; }
            set { topicName = value; }
        }

        private LinkLabel linkLabel;

        public LinkLabel Linklabel
        {
            get { return linkLabel; }
            set { linkLabel = value; }
        }

        private Action<string> setLinkLabel;
        public Action<string> SetLinkLabel
        {
            get { return setLinkLabel; }
            set { setLinkLabel = value; }
        }
        //private set_TextDelegate  set_Text;

        //public set_TextDelegate  Set_text
        //{
        //    get { return set_Text; }
        //    set { set_Text = value; }
        //}


        //public int MyProperty { get; set; }




        public void Report()
        {
            string pageHtml;
            try
            {

                WebClient MyWebClient = new WebClient();

                MyWebClient.Credentials = CredentialCache.DefaultCredentials;//获取或设置用于对向Internet资源的请求进行身份验证的网络凭据。

                Byte[] pageData = MyWebClient.DownloadData(releaseUrl); //从指定网站下载数据

                pageHtml = Encoding.Default.GetString(pageData);
                //MessageBox.Show(pageHtml);
                int index = pageHtml.IndexOf("webreport=");

                if (pageHtml.Substring(index + 10, 1) == "1")
                {
                    //string strURL = "http://myapp.luobotou.org/statistics.aspx?name=wtg&ver=" + Application.ProductVersion;

                    string strURL = reportUrl + Application.ProductVersion;
                    System.Net.HttpWebRequest request;
                    // 创建一个HTTP请求
                    request = (HttpWebRequest)WebRequest.Create(strURL);
                    //request.Method="get";
                    HttpWebResponse response;
                    response = (HttpWebResponse)request.GetResponse();

                    using (StreamReader myreader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    { string responseText = myreader.ReadToEnd(); }
                    //myreader.Close();

                }


            }
            catch (WebException webEx)
            {

                Log.WriteLog("UpdateLog.log", webEx.ToString());

            }
        }
        public void Update()
        {
            string autoup = IniFile.ReadVal("Main", "AutoUpdate", Application.StartupPath + "\\files\\settings.ini");
            if (autoup == "0") { return; }
            //if (IsRegeditExit(Application.ProductName)) { if ((GetRegistData("nevercheckupdate")) == "1") { return; } }

            string pageHtml;
            try
            {

                WebClient MyWebClient = new WebClient();
                //MyWebClient.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");

                MyWebClient.Credentials = CredentialCache.DefaultCredentials;//获取或设置用于对向Internet资源的请求进行身份验证的网络凭据。

                Byte[] pageData = MyWebClient.DownloadData(releaseUrl); //从指定网站下载数据"http://bbs.luobotou.org/app/wintogo.txt"

                pageHtml = Encoding.UTF8.GetString(pageData);
                //essageBox.Show(pageHtml );
                int index = pageHtml.IndexOf("~");
                //String ver;
                Version newVer = new Version(pageHtml.Substring(index + 1, 7));
                Version currentVer = new Version(Application.ProductVersion);
                //ver = pageHtml.Substring(index + 1, 7);
                if (newVer > currentVer)
                {
                    Update frmf = new Update(newVer.ToString());
                    frmf.ShowDialog();
                }

            }
            catch (WebException webEx)
            {
                //Console.WriteLine(webEx.Message.ToString());
                Log.WriteLog("UpdateLog.log", webEx.ToString());
            }
        }



        public void Showad()
        {
            string pageHtml1;
            try
            {
                WebClient MyWebClient = new WebClient();

                MyWebClient.Credentials = CredentialCache.DefaultCredentials;//获取或设置用于对向Internet资源的请求进行身份验证的网络凭据。

                Byte[] pageData = MyWebClient.DownloadData("http://bbs.luobotou.org/app/wintogo.txt"); //从指定网站下载数据

                pageHtml1 = Encoding.UTF8.GetString(pageData);
                int index = pageHtml1.IndexOf("announcement=");
                int indexbbs = pageHtml1.IndexOf("bbs=");
                if (pageHtml1.Substring(index + 13, 1) != "0" && MsgManager.ci.EnglishName != "English")
                {
                    if (pageHtml1.Substring(indexbbs + 4, 1) == "1")
                    {
                        string pageHtml;


                        WebClient MyWebClient1 = new WebClient();

                        MyWebClient1.Credentials = CredentialCache.DefaultCredentials;//获取或设置用于对向Internet资源的请求进行身份验证的网络凭据。

                        Byte[] pageData1 = MyWebClient1.DownloadData("http://bbs.luobotou.org/portal.php"); //从指定网站下载数据

                        pageHtml = Encoding.UTF8.GetString(pageData1);
                        //MessageBox.Show(pageHtml);
                        int index1 = pageHtml.IndexOf("<ul><li><a href=");
                        for (int i = 0; i < 10; i++)
                        {
                            int LinkStartIndex = pageHtml.IndexOf("<li><a href=", index1) + 13;
                            int LinkEndIndex = pageHtml.IndexOf("\"", LinkStartIndex);
                            int TitleStartIndex = pageHtml.IndexOf("title=", LinkEndIndex) + 7;
                            int TitleEndIndex = pageHtml.IndexOf("\"", TitleStartIndex);

                            topicLink[i] = pageHtml.Substring(LinkStartIndex, LinkEndIndex - LinkStartIndex);
                            topicName[i] = pageHtml.Substring(TitleStartIndex, TitleEndIndex - TitleStartIndex);
                            index1 = LinkEndIndex;
                            //topicstring 
                            //int adprogram = index1 + Application.ProductName.Length + 1;

                        }
                        #region OldCode
                        //string portal_block = pageHtml.Substring;
                        //String adtitle;
                        ////MessageBox.Show(adprogram.ToString() + " " + startindex);
                        //adtitle = pageHtml.Substring(adprogram, startindex - adprogram);

                        //adlink = pageHtml.Substring(startindex + 1, endindex - startindex - 1);
                        //linkLabel2.Invoke(Set_Text, new object[] { adtitle });
                        //MessageBox.Show("");

                        //MessageBox.Show(adtitle + "     " + adlink);
                        #endregion
                    }

                    {

                        //MessageBox.Show("Test");
                        string pageHtml;
                        WebClient MyWebClient1 = new WebClient();

                        MyWebClient1.Credentials = CredentialCache.DefaultCredentials;//获取或设置用于对向Internet资源的请求进行身份验证的网络凭据。
                        MyWebClient1.Encoding = Encoding.UTF8;
                        pageHtml = MyWebClient1.DownloadString("http://bbs.luobotou.org/app/announcement.txt");

                        //Byte[] pageData1 = MyWebClient1.DownloadData("http://bbs.luobotou.org/app/announcement.txt"); //从指定网站下载数据
                        //pageHtml = Encoding.UTF8.GetString(pageData1);
                        //MessageBox.Show(pageHtml);
                        //int index1 = pageHtml.IndexOf(Application.ProductName);
                        //int startindex = pageHtml.IndexOf("~", index1);
                        //int endindex = pageHtml.IndexOf("结束", index1);
                        //int adprogram = index1 + Application.ProductName.Length + 1;
                        Match match = Regex.Match(pageHtml, Application.ProductName + "=(.+)~(.+)结束");
                        //Set_Text(match.Groups[1].Value);
                        string adlink;
                        adlink = match.Groups[2].Value;
                        String adtitle;
                        adtitle = match.Groups[1].Value;
                        ////MessageBox.Show(adprogram.ToString() + " " + startindex);
                        //adtitle = pageHtml.Substring(adprogram, startindex - adprogram);
                        //adtitles = adtitle;
                        //adlink = pageHtml.Substring(startindex + 1, endindex - startindex - 1);
                        //Set_Text(adtitle);
                        //Set_Text = new set_TextDelegate(set_textboxText); //实例化
                        //MessageBox.Show("Test");
                        //Form1.SetText(adtitle);

                        linkLabel.Invoke(setLinkLabel, new object[] { adtitle });
                        linkLabel.Tag = adlink;
                        //linkLabel2(Set_Text);
                        //MessageBox.Show("");
                        //writeprogress .linklabel1
                        //MessageBox.Show(adtitle + "     " + adlink);

                    }
                }
            }
            catch (Exception ex) 
            {
                Log.WriteLog("UpdateLog.log", ex.ToString());

                //Console.WriteLine(ex.Message);
            }


        }



    }
}
