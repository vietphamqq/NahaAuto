using System;

namespace NahaAuto.Code
{
    public sealed class GoogleAccountModel
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public DateTime BirthDay { get; set; }

        public bool IsMale { get; set; }

        public string PhoneNumer { get; set; }

        public override string ToString()
        {
            var genderString = IsMale ? "Male" : "Female";

            return $"{FirstName } {LastName} - {genderString}";
        }
    }
}