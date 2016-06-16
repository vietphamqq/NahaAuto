using System;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System.Threading;

namespace NahaAuto.Code
{
    public abstract class AutoWebBase
    {
        public static string DownloadFolder;

        private const int DefaultTimeout = 30;

        protected IWebDriver Driver { get; set; }

        public void Close()
        {
            Driver.Close();
        }

        public abstract void Setup();

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
            Driver.Navigate().GoToUrl(url);
        }

        public void MaximiseWindow()
        {
            Driver.Manage().Window.Maximize();
        }

        public string GetAttributeFrom(By selector, string attributeName, int timeoutInSeconds = DefaultTimeout)
        {
            return RetryOnStale(() => Driver.FindElement(selector, timeoutInSeconds).GetAttribute(attributeName));
        }

        public string GetTextFrom(By selector, int timeoutInSeconds = DefaultTimeout)
        {
            return RetryOnStale(() => Driver.FindElement(selector, timeoutInSeconds).Text);
        }

        public void ClearControl(By selector, int timeoutInSeconds = DefaultTimeout)
        {
            RetryOnStale(() => Driver.FindElement(selector, timeoutInSeconds).Clear());
        }

        public void EnterTextIn(By selector, string text, int timeoutInSeconds = DefaultTimeout)
        {
            RetryOnStale(() => Driver.FindElement(selector, timeoutInSeconds).SendKeys(text));
        }

        public void ClickOn(By selector, int timeoutInSeconds = DefaultTimeout)
        {
            RetryOnStale(() => Driver.FindElement(selector, timeoutInSeconds).Click());
        }

        public void SelectFrom(By selector, string text, int timeoutInSeconds = DefaultTimeout)
        {
            RetryOnStale(() => new SelectElement(Driver.FindElement(selector, timeoutInSeconds)).SelectByText(text));
            WaitForPageReload();
        }

        public void SelectCheckbox(By selector, int timeoutInSeconds = DefaultTimeout)
        {
            RetryOnStale(() => Checkbox(true, Driver.FindElement(selector, timeoutInSeconds)));
        }

        public void DeselectCheckbox(By selector, int timeoutInSeconds = DefaultTimeout)
        {
            RetryOnStale(() => Checkbox(false, Driver.FindElement(selector, timeoutInSeconds)));
        }

        private void Checkbox(bool isSelected, IWebElement checkbox)
        {
            if (checkbox.Selected == isSelected)
                return;

            ScrollToElement(checkbox);
            checkbox.Click();
        }

        public void ScrollToElement(By selector, int timeoutInSeconds = DefaultTimeout)
        {
            ScrollToElement(Driver.FindElement(selector, timeoutInSeconds));
        }

        private void ScrollToElement(IWebElement element)
        {
            // Down arrow is clicked to ensure element is not hidden by the bottom navigation bar.
            RetryOnStale(() => new Actions(Driver).MoveToElement(element).SendKeys(Keys.ArrowDown).Perform());
        }

        public void ScrollTo(string elementId)
        {
            RetryOnStale(
                () =>
                new Actions(Driver).MoveToElement(Driver.FindElement(By.Id(elementId)))
                    .SendKeys(Keys.ArrowDown)
                    .Perform());
        }

        public int GetCountOf(By selector, int timeoutInSeconds = DefaultTimeout)
        {
            return RetryOnStale(() => Driver.FindElements(selector, timeoutInSeconds).Count);
        }

        public void WaitTillVisible(By selector, int timeoutInSeconds = DefaultTimeout)
        {
            new WebDriverWait(Driver, TimeSpan.FromSeconds(timeoutInSeconds)).Until(
                ExpectedConditions.ElementIsVisible(selector));
        }

        public void WaitForPopup()
        {
            new WebDriverWait(Driver, TimeSpan.FromSeconds(DefaultTimeout)).Until(d => d.WindowHandles.Count > 1);
        }

        public void Wait(int inMilSecond)
        {
            //new WebDriverWait(Driver, TimeSpan.FromSeconds(inMilSecond)).Until(d => false);
            Thread.Sleep(inMilSecond);
        }

        public void WaitForPageReload()
        {
            WaitForPageReload(DefaultTimeout);
        }

        public void WaitForPageReload(int maxWaitTimeInSeconds)
        {
            var state = string.Empty;
            try
            {
                var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(maxWaitTimeInSeconds));
                wait.Until(
                    d =>
                    {
                        try
                        {
                            state =
                                ((IJavaScriptExecutor)Driver).ExecuteScript(@"return document.readyState")
                                    .ToString();
                        }
                        catch (InvalidOperationException)
                        {
                            //Ignore
                        }
                        catch (NoSuchWindowException)
                        {
                            //when pop-up is closed, switch to last windows
                            Driver.SwitchTo().Window(Driver.WindowHandles.Last());
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
                if (Driver.WindowHandles.Count == 1)
                {
                    Driver.SwitchTo().Window(Driver.WindowHandles[0]);
                }
                state = ((IJavaScriptExecutor)Driver).ExecuteScript(@"return document.readyState").ToString();
                if (
                    !(state.Equals("complete", StringComparison.InvariantCultureIgnoreCase)
                      || state.Equals("loaded", StringComparison.InvariantCultureIgnoreCase))) throw;
            }
        }

        public void SwitchToFrame()
        {
            new WebDriverWait(Driver, TimeSpan.FromSeconds(DefaultTimeout)).Until(
                d => Driver.SwitchTo().Frame(0));
        }

        public void AcceptAlert()
        {
            AcceptAlert(DefaultTimeout);
        }

        public void AcceptAlert(int timeout)
        {
            new WebDriverWait(Driver, TimeSpan.FromSeconds(timeout)).Until(d => Driver.SwitchTo().Alert())
                .Accept();
        }

        public string GetCurrentWindow()
        {
            return Driver.CurrentWindowHandle;
        }

        public void SwitchToNewWindow()
        {
            new WebDriverWait(Driver, TimeSpan.FromSeconds(60)).Until(
                d =>
                {
                    if (Driver.WindowHandles.Count == 1) return false;
                    return true;
                });

            Driver.SwitchTo().Window(Driver.WindowHandles[Driver.WindowHandles.Count - 1]);
        }

        public void SwitchToWindow(string window)
        {
            Driver.SwitchTo().Window(window);
        }

        public void SwitchToMainWindow()
        {
            Driver.SwitchTo().Window(Driver.WindowHandles[0]);
        }
    }
}