using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NahaAuto.Code;
using Tesseract;

namespace NahaAuto.Model
{
    public class GoogleAccountViewModel : ViewModelBase
    {
        public ICommand CreateAccount { get; }

        public ICommand LoadExcelAccount { get; }

        public ICommand CreateAllAccount { get; }

        public ICommand CreateRandom { get; }

        public GoogleAccountViewModel()
        {
            Errors = new ObservableCollection<string>();

            CreateAccount = new RelayCommand(async () =>
            {
                var runner = new CreateGoogleAccountRunner();
                runner.CapchaParser(ExtractTextFromImage);
                await runner.DoTask(CurrentItem);
            });

            LoadExcelAccount = new RelayCommand(() =>
            {
                using (var googleAccount = new ParseAccountExcel(ExcelAccountFile))
                {
                    var items = googleAccount.Load();

                    var errors = googleAccount.Errors;
                    Errors.Clear();
                    if (errors != null)
                    {
                        foreach (var error in errors)
                        {
                            Errors.Add(error);
                        }
                    }

                    if (items == null)
                    {
                        Accounts?.Clear();
                    }
                    else
                    {
                        Accounts = new ObservableCollection<GoogleAccountModel>(items);
                    }
                }
            });

            CreateAllAccount = new RelayCommand(() =>
            {
            });

            CreateRandom = new RelayCommand(() =>
            {
                var filePath = Path.GetDirectoryName(ExcelAccountFile) + $@"\{DateTime.Now.ToString("yyyy-mm-dd HHMMss")}.xlsx";
                using (var createRandom = new CreateAccountExcel(ExcelAccountFile))
                {
                    createRandom.Save(filePath);
                }

                System.Diagnostics.Process.Start(filePath);
            });
        }

        public string ExtractTextFromImage(string url)
        {
            var regex = new Regex(@"\d+");

            new WebClient().DownloadFile(url, @"./capcha_cache.jpg");
            try
            {
                using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))
                {
                    using (var img = Pix.LoadFromFile(@"./capcha_cache.jpg"))
                    {
                        using (var page = engine.Process(img))
                        {
                            var text = page.GetText();

                            var match = regex.Match(text ?? "");
                            if (match.Success && match.Value.Length >= 3)
                            {
                                return match.Value;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected Error: " + e.Message);
                Console.WriteLine("Details: ");
                Console.WriteLine(e.ToString());
            }

            return string.Empty;
        }

        public ObservableCollection<string> Errors { get; }

        public ObservableCollection<GoogleAccountModel> accounts;

        public ObservableCollection<GoogleAccountModel> Accounts
        {
            get
            {
                return accounts;
            }
            set
            {
                Set(ref accounts, value);
            }
        }

        private string excelAccountFile = @"C:\Users\diepnguyenv\Desktop\projects\nah\NahaAuto\Template\CreateAccount.xlsx";

        public string ExcelAccountFile
        {
            get { return excelAccountFile; }
            set { Set(ref excelAccountFile, value); }
        }

        private GoogleAccountModel currentItem;

        public GoogleAccountModel CurrentItem
        {
            get { return currentItem; }
            set { Set(ref currentItem, value); }
        }

        private Status currentStatus;

        private Status CurrentStatus
        {
            get { return currentStatus; }
            set { Set(ref currentStatus, value); }
        }
    }
}