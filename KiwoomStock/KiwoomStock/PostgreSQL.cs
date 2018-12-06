using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace KiwoomStock
{
    public class PostgreSQL
    {
        private string connString;

        public PostgreSQL()
        {
            List<string> dbInfo = new List<string>();
            System.IO.StreamReader file = new System.IO.StreamReader(@"D:/stockDB.info");
            while (!file.EndOfStream)
            {
                dbInfo.Add(file.ReadLine());
            }
            file.Close();

            connString = string.Format("Host={0};Database={1};Username={2};Password={3}", dbInfo[0], dbInfo[1], dbInfo[2], dbInfo[3]);            
        }

        public bool insertStockData(string code, int price, int volume)
        {
            int nRows = -1;

            using (var conn = new NpgsqlConnection(connString))
            {
                try
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;

                        cmd.CommandText =
                            String.Format(
                                @"
                                INSERT INTO stock_data (code, datetime, price, volume) VALUES ({0}, now(), {1}, {2});
                            ",
                                code, price, volume
                                );

                        nRows = cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message);
                }
            }

            return nRows > 0 ? true : false;
        }

    }
}
