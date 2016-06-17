using OpenQA.Selenium;
using System;
using System.Threading.Tasks;

namespace NahaAuto.Code
{
    public abstract class CreateGoogleAccount : AutoWebBase, ITaskRunner<GoogleAccountModel>
    {
        private Action<Status> Notification;
        private Action<string> openCapchaWindow;
        private Func<string, string> extractTextFromImage;

        public bool IsEnterCapchaByHand { get; set; } = false;

        public void ActionIfCapchaByHand(Action<string> openCapchaWindow)
        {
            this.openCapchaWindow = openCapchaWindow;
        }

        public void CapchaParser(Func<string, string> extractTextFromImage)
        {
            this.extractTextFromImage = extractTextFromImage;
        }

        private TaskCompletionSource<string> completeParseCapcha;

        public void SetCapchaByHand(string value)
        {
            completeParseCapcha.SetResult(value);
        }

        public async Task DoTask(GoogleAccountModel model)
        {
            Setup();
            GoToUrl(@"https://accounts.google.com/signup");
            MaximiseWindow();
            EnterTextIn(By.Id("FirstName"), model.FirstName);
            EnterTextIn(By.Id("LastName"), model.LastName);
            EnterTextIn(By.Id("GmailAddress"), model.UserName);

            EnterTextIn(By.Id("Passwd"), model.Password);
            EnterTextIn(By.Id("PasswdAgain"), model.Password);

            ChooseBirthDay(model.BirthDay);
            ChooseGender(model.IsMale);

            EnterTextIn(By.Id("RecoveryPhoneNumber"), model.PhoneNumer);

            var textCapcha = string.Empty;

            if (!IsEnterCapchaByHand)
            {
                var tryTime = 0;
                while (string.IsNullOrEmpty(textCapcha) && tryTime++ < 100)
                {
                    if (tryTime > 1)
                    {
                        ClickOn(By.Id("recaptcha_reload_btn"));
                        Wait(1000);
                    }

                    textCapcha = extractTextFromImage.Invoke(GetAttributeFrom(By.Id("recaptcha_challenge_image"), "src"));
                }
            }
            else
            {
                openCapchaWindow?.Invoke(GetAttributeFrom(By.Id("recaptcha_image"), "src"));
                textCapcha = await completeParseCapcha.Task;
            }

            EnterTextIn(By.Id("recaptcha_response_field"), textCapcha);

            SelectCheckbox(By.Id("TermsOfService"));

            //ClickOn(By.Id("submitbutton"));

            //Close();
        }

        public void Process(Action<Status> notification)
        {
            Notification = notification;
        }

        private void ChooseBirthDay(DateTime birthDay)
        {
            ClickOn(By.Id("BirthMonth"));
            Wait(500);
            ClickOn(By.Id(":" + birthDay.Month.ToString("X").ToLower()));

            EnterTextIn(By.Id("BirthDay"), Convert.ToString(birthDay.Day));
            EnterTextIn(By.Id("BirthYear"), Convert.ToString(birthDay.Year));
        }

        private void ChooseGender(bool isMale)
        {
            ClickOn(By.Id("Gender"));
            Wait(500);
            ClickOn(By.Id(isMale ? ":f" : ":e"));
        }
    }
}