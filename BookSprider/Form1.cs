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

            public string _name1;
            public string _page1;
            public string _name2;
            public string _page2;
        }

        public Form1()
        {
            InitializeComponent();
        }

        List<string> PageLinkList = new List<string>();

        List<ChapterNode> ChapterList = new List<ChapterNode>();

        void Page1(int _index)
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

            Page2(ref _node,_index);
        }

        void Page2(ref ChapterNode _node,int _index)
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
            ShowChapter(ChapterList.Count - 1);
        }

        void ShowChapter(int _index)
        {
            textBox1.Text = "";
            textBox1.AppendText(ChapterList[_index]._name1 + "\r\n");
            textBox1.AppendText(ChapterList[_index]._page1 + "\r\n");
            textBox1.AppendText(ChapterList[_index]._name2 + "\r\n");
            textBox1.AppendText(ChapterList[_index]._page2 + "\r\n");
        }

        void GetList(string _html)
        {
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

            GetList(pageHtml);
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
                Page1(listBox1.SelectedIndex);
            }
        }

        void Save()
        {
            string filePath = Directory.GetCurrentDirectory() + "\\" + Process.GetCurrentProcess().ProcessName + ".txt";
            if (File.Exists(filePath))
                File.Delete(filePath);

            FileStream fs = new FileStream(filePath, FileMode.Create);
            StreamWriter _writer = new StreamWriter(fs);

            for (int i = 0; i < PageLinkList.Count; i++)
            {
                Page1(i);

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
            Save();
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
    }
}
