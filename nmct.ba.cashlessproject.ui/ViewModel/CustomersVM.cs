using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using nmct.ba.cashlessproject.classlibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace nmct.ba.cashlessproject.ui.ViewModel
{

    class CustomersVM : ObservableObject, IPage
    {
        public CustomersVM()
        {
            GetCustomers();
        }

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

        public string Name
        {
            get { return "Customers"; }
        }

        private ObservableCollection<Customer> _customers;
        public ObservableCollection<Customer> Customers
        {
            get { return _customers; }
            set { _customers = value; OnPropertyChanged("Customers"); }
        }

        private Customer _selectedCustomer;
        public Customer SelectedCustomer
        {
            get { return _selectedCustomer; }
            set
            {
                _selectedCustomer = value;
                OnPropertyChanged("SelectedCustomer");
                DeleteCustomerCommand.RaiseCanExecuteChanged();
                SaveCustomerCommand.RaiseCanExecuteChanged();
            }
        }

        private async void GetCustomers()
        {
            Console.WriteLine("Getting Customers");
            if (ApplicationVM.token == null) return;
;            using (HttpClient client = new HttpClient())
            {
                client.SetBearerToken(ApplicationVM.token.AccessToken);
                HttpResponseMessage response = await client.GetAsync("http://localhost:61505/api/Customer");
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    Customers = JsonConvert.DeserializeObject<ObservableCollection<Customer>>(json);
                }
                else
                {
                    Customers = null;
                }
            }
        }

        private bool IsCustomerSelected()
        {
            return SelectedCustomer != null;
        }


        public ICommand AddCustomerCommand
        {
            get { return new RelayCommand(AddCustomer); }
        }

        private void AddCustomer()
        {
            if (SelectedCustomer != null && SelectedCustomer.CustomerName == "<New Customer>") return;
            Customer NewCustomer = new Customer { ID = -1, CustomerName = "<New Customer>", Balance = 0 };
            Customers.Add(NewCustomer);
            SelectedCustomer = NewCustomer;
        }

        private RelayCommand _deleteCustomerCommand;
        public RelayCommand DeleteCustomerCommand
        {
            get
            {
                if (_deleteCustomerCommand == null)
                    _deleteCustomerCommand = new RelayCommand(DeleteCustomer, IsCustomerSelected);
                return _deleteCustomerCommand;
            }
        }


        private async void DeleteCustomer()
        {
            if (SelectedCustomer != null && SelectedCustomer.ID == -1) { Customers.Remove(SelectedCustomer); return; }

            using (HttpClient client = new HttpClient())
            {
                client.SetBearerToken(ApplicationVM.token.AccessToken);
                HttpResponseMessage response = await client.DeleteAsync("http://localhost:61505/api/Customer/" + SelectedCustomer.ID);
                if (response.IsSuccessStatusCode)
                {
                    Customers.Remove(SelectedCustomer);
                }

            }
        }

        private RelayCommand _saveCustomerCommand;
        
        public RelayCommand SaveCustomerCommand
        {
            get
            {
                if (_saveCustomerCommand == null)
                    _saveCustomerCommand = new RelayCommand(SaveCustomer, IsCustomerSelected);
                return _saveCustomerCommand;
            }
        }


        private async void SaveCustomer()
        {
            using (HttpClient client = new HttpClient())
            {
                client.SetBearerToken(ApplicationVM.token.AccessToken);
                string content = JsonConvert.SerializeObject(SelectedCustomer);
                if (SelectedCustomer.ID == -1)
                {
                    HttpResponseMessage response = await client.PostAsync("http://localhost:61505/api/Customer", new StringContent(content, Encoding.UTF8, "application/json"));
                }
                else
                {
                    HttpResponseMessage response = await client.PutAsync("http://localhost:61505/api/Customer", new StringContent(content, Encoding.UTF8, "application/json"));
                }
                GetCustomers();
            }
        }

    }
}
