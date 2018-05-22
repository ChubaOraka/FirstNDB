using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.Web;
using System.IO;

namespace TechSuperHeroesLightningData
{

    public enum LogActivityCategory
    {
        DatabaseActivity,
        DatabaseException,
        Exception,
        MethodCall
    }

    public class DbLumberJack
    {
        private Stopwatch sw { get; set; }
        private MethodBase mb { get; set; }
        private HttpContext httpCtx { get; set; }

        public string currentPage { get; set; }
        private string currentUser { get; set; }
        


        //         10        20        30
        // 123456789012345678901234567890123456
        // 61FFB6F0-AE23-4588-A673-BA6674A7EB17

        public static void LogActivity(string pUserID, string pPageName, LogActivityCategory pCategory, string pEventName, long pTimeElapsed, string pDetails, DbTask pDbt)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("INSERT INTO [ActivityLog] ([UserID],[PageName],[Category],[EventName],[TimeElapsed],[Details],[ConnString]) ");
            sb.AppendLine("VALUES (");
            sb.Append(DbUtility.DelimitText(pUserID) + ",");
            sb.Append(DbUtility.DelimitText(pPageName) + ",");
            sb.Append("'" + pCategory.ToString() + "',");
            sb.Append("'" + pEventName + "',");
            sb.Append(DbUtility.DelimitNonText(pTimeElapsed) + ",");
            sb.Append(DbUtility.DelimitText(pDetails) + ",");
            sb.Append(DbUtility.DelimitText(pDbt.ConnectionName));
            sb.Append(")");
            string queryString = sb.ToString();
            var dbt = new DbTask("Aw_ActivityLog", queryString);
            
            dbt.DbTaskSummaryOn = false;
            DbWorker.ExecuteQuery(ref dbt, delegate(int pCount, Exception pExc)
            {
                if (pExc != null)
                {
                    Debug.WriteLine("Error Writing ActivityLog");
                    Debug.WriteLine("Sql=");
                    Debug.WriteLine(queryString);
                    Debug.WriteLine(DBMaria.PrettyException(pExc));
                }
            });
        }


        public DbLumberJack(MethodBase pMB,HttpContext pHttp)
        {
            this.sw = new Stopwatch();
            this.mb = pMB;
            sw.Start();
            httpCtx = pHttp;
        }

        public void LogComplete()
        {
            sw.Stop();
            Debug.WriteLine(string.Format("{0} took {1}ms", mb.Name, sw.ElapsedMilliseconds));
        }

        private void FileAppend(string pFileNameAndPath, string pText)
        {
            File.AppendAllText(pFileNameAndPath, pText + Environment.NewLine);
        }
    }
}