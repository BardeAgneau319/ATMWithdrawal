using ATMWithdrawal.Controler;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ATMWithdrawal
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {

        private void Application_Startup(object sender, StartupEventArgs e)
        {

            WithdrawalControler controler = new WithdrawalControler();
            // Create the startup window
            AmountSelectionView wnd = new AmountSelectionView(controler);
            // Show the window
            wnd.Show();
        }
    }
}
