using System;
using System.Data.SqlClient;
using System.Reflection;
using System.Web;

namespace TechSuperHeroesLightningData
{
    public class DbTask : DbTaskKernel
    {
        public DbTask()
        {
            throw new Exception("DBTask cannot be called without a constructor");
        }


        public DbTask(string pConnectionName)
        {
            DbTaskDefault(pConnectionName,null);
        }

        public DbTask(string pConnectionName, string pSql)
        {
            DbTaskDefault(pConnectionName, null);
            this.Sql = pSql;
        }

        public DbTask(string pConnectionName, string pSql, params object[] pQueryParms)
        {
            DbTaskDefault(pConnectionName, pQueryParms);
            this.Sql = pSql;
        }

        public DbTask(string pConnectionName, string pSql, MethodBase pMB, params object[] pQueryParms)
        {
            DbTaskDefault(pConnectionName, pQueryParms);
            this.Sql = pSql;
            this.MethodBase = pMB;
        }


        public DbTask(string pConnectionName, string pSql, MethodBase pMB, HttpContext pHttpCtx, params object[] pQueryParms)
        {
            DbTaskDefault(pConnectionName, pQueryParms);
            this.Sql = pSql;
            this.MethodBase = pMB;
            this.HttpCtx = pHttpCtx;
        }

        public DbTask(string pConnectionName, SqlCommand pCmd, params object[] pQueryParms)
        {
            DbTaskDefault(pConnectionName, pQueryParms);
            this.SqlCmd = pCmd;
        }

        public DbTask(string pConnectionName, SqlCommand pCmd, MethodBase pMB, params object[] pQueryParms)
        {
            DbTaskDefault(pConnectionName, pQueryParms);
            this.SqlCmd = pCmd;
            this.IsSqlCommand = true;
            this.MethodBase = pMB;
        }

        public DbTask(string pConnectionName, SqlCommand pCmd, MethodBase pMB, HttpContext pHttpCtx, params object[] pQueryParms)
        {
            DbTaskDefault(pConnectionName, pQueryParms);
            this.SqlCmd = pCmd;
            this.IsSqlCommand = true;
            this.MethodBase = pMB;
            this.HttpCtx = pHttpCtx;
        }



    }
}