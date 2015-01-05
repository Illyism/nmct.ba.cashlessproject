using be.belgium.eid;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using nmct.ba.cashlessproject.classlibrary;
using nmct.ba.cashlessproject.customer.Helper;
using nmct.ba.cashlessproject.customer.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Thinktecture.IdentityModel.Client;

namespace nmct.ba.cashlessproject.customer.ViewModel
{
    public class MainPageVM : ObservableObject, IPage
    {
        public MainPageVM()
        {
            CurrentCustomer = ApplicationVM.Customer; 
        }
        public MainPageVM(Customer customer)
        {
            CurrentCustomer = customer;
        }

        ApplicationVM appvm = App.Current.MainWindow.DataContext as ApplicationVM;

        public string Name
        {
            get { return "Main Page"; }
        }

        private string _error;
        public string Error
        {
            get { return _error; }
            set { _error = value; OnPropertyChanged("Error"); }
        }

        public ICommand LogOutCommand
        {
            get { return new RelayCommand(LogOut); }
        }
        public void LogOut()
        {
            appvm.LogOut();
        }

        private Customer _currentCustomer;
        public Customer CurrentCustomer
        {
            get
            {
                return _currentCustomer;
            }
            set
            {
                _currentCustomer = value;
                OnPropertyChanged("CurrentCustomer");
            }
        }

        private double _customerTopUpBalance;
        public double CustomerTopUpBalance
        {
            get
            {
                return _customerTopUpBalance;
            }
            set
            {
                _customerTopUpBalance = value;
                OnPropertyChanged("CustomerTopUpBalance");
                TopUpCommand.RaiseCanExecuteChanged();
                AddBankNoteCommand.RaiseCanExecuteChanged();
            }
        }
        private RelayCommand<double> _addBankNoteCommand;
        public RelayCommand<double> AddBankNoteCommand
        {
            get
            {
                if (_addBankNoteCommand == null)
                    _addBankNoteCommand = new RelayCommand<double>(AddBankNote, CheckBankNote);
                return _addBankNoteCommand;
            }
        }

        private bool CheckBankNote(double bank)
        {
            double newBalance = CustomerTopUpBalance + bank;
            return (newBalance <= 100 && newBalance >= 0);
        }

        private void AddBankNote(double bank)
        {
            CustomerTopUpBalance += bank;
        }

        private RelayCommand _topUpCommand;
        public RelayCommand TopUpCommand
        {
            get
            {
                if (_topUpCommand == null)
                    _topUpCommand = new RelayCommand(TopUp, AreBankNotesAdded);
                return _topUpCommand;
            }
        }
        

        private async void TopUp()
        {
            await SaveTopUp();
            CustomerTopUpBalance = 0;
            GetCustomer(CurrentCustomer);
        }

        private bool AreBankNotesAdded()
        {
            return CustomerTopUpBalance > 0;
        }

        private async Task SaveTopUp()
        {
            Customer customer = CurrentCustomer;
            customer.Balance = customer.Balance + CustomerTopUpBalance;
            using (HttpClient client = new HttpClient())
            {
                client.SetBearerToken(ApplicationVM.token.AccessToken);
                string content = JsonConvert.SerializeObject(customer);
                HttpResponseMessage response = await client.PutAsync("http://localhost:61505/api/Customer", new StringContent(content, Encoding.UTF8, "application/json"));
                if (!response.IsSuccessStatusCode) ApplicationVM.AddErrorLog("Failed to top up", response.ReasonPhrase);
            }
        }

        private async void GetCustomer(Customer customer)
        {
            Console.WriteLine("Getting Customer");
            if (ApplicationVM.token == null) return;
            using (HttpClient client = new HttpClient())
            {
                client.SetBearerToken(ApplicationVM.token.AccessToken);
                HttpResponseMessage response = await client.GetAsync("http://localhost:61505/api/Customer/" + customer.NationalNumber);
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    CurrentCustomer = JsonConvert.DeserializeObject<Customer>(json);
                    ApplicationVM.Customer = CurrentCustomer;
                }
                else
                {
                    Error = "No customer named " + customer.CustomerName;
                    ApplicationVM.AddErrorLog("No customer found", customer.NationalNumber);
                    CurrentCustomer = null;
                    ApplicationVM.Customer = null;
                }
            }
        }
    }
}
