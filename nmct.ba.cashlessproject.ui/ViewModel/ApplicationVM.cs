using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using nmct.ba.cashlessproject.ui.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Thinktecture.IdentityModel.Client;

namespace nmct.ba.cashlessproject.ui.ViewModel
{
    public class ApplicationVM : ObservableObject
    {
        public static TokenResponse token = null;
        public AccountWindow AccountWindow = null;

        public ApplicationVM()
        {
            CurrentPage = new LoginVM();
        }

        private ObservableCollection<IPage> _pages;
        public ObservableCollection<IPage> Pages
        {
            get {
                Console.WriteLine("Get Pages");
                if (_pages == null)
                    _pages = new ObservableCollection<IPage>();
                return _pages;
            }
        }

        private IPage _currentPage;
        public IPage CurrentPage
        {
            get { return _currentPage; }
            set { _currentPage = value; OnPropertyChanged("CurrentPage"); }
        }

        public ICommand ChangePageCommand
        {
            get { return new RelayCommand<IPage>(ChangePage);  }
        }

        public void ChangePage(IPage page)
        {
            CurrentPage = page;
        }

        public void LoggedIn()
        {
            Pages.Add(new CustomersVM());
            Pages.Add(new RegistersVM());
            Pages.Add(new EmployeesVM());
            Pages.Add(new ProductsVM());
            Pages.Add(new SalesVM());
            OnPropertyChanged("Pages");

            ChangePage(Pages[0]);
        }

        internal void LogOut()
        {
            ApplicationVM.token = null;
            Pages.Clear();
            OnPropertyChanged("Pages");

            ChangePage(new LoginVM());
        }

        
    }
}
