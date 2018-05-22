using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;

namespace TechSuperHeroesLightningData
{
    public static class SqlQueryExpander
    {

        public static string TSqlFromFromCommand(this SqlCommand cmd)
        {
            StringBuilder commandTxt = new StringBuilder();
            commandTxt.Append("DECLARE ");
            List<string> paramlst = new List<string>();
            foreach (SqlParameter parms in cmd.Parameters)
            {
                paramlst.Add(parms.ParameterName);
                commandTxt.Append(parms.ParameterName + " AS ");
                commandTxt.Append(parms.SqlDbType.ToString());
                commandTxt.Append(",");
            }

            if (commandTxt.ToString().Substring(commandTxt.Length - 1, 1) == ",")
                commandTxt.Remove(commandTxt.Length - 1, 1);
            commandTxt.AppendLine();
            int rownr = 0;
            foreach (SqlParameter parms in cmd.Parameters)
            {
                string val = String.Empty;
                if (parms.DbType.Equals(DbType.String) || parms.DbType.Equals(DbType.DateTime))
                    val = "'" + Convert.ToString(parms.Value).Replace(@"\", @"\\").Replace("'", @"\'") + "'";
                if (parms.DbType.Equals(DbType.Int16) || parms.DbType.Equals(DbType.Int32) || parms.DbType.Equals(DbType.Int64) || parms.DbType.Equals(DbType.Decimal) || parms.DbType.Equals(DbType.Double))
                    val = Convert.ToString(parms.Value);

                commandTxt.AppendLine();
                commandTxt.Append("SET " + paramlst[rownr].ToString() + " = " + val.ToString() + ";");
                rownr += 1;
            }
            commandTxt.AppendLine();
            commandTxt.AppendLine();
            commandTxt.Append(cmd.CommandText);
            return commandTxt.ToString();
        }


        public static string TSqlFromFromCommandImproved(this SqlCommand cmd)
        {
            StringBuilder commandTxt = new StringBuilder();
            commandTxt.Append("DECLARE ");
            List<string> paramlst = new List<string>();
            foreach (SqlParameter parms in cmd.Parameters)
            {
                paramlst.Add(parms.ParameterName);
                commandTxt.Append(parms.ParameterName + " AS ");
                commandTxt.Append(parms.SqlDbType.ToString());
                commandTxt.Append(",");
            }

            if (commandTxt.ToString().Substring(commandTxt.Length - 1, 1) == ",")
                commandTxt.Remove(commandTxt.Length - 1, 1);
            commandTxt.AppendLine();
            int rownr = 0;
            foreach (SqlParameter parms in cmd.Parameters)
            {
                string val = String.Empty;
                if (parms.DbType.Equals(DbType.String) || parms.DbType.Equals(DbType.DateTime))
                    val = "'" + Convert.ToString(parms.Value).Replace(@"\", @"\\").Replace("'", @"\'") + "'";
                if (parms.DbType.Equals(DbType.Int16) || parms.DbType.Equals(DbType.Int32) || parms.DbType.Equals(DbType.Int64) || parms.DbType.Equals(DbType.Decimal) || parms.DbType.Equals(DbType.Double))
                    val = Convert.ToString(parms.Value);

                commandTxt.AppendLine();
                commandTxt.Append("SET " + paramlst[rownr].ToString() + " = " + val.ToString() + ";");
                rownr += 1;
            }
            commandTxt.AppendLine();
            commandTxt.AppendLine();
            commandTxt.Append(cmd.CommandText);
            return commandTxt.ToString();
        }

    } // end class 
} // end namespace
