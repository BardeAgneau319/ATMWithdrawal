using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATMWithdrawal.Model
{
    class WtihdrawalModel
    {
        private OracleConnection db;
        public string AccountId { get; set; }
        public int ATMId { get; set; }

        private const string HOST = "studentoracle.students.ittralee.ie";
        private const string USERID = "t00204454";
        private const string PASSWORD = "m5qrgpsd";
        private const string CONNECTIONSTRING = "Data Source=" + HOST + "; User Id=" + USERID + "; Password=" + PASSWORD + "; Integrated Security=no;";

        public WtihdrawalModel()
        {
            db = new OracleConnection(CONNECTIONSTRING);
        }

        public void ResetAccount()
        {
            // Delete all the trasactions associated with the card
            // Set the account balance to 15000
        }

        public void CreateTransaction(int amount)
        {
            // Create a new transaction in the DB (a trigger automaticly set the new account balance
        }

        public void UpdateNotes(int e5notes, int e10notes, int e20notes, int e50notes, int e100notes, int e200notes, int e500notes)
        {
            // Update the number of notes in the ATM
        }

    }

   
}
