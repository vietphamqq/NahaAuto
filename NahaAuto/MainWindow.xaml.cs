using NahaAuto.Code;
using System.Windows;

namespace NahaAuto
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var runner = new CreateGoogleAccountRunner();
            runner.DoTask(new GoogleAccountModel()
            {
                UserName = "DiepNV2016123",
                FirstName = "Diep",
                LastName = "NGuyen Van",
                BirthDay = new System.DateTime(1990, 11, 20),
                IsMale = true,
                Password = "@q1w2e3r4t5y6",
                PhoneNumer = "1677058577"
            });
        }
    }
}