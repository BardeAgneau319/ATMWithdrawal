using ATMWithdrawal.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        }

    }
}
