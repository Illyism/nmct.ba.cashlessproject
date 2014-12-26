using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using nmct.ba.cashlessproject.classlibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Windows.Input;

namespace nmct.ba.cashlessproject.ui.ViewModel
{
    class ProductsVM : ObservableObject, IPage
    {
        public ProductsVM()
        {
            GetProducts();
        }

        
        public string Name
        {
            get { return "Products"; }
        }

        private ObservableCollection<Product> _products;
        public ObservableCollection<Product> Products
        {
            get { return _products; }
            set { _products = value; OnPropertyChanged("Products"); }
        }

        private Product _selectedProduct;
        public Product SelectedProduct
        {
            get { return _selectedProduct; }
            set { 
                _selectedProduct = value; 
                OnPropertyChanged("SelectedProduct"); 
                DeleteProductCommand.RaiseCanExecuteChanged();
                SaveProductCommand.RaiseCanExecuteChanged();
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
                    Products = JsonConvert.DeserializeObject<ObservableCollection<Product>>(json);
                }
                else
                {
                    Products = null;
                }
            }
        }

        private bool IsProductSelected()
        {
            return SelectedProduct != null;
        }


        public ICommand AddProductCommand
        {
            get { return new RelayCommand(AddProduct); }
        }

        private void AddProduct()
        {
            if (SelectedProduct != null && SelectedProduct.ProductName == "<New Product>") return;
            Product NewProduct = new Product { ID = -1, ProductName = "<New Product>", Price = 0};
            Products.Add(NewProduct);
            SelectedProduct = NewProduct;
        }

        private RelayCommand _deleteProductCommand;
        public RelayCommand DeleteProductCommand
        {
            get
            {
                if (_deleteProductCommand == null)
                    _deleteProductCommand = new RelayCommand(DeleteProduct, IsProductSelected);
                return _deleteProductCommand;
            }
        }

        
        private async void DeleteProduct()
        {
            if (SelectedProduct != null && SelectedProduct.ID == -1) { Products.Remove(SelectedProduct); return; }

            using (HttpClient client = new HttpClient())
            {
                client.SetBearerToken(ApplicationVM.token.AccessToken);
                HttpResponseMessage response = await client.DeleteAsync("http://localhost:61505/api/Product/" + SelectedProduct.ID);
                if (response.IsSuccessStatusCode)
                {
                    Products.Remove(SelectedProduct);
                }

            }
        }

        private RelayCommand _saveProductCommand;
        public RelayCommand SaveProductCommand
        {
            get
            {
                if (_saveProductCommand == null)
                    _saveProductCommand = new RelayCommand(SaveProduct, IsProductSelected);
                return _saveProductCommand;
            }
        }


        private async void SaveProduct()
        {
            using (HttpClient client = new HttpClient())
            {
                client.SetBearerToken(ApplicationVM.token.AccessToken);
                string content = JsonConvert.SerializeObject(SelectedProduct);
                if (SelectedProduct.ID == -1)
                {
                    HttpResponseMessage response = await client.PostAsync("http://localhost:61505/api/Product", new StringContent(content, Encoding.UTF8, "application/json"));
                }
                else
                {
                    HttpResponseMessage response = await client.PutAsync("http://localhost:61505/api/Product", new StringContent(content, Encoding.UTF8, "application/json"));
                }
                GetProducts();
            }
        }

    }
}
