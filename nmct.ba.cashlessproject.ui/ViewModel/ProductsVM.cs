using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using nmct.ba.cashlessproject.classlibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;

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

        private async void GetProducts()
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync("http://localhost:49321/api/Product");
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    Products = JsonConvert.DeserializeObject<ObservableCollection<Product>>(json);
                }
            }
        }

    }
}
