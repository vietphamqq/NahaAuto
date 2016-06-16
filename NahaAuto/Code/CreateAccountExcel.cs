using System;
using System.Collections.Generic;
using Syncfusion.XlsIO;
using System.Linq;
using Humanizer;

namespace NahaAuto.Code
{
    public class CreateAccountExcel : ParseExcelBase<List<RandomAccountModel>>
    {
        public CreateAccountExcel(string filePath) : base(filePath)
        {
            RowData = 4;
        }

        public override IWorksheet SelectWorkSheet()
        {
            return Workbook.Worksheets[1];
        }

        public void Save(string filePath)
        {
            if (ExcelEngine == null)
                return;

            var items = CreateRadom();

            var workSheet = Workbook.Worksheets[0];

            var rowIndex = 4;
            foreach(var item in items)
            {
                workSheet[rowIndex, 1].Value2 = item.FirstName;
                workSheet[rowIndex, 2].Value2 = item.LastName;
                workSheet[rowIndex, 3].Value2 = item.UserName;
                workSheet[rowIndex, 4].Value2 = item.Password;
                workSheet[rowIndex, 5].Value2 = item.BirthDay;
                workSheet[rowIndex, 6].Value2 = item.IsMale.ToString().ToUpper();
                workSheet[rowIndex, 7].Value2 = item.PhoneNumer;
                rowIndex++;
            }

            Workbook.SaveAs(filePath);
        }

        public List<GoogleAccountModel> CreateRadom()
        {
            var items = Load()?.SelectMany(x => x);
            if ((items?.Count() ?? 0) == 0)
                return null;

            var itemsResult = new List<GoogleAccountModel>();
            var maleAccounts = FilterAccount(items?.Where(x => x.IsMale),true);
            var femaleAccounts = FilterAccount(items?.Where(x => !x.IsMale), false);

            itemsResult.AddRange(maleAccounts);
            itemsResult.AddRange(femaleAccounts);

            return itemsResult;
        }

        private List<GoogleAccountModel> FilterAccount(IEnumerable<RandomAccountModel> accounts, bool isMale)
        {
            if (accounts == null)
                return null;

            var firstName = accounts.Select(x => x.FirstName).Where(x => !string.IsNullOrWhiteSpace(x));
            var lastName = accounts.Select(x => x.LastName).Where(x => !string.IsNullOrWhiteSpace(x));
            var midName = accounts.Select(x => x.MidName).Where(x => !string.IsNullOrWhiteSpace(x));
            var from = accounts.Where(x=>x.FromYear > 0).Min(x => x.FromYear);
            var to = accounts.Where(x => x.ToYear > 0).Max(x => x.ToYear);

            var maleAccounts = CreateAccount(isMale, firstName, midName, lastName, from, to);

            return maleAccounts;
        }

        private List<GoogleAccountModel> CreateAccount(bool isMale, IEnumerable<string> fNames, IEnumerable<string> mNames, IEnumerable<string> lNames, int fromYear, int toYear)
        {
            return (from fname in fNames
                    from lName in lNames.Where(x=>x!=fname)
                    from midName in mNames.Where(x => x != fname && x!=lName)

                    let birthDay = new DateTime(new Random().Next(fromYear, toYear), new Random().Next(1, 12), new Random().Next(1, 28))
                    select new GoogleAccountModel
                    {
                        FirstName = fname.Humanize(LetterCasing.Title),
                        LastName = midName.Humanize(LetterCasing.Title) + " " + lName.Humanize(LetterCasing.Title),
                        UserName = $@"{fname.ToLower()}.{lName}{midName}.{birthDay.ToString("yyMMdd")}{fname.First()}{midName.First()}{lName.First()}".ToLower().Replace(" ", ""),
                        BirthDay = birthDay,
                        IsMale = isMale,
                        Password = Guid.NewGuid().ToString("N").ToLower().Substring(12),
                        PhoneNumer = "1677058578"
                    }).ToList();
        }

        public override IEnumerable<string> DefinedMapping()
        {
            return new[] 
            {
                "MaleFirstName",
                "MaleMidName",
                "MaleLastName",
                "MaleFromDate",
                "MaleToDate",
                "FemaleFirstName",
                "FemaleMidName",
                "FemaleLastName",
                "FemaleFromDate",
                "FemaleToDate"
            };
        }
        public override List<RandomAccountModel> Parse(int row)
        {
            Func<string, object> Get = (header) =>
            {
                var data = Worksheet[row, HeaderMapping[header]].Value2;
                if (data is string && string.IsNullOrEmpty(data as string))
                {
                    return null;
                }

                return data;
            };

            try
            {
                var maleFirstName = Convert.ToString(Get("MaleFirstName") ?? string.Empty);
                var maleMidName = Convert.ToString(Get("MaleMidName") ?? string.Empty);
                var maleLastName = Convert.ToString(Get("MaleLastName") ?? string.Empty);
                var maleFromDate = Convert.ToInt32(Get("MaleFromDate") ?? 0);
                var maleToDate = Convert.ToInt32(Get("MaleToDate") ?? 0);

                var femaleFirstName = Convert.ToString(Get("FemaleFirstName") ?? string.Empty);
                var femaleMidName = Convert.ToString(Get("FemaleMidName") ?? string.Empty);
                var femaleLastName = Convert.ToString(Get("FemaleLastName") ?? string.Empty);
                var femaleFromDate = Convert.ToInt32(Get("FemaleFromDate") ?? 0);
                var femaleToDate = Convert.ToInt32(Get("FemaleToDate") ?? 0);

                return new List<RandomAccountModel>
                {
                    new RandomAccountModel
                    {
                        FirstName = maleFirstName,
                        MidName = maleMidName,
                        LastName = maleLastName,
                        FromYear = maleFromDate,
                        ToYear = maleToDate ,
                        IsMale  = true
                    },
                    new RandomAccountModel
                    {
                        FirstName = femaleFirstName,
                        MidName = femaleMidName,
                        LastName = femaleLastName,
                        FromYear = femaleFromDate,
                        ToYear = femaleToDate ,
                        IsMale  = false
                    },
                };
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}