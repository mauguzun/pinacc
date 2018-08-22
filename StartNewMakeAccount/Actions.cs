using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
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
        private List<string> emails;

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
            string[] emails = File.ReadAllLines("email.txt");
            try
            {
                var random = new Random();
                this.name = File.ReadLines("names.txt").ToArray();
                this.emails = File.ReadAllLines("gmail.txt").ToList();

                email = $"mauguzun+{name[random.Next(1500, 3000)].Trim()}{name[random.Next(0, 1500)].Trim()}@gmail.com";

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
                    for (int i = 0; i < 20; i++)
                    {
                        driver.FindElementById("userFirstName").SendKeys(Keys.Backspace);
                        driver.FindElementById("userLastName").SendKeys(Keys.Backspace);

                    }
                   var selectElement = new SelectElement(driver.FindElementById("accountBasicsCountry"));
                    selectElement.SelectByValue("US");


                    driver.FindElementById("userFirstName").SendKeys(name[new Random().Next(0, 3000)]);
                    driver.FindElementById("userLastName").SendKeys(name[new Random().Next(0, 3000)]);
                    var buttons = driver.FindElementsByTagName("button");
                    foreach (IWebElement button in buttons)
                    {
                        if (button.Text.Contains("Save"))
                            button.Click();
                    }
                    settings = true;
                    // tut

                    driver.Url = "https://pinterest.com/";

                    var wrappers = driver.FindElementsByClassName("pinWrapper");
                    wrappers[1].Click();

                    var sv = driver.FindElementByClassName("PulsarNewInnerCircle");
                    Thread.Sleep(TimeSpan.FromSeconds(3));
                    OpenQA.Selenium.Interactions.Actions action = new OpenQA.Selenium.Interactions.Actions(driver);
                    action.MoveToElement(sv).DoubleClick().Build().Perform();

                    driver.FindElementById("boardEditName").SendKeys(name[new Random().Next(0, 3000)].Trim());
                 


                    var saves = driver.FindElementsByTagName("button");
                    foreach (var save in saves)
                    {

                        if (save.Text.Contains("Create"))
                        {
                            try
                            {
                                save.Click();
                                Thread.Sleep(TimeSpan.FromSeconds(5));
                            }
                            catch
                            {

                            }


                        }


                    }



                }
                catch
                {

                }
            }

        }


    }
}
