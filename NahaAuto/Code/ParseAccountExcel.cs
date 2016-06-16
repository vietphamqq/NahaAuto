using System;
using System.Collections.Generic;

namespace NahaAuto.Code
{
    public class ParseAccountExcel : ParseExcelBase<GoogleAccountModel>
    {
        
        public ParseAccountExcel(string filePath) : base(filePath)
        {
            RowData = 4;
        }
        public override IEnumerable<string> DefinedMapping()
        {
            return new[] { "FirstName",
            "LastName",
            "UserName",
            "Password",
            "BirthDay",
            "IsMale",
            "PhoneNumber",
            "FacebookUserName" };
        }
        public override GoogleAccountModel Parse(int row)
        {
            try
            {
                var firstName = Convert.ToString(Worksheet[row, HeaderMapping["FirstName"]].Value2);
                var lastName = Convert.ToString(Worksheet[row, HeaderMapping["LastName"]].Value2);
                var userName = Convert.ToString(Worksheet[row, HeaderMapping["UserName"]].Value2);
                var password = Convert.ToString(Worksheet[row, HeaderMapping["Password"]].Value2);
                var isMale = Convert.ToBoolean(Worksheet[row, HeaderMapping["IsMale"]].Value2);
                var phoneNumber = Convert.ToString(Worksheet[row, HeaderMapping["PhoneNumber"]].Value2);
                var birthDay = Convert.ToDateTime(Worksheet[row, HeaderMapping["BirthDay"]].Value2);

                return new GoogleAccountModel
                {
                    FirstName = firstName,
                    LastName = lastName,
                    UserName = userName,
                    BirthDay = birthDay,
                    Password  = password,
                    IsMale  = isMale,
                    PhoneNumer  = phoneNumber
                };
            }
            catch(Exception ex)
            {
                return null;
            }
        }
    }
}