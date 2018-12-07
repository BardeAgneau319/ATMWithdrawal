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
        private OracleConnection connection;
        public int AccountId { get; set; }
        public int ATMId { get; set; }
        public string CardNumber { get; set; }

        public int Nb5eNotes { get; private set; }
        public int Nb10eNotes { get; private set; }
        public int Nb20eNotes { get; private set; }
        public int Nb50eNotes { get; private set; }
        public int Nb100eNotes { get; private set; }
        public int Nb200eNotes { get; private set; }
        public int Nb500eNotes { get; private set; }

        private const string HOST = "oracle/orcl";
        private const string USERID = "t00204454";
        private const string PASSWORD = "m5qrgpsd";
        private const string CONNECTIONSTRING = "Data Source=" + HOST + "; User Id=" + USERID + "; Password=" + PASSWORD + ";";

        public WtihdrawalModel(int AccountID, int ATMID, string CardId)
        {
            connection = new OracleConnection(CONNECTIONSTRING);
            this.ATMId = ATMID;
            this.CardNumber = CardId;

            this.ResetDB(15000, 10);
            this.UpdateModelNotesNumber();

            try
            {
                this.connection.Open();
                string query = "SELECT ACCOUNTID FROM CARD WHERE CARDNUMBER=:cardnumber";
                OracleCommand cmd = new OracleCommand(query, connection);
                cmd.Parameters.Add(":cardnumber", OracleDbType.Varchar2).Value = this.CardNumber;
                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    this.AccountId = reader.GetInt32(0); ;
                }
                else
                {
                    MessageBox.Show("Error while getting account id");
                    this.ATMId = ATMID;
                }
                reader.Close();
            }
            catch (OracleException e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                connection.Close();
            }


        }

        public void ResetDB(float accountBalance, int nbNotes)
        {
            // Reset the account balance and delete all the transactions performed with a card
            try
            {
                this.connection.Open();

                // Reset the balance
                string query = "UPDATE ACCOUNT " +
                               "SET BALANCE=:balance " + 
                               "WHERE ACCOUNTID=:accountid";

                OracleCommand cmd = new OracleCommand(query, connection);
                cmd.Parameters.Add(":balance", OracleDbType.Double).Value = accountBalance;
                cmd.Parameters.Add(":accountid", OracleDbType.Int32).Value = this.AccountId;

                cmd.ExecuteNonQuery();

                // Delete the transactions
                query = "DELETE FROM TRANSACTION WHERE CARDNUMBER=:cardnumber";
                cmd = new OracleCommand(query, connection);
                cmd.Parameters.Add(":cardnumber", OracleDbType.Varchar2).Value = this.CardNumber;
                cmd.ExecuteNonQuery();

                // Set number of notes
                query =        "UPDATE ATM " +
                               "SET E5NOTES=:nbNotes, E10NOTES=:nbNotes, E20NOTES=:nbNotes, E50NOTES=:nbNotes, E100NOTES=:nbNotes, E200NOTES=:nbNotes, E500NOTES=:nbNotes " +
                               "WHERE ATMID=:atmid";
                cmd = new OracleCommand(query, connection);
                cmd.Parameters.Add(":nbNotes", OracleDbType.Int32).Value = nbNotes;
                cmd.Parameters.Add(":atmid", OracleDbType.Int32).Value = this.ATMId;
                UpdateModelNotesNumber();
            }
            catch (OracleException e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        // Create a new transaction in the DB and update the account amount and the number of notes
        public bool Withdraw(float amount, int usedE5Notes, int usedE10Notes, int usedE20Notes, int usedE50Notes, int usedE100Notes, int usedE200Notes, int usedE500Notes)
        {
            try
            {
                this.connection.Open();
            }
            catch (OracleException e)
            {
                MessageBox.Show(e.Message);
                return false;
            }

            // Use a transaction because the two following SQL commands must be executed to perform a correct withdrawal
            OracleTransaction transaction = connection.BeginTransaction();

            try
            {
                OracleCommand cmd = connection.CreateCommand();
                cmd.Transaction = transaction;

                cmd.CommandText = "UPDATE ATM" +
                                  "SET E5NOTES=E5NOTES-:e5notes, E10NOTES=E10NOTES-:e10notes, E20NOTES=E20NOTES-:e20notes, E50NOTES=E50NOTES-:e50notes, " + 
                                      "E100NOTES=E100NOTES-:e100notes, E200NOTES=E200NOTES-:e200notes, E500NOTES=E500NOTES-:e500notes" +
                                  "WHERE ATMID=:atmid";
                cmd.Parameters.Add(":e5notes", OracleDbType.Int32).Value = usedE5Notes;
                cmd.Parameters.Add(":e10notes", OracleDbType.Int32).Value = usedE10Notes;
                cmd.Parameters.Add(":e20notes", OracleDbType.Int32).Value = usedE20Notes;
                cmd.Parameters.Add(":e50notes", OracleDbType.Int32).Value = usedE50Notes;
                cmd.Parameters.Add(":e100notes", OracleDbType.Int32).Value = usedE100Notes;
                cmd.Parameters.Add(":e200notes", OracleDbType.Int32).Value = usedE200Notes;
                cmd.Parameters.Add(":e500notes", OracleDbType.Int32).Value = usedE500Notes;
                cmd.Parameters.Add(":atmid", OracleDbType.Int32).Value = this.ATMId;
                cmd.ExecuteNonQuery();

                // Create a new banque transaction in the DB (a trigger automaticly set the new account balance)
                cmd.Parameters.Clear();
                cmd.CommandText = "INSERT INTO TRANSACTION(TRANSACTIONID, TRANSACTIONTYPE, AMOUNT) VALUES (TRANSACTIONID_seq.next, 'W', :amout)";
                cmd.Parameters.Add(":amout", OracleDbType.Double).Value = amount;
                cmd.ExecuteNonQuery();

                transaction.Commit();
                UpdateModelNotesNumber();
            }
            catch (OracleException e)
            {
                transaction.Rollback();
                MessageBox.Show(e.Message);
                return false;
            }
            finally
            {
                connection.Close();
            }

            return true;
        }

        private void UpdateModelNotesNumber()
        {

            try
            {
                // Don't open the connection because it supposed to be already opened when the method is called
                OracleCommand command = this.connection.CreateCommand();
                string query = "SELECT E5NOTES, E10NOTES, E20NOTES, E50NOTES, E100NOTES, E200NOTES, E500NOTES FROM ATM WHERE ATMID=:atmid";
                OracleCommand cmd = new OracleCommand(query, connection);
                cmd.Parameters.Add(":atmid", OracleDbType.Int32).Value = this.ATMId;
                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    this.Nb5eNotes = reader.GetInt32(0);
                    this.Nb10eNotes = reader.GetInt32(1);
                    this.Nb20eNotes = reader.GetInt32(2);
                    this.Nb50eNotes = reader.GetInt32(3);
                    this.Nb100eNotes = reader.GetInt32(4);
                    this.Nb200eNotes = reader.GetInt32(5);
                    this.Nb500eNotes = reader.GetInt32(6);
                }
                else
                {
                    MessageBox.Show("Error while getting the nuber of notes");
                }
                reader.Close();
            }
            catch(OracleException e)
            {
                MessageBox.Show(e.Message);
            }

        }
    }

   
}
