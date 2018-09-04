using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StartNewMakeAccount
{
    class Steps
    {
        private ChromeDriver driver;

        private bool gender = false;
        private bool country = false;
        private bool card = false;
        private bool scip = false;
        private bool main = false;
        private bool settings = false;
        private bool url = false;

        private string email;
     

        protected string[] name;

        const string PASSWORD = "trance333";

        public Steps(ChromeDriver driver)
        {
            this.driver = driver;
        }

        public bool Settings()
        {
            return this.settings;
        }

        public bool MakeLogin()
        {
           
            try
            {
                var random = new Random();
                this.name = File.ReadLines("names.txt").ToArray();
                string[] emails = File.ReadAllLines("gmail.txt");
                

                email = $"{emails[random.Next(0, emails.Length)]}+{name[random.Next(1500, 3000)].Trim()}{name[random.Next(0, 1500)].Trim()}@gmail.com";

                string current_name = name[new Random().Next(0, 3000)];

                driver.Url = "http://pinterest.com/logout";
                driver.FindElementByXPath("//input[@name='id']").SendKeys(email);
                driver.FindElementByXPath("//input[@name='password']").SendKeys(PASSWORD);

                driver.Manage().Timeouts().ImplicitWait = new TimeSpan(0, 0, 5);
                var ages = driver.FindElementsByCssSelector("[name=age]");
                if (ages.Count() > 0)
                {
                    ages[0].SendKeys(new Random().Next(19, 40).ToString());
                }
                driver.Manage().Timeouts().ImplicitWait = new TimeSpan(0, 0, 55);
                driver.FindElementByCssSelector("button.red").Click();

                File.AppendAllText(DateTime.Now.ToString("yyyyMMdd") + ".txt", $"{email}:{PASSWORD}{Environment.NewLine}");

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            finally
            {
               
              
            }

        }

        public void CheckPage()
        {
            if (!gender && driver.FindElementsByCssSelector(".NuxGenderStep__headerContent").Count == 1)
            {
                var buttons = driver.FindElementsByTagName("button");
                foreach (var item in buttons)
                {
                    if(item.Text.ToLower().Contains("female"))
                    {
                        item.Click();
                    }
                }
                //Female
            }
            if (!gender && driver.FindElementsByCssSelector("label[for='female']").Count == 1)
            {
                try
                {
                    var x = driver.FindElementsByCssSelector("label[for='female']");
                    x[0].Click();
                    gender = true;
                }
                catch { }

            }
            if (!gender && driver.FindElementsById("female") != null)
            {
                try
                {
                    driver.FindElementById("female").Click();
                    gender = true;
                }
                catch { }


            }
            if (!country && driver.FindElementById("newUserCountry") != null)
            {
                try
                {
                    driver.FindElementByCssSelector(".NuxContainer__NuxStepContainer button").Click();
                    country = true;
                }
                catch { }
            }

            if (!card && driver.FindElementsByCssSelector(".NuxInterest").Count > 1)
            {

                int result = 0;
                try
                {
                    var cards = driver.FindElementsByCssSelector(".NuxInterest");
                    for (int i = 0; i < 100; i++)
                    {

                        if (result > 5)
                        {
                            var button = driver.FindElementsByCssSelector(".NuxPickerFooter button");
                            if (button.Count > 0)
                            {
                                button[0].Click();
                                card = true;
                                break;
                            }

                        }

                        try
                        {
                            if (cards[i].Enabled && cards[i].Displayed)
                            {
                                OpenQA.Selenium.Interactions.Actions action = new OpenQA.Selenium.Interactions.Actions(driver);
                                action.MoveToElement(cards[i]).DoubleClick().Build().Perform();
                                //   WebElement we = webdriver.findElement(By.xpath("html/body/div[13]/ul/li[4]/a"));
                                //    action.moveToElement(we).moveToElement(webdriver.findElement(By.xpath("/expression-here"))).click().build().perform();
                                //}
                                //cards[i].Click();
                                result++;
                            }
                            else
                            {
                                Console.WriteLine("not possible");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            cards = driver.FindElementsByCssSelector(".NuxInterest");
                        }

                    }

                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }

            }

            if (!scip && card && driver.FindElementsByCssSelector(".NuxExtensionUpsell__optionalSkip").Count != 0)
            {
                try
                {
                    var scripButton = driver.FindElementsByCssSelector(".NuxExtensionUpsell__optionalSkip");
                    scripButton[0].Click();
                    scip = true;

                }
                catch
                { }
            }


            if (card && !main)
            {
                try
                {
                    var body = driver.FindElementsByCssSelector(".mainContainer");


                    //if(body.Count > 0)
                    body[0].Click();

                    Thread.Sleep(TimeSpan.FromSeconds(15));

                    var buttons = driver.FindElementsByTagName("button");
                    foreach (IWebElement button in buttons)
                    {
                        Console.WriteLine(button.Text);
                        if (button.Text.Contains("Got"))
                        {
                            OpenQA.Selenium.Interactions.Actions action = new OpenQA.Selenium.Interactions.Actions(driver);
                            action.MoveToElement(button).DoubleClick().Build().Perform();

                        }
                    }



                    Thread.Sleep(TimeSpan.FromSeconds(5));
                    driver.FindElementByTagName("body").Click();


                    driver.Url = "https://www.pinterest.com/";
                    Thread.Sleep(TimeSpan.FromSeconds(15));

                    main = true;
                }
                catch
                {

                }
            }

            if (main && !url)
            {
                try
                {
                    Thread.Sleep(TimeSpan.FromSeconds(5));

                    if (driver.Url.EndsWith("/settings/"))
                        url = true;

                    driver.Url = "https://pinterest.com/settings";


                }
                catch
                {


                }
            }

            if (url && !settings)
            {
                try
                {
                    for (int i = 0; i < 50; i++)
                    {
                        driver.FindElementById("userFirstName").SendKeys(Keys.Backspace);
                        driver.FindElementById("userLastName").SendKeys(Keys.Backspace);

                    }
                    var selectElement = new SelectElement(driver.FindElementById("accountBasicsCountry"));
                    selectElement.SelectByValue("US");


                    driver.FindElementById("userFirstName").SendKeys(PrettyName());
                    driver.FindElementById("userLastName").SendKeys(PrettyName());

                    string[] cities = File.ReadAllLines("city_names.txt");
                    driver.FindElementById("userLocation").SendKeys(cities[new Random().Next(0, cities.Count())]);

                    string userName = driver.FindElementById("userUserName").GetAttribute("value");

                    //malmopianoilianaruby
                    SaveSettings();
                    settings = true;
                    //$$("input[type=file]")
                    Thread.Sleep(new TimeSpan(0, 0, 5));



                    //data-test-id="createBoardCard"
                    // tut

                    driver.Url = "https://pinterest.com/" + userName + "/boards";
                    Thread.Sleep(new TimeSpan(0, 0, 5));
                    var createBoards = driver.FindElementsByCssSelector("[data-test-id=createBoardCard]");
                    Thread.Sleep(new TimeSpan(0, 0, 5));
                    foreach (var item in createBoards)
                    {
                        if (item.Enabled && item.Displayed)
                        {
                            item.Click();

                        }
                    }

                    var input = driver.FindElementById("boardEditName");
                    input.SendKeys(PrettyName());

                    var buttonsCreate = driver.FindElementsByTagName("button");
                    foreach (var item in buttonsCreate)
                    {
                        if (item.Text.Contains("Create") && item.Enabled && item.Displayed)
                        {
                            item.Click();
                            Thread.Sleep(new TimeSpan(0, 0, 15));
                            AddImage();


                        }
                    }
                   






                }
                catch
                {

                }
            }

        }

        private void AddImage()
        {
            driver.Url = "https://pinterest.com/settings";
            driver.FindElementById("userLastName").SendKeys(Keys.Space);
            var buttons = driver.FindElementsByTagName("button");
            foreach (var item in buttons)
            {
                var xt = item.Text;
                if (item.Text.Contains("Change picture"))
                {

                    try
                    {
                        driver.FindElementById("userLastName").SendKeys(Keys.Backspace);
                        OpenQA.Selenium.Interactions.Actions action = new OpenQA.Selenium.Interactions.Actions(driver);
                        action.MoveToElement(item);
                        item.Click();
                        Thread.Sleep(new TimeSpan(0, 0, 5));

                        IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                        string count = (string)js.ExecuteScript("document.querySelector('input[type=file]').setAttribute('style', 'display:block');");

                        var image = driver.FindElementByCssSelector("input[type=file]");
                        if (image != null && image.Enabled && image.Displayed)
                        {
                            var x = new ImageRepository();
                            x.LoadImages();
                            string imgPath = x.Random();
                            image.SendKeys(imgPath);
                            Thread.Sleep(new TimeSpan(0, 0, 15));
                            SaveSettings();
                            Thread.Sleep(new TimeSpan(0, 0, 5));
                            x.Delete(imgPath);
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("exception on imnage iploade", ex.Message);
                    }
                    //item.Click();
                }
            }
        }

        private bool SaveSettings()
        {
            var buttons = driver.FindElementsByTagName("button");
            foreach (IWebElement button in buttons)
            {
                if (button.Text.Contains("Save"))
                    button.Click();
            }

            return true;
        }

        private string PrettyName()
        {
            string prettyName =  name[new Random().Next(0, 3000)];
            string res =  prettyName.ToLower().Trim();
            return  new CultureInfo("en-US").TextInfo.ToTitleCase(res);


        }
    }
}
