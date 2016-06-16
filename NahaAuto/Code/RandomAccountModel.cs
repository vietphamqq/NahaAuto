using System;

namespace NahaAuto.Code
{
    public class RandomAccountModel
    {
        public string FirstName { get; set; }
        public string MidName { get; set; }
        public string LastName { get; set; }

        public int FromYear { get; set; }

        public int ToYear { get; set; }

        public bool IsMale { get; set; }
    }
}