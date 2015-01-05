using GalaSoft.MvvmLight.CommandWpf;
using nmct.ba.cashlessproject.ui.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace nmct.ba.cashlessproject.ui.ViewModel
{
    public class MenuVM : ObservableObject
    {
        public MenuVM()
        {

        }

        Window window = App.Current.MainWindow;
        ApplicationVM appvm = App.Current.MainWindow.DataContext as ApplicationVM;

        public ICommand ExitCommand
        {
            get { return new RelayCommand(Exit); }
        }
        public void Exit()
        {
            window.Close();
        }

        public ICommand LogOutCommand
        {
            get { return new RelayCommand(LogOut, isLoggedIn); }
        }
        public void LogOut()
        {
            appvm.LogOut();
        }
        public Boolean isLoggedIn()
        {
            return ApplicationVM.token != null;
        }


        public ICommand AccountCommand
        {
            get { return new RelayCommand(AccountOpen, isLoggedIn); }
        }

        private void AccountOpen()
        {
            appvm.AccountWindow = new AccountWindow();
            appvm.AccountWindow.Show();

        }

        public ICommand HelpCommand
        {
            get { return new RelayCommand(Help); }
        }

        private void Help()
        {
            System.Diagnostics.Process.Start("http://il.ly");
        }

        
    }
}
