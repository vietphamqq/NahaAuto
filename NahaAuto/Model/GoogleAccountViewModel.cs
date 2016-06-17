using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NahaAuto.Code;
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

            CreateAccount = new RelayCommand(() =>
            {
                var runner = new CreateGoogleAccountRunner();
                runner.DoTask(CurrentItem);
            });

            LoadExcelAccount = new RelayCommand(() =>
            {
                using (var googleAccount = new ParseAccountExcel(ExcelAccountFile))
                {
                    var items = googleAccount.Load();

                    var errors =  googleAccount.Errors;
                    Errors.Clear();
                    foreach(var error in errors)
                    {
                        Errors.Add(error);
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

            CreateAllAccount = new RelayCommand(() => {

            });

            CreateRandom = new RelayCommand(() => {
                var filePath = Path.GetDirectoryName(ExcelAccountFile) + $@"\{DateTime.Now.ToString("yyyy-mm-dd HHMMss")}.xlsx";
                using (var createRandom = new CreateAccountExcel(ExcelAccountFile))
                {
                    createRandom.Save(filePath);
                }

                System.Diagnostics.Process.Start(filePath);
            });
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


        private string excelAccountFile = @"C:\Users\diepnguyenv\Desktop\projects\nah\NahaAuto\CreateAccount.xlsx";

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