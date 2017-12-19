using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;

//using System.Web;
//using HtmlAgilityPack;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        List<News> newslist = new List<News>();                                                                                  //Глобальний ліст класів для новин і дати
        public Form1()
        {
            InitializeComponent();
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            HttpWebRequest request = WebRequest.Create("http://www.pravda.com.ua/") as HttpWebRequest;                          //Веб запит get
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            Stream receiveStream = response.GetResponseStream();
            StreamReader readStream = new StreamReader(receiveStream, Encoding.GetEncoding(1251));            
            var rr = readStream.ReadToEnd();
            string[] stringSeparators = new string[] { "<div class=\"article__time\">" };                                       //use regular 
            string time = Regex.Match(rr, "<div class=\"article__time\">(.*)</div>").ToString();
            string[] split = time.Split(stringSeparators, StringSplitOptions.None);                     
            foreach (string element in split)
            {
                if (element != "")
                {                    
                    newslist.Add(new News(element));                    
                }
            }            
            listView1.Columns.Add("Date", newslist.Count, HorizontalAlignment.Left);
            listView1.Columns.Add("News", newslist.Count, HorizontalAlignment.Left);            
            foreach (News el in newslist)
            {
                if (el.novina != "")
                {
                    var item2 = new ListViewItem(new[] { el.time.ToLongTimeString(), el.novina });
                    listView1.Items.Add(item2);
                }
            }
            response.Close();
            readStream.Close();
                                                                                                                                    //Спробував за допомогою xpath але шо то пошло не так
            /*       string Url1 = "http://www.pravda.com.ua/";
                   HtmlWeb web1 = new HtmlWeb();
                   HtmlDocument doc = web1.Load(Url1);

                   string metascore = doc.DocumentNode.SelectNodes("//div[@id='article__title")[0].InnerText;
                   string userscore = doc.DocumentNode.SelectNodes("//*[@id=\"article__time\"]/html/body/div[1]/div[3]/div[3]/div/div[1]/div/div[1]/div[3]/div[2]/div/div[1]/div[1]")[0].InnerText;
                  // string summary = doc.DocumentNode.SelectNodes("//*[@id=\"article__time\"]/div[3]/div/div[2]/div[2]/div[1]/ul/li/span[2]/span/span[1]")[0].InnerText;

           */
        }

        class News                                                                                                                  //клас в якому конструктор розбиває стрічку відділяє час від новин
        {
           public DateTime time;
           public string novina;
            public News(string str)
            {
                time = Convert.ToDateTime(str.Substring(0, 5));
                string[] stringSeparators = new string[] { "\">" };
                string[] stringSeparators2 = new string[] { "</a>" };
                string[] stringSeparators3 = new string[] { "<em>" };
                string[] split1 = str.Split(stringSeparators, StringSplitOptions.None);
                string[] split2 = split1[2].Split(stringSeparators2, StringSplitOptions.None);
                string[] split3 = split2[0].Split(stringSeparators3, StringSplitOptions.None);              
                novina = split3[0];
            }
        }      
        private int sortColumn = -1;
        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)               
        {
            if (e.Column != sortColumn)
            {
                sortColumn = e.Column;
                listView1.Sorting = SortOrder.Ascending;
            }
            else
            {
                if (listView1.Sorting == SortOrder.Ascending)
                    listView1.Sorting = SortOrder.Descending;
                else
                    listView1.Sorting = SortOrder.Ascending;
            }
            listView1.Sort();
            this.listView1.ListViewItemSorter = new ListViewItemComparer(e.Column);
        }
    }
    class ListViewItemComparer : IComparer                                               //для сортування колонок                 
    {
        public SortOrder Order = SortOrder.Ascending;
        public int Column;

        public ListViewItemComparer()
        {
            Column = 0;
        }
        public ListViewItemComparer(int column)
        {
            Column = column;
        }
        public int Compare(object x, object y)//муть
        {
            int returnVal = String.Compare(((ListViewItem)x).SubItems[Column].Text,
            ((ListViewItem)y).SubItems[Column].Text);
            if (Order == SortOrder.Descending)
                return -returnVal;
            else
                return returnVal;
        }
    }
}
    

