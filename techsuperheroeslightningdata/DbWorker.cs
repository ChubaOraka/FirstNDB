using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace TechSuperHeroesLightningData
{
        public enum DbWorkerAsyncType
        {
            AnyOrder,
            Sequential
        }

        public class DbWorker: DbWorkerKernel
        {
            public static int ErrorCount { set; get; }



        public static bool AttemptTransaction(params DbTask[] pDbt)
            {
                throw new Exception("DBWorker does not do that yet!");
                return false;
            }

            public static bool AttemptTransaction(List<DbTask> plstDbt)
            {
                throw new Exception("DBWorker does not do that yet!");
                return false;
            }

            public static bool RunTheseAsync(DbWorkerAsyncType p2, params DbTask[] pDbt)
            {
                throw new Exception("DBWorker does not do that yet!");
                return false;
            }

            public static bool RunTheseAsync(DbWorkerAsyncType p2, List<DbTask> pLstDbt)
            {
                throw new Exception("DBWorker does not do that yet!");
                return false;
            }

        }// end class

    } // end namespace