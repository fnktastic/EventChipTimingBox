namespace System.Runtime.CompilerServices.RFID
{
    using System;
    using System.Data;
    using System.Data.SqlClient;

    public static class Extension
    {
        public static bool CanOpen(SqlConnection connection)
        {
            try
            {
                if (connection == null)
                {
                    return false;
                }
                connection.Open();
                bool flag = connection.State == ConnectionState.Open;
                connection.Close();
                return flag;
            }
            catch
            {
                return false;
            }
        }
    }
}

