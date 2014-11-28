using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace nmct.ba.cashlessproject.ui.ViewModel
{
    public class ApplicationVM : ObservableObject
    {
        public ApplicationVM()
        {
            Pages.Add(new CustomersVM());
            Pages.Add(new EmployeesVM());
            Pages.Add(new ErrorLogsVM());
            Pages.Add(new OrganisationsVM());
            Pages.Add(new ProductsVM());
            Pages.Add(new RegistersVM());
            Pages.Add(new SalesVM());
        }

        private List<IPage> _pages;
        public List<IPage> Pages
        {
            get {
                if (_pages == null)
                    _pages = new List<IPage>();
                return _pages;
            }
        }

        private object _currentPage;

        public object CurrentPage
        {
            get { return _currentPage; }
            set { _currentPage = value; OnPropertyChanged("CurrentPage"); }
        }

        public ICommand ChangePageCommand
        {
            get { return new RelayCommand<IPage>(ChangePage);  }
        }

        private void ChangePage(IPage page)
        {
            CurrentPage = page;
        }
        
        
    }
}
