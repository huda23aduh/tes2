using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace tes2_huda
{
    class Huda_payment_class
    {
        

        public string get_request_to_api(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            try
            {
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    return reader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                WebResponse errorResponse = ex.Response;
                using (Stream responseStream = errorResponse.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
                    String errorText = reader.ReadToEnd();
                    // log errorText
                }
                throw;
            }
        }

        public string fetch_data_from_oracle(string transaction_code)
        {
            string cs = "User Id=mnc_subscribe;Password=mncsubsd3v;Data Source=192.168.177.102:1521/MNCSVDRC";
            string amount = null;
            OracleConnection conn = null;
            OracleTransaction transaction = null;
            
            try
            {
                conn = new OracleConnection(cs);
                conn.Open();
                transaction = conn.BeginTransaction();
                

                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = "SELECT * FROM fntxrecpay_test@igateway WHERE TRANSACTIONCODE = " + ":icc_cust_nbr";
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("icc_cust_nbr", transaction_code);
                cmd.ExecuteNonQuery();

                int c = count_data_in_oracle(transaction_code);
                
                Console.WriteLine("total row = " + c);

                OracleDataReader dr = cmd.ExecuteReader();
                
                while (dr.Read())
                {
                    int the_field_count = dr.FieldCount;
                    for(int a=0; a < dr.FieldCount; a++)
                    {
                        Console.WriteLine(dr.GetValue(a));
                        amount = dr.GetValue(2).ToString();
                    }

                    
                }

                //Console.Read();

                

            }
            catch (OracleException ex)
            {
                try
                {
                    transaction.Rollback();

                }
                catch (OracleException ex1)
                {
                    Console.WriteLine("Error: {0}", ex1.ToString());
                }

                Console.WriteLine("Error: {0}", ex.ToString());

            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
            return amount;
        }
        public int count_data_in_oracle(string cust_id_param)
        {
            string cs = "User Id=mnc_subscribe;Password=mncsubsd3v;Data Source=192.168.177.102:1521/MNCSVDRC";

            OracleConnection conn = null;
            OracleTransaction transaction = null;

            //int count = 0;
            int data_exist = 1;
            try
            {
                conn = new OracleConnection(cs);
                conn.Open();

                //OracleCommand command = new OracleCommand();
                //command.CommandText = "Select count(*) from custinq_idv_x where NOPEL = :SomeValue";
                //command.CommandType = CommandType.Text;



                var commandText = "Select count(*) from fntxrecpay_test@igateway where TRANSACTIONCODE = :SomeValue";

                using (OracleConnection connection = new OracleConnection(cs))
                using (OracleCommand command = new OracleCommand(commandText, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add("SomeValue", cust_id_param);
                    command.Connection.Open();




                    //command.ExecuteNonQuery();
                    object count = command.ExecuteScalar();


                    if (count.ToString() == "0") //kondisi jika blm ada data
                        data_exist = 0;

                    command.Connection.Close();

                }

            }
            catch (OracleException ex)
            {
                try
                {
                    transaction.Rollback();


                }
                catch (OracleException ex1)
                {
                    Console.WriteLine("Error: {0}", ex1.ToString());
                }

                Console.WriteLine("Error: {0}", ex.ToString());

            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }


            return data_exist;
        }
    }
}
