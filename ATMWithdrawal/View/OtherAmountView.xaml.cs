using ATMWithdrawal.Controler;
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
using System.Windows.Shapes;

namespace ATMWithdrawal.View
{
    /// <summary>
    /// Logique d'interaction pour Window1.xaml
    /// </summary>
    public partial class OtherAmountView : Window
    {
        public WithdrawalControler controler;

        public OtherAmountView()
        {
            InitializeComponent();
        }

        public OtherAmountView(WithdrawalControler controler)
        {
            this.controler = controler;
            InitializeComponent();
        }

        private void Cancel_click(object sender, RoutedEventArgs e)
        {
            new AmountSelectionView(this.controler);
            this.Close();
        }

        private void Ok_click(object sender, RoutedEventArgs e)
        {
            if(this.AmountTB.Text != "")
            {
                new AmountSelectionView().Show();
                this.Close();
                this.controler.Withdraw(Int32.Parse(this.AmountTB.Text));
            }
        }


        // Check that text box content is digit only
        // Source: https://social.msdn.microsoft.com/Forums/Azure/en-US/3052dea6-2801-4c7f-9d29-a9af91c92aef/wpf-textbox-only-enter-numbers-using-previewkeydown-ekey-but-keynumpad-can-only-be-entered-in?forum=wpf
        private void AmountTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            //Get the location of the current cursor click and save it

            TextChange[] changes = new TextChange[e.Changes.Count];
            e.Changes.CopyTo(changes, 0);

            //Capture the current mouse or pointer to the location of the variable
            int offset = changes[0].Offset;
            int totalCount = changes[0].AddedLength;

            //How many characters are currently added in total, starting at 0 or more
            if (totalCount > 0)
            {
                //Copy the entire string to the current string to the temporary area
                char[] charArray = new char[textBox.Text.Length];
                textBox.Text.CopyTo(0, charArray, 0, charArray.Length);
                StringBuilder sbu = new StringBuilder(new string(charArray));

                //From the increase that variable position began to detect whether the number, if not directly kill the current characters
                for (int i = sbu.Length - 1; i >= offset; --i)
                {
                    if (charArray[i] < 48 || charArray[i] > 57)
                    {
                        sbu = sbu.Remove(i, 1);
                    }
                }
                //Because the direct operation of the Text, so only this, otherwise call AppendText and other ways will lead to death cycle（TextBoxChanged）
                textBox.Text = sbu.ToString();
                //The current position + the total input character length (the cursor is positioned later)
                textBox.Select(offset + totalCount, 0);
            }
        }
    }
}
