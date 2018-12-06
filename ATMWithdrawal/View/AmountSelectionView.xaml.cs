using ATMWithdrawal.Controler;
using ATMWithdrawal.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ATMWithdrawal
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class AmountSelectionView : Window
    {

        public WithdrawalControler controler;

        public ICommand AmountSelectionCommand { get; private set; }

        public AmountSelectionView()
        {
            InitializeComponent();
        }

        public AmountSelectionView(WithdrawalControler controler)
        {
            this.controler = controler;
            InitializeComponent();
        }

        private void Amount_Click(object sender, RoutedEventArgs e)
        {
            int amount;

            if(e20Button.Equals(sender))
            {
                amount = 20;
            }
            else if(e40Button.Equals(sender))
            {
                amount = 40;
            }
            else if (e60Button.Equals(sender))
            {
                amount = 60;
            }
            else if (e100Button.Equals(sender))
            {
                amount = 100;
            }
            else
            {
                amount = 200;
            }

            this.controler.Withdraw(amount);
        }

        private void Other_Amount_Click(object sender, RoutedEventArgs e)
        {
            OtherAmountView wnd = new OtherAmountView(controler);
            wnd.Show();
            this.Close();
        }
    }
}
