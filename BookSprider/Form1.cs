using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace BookSprider
{
    public partial class Form1 : Form
    {
        public class ChapterNode
        {
            public void Repair()
            {
                _name1 = Regex.Replace(_name1, @"\s", "");
                _page1 = Regex.Replace(_page1, @"\s", "");
                _name2 = Regex.Replace(_name2, @"\s", "");
                _page2 = Regex.Replace(_page2, @"\s", "");
            }

            public void Repair_zaidu_la()
            {
                int _s = _page1.IndexOf("】，");
                int _e = _page1.IndexOf("加入书签");
                char[] _tmpBuf = new char[_e - (_s + 12)];
                _page1.CopyTo(_s + 12, _tmpBuf, 0, _e - (_s + 12));
                _page1 = new string(_tmpBuf);
                //_page1 = _page1.Replace(", " ");
            }

            public void Repair_NBSP_MDASH()
            {
                _page1 = _page1.Replace("&nbsp;", " ");
                _page2 = _page2.Replace("&nbsp;", " ");

                _page1 = _page1.Replace("&mdash;", "-");
                _page2 = _page2.Replace("&mdash;", "-");
            }
            
            public string _name1 = "";
            public string _page1 = "";
            public string _name2 = "";
            public string _page2 = "";
        }

        public Form1()
        {
            InitializeComponent();
        }

        List<string> PageLinkList = new List<string>();

        List<ChapterNode> ChapterList = new List<ChapterNode>();

        void Page1_www_ldks_cc(int _index)
        {
            string _link = string.Format("https://www.ldks.cc{0}", PageLinkList[_index]);
            WebClient MyWebClient = new WebClient();
            MyWebClient.Credentials = CredentialCache.DefaultCredentials;//获取或设置用于向Internet资源的请求进行身份验证的网络凭据
            Byte[] pageData = MyWebClient.DownloadData(_link); //从指定网站下载数据
            string pageHtml = Encoding.UTF8.GetString(pageData);

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(pageHtml);

            ChapterNode _node = new ChapterNode();

            HtmlAgilityPack.HtmlNode _n1 = doc.DocumentNode.SelectSingleNode("//div[@class='content']//h1");

            _node._name1 = _n1.InnerText;

            HtmlAgilityPack.HtmlNode _n2 = doc.DocumentNode.SelectSingleNode("//div[@class='content']//div[@class='showtxt']");

            _node._page1 = _n2.InnerText;

            Page2_www_ldks_cc(ref _node,_index);
        }

        void Page2_www_ldks_cc(ref ChapterNode _node,int _index)
        {
            string _link = string.Format("https://www.ldks.cc{0}", PageLinkList[_index]);
            _link = _link.Replace(".html", "_2.html");
            WebClient MyWebClient = new WebClient();
            MyWebClient.Credentials = CredentialCache.DefaultCredentials;//获取或设置用于向Internet资源的请求进行身份验证的网络凭据
            Byte[] pageData = MyWebClient.DownloadData(_link); //从指定网站下载数据
            string pageHtml = Encoding.UTF8.GetString(pageData);

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(pageHtml);
            HtmlAgilityPack.HtmlNode _n1 = doc.DocumentNode.SelectSingleNode("//div[@class='content']//h1");

            _node._name2 = _n1.InnerText;

            HtmlAgilityPack.HtmlNode _n2 = doc.DocumentNode.SelectSingleNode("//div[@class='content']//div[@class='showtxt']");

            _node._page2 = _n2.InnerText;

            _node.Repair();
            ChapterList.Add(_node);
            ShowChapter_www_ldks_cc(ChapterList.Count - 1);
        }

        void ShowChapter_www_ldks_cc(int _index)
        {
            textBox1.Text = "";
            textBox1.AppendText(ChapterList[_index]._name1 + "\r\n");
            textBox1.AppendText(ChapterList[_index]._page1 + "\r\n");
            textBox1.AppendText(ChapterList[_index]._name2 + "\r\n");
            textBox1.AppendText(ChapterList[_index]._page2 + "\r\n");
        }

        void GetList_www_ldks_cc(string _html)
        {
            ChapterList.Clear();
            PageLinkList.Clear();
            listBox1.Items.Clear();

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(_html);

            HtmlAgilityPack.HtmlNode _n = doc.DocumentNode.SelectSingleNode("//div[@class='listmain']");
            //textBox1.Text = _n.InnerHtml;

            HtmlAgilityPack.HtmlDocument doc2 = new HtmlAgilityPack.HtmlDocument();
            doc2.LoadHtml(_n.InnerHtml);

            HtmlAgilityPack.HtmlNodeCollection _n2 = doc.DocumentNode.SelectNodes("//a");

            foreach (HtmlAgilityPack.HtmlNode _node in _n2)
            {
                string _ht = _node.OuterHtml;

                if (_ht.IndexOf("www.ldks.cc") > 0)
                {
                    continue;
                }

                if (_ht.IndexOf("第") > 0 && _ht.IndexOf("章") > 0)
                {
                    int _s1 = _ht.IndexOf("href=");
                    int _e1 = _ht.IndexOf(">第");

                    char[] _link = new char[_e1 - _s1 - 7];
                    _ht.CopyTo(_s1 + 6, _link, 0, _e1 - _s1 - 7);

                    string _link_str = new string(_link);

                    PageLinkList.Add(_link_str);
                    listBox1.Items.Add(_link_str);
                    //textBox1.AppendText(_link_str + "\r\n");
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            WebClient MyWebClient = new WebClient();
            MyWebClient.Credentials = CredentialCache.DefaultCredentials;//获取或设置用于向Internet资源的请求进行身份验证的网络凭据
            Byte[] pageData = MyWebClient.DownloadData("https://www.ldks.cc/39_39168/"); //从指定网站下载数据
            string pageHtml = Encoding.UTF8.GetString(pageData);  //如果获取网站页面采用的是GB2312，则使用这句    
                                                                  //string pageHtml = Encoding.UTF8.GetString(pageData); //如果获取网站页面采用的是UTF-8，则使用这句
                                                                  //Console.WriteLine(pageHtml);//在控制台输入获取的内容

            GetList_www_ldks_cc(pageHtml);
            //textBox1.Text = pageHtml;




            // Regex _regex = new Regex("<div /class=\"listmain\">(.*?<div>)");
            //Match mc = _regex.Match(pageHtml);
            //textBox1.Text = mc.Value;

            //             using (StreamWriter sw = new StreamWriter("c:\\test\\ouput.html"))//将获取的内容写入文本
            //             {
            //                 sw.Write(pageHtml);
            //             }
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            if(listBox1.SelectedIndex >= 0)
            {
                //Page1_www_ldks_cc(listBox1.SelectedIndex);
            }
        }

        void Save_www_ldks_cc()
        {
            string filePath = Directory.GetCurrentDirectory() + "\\" + Process.GetCurrentProcess().ProcessName + "www_ldks_cc.txt";
            if (File.Exists(filePath))
                File.Delete(filePath);

            FileStream fs = new FileStream(filePath, FileMode.Create);
            StreamWriter _writer = new StreamWriter(fs);

            for (int i = 0; i < PageLinkList.Count; i++)
            {
                Page1_www_ldks_cc(i);

                if ( i < ChapterList.Count)
                {
                    _writer.WriteLine(ChapterList[i]._name1);
                    _writer.WriteLine(ChapterList[i]._page1);
                    _writer.WriteLine(ChapterList[i]._name2);
                    _writer.WriteLine(ChapterList[i]._page2);
                    _writer.Flush();
                }
            }

            _writer.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Save_www_ldks_cc();
        }

        void sss()
        {
            Document document;
            PdfAWriter writer;
            try
            {
                string filename = Directory.GetCurrentDirectory() + "\\" + "TestCreatePdfA_1.pdf";
                FileStream fos = new FileStream(filename, FileMode.Create);

                document = new Document();

                writer = PdfAWriter.GetInstance(document, fos, PdfAConformanceLevel.PDF_A_1B);
                //writer.CreateXmpMetadata();

                document.Open();

                iTextSharp.text.Font font = FontFactory.GetFont(Directory.GetCurrentDirectory() + "\\" + "FreeMonoBold.ttf", BaseFont.WINANSI, BaseFont.EMBEDDED, 12);
                document.Add(new Paragraph("Hello World", font));

                FileStream iccProfileFileStream = File.Open(Directory.GetCurrentDirectory() + "\\" + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read, FileShare.Read);
                ICC_Profile icc = ICC_Profile.GetInstance(iccProfileFileStream);
                iccProfileFileStream.Close();

                writer.SetOutputIntents("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", icc);
                document.Close();
            }
            catch (PdfAConformanceException e)
            {
                //Assert.Fail("PdfAConformance exception should not be thrown: " + e.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            sss();
            return;
            iTextSharp.text.Document _pdf = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4, 50, 20, 20, 20);


            string filePath = Directory.GetCurrentDirectory() + "\\" + Process.GetCurrentProcess().ProcessName + ".pdf";
            if (File.Exists(filePath))
                File.Delete(filePath);

            FileStream fs = new FileStream(filePath, FileMode.Create);

            iTextSharp.text.pdf.PdfAWriter _pdfWriter = iTextSharp.text.pdf.PdfAWriter.GetInstance(_pdf, fs, iTextSharp.text.pdf.PdfAConformanceLevel.PDF_A_1B);

            _pdfWriter.Open();

            //iTextSharp.text.pdf.BaseFont _bf = iTextSharp.text.pdf.BaseFont.CreateFont("c:\\windows\\fonts\\SIMYOU.TTF", "Identity-H", false);
            //iTextSharp.text.Font _font = new iTextSharp.text.Font(_bf);
            iTextSharp.text.Font _font = iTextSharp.text.FontFactory.GetFont("FreeMonoBold.ttf", iTextSharp.text.pdf.BaseFont.WINANSI, iTextSharp.text.pdf.BaseFont.EMBEDDED, 12);


            iTextSharp.text.Paragraph paragraph;

            string _tf = Directory.GetCurrentDirectory() + "\\1.txt";
            string[] lines = System.IO.File.ReadAllLines(_tf, Encoding.GetEncoding("utf-8"));

            foreach (string line in lines)
            {
                paragraph = new iTextSharp.text.Paragraph(line, _font);
                _pdfWriter.Add(paragraph);
                _pdf.Close();
                break;
            }

            

        }

        void Page1_www_zaidu_la(int _index)
        {
            string _link = string.Format("https://www.zaidu.la{0}", PageLinkList[_index]);
            WebClient MyWebClient = new WebClient();
            MyWebClient.Credentials = CredentialCache.DefaultCredentials;//获取或设置用于向Internet资源的请求进行身份验证的网络凭据
            Byte[] pageData = MyWebClient.DownloadData(_link); //从指定网站下载数据
            string pageHtml = Encoding.UTF8.GetString(pageData);

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(pageHtml);

            HtmlAgilityPack.HtmlNode _n1 = doc.DocumentNode.SelectSingleNode("//div[@class='zhangjieTXT']");

            ChapterList[_index]._page1 = _n1.InnerText;


            ChapterList[_index].Repair_zaidu_la();
            ChapterList[_index].Repair();

            ShowChapter_www_zaidu_la(_index);
        }

        void ShowChapter_www_zaidu_la(int _index)
        {
            textBox1.Text = "";
            textBox1.AppendText(ChapterList[_index]._name1 + "\r\n");
            textBox1.AppendText(ChapterList[_index]._page1 + "\r\n");
        }

        void GetList_www_zaidu_la(string _html)
        {
            ChapterList.Clear();
            PageLinkList.Clear();
            listBox1.Items.Clear();

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(_html);

            HtmlAgilityPack.HtmlNode _n = doc.DocumentNode.SelectSingleNode("//div[@class='box_con']//div[@id='list']");
            //textBox1.Text = _n.InnerHtml;

            HtmlAgilityPack.HtmlDocument doc2 = new HtmlAgilityPack.HtmlDocument();
            doc2.LoadHtml(_n.InnerHtml);

            HtmlAgilityPack.HtmlNodeCollection _n2 = doc2.DocumentNode.SelectNodes("//dd");

            bool _begin = false;
            foreach (HtmlAgilityPack.HtmlNode _node in _n2)
            {
                string _ht = _node.OuterHtml;
                
//                 if (_ht.IndexOf("上架感言") >= 0)
//                 {
//                     continue;
//                 }

                if (_ht.IndexOf("0001 赔我一条裤子") >= 0)
                {
                    _begin = true;
                }

                if (!_begin)
                {
                    continue;
                }

                if (_ht.IndexOf("=\"") > 0 && _ht.IndexOf("\">") > 0)
                {
                    int _s1 = _ht.IndexOf("href=");
                    int _e1 = _ht.IndexOf("\">");

                    char[] _link = new char[_e1 - _s1 - 6];
                    _ht.CopyTo(_s1 + 6, _link, 0, _e1 - _s1 - 6);

                    string _link_str = new string(_link);

                    PageLinkList.Add(_link_str);

                    string _listtxt = string.Format("{0} [{1}]", _node.InnerText, _link_str);

                    ChapterNode _chapterNode = new ChapterNode();
                    _chapterNode._name1 = _node.InnerText;

                    ChapterList.Add(_chapterNode);

                    int _idx = listBox1.Items.Add(_listtxt);
                    _idx++;
//                     if (_ht.IndexOf(_idx.ToString()) < 0)
//                     {
// 
//                     }
                    //textBox1.AppendText(_link_str + "\r\n");
                }
                else if (_ht.IndexOf("='") > 0 && _ht.IndexOf("'>") > 0)
                {
                    int _s1 = _ht.IndexOf("href=");
                    int _e1 = _ht.IndexOf("'>");

                    char[] _link = new char[_e1 - _s1 - 6];
                    _ht.CopyTo(_s1 + 6, _link, 0, _e1 - _s1 - 6);

                    string _link_str = new string(_link);

                    PageLinkList.Add(_link_str);

                    string _listtxt = string.Format("{0} [{1}]", _node.InnerText, _link_str);

                    ChapterNode _chapterNode = new ChapterNode();
                    _chapterNode._name1 = _node.InnerText;

                    ChapterList.Add(_chapterNode);

                    int _idx = listBox1.Items.Add(_listtxt);
                    _idx++;
//                     if (_ht.IndexOf(_idx.ToString()) < 0)
//                     {
// 
//                     }
                    //textBox1.AppendText(_link_str + "\r\n");
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            WebClient MyWebClient = new WebClient();
            MyWebClient.Credentials = CredentialCache.DefaultCredentials;//获取或设置用于向Internet资源的请求进行身份验证的网络凭据
            Byte[] pageData = MyWebClient.DownloadData("https://www.zaidu.la/zaidu221881/"); //从指定网站下载数据
            string pageHtml = Encoding.UTF8.GetString(pageData);  //如果获取网站页面采用的是GB2312，则使用这句    
                                                                  //string pageHtml = Encoding.UTF8.GetString(pageData); //如果获取网站页面采用的是UTF-8，则使用这句
                                                                  //Console.WriteLine(pageHtml);//在控制台输入获取的内容

            GetList_www_zaidu_la(pageHtml);
        }

        void Save_www_zaidu_la()
        {
            string filePath = Directory.GetCurrentDirectory() + "\\" + Process.GetCurrentProcess().ProcessName + "www_zaidu_la.txt";
            if (File.Exists(filePath))
                File.Delete(filePath);

            FileStream fs = new FileStream(filePath, FileMode.Create);
            StreamWriter _writer = new StreamWriter(fs);

            for (int i = 0; i < PageLinkList.Count; i++)
            {
                Page1_www_zaidu_la(i);

                if (i < ChapterList.Count)
                {
                    _writer.WriteLine(ChapterList[i]._name1);
                    _writer.WriteLine(ChapterList[i]._page1);
                    _writer.Flush();
                }
            }

            _writer.Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Save_www_zaidu_la();
            //Page1_www_zaidu_la(0);
        }

        void GetList_www_biquge_info(string _html)
        {
            ChapterList.Clear();
            PageLinkList.Clear();
            listBox1.Items.Clear();

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(_html);

            HtmlAgilityPack.HtmlNode _n = doc.DocumentNode.SelectSingleNode("//div[@class='box_con']//div[@id='list']");
            //textBox1.Text = _n.InnerHtml;

            HtmlAgilityPack.HtmlDocument doc2 = new HtmlAgilityPack.HtmlDocument();
            doc2.LoadHtml(_n.InnerHtml);

            HtmlAgilityPack.HtmlNodeCollection _n2 = doc2.DocumentNode.SelectNodes("//dd");

            bool _begin = false;
            foreach (HtmlAgilityPack.HtmlNode _node in _n2)
            {
                string _ht = _node.OuterHtml;

                //                 if (_ht.IndexOf("上架感言") >= 0)
                //                 {
                //                     continue;
                //                 }

                if (_ht.IndexOf("0001 赔我一条裤子") >= 0)
                {
                    _begin = true;
                }

                if (!_begin)
                {
                    continue;
                }

                if (_ht.IndexOf("=\"") > 0 && _ht.IndexOf("\">") > 0)
                {
                    int _s1 = _ht.IndexOf("href=");
                    int _e1 = _ht.IndexOf("\" title");

                    char[] _link = new char[_e1 - _s1 - 6];
                    _ht.CopyTo(_s1 + 6, _link, 0, _e1 - _s1 - 6);

                    string _link_str = new string(_link);

                    PageLinkList.Add(_link_str);

                    string _listtxt = string.Format("{0} [{1}]", _node.InnerText, _link_str);

                    ChapterNode _chapterNode = new ChapterNode();
                    _chapterNode._name1 = _node.InnerText;

                    ChapterList.Add(_chapterNode);

                    int _idx = listBox1.Items.Add(_listtxt);

                    //Page1_www_biquge_info(_idx);
                }
            }
        }

        void Page1_www_biquge_info(int _index)
        {
            string _link = string.Format("https://www.biquge.info/55_55472/{0}", PageLinkList[_index]);
            WebClient MyWebClient = new WebClient();
            MyWebClient.Credentials = CredentialCache.DefaultCredentials;//获取或设置用于向Internet资源的请求进行身份验证的网络凭据
            Byte[] pageData = MyWebClient.DownloadData(_link); //从指定网站下载数据
            string pageHtml = Encoding.UTF8.GetString(pageData);

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(pageHtml);

            HtmlAgilityPack.HtmlNode _n1 = doc.DocumentNode.SelectSingleNode("//div[@id='content']");

            ChapterList[_index]._page1 = _n1.InnerText;

            ChapterList[_index].Repair_NBSP_MDASH();
            ChapterList[_index].Repair();

            ShowChapter_www_biquge_info(_index);
        }

        void ShowChapter_www_biquge_info(int _index)
        {
            textBox1.Text = "";
            textBox1.AppendText(ChapterList[_index]._name1 + "\r\n");
            textBox1.AppendText(ChapterList[_index]._page1 + "\r\n");

            toolStripStatusLabel1.Text = string.Format("State: {0} / {1}", _index, PageLinkList.Count);
            statusStrip1.Refresh();
        }

        void Save_www_biquge_info()
        {
            string filePath = Directory.GetCurrentDirectory() + "\\" + Process.GetCurrentProcess().ProcessName + "_www_biquge_info.txt";
            if (File.Exists(filePath))
                File.Delete(filePath);

            FileStream fs = new FileStream(filePath, FileMode.Create);
            StreamWriter _writer = new StreamWriter(fs);

            for (int i = 0; i < PageLinkList.Count; i++)
            {
                Page1_www_biquge_info(i);

                if (i < ChapterList.Count)
                {
                    _writer.WriteLine(ChapterList[i]._name1);
                    _writer.WriteLine(ChapterList[i]._page1);
                    _writer.Flush();
                }
            }

            _writer.Close();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            

            //"正在手打中，稍后即将更新！"
            string filePath = Directory.GetCurrentDirectory() + "\\" + Process.GetCurrentProcess().ProcessName + "www_zaidu_la.txt";
            FileStream fs = new FileStream(filePath, FileMode.Open);
            StreamReader _reader = new StreamReader(fs);
            
            while (_reader.EndOfStream == false)
            {
                string _linename = _reader.ReadLine();
                string _linetxt = _reader.ReadLine();

                if (_linetxt == "正在手打中，稍后即将更新！")
                {
                    
                }
            }

            _reader.Close();
            //StreamWriter _writer = new StreamWriter(fs);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            WebClient MyWebClient = new WebClient();
            MyWebClient.Credentials = CredentialCache.DefaultCredentials;//获取或设置用于向Internet资源的请求进行身份验证的网络凭据
            Byte[] pageData = MyWebClient.DownloadData("https://www.biquge.info/55_55472/"); //从指定网站下载数据
            string pageHtml = Encoding.UTF8.GetString(pageData);

            GetList_www_biquge_info(pageHtml);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Save_www_biquge_info();
        }
    }
}
