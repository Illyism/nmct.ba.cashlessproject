using be.belgium.eid;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using nmct.ba.cashlessproject.classlibrary;
using nmct.ba.cashlessproject.employee.Helper;
using nmct.ba.cashlessproject.employee.View;
using nmct.ba.cashlessproject.employee.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Thinktecture.IdentityModel.Client;

namespace nmct.ba.cashlessproject.employee.ViewModel
{
    public class MainPageVM : ObservableObject, IPage
    {
        public MainPageVM()
        {
            GetProducts();
        }

        ApplicationVM appvm = App.Current.MainWindow.DataContext as ApplicationVM;

        public string Name
        {
            get { return "Main Page";  }
        }

        public Employee CurrentEmployee
        {
            get { return ApplicationVM.Employee; }
        }

        private Customer _currentCustomer;
        public Customer CurrentCustomer
        {
            get { return _currentCustomer; }
            set
            {
                _currentCustomer = value;
                OnPropertyChanged("CurrentCustomer");
                ApplicationVM.Customer = CurrentCustomer;
                AuthenticateCustomerCommand.RaiseCanExecuteChanged();
                LogOutCustomerCommand.RaiseCanExecuteChanged();
                NewSaleCommand.RaiseCanExecuteChanged();
            }
        }

        private ObservableCollection<Sale> _sales;
        public ObservableCollection<Sale> Sales
        {
            get {
                if (_sales == null) _sales = new ObservableCollection<Sale>();
                return _sales;
            }
            set
            {
                _sales = value;
                OnPropertyChanged("Sales");
                SaveSalesCommand.RaiseCanExecuteChanged();
            }
        }


        public ICommand LogOutCommand
        {
            get { return new RelayCommand(LogOut); }
        }
        public void LogOut()
        {
            appvm.LogOut();
        }


        private bool IsNoCustomerAuthenticated()
        {
            return CurrentCustomer == null;
        }
        private bool IsCustomerAuthenticated()
        {
            return CurrentCustomer != null;
        }

        private RelayCommand _authenticateCustomerCommand;
        public RelayCommand AuthenticateCustomerCommand
        {
            get {
                if (_authenticateCustomerCommand == null)
                    _authenticateCustomerCommand = new RelayCommand(AuthenticateCustomer, IsNoCustomerAuthenticated); 
                return _authenticateCustomerCommand;}
        }
        private RelayCommand _logOutCustomerCommand;
        public RelayCommand LogOutCustomerCommand
        {
            get
            {
                if (_logOutCustomerCommand == null)
                    _logOutCustomerCommand = new RelayCommand(LogOutCustomer, IsCustomerAuthenticated);
                return _logOutCustomerCommand;
            }
        }
        private RelayCommand _newSaleCommand;
        public RelayCommand NewSaleCommand
        {
            get
            {
                if (_newSaleCommand == null)
                    _newSaleCommand = new RelayCommand(NewSale, IsCustomerAuthenticated);
                return _newSaleCommand;
            }
        }

        public List<SaleWindow> SaleWindows = new List<SaleWindow>();

        public void NewSale()
        {
            SaleWindow window = new SaleWindow();
            SaleVM saleVM = window.DataContext as SaleVM;
            saleVM.Parent = this;
            saleVM.Products = Products;
            saleVM.CustomerBalance = CurrentCustomer.Balance;
            window.Show();
            SaleWindows.Add(window);
        }

        public void LogOutCustomer()
        {
            CurrentCustomer = null;
            Sales = null;
        }

        public void AuthenticateCustomer()
        {
            Login();
        }

        private string _error;
        public string Error
        {
            get { return _error; }
            set { _error = value; OnPropertyChanged("Error"); }
        }

        private async void GetProducts()
        {
            Console.WriteLine("Getting Products");
            if (ApplicationVM.token == null) return;
            using (HttpClient client = new HttpClient())
            {
                client.SetBearerToken(ApplicationVM.token.AccessToken);
                HttpResponseMessage response = await client.GetAsync("http://localhost:61505/api/Product");
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    Products = JsonConvert.DeserializeObject<ObservableCollection<Product>>(json);
                }
                else
                {
                    ApplicationVM.AddErrorLog("No products", response.ReasonPhrase);
                    Products = null;
                }
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

                }
                else
                {
                    ApplicationVM.AddErrorLog("No customer", customer.CustomerName + " " + customer.NationalNumber);
                    Error = "No customer named " + customer.CustomerName;
                    CurrentCustomer = null;
                }
            }
        }

        private async void Login()
        {
            ApplicationVM appvm = App.Current.MainWindow.DataContext as ApplicationVM;
            if (!ApplicationVM.token.IsError)
            {
                try
                {
                    Customer customer = Identity.CustomerFromCardReader();
                    // Customer customer = new Customer { NationalNumber = "93071251560", CustomerName = "Ilias" };
                    GetCustomer(customer);
                    appvm.LoggedIn();
                }
                catch (BEID_ExNoReader ex)
                {
                    Error = "No Reader Detected";
                    ApplicationVM.AddErrorLog("No Reader Detected", ex.Message);
                    Console.WriteLine(ex.Message);
                }
                catch (BEID_ExNoCardPresent ex)
                {
                    Error = "No Card Detected";
                    ApplicationVM.AddErrorLog("No Card Detected", ex.Message);
                    Console.WriteLine(ex.Message);
                }
                catch (Exception ex)
                {
                    ApplicationVM.AddErrorLog("Reader error", ex.Message);
                    Error = ex.Message;
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {
                ApplicationVM.AddErrorLog("Token error", ApplicationVM.token.Error);
                Error = ApplicationVM.token.Error;
            }
        }

        public ObservableCollection<Product> Products { get; set; }

        public void AddSales(ObservableCollection<Sale> NewSales)
        {
            foreach (Sale s in NewSales)
                Sales.Add(s);
            SaveSalesCommand.RaiseCanExecuteChanged();
        }

        private bool IsSaleSelected()
        {
            return SelectedSale != null;
        }

        private RelayCommand _removeSaleCommand;
        public RelayCommand RemoveSaleCommand
        {
            get
            {
                if (_removeSaleCommand == null)
                    _removeSaleCommand = new RelayCommand(RemoveSale, IsSaleSelected);
                return _removeSaleCommand;
            }
        }
        private void RemoveSale()
        {
            Sales.Remove(SelectedSale);
        }

        private Sale _selectedSale;
        public Sale SelectedSale
        {
            get
            {
                return _selectedSale;
            }
            set
            {
                _selectedSale = value;
                OnPropertyChanged("SelectedSale");
                RemoveSaleCommand.RaiseCanExecuteChanged();
            }
        }


        private RelayCommand _saveSalesCommand;
        public RelayCommand SaveSalesCommand
        {
            get
            {
                if (_saveSalesCommand == null)
                    _saveSalesCommand = new RelayCommand(SaveSales, AreSalesAdded);
                return _saveSalesCommand;
            }
        }

        private async void SaveSales()
        {
            double finalBalance = CurrentCustomer.Balance;

            foreach (Sale sale in Sales)
                finalBalance -= sale.TotalPrice;
            if (finalBalance < 0)
            {
                ApplicationVM.AddErrorLog("Total price exceeds customer balance", finalBalance.ToString());
                if (MessageBox.Show("Total price exceeds customer balance. Proceed?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.No)
                {
                    return;
                }
            }
            
            foreach (Sale sale in Sales)
                await SaveSale(sale);

            Sales = null;
            GetCustomer(CurrentCustomer);
            
        }

        private bool AreSalesAdded()
        {
            return Sales != null && Sales.Count > 0;
        }

        private async Task SaveSale(Sale sale)
        {
            using (HttpClient client = new HttpClient())
            {
                client.SetBearerToken(ApplicationVM.token.AccessToken);
                string content = JsonConvert.SerializeObject(sale);
                HttpResponseMessage response = await client.PostAsync("http://localhost:61505/api/Sale", new StringContent(content, Encoding.UTF8, "application/json"));
                if (!response.IsSuccessStatusCode) ApplicationVM.AddErrorLog("Failed saving sales", response.ReasonPhrase);
            }
        }
    }
}
