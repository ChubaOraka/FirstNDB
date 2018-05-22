using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace TechSuperHeroesLightningData
{
    public enum TypeOfQuery
    {
        AdHocSQL,
        SProc,
        CommandObject
    }


    public abstract class DbWorkerKernel
    {
        private static Stopwatch TimerStart()
        {
            var sw = new Stopwatch();
            sw.Start();
            return sw;
        }

        private static void TimerEnd(DbTask pDbt, Stopwatch psw)
        {
            pDbt.SW = psw;
            psw.Stop();
        }

        private static void SqlParameterSetup(ref DbTask dbt)
        {
            SqlConnection conn = ConnectionOpen(dbt.ConnectionName, ref dbt);
            if (ConnectionNotOpenHappened(ref dbt, MethodBase.GetCurrentMethod())) return;
           
            dbt.SqlCmd.Connection = conn;
            if (dbt.Sql != null)
            {
                dbt.SqlCmd.CommandText = dbt.Sql;
                dbt.SqlCmd.CommandType = DbUtility.IsAdHocSql(dbt.Sql) == false ? CommandType.StoredProcedure : CommandType.Text;
            }
            bool parametersExist = true;
            var parmSP = dbt.QueryParameters;
            if (parmSP == null) parametersExist = false;
            if (parmSP.Length == 0) parametersExist = false;
            if (!parametersExist) return;
            if (parmSP.Length % 2 != 0)
            {
                throw new Exception("parameters are not paired");
            }

            for (int countParams = 0; countParams < parmSP.Length; countParams += 2)
            {
                SqlParameter param = dbt.SqlCmd.CreateParameter();
                param.ParameterName = parmSP[countParams].ToString();
                param.Direction = ParameterDirection.Input;
                param.Value = parmSP[countParams + 1];
                dbt.SqlCmd.Parameters.Add(param);
            }
        }

        public static bool PingConnection(string pConnString)
        {
            var Dbt = new DbTask(pConnString);
            ConnectionOpen(pConnString, ref Dbt);
            return Dbt.ConnectionWasSuccessful;
        }

        private static SqlConnection ConnectionOpen(string pConnString, ref DbTask pDbt)
        {
            SqlConnection conn=null;
            try
            {
                string connString = System.Configuration.ConfigurationManager.ConnectionStrings[pConnString].ConnectionString;
                conn = new SqlConnection(connString);
                conn.Open();
                pDbt.ConnectionWasSuccessful = true;
                pDbt.ConnectionFailedException = null;
            }
            catch (Exception exc1)
            {
                Debug.WriteLine("ConnectionOpen exception =>" + exc1.Message);
                pDbt.ConnectionWasSuccessful = false;
                pDbt.ConnectionFailedException = exc1;
            }
            return conn;
        }

        private static bool ConnectionNotOpenHappened(ref DbTask pDbt, MethodBase pMB)
        {
            if (pDbt.ConnectionWasSuccessful == true) return false;

            if (pDbt.ConnectionWasSuccessful == false)
            {
                // TODO: ActivityLog entry
                Debug.WriteLine("Connection Did Not Open No Database Operation Attempted");
                Debug.WriteLine(DBMaria.PrettyMethodBase(pMB));
                Debug.WriteLine(DBMaria.PrettyException(pDbt.ConnectionFailedException));
                return true;                
            }

            return true;
        }

        
        public static void DebugSql(DbTask pDbt)
        {
            //if (ConnectionNotOpenHappened(ref pDbt,MethodBase.GetCurrentMethod())) return;

            var DBDeets = new DbTaskSummary {QueryRaw = pDbt.Sql};

            //if (pDbt.Sql.ToLower().IndexOf("insert into [activitylog]") > -1) return;

            if (pDbt.Sql != null & DbUtility.IsAdHocSql(pDbt.Sql))
            {
                DBDeets.QueryType = TypeOfQuery.AdHocSQL;
            }

            if (DBDeets.QueryType == TypeOfQuery.AdHocSQL)
            {
                DBDeets.QueryRaw = pDbt.Sql;
                DBDeets.QueryPrettyFormatted = DBMaria.PrettySQL(pDbt.Sql);
                DBDeets.QueryAsExecutableText = DBDeets.QueryPrettyFormatted;
            }
            else
            {
                DBDeets.QueryType = TypeOfQuery.SProc;

                // TODO function in Utility
                StringBuilder sbSprocCall = new StringBuilder();
                sbSprocCall.Append("EXEC " + pDbt.SqlCmd.CommandText + " ");
                foreach (SqlParameter parm in pDbt.SqlCmd.Parameters)
                {
                    sbSprocCall.AppendFormat(" {0}={1}, ", parm.ParameterName,
                        DbUtility.DelimitObject(parm.Value));
                }
                string sprocCallExpanded = sbSprocCall.ToString();
                // remove the trailing comma
                if (sprocCallExpanded.EndsWith(", "))
                {
                    sprocCallExpanded = sprocCallExpanded.Substring(0, sprocCallExpanded.Length - 2);
                }
                DBDeets.QueryPrettyFormatted = sprocCallExpanded;
                DBDeets.QueryAsExecutableText = DBDeets.QueryPrettyFormatted;
            }

            if (pDbt.IsSqlCommand)
            {
                DBDeets.QueryType = TypeOfQuery.CommandObject;
                DBDeets.QueryAsExecutableText = pDbt.SqlCmd.TSqlFromFromCommand();
            }
            DBDeets.MthdBsPretty = DBMaria.PrettyMethodBase(pDbt.MethodBase); 


            // timing transfer
            DBDeets.MilliSecondsElapsed = pDbt.SW.ElapsedMilliseconds;
            DBDeets.TimingPretty = DBMaria.PrettyDBtTiming(pDbt);

            if (pDbt.DbQueryType == DbQueryType.DataReader || pDbt.DbQueryType == DbQueryType.DataTable)
            {
                // TODO: may need tweaking with multiple result sets
                DBDeets.BlackBoxAsText=DBMaria.PrettyDataTable(pDbt.LogCaptureDataset.Tables[0],Schema:pDbt.Schema);
                DBDeets.BlackBox = pDbt.LogCaptureDataset;
            }

            DBDeets.RawExceptionQuery = pDbt.ExceptionQuery;
            DBDeets.RawExceptionDelegate = pDbt.ExceptionDelegate;
            DBDeets.ExceptionPretty = string.Empty;
            if(DBDeets.RawExceptionQuery!=null) DBDeets.ExceptionPretty = "QueryException=" + DBMaria.PrettyException(pDbt.ExceptionQuery);
            if(DBDeets.RawExceptionDelegate != null) DBDeets.ExceptionPretty += System.Environment.NewLine + "DelegateException=" + DBMaria.PrettyException(pDbt.ExceptionDelegate);
            Debug.WriteLine(DBMaria.PrettyDBDeets(DBDeets));
        }

        public static T ExecuteScalar<T>(ref DbTask dbt)
        {
            var sw = TimerStart();
            try
            {
                dbt.DbQueryType = DbQueryType.ExecScalar;
                SqlParameterSetup(ref dbt);
                if (ConnectionNotOpenHappened(ref dbt, MethodBase.GetCurrentMethod())) return default(T);

                object objReturn = dbt.SqlCmd.ExecuteScalar();
                if (objReturn == null)
                {
                    dbt.DebugExtendedInfo = dbt.ScalarNull.ToString() + " scalar returned null value";
                    return (T)dbt.ScalarNull;
                }

                //bool conversionComplete = false;
                if (typeof(T) == typeof(System.Boolean))
                {
                    bool temp = false;
                    bool.TryParse(objReturn.ToString(), out temp);
                    objReturn = temp;
                }

                if (typeof(T) == typeof(System.Int32))
                {
                    Int32 temp = 0;
                    Int32.TryParse(objReturn.ToString(), out temp);
                    objReturn = temp;
                }

                if (typeof(T) == typeof(System.Int64))
                {
                    Int64 temp = 0;
                    Int64.TryParse(objReturn.ToString(), out temp);
                    objReturn = temp;
                }

                if (typeof(T) == typeof(System.DateTime))
                {
                    DateTime temp = DateTime.MinValue;
                    DateTime.TryParse(objReturn.ToString(), out temp);
                    objReturn = temp;
                }

                if (typeof(T) == typeof(System.String))
                {
                    objReturn = objReturn.ToString();
                }

                dbt.DebugExtendedInfo = objReturn.ToString();
                return (T)objReturn;
            }
            catch (System.NullReferenceException)
            {
                dbt.DebugExtendedInfo = dbt.ScalarNull.ToString() + " scalar returned null reference";
                return (T)dbt.ScalarNull;
            }
            catch (Exception exc1)
            {
                dbt.ExceptionQuery = exc1;
                dbt.DebugExtendedInfo = default(T).ToString() + " scalar threw exception returned default type";
                return default(T);
            }
            finally
            {
                TimerEnd(dbt, sw);
                DebugSql(dbt);
                if (dbt.SqlCmd.Connection.State == ConnectionState.Open)
                {
                    dbt.SqlCmd.Connection.Close();
                }

                dbt.SqlCmd.Connection.Dispose();
            }
        }


        public static void ExecuteQuery(ref DbTask dbt, QueryFinished pEvent)
        {
            var sw = TimerStart();
            try
            {
                dbt.DbQueryType = DbQueryType.ExecQuery;
                SqlParameterSetup(ref dbt);
                if (ConnectionNotOpenHappened(ref dbt, MethodBase.GetCurrentMethod()))
                {
                    pEvent(0, new Exception("ExecuteQuery not attempted. Connection was not open."));
                }
                int count = dbt.SqlCmd.ExecuteNonQuery();
                //dbt.DebugExtendedInfo = string.Format("Rows affected={0}", count);
                pEvent(count, null);
            }
            catch (Exception exc1)
            {
                dbt.ExceptionQuery = exc1;
                pEvent(0, exc1);
            }
            finally
            {
                TimerEnd(dbt, sw);
                DebugSql(dbt);
                if (dbt.SqlCmd.Connection.State == ConnectionState.Open)
                {
                    dbt.SqlCmd.Connection.Close();
                }
                dbt.SqlCmd.Connection.Dispose();
            }
        }


        public static void GetDataTable(ref DbTask dbt, DataTableArrived pEvent)
        {
            DataTable dtblReturn = new DataTable();
            DbWorker.GetDataReader(ref dbt, delegate(SqlDataReader reader, Exception exc)
            {
                dtblReturn.Load(reader);
            });
            pEvent(dtblReturn, null);
        }

        public static void GetDataReader(ref DbTask dbt, DataReaderArrived pEvent)
        {
            SqlDataReader rdr = null;
            var sw = TimerStart();
            try
            {
                dbt.DbQueryType = DbQueryType.DataReader;
                SqlParameterSetup(ref dbt);
                if (ConnectionNotOpenHappened(ref dbt, MethodBase.GetCurrentMethod()))
                {
                    pEvent(null, new Exception("GetDataReader ignored. Connection never opened."));
                    Debug.WriteLine("Connection Blewup!");
                    return;
                }
                if (dbt.LogDataCapture)
                {
                    var dtblCapture = new DataTable();
                    var rdrTemp = dbt.SqlCmd.ExecuteReader();
                    dtblCapture.Load(rdrTemp);
                    rdrTemp.Close();
                    if (dbt.LogCaptureDataset == null)
                    {
                        dbt.LogCaptureDataset = new DataSet();
                    }

                    dbt.LogCaptureDataset.Tables.Add(dtblCapture);
                }
                rdr = dbt.SqlCmd.ExecuteReader();
                dbt.Schema = rdr.GetSchemaTable();
                StringBuilder sbFieldList = new StringBuilder();
                for (int i = 0; i < rdr.FieldCount; i++)
                {
                    sbFieldList.Append(rdr.GetName(i) + "|");
                }
                dbt.DebugExtendedInfo = string.Format("ColCount={0}; {1}", rdr.FieldCount, sbFieldList.ToString());
                try
                {
                    pEvent(rdr, null);
                }
                catch (Exception excDel)
                {
                    dbt.ExceptionDelegate = excDel;
                }
            }
            catch (Exception exc1)
            {
                dbt.ExceptionQuery = exc1;
                try
                {
                    pEvent(null, exc1);
                }
                catch (Exception excDel)
                {
                    dbt.ExceptionDelegate = excDel;
                    if (excDel.Message == "Invalid attempt to read when no data is present.")
                    {
                        // TODO is this best approach
                        // Don't throw this error just make it a warning
                        //dbt.ExceptionDelegate = new Exception("DataReader was empty but code attempted to read data", excDel);
                        dbt.ExceptionDelegate = null;
                        Debug.WriteLine("Warning DataReader was empty but code attempted to read data");
                    }
                }
            }
            finally
            {
                TimerEnd(dbt, sw);
                DebugSql(dbt);
                if (rdr != null) rdr.Dispose();
                if (dbt.SqlCmd.Connection.State == ConnectionState.Open)
                {
                    dbt.SqlCmd.Connection.Close();
                }
                dbt.SqlCmd.Connection.Dispose();
            }

        }
    }
}
