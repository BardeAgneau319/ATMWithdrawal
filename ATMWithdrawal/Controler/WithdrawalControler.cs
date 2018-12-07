using ATMWithdrawal.Model;
using ATMWithdrawal.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ATMWithdrawal.Controler
{

    public class WithdrawalControler
    {
        WtihdrawalModel model;

        const int DEFAULT_ATMID = 1;
        const string DEFAULT_CARDNUMBER = "5530460764803213";
        const int DEFAULT_ACCOUNTID = 2;

        public WithdrawalControler()
        {
            model = new WtihdrawalModel(DEFAULT_ACCOUNTID, DEFAULT_ATMID, DEFAULT_CARDNUMBER);
            model.ResetDB(15000, 10);
        }


        public void Withdraw(int amount)
        {
            if(amount%5!=0)
            {
                MessageBox.Show("The transaction amount must be a multiple of 5");
                return;
            }

            int amountLeft = amount;
            int used500eNotes = ComputeNbNotes(ref amountLeft, this.model.Nb500eNotes, 500);
            int used200eNotes = ComputeNbNotes(ref amountLeft, this.model.Nb200eNotes, 200);
            int used100eNotes = ComputeNbNotes(ref amountLeft, this.model.Nb100eNotes, 100);
            int used50eNotes = ComputeNbNotes(ref amountLeft, this.model.Nb50eNotes, 50);
            int used20eNotes = ComputeNbNotes(ref amountLeft, this.model.Nb20eNotes, 20);
            int used10eNotes = ComputeNbNotes(ref amountLeft, this.model.Nb10eNotes, 10);
            int used5eNotes = ComputeNbNotes(ref amountLeft, this.model.Nb5eNotes, 5);
            
            if (amountLeft!=0)
            {
                MessageBox.Show("The ATM doesn't have enough notes to perform the transaction.");
                return;
            }

            if (model.Withdraw(amount, used5eNotes, used10eNotes, used20eNotes, used50eNotes, used100eNotes, used200eNotes, used500eNotes))
            {
                WithdrawalPerformedView window = new WithdrawalPerformedView();
                window.E5NotesTb.Text = used5eNotes.ToString();
                window.E10NotesTb.Text = used10eNotes.ToString();
                window.E20NotesTb.Text = used20eNotes.ToString();
                window.E50NotesTb.Text = used50eNotes.ToString();
                window.E100NotesTb.Text = used100eNotes.ToString();
                window.E200NotesTb.Text = used200eNotes.ToString();
                window.E500NotesTb.Text = used500eNotes.ToString();
                window.Show();
            }
        }

        private int ComputeNbNotes(ref int amount, int nbNotes, int noteValue)
        {
            int usedNotes = amount / noteValue;
            if(usedNotes>nbNotes)
            {
                usedNotes = nbNotes;
            }
            amount -= usedNotes * noteValue;
            return usedNotes;
        }

    }

}
