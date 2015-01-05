using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using nmct.ba.cashlessproject.classlibrary;
using nmct.ba.cashlessproject.employee.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace nmct.ba.cashlessproject.employee.ViewModel
{
    public class SaleVM : ObservableObject
    {
        public MainPageVM Parent { get; set; }

        private double _customerBalance;
        public double CustomerBalance
        {
            get
            {
                return _customerBalance;
            }
            set
            {
                _customerBalance = value;
                OnPropertyChanged("CustomerBalance");
                SaveSalesCommand.RaiseCanExecuteChanged();
            }
        }

        public ObservableCollection<Product> _products;
        public ObservableCollection<Product> Products {
            get
            {
                return _products;
            }
            set
            {
                _products = value;
                OnPropertyChanged("Products");
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

        private double _totalPrice;
        public double TotalPrice
        {
            get
            {
                return _totalPrice;
            }
            set
            {
                _totalPrice = value;
                OnPropertyChanged("TotalPrice");
                CustomerBalance = Parent.CurrentCustomer.Balance - TotalPrice;
            }
        }
        

        public ICommand CancelCommand
        {
            get { return new RelayCommand<Window>(Cancel); }
        }
        public void Cancel(Window window)
        {
            window.Close();
        }

        

        private Product _selectedProduct;
        public Product SelectedProduct
        {
            get
            {
                return _selectedProduct;
            }
            set
            {
                _selectedProduct = value;
                OnPropertyChanged("SelectedProduct");
                AddSaleCommand.RaiseCanExecuteChanged();
            }
        }
        private RelayCommand _addSaleCommand;
        public RelayCommand AddSaleCommand
        {
            get
            {
                if (_addSaleCommand == null)
                    _addSaleCommand = new RelayCommand(EditOrAddSale, IsProductSelected);
                return _addSaleCommand;
            }
        }

        private bool IsProductSelected()
        {
            return SelectedProduct != null;
        }
        
        public void EditOrAddSale()
        {
            Sale sale = null;
            if (Sales != null)
                sale = Sales.FirstOrDefault(s => s.ProductID == SelectedProduct.ID);
            if (sale == null)
            {
                sale = new Sale
                {
                    ProductName = SelectedProduct.ProductName,
                    ProductID = SelectedProduct.ID,
                    RegisterID = ApplicationVM.Register.ID,
                    RegisterName = ApplicationVM.Register.RegisterName,
                    CustomerID = ApplicationVM.Customer.ID,
                    CustomerName = ApplicationVM.Customer.CustomerName,
                    ID = -1,
                    Amount = 1,
                    TotalPrice = SelectedProduct.Price * 1,
                    Timestamp = DateTime.Now
                };
                Sales.Add(sale);
            } else {
                sale.Amount++;
                sale.TotalPrice = SelectedProduct.Price * sale.Amount;
                Sales.Remove(sale);
                Sales.Add(sale);
            }
            TotalPrice = Sales.Sum(s => s.TotalPrice);
            SaveSalesCommand.RaiseCanExecuteChanged();
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
                IncrAmountCommand.RaiseCanExecuteChanged();
                DecrAmountCommand.RaiseCanExecuteChanged();
            }
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
            TotalPrice = Sales.Sum(s => s.TotalPrice);
            SaveSalesCommand.RaiseCanExecuteChanged();
        }

        private RelayCommand _incrAmountCommand;
        public RelayCommand IncrAmountCommand
        {
            get
            {
                if (_incrAmountCommand == null)
                    _incrAmountCommand = new RelayCommand(IncrAmountSale, IsSaleSelected);
                return _incrAmountCommand;
            }
        }
        private void IncrAmountSale()
        {
            Sale sale = SelectedSale;
            sale.Amount++;
            sale.TotalPrice = (Products.First(p => p.ID == sale.ProductID)).Price * sale.Amount;
            TotalPrice = Sales.Sum(s => s.TotalPrice);
            Sales.Remove(SelectedSale);
            Sales.Add(sale);
        }

        private RelayCommand _decrAmountCommand;
        public RelayCommand DecrAmountCommand
        {
            get
            {
                if (_decrAmountCommand == null)
                    _decrAmountCommand = new RelayCommand(DecrAmountSale, IsSaleSelected);
                return _decrAmountCommand;
            }
        }
        private void DecrAmountSale()
        {
            Sale sale = SelectedSale;
            sale.Amount--;
            sale.TotalPrice = (Products.First(p => p.ID == sale.ProductID)).Price * sale.Amount;
            TotalPrice = Sales.Sum(s => s.TotalPrice);
            Sales.Remove(SelectedSale);
            if(sale.Amount>0) Sales.Add(sale);
            SaveSalesCommand.RaiseCanExecuteChanged();
        }

        private RelayCommand<Window> _saveSalesCommand;
        public RelayCommand<Window> SaveSalesCommand
        {
            get
            {
                if (_saveSalesCommand == null)
                    _saveSalesCommand = new RelayCommand<Window>(SaveSales, CanSaveSales);
                return _saveSalesCommand;
            }
        }

        private void SaveSales(Window window)
        {
            Parent.AddSales(Sales);
            window.Close();
        }

        private bool CanSaveSales(Window window)
        {
            return Sales != null && Sales.Count > 0 && _customerBalance > 0;
        }

        
    }
}
