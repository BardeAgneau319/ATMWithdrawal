using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATMWithdrawal.Model
{
    class WtihdrawalModel
    {
        private OracleConnection db;
        public int AccountId { get; set; }
        public int ATMId { get; set; }
        public string CardId { get; set; }

        private const string HOST = "studentoracle.students.ittralee.ie";
        private const string USERID = "t00204454";
        private const string PASSWORD = "m5qrgpsd";
        private const string CONNECTIONSTRING = "Data Source=" + HOST + "; User Id=" + USERID + "; Password=" + PASSWORD + "; Integrated Security=no;";

        public WtihdrawalModel()
        {
            db = new OracleConnection(CONNECTIONSTRING);
        }

        public void ResetDB(int accountBalance)
        {
            // Reset the account balance and delete all the transaction performed with a card
            try
            {
                this.db.Open();

                // Reset the balance
                string query = "UPDATE ACCOUNT" +
                               "SET BALANCE=@balance" + 
                               "WHERE ACCOUNTID=@accountid";

                OracleCommand cmd = new OracleCommand(query, db);
                cmd.Parameters.Add("@balance", SqlDbType.Float).Value = accountBalance;
                cmd.Parameters.Add("@accountid", SqlDbType.Int).Value = this.AccountId;

                cmd.ExecuteNonQuery();

                // Delete the transactions 
                cmd.CommandText = "DELETE FROM TRANSACTION WHERE CARDNUMBER=@cardnumber";
                cmd.Parameters.Add("@balance", SqlDbType.VarChar).Value = this.CardId;
                cmd.ExecuteNonQuery();
            }
            catch (OracleException e)
            {

            }
            finally
            {
                db.Close();
            }
        }

        public void CreateTransaction(int amount)
        {
            // Create a new transaction in the DB (a trigger automaticly set the new account balance)
            try
            {
                this.db.Open();
                string query = "INSERT INTO TRANSACTION(TRANSACTIONID, TRANSACTIONTYPE, AMOUT) VALUES (TRANSACTIONID_seq.next, 'W', @amout)";

                OracleCommand cmd = new OracleCommand(query, db);
                cmd.Parameters.Add("@amout", SqlDbType.Int).Value = amount;

                cmd.ExecuteNonQuery();
            }
            catch (OracleException e)
            {

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
                               "SET E5NOTES=@e5notes, E10NOTES=@e10notes, E20NOTES=@e20notes, E50NOTES=@e50notes, E100NOTES=@e100notes, E200NOTES=@e200notes, E500NOTES=@e500notes" +
                               "WHERE ATMID=@atmid";

            
                OracleCommand cmd = db.CreateCommand();

                cmd.Parameters.Add("@e5notes", SqlDbType.Int).Value = e5notes;
                cmd.Parameters.Add("@e10notes", SqlDbType.Int).Value = e10notes;
                cmd.Parameters.Add("@e20notes", SqlDbType.Int).Value = e20notes;
                cmd.Parameters.Add("@e50notes", SqlDbType.Int).Value = e50notes;
                cmd.Parameters.Add("@e100notes", SqlDbType.Int).Value = e100notes;
                cmd.Parameters.Add("@e200notes", SqlDbType.Int).Value = e200notes;
                cmd.Parameters.Add("@e500notes", SqlDbType.Int).Value = e500notes;
                cmd.Parameters.Add("@atmid", SqlDbType.Int).Value = this.ATMId;

                cmd.ExecuteNonQuery();
            }
            catch(OracleException e)
            {

            }
            finally
            {
                db.Close();
            }


            // Update the number of notes in the ATM
        }

    }

   
}
