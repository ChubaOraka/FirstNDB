using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace TechSuperHeroesLightningData
{
    public delegate void DataReaderArrived(SqlDataReader pRdr, Exception pExc);
    public delegate void DataTableArrived(DataTable pDtbl, Exception pExc);
    public delegate void QueryFinished(int pCountRecs, Exception pExc);

    public enum DbQueryType
    {
        Unknown,
        ExecScalar,
        DataReader,
        DataTable,
        ExecQuery
    }

    public abstract class DbTaskKernel
        {
            public bool DbTaskSummaryOn { get; set; }
            public bool TestingMode { get; set; }
            public int TimeOut { get; set; } // todo: make sure all operations respond to this

            public string ConnectionName { set; get; }
            public string Sql { set; get; }
            public SqlCommand SqlCmd { set; get; }
            public MethodBase MethodBase { set; get; }
            public object[] QueryParameters { set; get; }
            public Exception ExceptionQuery { set; get; }
            public Exception ExceptionDelegate { set; get; }
            public DbQueryType DbQueryType { set; get; }
            public Stopwatch SW { get; set; }
            public HttpContext HttpCtx { get; set; }
            public object ScalarNull { get; set; }
            public string DebugExtendedInfo { get; set; }
            public bool IsSqlCommand { get; set; }
            public string SqlCommandExpanded { get; set; }
            public bool LogDataCapture { get; set; }
            public DataSet LogCaptureDataset { get; set; }
            public DataTable Schema { get; set; }
            public bool ConnectionWasSuccessful { get; set; }
            public Exception ConnectionFailedException { get; set; }

            protected void DbTaskDefault(string pConnectionName, params object[] pQueryParms)
            {
                this.ConnectionName = pConnectionName;
                this.Sql = null;
                this.SqlCmd = new SqlCommand();
                
                this.MethodBase = null;
                this.HttpCtx = null;
                
                this.QueryParameters = pQueryParms;
                this.ExceptionQuery = null;
                this.ExceptionDelegate = null;
                this.DbTaskSummaryOn= true;
                this.ScalarNull = null;
                this.DebugExtendedInfo = String.Empty;
                this.IsSqlCommand = false;

                this.LogDataCapture = true;
                this.LogCaptureDataset = null;

                this.ConnectionWasSuccessful = false;
                this.ConnectionFailedException = null;
            }
        } // end class

    } // end namespace

