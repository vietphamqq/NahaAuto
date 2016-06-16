﻿using System.Collections.ObjectModel;
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

        public GoogleAccountViewModel()
        {
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
                    Accounts = new ObservableCollection<GoogleAccountModel>(items);
                }
            });

            CreateAllAccount = new RelayCommand(() => {
                
            });
        }

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


        private string excelAccountFile = @"C:\Users\diepnguyenv\Desktop\TemplateAccount.xlsx";

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