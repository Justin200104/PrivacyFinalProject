using PrivacyFinalProject.Services;
using System.Windows.Input;
using PrivacyFinalProject.Commands;


namespace PrivacyFinalProject.ViewModel
{
    internal class LoginViewModel : ViewModelBase
    {
        public ICommand NavigateLoginCommand { get; }

        public LoginViewModel(INavigationService loginNavigationService)
        {
            NavigateLoginCommand = new NavigateCommand(loginNavigationService);
        }
    }
}
