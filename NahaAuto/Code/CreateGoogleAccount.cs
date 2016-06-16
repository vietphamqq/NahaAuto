using OpenQA.Selenium;
using System;

namespace NahaAuto.Code
{
    public abstract class CreateGoogleAccount : AutoWebBase, ITaskRunner<GoogleAccountModel>
    {
        Action<Status> Notification;

        public void DoTask(GoogleAccountModel model)
        {
            Setup();
            GoToUrl(@"https://accounts.google.com/signup");
            MaximiseWindow();
            EnterTextIn(By.Id("FirstName"), model.FirstName);
            EnterTextIn(By.Id("LastName"), model.LastName);
            EnterTextIn(By.Id("GmailAddress"), model.UserName);

            var password = Guid.NewGuid().ToString("N").ToLower();
                                    
            EnterTextIn(By.Id("Passwd"), model.Password);
            EnterTextIn(By.Id("PasswdAgain"), model.Password);

            ChooseBirthDay(model.BirthDay);
            ChooseGender(model.IsMale);

            EnterTextIn(By.Id("RecoveryPhoneNumber"), model.PhoneNumer);

            EnterTextIn(By.Id("recaptcha_response_field"), "0177");

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