using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace nmct.ba.cashlessproject.ui.ViewModel
{
    
    public class AccountVM : ObservableObject
    {
        public ApplicationVM applicationVM
        {
            get
            {
                return App.Current.MainWindow.DataContext as ApplicationVM;
            }
            set
            {
                OnPropertyChanged("applicationVM");
            }
        }

        private string _newPassword;
        public string NewPassword
        {
            get { return _newPassword; }
            set {
                _newPassword = value;
                SubmitPasswordCommand.RaiseCanExecuteChanged();
                OnPropertyChanged("NewPassword");
            }
        }

        private bool _isSending;
        public bool IsSending
        {
            get { return _isSending; }
            set
            {
                _isSending = value;
                SubmitPasswordCommand.RaiseCanExecuteChanged();
                OnPropertyChanged("IsSending");
            }
        }

        private RelayCommand _submit;
        public RelayCommand SubmitPasswordCommand
        {
            get {
                if (_submit == null)
                    _submit = new RelayCommand(SubmitPassword, CanSubmitPassword);
                return _submit;
            }
        }

        public bool CanSubmitPassword()
        {
            return !String.IsNullOrWhiteSpace(NewPassword) && !IsSending;
        }

        public async void SubmitPassword()
        {
            Console.WriteLine("Submitting Password");
            IsSending = true;
            using (HttpClient client = new HttpClient())
            {
                client.SetBearerToken(ApplicationVM.token.AccessToken);
                string content = "{\"password\" : \"" + NewPassword + "\"}";
                HttpResponseMessage response = await client.PostAsync("http://localhost:61505/api/Organisation/Password/", new StringContent(content, Encoding.UTF8, "application/json"));
                IsSending = false;
            }
        }

        public ICommand LogOutCommand
        {
            get { return new RelayCommand<Window>(LogOut); }
        }
        public void LogOut(Window window)
        {
            applicationVM.LogOut();
            window.Close();
        }
    }
}
