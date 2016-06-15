using System;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using TechTalk.SpecFlow;
using OpenQA.Selenium.Interactions;

namespace Symphony.SystemTests
{
    public class WebTester
    {
        public static string DownloadFolder;

        private const int DefaultTimeout = 30;

        public IWebDriver CurrentDriver { get; set; }

        public string GetTestName()
        {
            return TestContext.CurrentContext.Test.Name;
        }

        [SetUp]
        public void TestSetup()
        {
            // The options below disable Chrome developer extensions, preventing random alert dialogues regarding Chrome developer mode
            // Source: http://stackoverflow.com/questions/23087724/chromedriver-disable-developer-mode-extensions-on-automation
            var options = new ChromeOptions();
            options.AddArguments("--disable-extensions");

            DownloadFolder = WindowsFolders.GetPath(WinFolder.Downloads);
            if (!Directory.Exists(DownloadFolder))
            {
                DownloadFolder = @"C:\Temp";
                options.AddUserProfilePreference("download.default_directory", DownloadFolder);
            }

            CurrentDriver = new ChromeDriver(options);
            
        }


        public string CleanFileName(string fname)
        {
            string invalidChars = Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return Regex.Replace(fname, invalidRegStr, "_");

        }

        public void RetryOnTimeout(Action executable)
        {
            try
            {
                executable();
            }
            catch (WebDriverTimeoutException)
            {
                executable();
            }
        }
        
        private static void RetryOnStale(Action executable)
        {
            try
            {
                executable();
            }
            catch (StaleElementReferenceException)
            {
                executable();
            }
        }

        private static T RetryOnStale<T>(Func<T> executable)
        {
            try
            {
                return executable();
            }
            catch (StaleElementReferenceException)
            {
                return executable();
            }
        }

        public void GoToUrl(string url)
        {
            CurrentDriver.Navigate().GoToUrl(url);
        }

        public void MaximiseWindow()
        {
            CurrentDriver.Manage().Window.Maximize();
        }
        
        public void ClickOn(string linkText, int timeoutInSeconds = DefaultTimeout)
        {
            RetryOnStale(() => CurrentDriver.FindElement(By.LinkText(linkText), timeoutInSeconds).Click());
        }

        public string GetAttributeFrom(string linkTextToFind, string attributeName, int timeoutInSeconds = DefaultTimeout)
        {
            return RetryOnStale(() => CurrentDriver.FindElement(By.LinkText(linkTextToFind), timeoutInSeconds).GetAttribute(attributeName));
        }

        public string GetAttributeFrom(By locator, string attributeName, int timeoutInSeconds = DefaultTimeout)
        {
            return RetryOnStale(() => CurrentDriver.FindElement(locator, timeoutInSeconds).GetAttribute(attributeName));
        }

        public string GetAttributeFrom(Cid control, string attributeName, int timeoutInSeconds = DefaultTimeout)
        {
            return RetryOnStale(() => CurrentDriver.FindElement(By.Id(control.ToFindString()), timeoutInSeconds).GetAttribute(attributeName));
        }
        
        public string GetAttributeFromDisabled(Cid control, string attributeName, int timeoutInSeconds = 10)
        {
            return RetryOnStale(() => CurrentDriver.FindElement(By.Id(control.ToFindString()), timeoutInSeconds, true).GetAttribute(attributeName));
        }

        public string GetTextFrom(Cid control, int timeoutInSeconds = DefaultTimeout)
        {
            return RetryOnStale(() => CurrentDriver.FindElement(By.Id(control.ToFindString()), timeoutInSeconds).Text);
        }

        public string GetTextFrom(By locator, int timeoutInSeconds = DefaultTimeout)
        {
            return RetryOnStale(() => CurrentDriver.FindElement(locator, timeoutInSeconds).Text);
        }

        public void ClearControl(Cid control, int timeoutInSeconds = DefaultTimeout)
        {
            RetryOnStale(() => CurrentDriver.FindElement(By.Id(control.ToFindString()), timeoutInSeconds).Clear());
        }

        public void EnterTextIn(Cid control, string text, int timeoutInSeconds = DefaultTimeout)
        {
            RetryOnStale(() => CurrentDriver.FindElement(By.Id(control.ToFindString()), timeoutInSeconds).SendKeys(text));
        }

        public void EnterTextIn(By locator, string text, int timeoutInSeconds = DefaultTimeout)
        {
            RetryOnStale(() => CurrentDriver.FindElement(locator, timeoutInSeconds).SendKeys(text));
        }

        public void ClickOn(By locator, int timeoutInSeconds = DefaultTimeout)
        {
            RetryOnStale(() => CurrentDriver.FindElement(locator, timeoutInSeconds).Click());
        }
        
        public void ClickOn(Cid control, int timeoutInSeconds = DefaultTimeout)
        {
            RetryOnStale(() => CurrentDriver.FindElement(By.Id(control.ToFindString()), timeoutInSeconds).Click());
        }

        public void ClickOn(Cls cssClass, int timeoutInSeconds = DefaultTimeout)
        {
            RetryOnStale(() => CurrentDriver.FindElement(By.ClassName(cssClass.ToFindString()), timeoutInSeconds).Click());
        }

        public void SelectFrom(Sel selector, string text, int timeoutInSeconds = DefaultTimeout)
        {
            RetryOnStale(() => new SelectElement(CurrentDriver.FindElement(By.Id(selector.ToFindString()), timeoutInSeconds)).SelectByText(text));
            WaitForPageReload();
        }

        public void SelectFrom(string byName, string text, int timeoutInSeconds = DefaultTimeout)
        {
            RetryOnStale(() => new SelectElement(CurrentDriver.FindElement(By.Name(byName), timeoutInSeconds)).SelectByText(text));
            WaitForPageReload();
        }
        public void SelectFrom(Sel selector, int index, int timeoutInSeconds = DefaultTimeout)
        {
            RetryOnStale(() => new SelectElement(CurrentDriver.FindElement(By.Id(selector.ToFindString()), timeoutInSeconds)).SelectByIndex(index));
            WaitForPageReload();
        }

        public void SelectFrom(string byName, int index, int timeoutInSeconds = DefaultTimeout)
        {
            RetryOnStale(() => new SelectElement(CurrentDriver.FindElement(By.Name(byName), timeoutInSeconds)).SelectByIndex(index));
            WaitForPageReload();
        }

        public void SelectCheckbox(Cid selector, int timeoutInSeconds = DefaultTimeout)
        {
            RetryOnStale(() => Checkbox(true, CurrentDriver.FindElement(By.Id(selector.ToFindString()), timeoutInSeconds)));
        }

        public void SelectCheckbox(By locator, int timeoutInSeconds = DefaultTimeout)
        {
            RetryOnStale(() => Checkbox(true, CurrentDriver.FindElement(locator, timeoutInSeconds)));
        }

        public void SelectCheckbox(IWebElement checkbox)
        {
            Checkbox(true, checkbox);
        }

        public void DeselectCheckbox(Cid selector, int timeoutInSeconds = DefaultTimeout)
        {
            RetryOnStale(() => Checkbox(false, CurrentDriver.FindElement(By.Id(selector.ToFindString()), timeoutInSeconds)));
        }

        public void DeselectCheckbox(By locator, int timeoutInSeconds = DefaultTimeout)
        {
            RetryOnStale(() => Checkbox(false, CurrentDriver.FindElement(locator, timeoutInSeconds)));
        }

        public void DeselectCheckbox(IWebElement checkbox)
        {
            Checkbox(false, checkbox);
        }

        private void Checkbox(bool isSelected, IWebElement checkbox)
        {
            if (checkbox.Selected == isSelected) 
                return;

            ScrollToElement(checkbox);
            checkbox.Click();
        }

        public void ScrollToElement(IWebElement element)
        {
            // Down arrow is clicked to ensure element is not hidden by the bottom nav bar.
            RetryOnStale(() => new Actions(CurrentDriver).MoveToElement(element).SendKeys(Keys.ArrowDown).Perform());
        }

        public void ScrollTo(Cid control)
        {
            RetryOnStale(
                () =>
                new Actions(CurrentDriver).MoveToElement(CurrentDriver.FindElement(By.Id(control.ToFindString())))
                    .SendKeys(Keys.ArrowDown)
                    .Perform());

        }

        public int GetCountOf(string linkText, int timeoutInSeconds = DefaultTimeout)
        {
            return RetryOnStale(() => CurrentDriver.FindElements(By.LinkText(linkText), timeoutInSeconds).Count);
        }

        public int GetCountOf(string linkText, Func<IWebElement, bool> where, int timeoutInSeconds = DefaultTimeout)
        {
            return RetryOnStale(() => CurrentDriver.FindElements(By.LinkText(linkText), timeoutInSeconds).Count(where));
        }
        
        public int GetCountOf(By locator, int timeoutInSeconds = DefaultTimeout)
        {
            return RetryOnStale(() => CurrentDriver.FindElements(locator, timeoutInSeconds).Count);
        }

        public void WaitTillVisible(Sel selector, int timeoutInSeconds = DefaultTimeout)
        {
            WaitTillVisible(selector.ToFindString(), timeoutInSeconds);
        }
        public void WaitTillVisible(Cid control, int timeoutInSeconds = DefaultTimeout)
        {
            WaitTillVisible(control.ToFindString(), timeoutInSeconds);
        }

        public void WaitTillVisible(string idToFind, int timeoutInSeconds = DefaultTimeout)
        {
            new WebDriverWait(CurrentDriver, TimeSpan.FromSeconds(timeoutInSeconds)).Until(
                ExpectedConditions.ElementIsVisible(By.Id(idToFind)));
        }

        public void WaitForPopup()
        {
            new WebDriverWait(CurrentDriver, TimeSpan.FromSeconds(DefaultTimeout)).Until(d => d.WindowHandles.Count > 1);
        }

        public void WaitForPageReload()
        {
            WaitForPageReload(DefaultTimeout);
        }

        public void WaitForPageReload(int maxWaitTimeInSeconds)
        {
            // this method has been blatantly plaigiarised from stack overflow.
            string state = string.Empty;
            try
            {
                WebDriverWait wait = new WebDriverWait(CurrentDriver, TimeSpan.FromSeconds(maxWaitTimeInSeconds));

                //Checks every 500 ms whether predicate returns true if returns exit otherwise keep trying till it returns ture
                wait.Until(
                    d =>
                    {
                        try
                        {
                            state =
                                ((IJavaScriptExecutor)CurrentDriver).ExecuteScript(@"return document.readyState")
                                    .ToString();
                        }
                        catch (InvalidOperationException)
                        {
                            //Ignore
                        }
                        catch (NoSuchWindowException)
                        {
                            //when popup is closed, switch to last windows
                            CurrentDriver.SwitchTo().Window(CurrentDriver.WindowHandles.Last());
                        }
                        //In IE7 there are chances we may get state as loaded instead of complete
                        return (state.Equals("complete", StringComparison.InvariantCultureIgnoreCase)
                                || state.Equals("loaded", StringComparison.InvariantCultureIgnoreCase));
                    });
            }
            catch (TimeoutException)
            {
                //sometimes Page remains in Interactive mode and never becomes Complete, then we can still try to access the controls
                if (!state.Equals("interactive", StringComparison.InvariantCultureIgnoreCase)) throw;
            }
            catch (NullReferenceException)
            {
                //sometimes Page remains in Interactive mode and never becomes Complete, then we can still try to access the controls
                if (!state.Equals("interactive", StringComparison.InvariantCultureIgnoreCase)) throw;
            }
            catch (WebDriverException)
            {
                if (CurrentDriver.WindowHandles.Count == 1)
                {
                    CurrentDriver.SwitchTo().Window(CurrentDriver.WindowHandles[0]);
                }
                state = ((IJavaScriptExecutor)CurrentDriver).ExecuteScript(@"return document.readyState").ToString();
                if (
                    !(state.Equals("complete", StringComparison.InvariantCultureIgnoreCase)
                      || state.Equals("loaded", StringComparison.InvariantCultureIgnoreCase))) throw;
            }
        }

        public void SwitchToFrame()
        {
            new WebDriverWait(CurrentDriver, TimeSpan.FromSeconds(DefaultTimeout)).Until(
                d => CurrentDriver.SwitchTo().Frame(0));
        }

        public void AcceptAlert()
        {
            int timeout = DefaultTimeout;
            AcceptAlert(timeout);
        }

        public void AcceptAlert(int timeout)
        {
            new WebDriverWait(CurrentDriver, TimeSpan.FromSeconds(timeout)).Until(d => CurrentDriver.SwitchTo().Alert())
                .Accept();
        }

        public string GetCurrentWindow()
        {
            return CurrentDriver.CurrentWindowHandle;
        }

        public void SwitchToNewWindow()
        {
            new WebDriverWait(CurrentDriver, TimeSpan.FromSeconds(60)).Until(
                d =>
                {
                    if (CurrentDriver.WindowHandles.Count == 1) return false;
                    return true;
                });

            CurrentDriver.SwitchTo().Window(CurrentDriver.WindowHandles[CurrentDriver.WindowHandles.Count - 1]);
        }

        public void SwitchToWindow(string window)
        {
            CurrentDriver.SwitchTo().Window(window);
        }

        public void SwitchToMainWindow()
        {
            CurrentDriver.SwitchTo().Window(CurrentDriver.WindowHandles[0]);
        }

    }
}