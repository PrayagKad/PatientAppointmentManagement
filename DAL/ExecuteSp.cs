
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

        public static int ExecuteNonQuery(string connection, string spname, Dictionary<string, object> param = null , DataTable breakTable = null)
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
                         // for doctor breaks only , 
                        if (breakTable != null)
                        {
                            SqlParameter breakParameter =
                                cmd.Parameters.AddWithValue(
                                    "@Breaks",
                                    breakTable);  //  @Breaks is not a normal parameter, t is a table containing multiple rows of type dbo.DoctorBreakType.

                            breakParameter.SqlDbType =
                                SqlDbType.Structured;

                            breakParameter.TypeName =
                                "dbo.DoctorBreakType";
                        }

                        con.Open();

                        // Get RETURN value from stored procedure
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
                string msg = e.ToString();

            }
            catch(Exception e)
            {
                Console.Out.WriteLine(e.ToString());
                return - 100;

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
                                cmd.Parameters.AddWithValue(i.Key, i.Value ?? DBNull.Value);
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


        // for sps that return errorcodes
        public static int ExecuteNonQueryErrorCode(string connection, string spname, Dictionary<string, object> param = null, DataTable dateTable =null , DataTable breakTable = null)
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

                        if (dateTable != null)
                        {
                            SqlParameter dateParameter =
                                cmd.Parameters.AddWithValue(
                                    "@ScheduleDates",
                                    dateTable);

                            dateParameter.SqlDbType =
                                SqlDbType.Structured;

                            dateParameter.TypeName =
                                "dbo.ScheduleDateType";
                        }

                        // for doctor breaks only , 
                        if (breakTable != null)
                        {
                            SqlParameter breakParameter =
                                cmd.Parameters.AddWithValue(
                                    "@Breaks",
                                    breakTable);  //  @Breaks is not a normal parameter, t is a table containing multiple rows of type dbo.DoctorBreakType.

                            breakParameter.SqlDbType =
                                SqlDbType.Structured;

                            breakParameter.TypeName =
                                "dbo.DoctorBreakType";
                        }



                        // handle errors as sp return number errors

                        SqlParameter returnParameter = cmd.Parameters.Add("@ReturnValue", SqlDbType.Int);
                        returnParameter.Direction = ParameterDirection.ReturnValue;

                        con.Open();

                        cmd.ExecuteNonQuery();


                        // Get RETURN value from stored procedure
                        return (int)returnParameter.Value;

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



    }
}
