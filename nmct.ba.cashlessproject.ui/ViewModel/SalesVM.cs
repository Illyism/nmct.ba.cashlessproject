using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using nmct.ba.cashlessproject.classlibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace nmct.ba.cashlessproject.ui.ViewModel
{
    class SalesVM : ObservableObject, IPage
    {
        public SalesVM()
        {
            GetRegisters();
            GetProducts();
            GetSales();
            FilterTypes = new ObservableCollection<string> {"Disabled", "Register", "Product"};
        }

        public string Name
        {
            get { return "Sales"; }
        }

        private ObservableCollection<Sale> _obj;
        public ObservableCollection<Sale> Sales
        {
            get { return _obj; }
            set { _obj = value; OnPropertyChanged("Sales"); }
        }

        


        private Sale _selectedSale;
        public Sale SelectedSale
        {
            get { return _selectedSale; }
            set
            {
                _selectedSale = value;
                OnPropertyChanged("SelectedSale");
                FilterByRegisterCommand.RaiseCanExecuteChanged();
                FilterByProductCommand.RaiseCanExecuteChanged();
            }
        }

        private string _filterType;
        public string FilterType
        {
            get { return _filterType; }
            set
            {
                _filterType = value;
                OnPropertyChanged("FilterType");
                UndoFilterCommand.RaiseCanExecuteChanged();
                SetFilterValues();
            }
        }

        

        private IFilterableType _filterValue;
        public IFilterableType FilterValue
        {
            get { return _filterValue; }
            set
            {
                _filterValue = value;
                OnPropertyChanged("FilterValue");
                GetSales();
            }
        }

        private ObservableCollection<string> _filTypes;
        public ObservableCollection<string> FilterTypes
        {
            get { return _filTypes; }
            set { _filTypes = value; OnPropertyChanged("FilterTypes"); }
        }

        private ObservableCollection<IFilterableType> _filValues;
        public ObservableCollection<IFilterableType> FilterValues
        {
            get { return _filValues; }
            set { _filValues = value; OnPropertyChanged("FilterValues"); }
        }



        private bool IsSaleSelected()
        {
            return SelectedSale != null;
        }

        private bool IsFilterEnabled()
        {
            return !String.IsNullOrEmpty(FilterType) && FilterType != "Disabled";
        }


        private async void GetSales()
        {
            Console.WriteLine("Getting Sales");
            if (ApplicationVM.token == null) return;
            using (HttpClient client = new HttpClient())
            {
                client.SetBearerToken(ApplicationVM.token.AccessToken);
                HttpResponseMessage response;

                if (FilterType == "Register" && FilterValue != null)
                    response = await client.GetAsync("http://localhost:61505/api/Sale/Register/" + FilterValue.ID);
                else if (FilterType == "Product" && FilterValue != null)
                    response = await client.GetAsync("http://localhost:61505/api/Sale/Product/" + FilterValue.ID);
                else
                    response = await client.GetAsync("http://localhost:61505/api/Sale");

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    Sales = JsonConvert.DeserializeObject<ObservableCollection<Sale>>(json);
                }
                else
                {
                    Sales = null;
                }
            }
        }


        private void SetFilterValues()
        {
            if (FilterType == "Register")
            {
                FilterValues = new ObservableCollection<IFilterableType>(Registers);
            }
            else if (FilterType == "Product")
            {
                FilterValues = new ObservableCollection<IFilterableType>(Products);
            }
            else
            {
                FilterValues = null;
            }
        }

        private RelayCommand _filterByRegisterCommand;
        public RelayCommand FilterByRegisterCommand
        {
            get
            {
                if (_filterByRegisterCommand == null)
                    _filterByRegisterCommand = new RelayCommand(FilterByRegister, IsSaleSelected);
                return _filterByRegisterCommand;
            }
        }

        private void FilterByRegister()
        {
            FilterType = "Register";
            SetFilterValues();
            FilterValue = GetFilterableById(SelectedSale.RegisterID);
        }

        private RelayCommand _filterByProductCommand;
        public RelayCommand FilterByProductCommand
        {
            get
            {
                if (_filterByProductCommand == null)
                    _filterByProductCommand = new RelayCommand(FilterByProduct, IsSaleSelected);
                return _filterByProductCommand;
            }
        }

        private void FilterByProduct()
        {
            FilterType = "Product";
            SetFilterValues();
            FilterValue = GetFilterableById(SelectedSale.ProductID);
        }

        private RelayCommand _undoFilter;
        public RelayCommand UndoFilterCommand
        {
            get
            {
                if (_undoFilter == null)
                    _undoFilter = new RelayCommand(UndoFilter, IsFilterEnabled);
                return _undoFilter;
            }
        }

        private void UndoFilter()
        {
            FilterType = "Disabled";
            SetFilterValues();
            FilterValue = null;
        }

        private async void GetRegisters()
        {
            Console.WriteLine("Getting Registers");
            if (ApplicationVM.token == null) return;
            using (HttpClient client = new HttpClient())
            {
                client.SetBearerToken(ApplicationVM.token.AccessToken);
                HttpResponseMessage response = await client.GetAsync("http://localhost:61505/api/Register");
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    Registers = JsonConvert.DeserializeObject<IEnumerable<Register>>(json);
                }
                else
                {
                    Registers = null;
                }
            }
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
                    Products = JsonConvert.DeserializeObject<IEnumerable<Product>>(json);
                }
                else
                {
                    Products = null;
                }
            }
        }

        private IFilterableType GetFilterableById(int id)
        {
            return FilterValues.FirstOrDefault(filt => filt.ID == id);
        }

        private IEnumerable<IFilterableType> _reg;
        public IEnumerable<IFilterableType> Registers
        {
            get { return _reg; }
            set { _reg = value; OnPropertyChanged("Registers"); }
        }
        private IEnumerable<IFilterableType> _products;
        public IEnumerable<IFilterableType> Products
        {
            get { return _products; }
            set { _products = value; OnPropertyChanged("Products"); }
        }
    }
}
