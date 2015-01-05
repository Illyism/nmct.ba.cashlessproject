using GalaSoft.MvvmLight.Command;
using nmct.ba.cashlessproject.classlibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Thinktecture.IdentityModel.Client;

namespace nmct.ba.cashlessproject.ui.ViewModel
{
    public class LoginVM : ObservableObject, IPage
    {
        public string Name
        {
            get { return "LoginPage";  }
        }

        private string _username;

        public string Username
        {
            get { return _username; }
            set { _username = value; OnPropertyChanged("Username"); }
        }

        private string _password;

        public string Password
        {
            get { return _password; }
            set { _password = value; OnPropertyChanged("Password");  }
        }

        private string _error;

        public string Error
        {
            get { return _error; }
            set { _error = value; OnPropertyChanged("Error"); }
        }

        public ICommand LoginCommand
        {
            get { return new RelayCommand(Login); }
        }

        private void Login()
        {
            ApplicationVM appvm = App.Current.MainWindow.DataContext as ApplicationVM;
            ApplicationVM.token = GetToken();

            if (!ApplicationVM.token.IsError)
            {
                appvm.LoggedIn();
            }
            else
            {
                Error = "Bad username or password"; //ApplicationVM.token.Error;
            }
        }

        private TokenResponse GetToken()
        {
            OAuth2Client client = new OAuth2Client(new Uri("http://localhost:61505/token"));
            return client.RequestResourceOwnerPasswordAsync(Username, Password).Result;
        }
        
    }
}
