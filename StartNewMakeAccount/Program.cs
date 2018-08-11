using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartNewMakeAccount
{
    class Program
    {
       static  bool show = false;
        static void Main(string[] args)
        {
            GetProxy.ProxyReader proxyReader = new GetProxy.ProxyReader();
            var list = proxyReader.GetList();


            // WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(70));
            int i = 0;
            Console.WriteLine("Show ?");
            if (Console.ReadLine() != "y")
                show = true;

            while (true)
            {


                if (i > 5)
                {
                    list.RemoveAt(0);
                    i = 0;
                }

                ChromeOptions option = new ChromeOptions();
                option.AddArgument($"--proxy-server={list.FirstOrDefault()}");  //
                option.AddArgument("no-sandbox");

                if(show)
                option.AddArgument("--window-position=-32000,-32000");
                ChromeDriver driver = new ChromeDriver(option);
                driver.Manage().Timeouts().ImplicitWait = new TimeSpan(0, 1, 30);

                Steps ac = new Steps(driver);
                if (ac.MakeLogin())
                {
                    while (ac.Settings() != true)
                    {
                        ac.CheckPage();

                    }
                    driver.Quit();
                    i++;
                }
                else
                {
                    list.RemoveAt(0);
                    driver.Quit();
                }
            }







        }


    }
}
