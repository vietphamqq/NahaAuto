using System;
using System.Collections.ObjectModel;
using System.Linq;

using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace NahaAuto.Code
{
    public static class WebDriverExtensions
    {
        public static IWebElement FindElement(this IWebDriver driver, By by, int timeoutInSeconds)
        {
            return FindElement(driver, by, timeoutInSeconds, false);
        }

        public static IWebElement FindElement(this IWebDriver driver, By by, int timeoutInSeconds, bool allowDisabled)
        {
            try
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds)).Until(
                    d =>
                        {
                            var element = driver.FindElement(by);
                            if (element.Displayed && (element.Enabled || allowDisabled))
                            {
                                return element;
                            }
                            return null;
                        });
                return wait;
            }
            catch (WebDriverTimeoutException)
            {
                return null;
            }
        }

        public static ReadOnlyCollection<IWebElement> FindElements(this IWebDriver driver, By by, int timeoutInSeconds)
        {
            try
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds)).Until(
                    d =>
                        {
                            var elements = driver.FindElements(by);
                            if (elements.Any(e => e.Displayed) && elements.Any(e => e.Enabled))
                            {
                                return elements;
                            }

                            return null;
                        });
                return wait;
            }
            catch (WebDriverTimeoutException)
            {
                return new ReadOnlyCollection<IWebElement>(new IWebElement[0]);
            }
        }
    }
}