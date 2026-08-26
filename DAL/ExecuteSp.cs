using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;


namespace AppointmentBooking.DAL
{

    // command , queries , validation
    public class ExecuteSp
    {

        public static int ExecuteNonQuery(string connection, string spname, Dictionary<string, object> param = null)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[connection].ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(spname, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        if (param != null)
                        {
                            foreach (var i in param)
                            {
                                cmd.Parameters.AddWithValue(i.Key, i.Value);
                            }

                        }

                        con.Open();

                        return cmd.ExecuteNonQuery();

                    }
                }
            }
            catch (SqlException e)
            {
                if (e.Number == 50001)
                {
                    return -1;
                }
            }

            return 0;
            


        }

        public static DataTable ExecuteSelectBy(string connection, string spname, Dictionary<string, object> param = null)
        {

            DataTable dt = new DataTable();
            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[connection].ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(spname, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        if (param != null)
                        {
                            foreach (var i in param)
                            {
                                cmd.Parameters.AddWithValue(i.Key, i.Value);
                            }
                        }
                        SqlDataAdapter da = new SqlDataAdapter(cmd);

                        da.Fill(dt);

                        return dt;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                return null;
            }


        }



    }
}
