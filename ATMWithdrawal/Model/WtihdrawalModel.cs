using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ATMWithdrawal.Model
{
    class WtihdrawalModel
    {
        private OracleConnection db;
        public int AccountId { get; set; }
        public int ATMId { get; set; }
        public string CardNumber { get; set; }

        private const string HOST = "oracle/orcl";
        private const string USERID = "t00204454";
        private const string PASSWORD = "m5qrgpsd";
        private const string CONNECTIONSTRING = "Data Source=" + HOST + "; User Id=" + USERID + "; Password=" + PASSWORD + ";";

        public WtihdrawalModel(int AccountID, int ATMID, string CardId)
        {
            db = new OracleConnection(CONNECTIONSTRING);
            //this.AccountId = AccountId;
            this.ATMId = ATMID;
            this.CardNumber = CardId;

            this.db.Open();
            string query = "SELECT ACCOUNTID FROM CARD WHERE CARDNUMBER=:cardnumber";
            OracleCommand cmd = new OracleCommand(query, db);
            cmd.Parameters.Add(":cardnumber", OracleDbType.Varchar2).Value = this.CardNumber;
            OracleDataReader dr = cmd.ExecuteReader();
            if(dr.Read())
            {
                this.AccountId = dr.GetInt32(0); ;
            }
            else
            {
                MessageBox.Show("Error while getting account id");
                this.ATMId = ATMID;
            }

            db.Close();

        }

        public void ResetDB(float accountBalance, int nbNotes)
        {
            // Reset the account balance and delete all the transactions performed with a card
            try
            {
                this.db.Open();

                // Reset the balance
                string query = "UPDATE ACCOUNT " +
                               "SET BALANCE=:balance " + 
                               "WHERE ACCOUNTID=:accountid";

                OracleCommand cmd = new OracleCommand(query, db);
                cmd.Parameters.Add(":balance", OracleDbType.Double).Value = accountBalance;
                cmd.Parameters.Add(":accountid", OracleDbType.Int32).Value = this.AccountId;

                cmd.ExecuteNonQuery();

                // Delete the transactions
                query = "DELETE FROM TRANSACTION WHERE CARDNUMBER=:cardnumber";
                cmd = new OracleCommand(query, db);
                cmd.Parameters.Add(":cardnumber", OracleDbType.Varchar2).Value = this.CardNumber;
                cmd.ExecuteNonQuery();

                // Set number of notes
                query =        "UPDATE ATM " +
                               "SET E5NOTES=:nbNotes, E10NOTES=:nbNotes, E20NOTES=:nbNotes, E50NOTES=:nbNotes, E100NOTES=:nbNotes, E200NOTES=:nbNotes, E500NOTES=:nbNotes " +
                               "WHERE ATMID=:atmid";
                cmd = new OracleCommand(query, db);
                cmd.Parameters.Add(":nbNotes", OracleDbType.Int32).Value = nbNotes;
                cmd.Parameters.Add(":atmid", OracleDbType.Int32).Value = this.ATMId;
            }
            catch (OracleException e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                db.Close();
            }
        }

        public void CreateTransaction(float amount)
        {
            // Create a new transaction in the DB (a trigger automaticly set the new account balance)
            try
            {
                this.db.Open();
                string query = "INSERT INTO TRANSACTION(TRANSACTIONID, TRANSACTIONTYPE, AMOUT) VALUES (TRANSACTIONID_seq.next, 'W', :amout)";

                OracleCommand cmd = new OracleCommand(query, db);
                cmd.Parameters.Add(":amout", OracleDbType.Double).Value = amount;

                cmd.ExecuteNonQuery();
            }
            catch (OracleException e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                db.Close();
            }
        }

        public void UpdateNotes(int e5notes, int e10notes, int e20notes, int e50notes, int e100notes, int e200notes, int e500notes)
        {
            try
            {
                this.db.Open();            
                string query = "UPDATE ATM" +
                               "SET E5NOTES=:e5notes, E10NOTES=:e10notes, E20NOTES=:e20notes, E50NOTES=:e50notes, E100NOTES=:e100notes, E200NOTES=:e200notes, E500NOTES=:e500notes" +
                               "WHERE ATMID=:atmid";

            
                OracleCommand cmd = db.CreateCommand();

                cmd.Parameters.Add(":e5notes", OracleDbType.Int32).Value = e5notes;
                cmd.Parameters.Add(":e10notes", OracleDbType.Int32).Value = e10notes;
                cmd.Parameters.Add(":e20notes", OracleDbType.Int32).Value = e20notes;
                cmd.Parameters.Add(":e50notes", OracleDbType.Int32).Value = e50notes;
                cmd.Parameters.Add(":e100notes", OracleDbType.Int32).Value = e100notes;
                cmd.Parameters.Add(":e200notes", OracleDbType.Int32).Value = e200notes;
                cmd.Parameters.Add(":e500notes", OracleDbType.Int32).Value = e500notes;
                cmd.Parameters.Add(":atmid", OracleDbType.Int32).Value = this.ATMId;

                cmd.ExecuteNonQuery();
            }
            catch(OracleException e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                db.Close();
            }


            // Update the number of notes in the ATM
        }

    }

   
}
