using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using NahaAuto.Model;

namespace NahaAuto.ViewModel
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<GoogleAccountViewModel>();
        }

        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();
        public GoogleAccountViewModel GoogleAccountViewModel => ServiceLocator.Current.GetInstance<GoogleAccountViewModel>();

        public static void Cleanup()
        {
        }
    }
}