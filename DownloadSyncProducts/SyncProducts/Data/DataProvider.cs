using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using SyncProducts.Helpers;
using System.Configuration;

namespace SyncProducts.Data
{
    public static class DataProvider
    {
        public static void GetDataTable(string sConn, string sql, out Result result)
        {
            result = new Result();
            DataTable dt = new DataTable();

            using (SqlConnection oConn = new SqlConnection(sConn))
            {
                oConn.Open();

                using (SqlCommand oCmd = new SqlCommand(sql, oConn))
                {
                    oCmd.CommandTimeout = 120;

                    try
                    {
                        SqlDataReader reader = oCmd.ExecuteReader();
                        dt.Load(reader);

                        result.ReturnObj = dt;
                        result.Success = true;
                    }
                    catch (Exception ex)
                    {
                        result.Success = false;
                        result.ErrForLog = string.Format("Failed to ExecuteQuery: {0}", sql);
                        Logger.LogError(ex, new List<string>() { result.ErrForLog });
                    }
                }
            }
        }

        public static void ExecuteQuery(string sConn, string sql, bool isScalar, out Result result)
        {
            result = new Result();

            using (SqlConnection oConn = new SqlConnection(sConn))
            {
                oConn.Open();

                using (SqlCommand oCmd = new SqlCommand(sql, oConn))
                {
                    try
                    {
                        if (isScalar)
                        {
                            result.ReturnObj = oCmd.ExecuteScalar();
                        }
                        else
                        {
                            oCmd.ExecuteNonQuery();
                        }
                        result.Success = true;
                    }
                    catch (Exception ex) {
                        result.Success = false;
                        result.ErrForLog = string.Format("Failed to ExecuteQuery: {0}", sql);
                        Logger.LogError(ex, new List<string>() { result.ErrForLog });
                    }
                }
            }
        }
    }
}