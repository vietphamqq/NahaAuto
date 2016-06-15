using System;
using OpenQA.Selenium.Chrome;

namespace NahaAuto.Code
{
    public class CreateGoogleAccountRunner : CreateGoogleAccount
    {
        public override void Setup()
        {
            // The options below disable Chrome developer extensions, preventing random alert dialogues regarding Chrome developer mode
            // Source: http://stackoverflow.com/questions/23087724/chromedriver-disable-developer-mode-extensions-on-automation
            var options = new ChromeOptions();
            //options.AddArguments("--disable-extensions");

            Driver = new ChromeDriver(options);
        }
    }
}