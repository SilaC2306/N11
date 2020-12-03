using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace n11
{
    public partial class Form1 : Form
    {
        List<Page> PageList;
        Page CurrrentPage = new Page();

        ChromeDriver driver = null;

        Timer tmUrl = null;

        bool isStarted = false;
        public Form1()
        {
            InitializeComponent();

            tmUrl = new Timer();
            tmUrl.Interval = 200;
            tmUrl.Tick += TmUrl_Tick;
            tmUrl.Start();
        }

        bool inTm = false;
        private void TmUrl_Tick(object seneder, EventArgs e)
        {
            if (!isStarted || inTm || driver == null) return;
            inTm = true;

            if (CurrrentPage.Url != driver.Url)
            {
                Page tPage = PageList.FirstOrDefault(x => driver.Url.Contains(x.Url));

                if (tPage != null && CurrrentPage.Url != tPage.Url)
                {
                    CurrrentPage = tPage;
                    richTextBox1.Text += Environment.NewLine + DateTime.Now.ToString() + "- Sayfa Değişti:" + CurrrentPage.Name;
                }
            }

            inTm = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (driver != null)
            {
                driver.Quit();
            }
        }
       
       
        private void btnStart_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = false;
            txtBaseUrl.Enabled = true;

            PageList = new List<Page>();

            PageList.Add(new Page
            {
                Url =txtBaseUrl.Text + "giris-yap",
                Name = "Giriş Sayfası",
                PageType = PageTypes.Login
            });

            driver = new ChromeDriver();
            driver.Navigate().GoToUrl(txtBaseUrl.Text);
           
            isStarted = true;
            btnFinish.Enabled = true;
            PageList.Add(new Page
            {
                Url = txtBaseUrl.Text + "",
                Name = "N11",
                PageType = PageTypes.Search
            });
            
            isStarted = true;
            btnFinish.Enabled = true;
            click("btnSignIn", ByTypes.ClassName);
            jobLogin();
            driver.Navigate().GoToUrl(txtBaseUrl.Text);




        }

        private void btnFinish_Click(object sender, EventArgs e)
        {
            btnFinish.Enabled = false;
            driver.Quit();
            btnStart.Enabled = true;
            isStarted = false;
            txtBaseUrl.Enabled = true;

        }

        //Giriş
        void jobLogin()
        {
            if (!isStarted || CurrrentPage.PageType != PageTypes.Login) return;

            
            sendKeys("email", ByTypes.Id,"scelik23898@gmail.com");
            sendKeys("password", ByTypes.Id,"Dg123654*");
            click("loginButton", ByTypes.ClassName);

            if (exist("locationToken", ByTypes.Id))
            {
                richTextBox1.Text += Environment.NewLine + "- Başarılı:";

            }
            else
            {
                richTextBox1.Text += Environment.NewLine + "- Başarısız:";
            }

        }

        //Arama
        void jobSearch()
        {
            if (!isStarted || CurrrentPage.PageType != PageTypes.Search) return;

            click("searchData", ByTypes.Id);
            sendKeys("searchData", ByTypes.Id, "samsung");
            click("searchBtn", ByTypes.ClassName);
            
          

        }
        private void btnSearch_Click(object sender, EventArgs e)
        {
            jobSearch();
        }


        private void btnLogin_Click(object sender, EventArgs e)
        {
            jobLogin();
        }

        IWebElement findElement(string param, ByTypes byType)
        {
            By by = By.Id(param);

            switch (byType)
            {
                case ByTypes.Id:
                    by = By.Id(param);
                    break;
                case ByTypes.Name:
                    by = By.Name(param);
                    break;
                case ByTypes.ClassName:
                    by = By.ClassName(param);
                    break;
                case ByTypes.XPath:
                    by = By.XPath(param);
                    break;
                default:
                    by = By.Id(param);
                    break;
            }

            return driver.FindElement(by);
        }

        void click(string param, ByTypes byType)
        {
            IWebElement element = findElement(param, byType);
            element.Click();
        }
        void sendKeys(string param, ByTypes byType, string value)
        {
            IWebElement element = findElement(param, byType);
            element.SendKeys(value);
        }
        bool exist(string param, ByTypes byType)
        {
            try
            {
                findElement(param, byType);

                return true;
            }
            catch
            {
                return false;
            }
        }


        public enum ByTypes
        {
            Id,
            Name,
            ClassName,
            XPath
        }

        public class Page
        {
            public string Url { get; set; }
            public string Name { get; set; }
            public PageTypes PageType { get; set; }
        }

        public enum PageTypes
        {
            Login = 1,
            Search = 2
            
        }

        private void txtBaseUrl_TextChanged(object sender, EventArgs e)
        {
        }

       
    }
}
