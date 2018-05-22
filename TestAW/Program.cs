using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechSuperHeroesLightningData;
using System.Data.SqlClient;
using System.Data;
using System.Reflection;

namespace TestAW
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Connecting and Displaying AdventureWorks Data");

            string queryString = "Select * from Person.Person";
            var dbt = new DbTask("aw", queryString, MethodBase.GetCurrentMethod());
            DbWorker.GetDataTable(ref dbt, delegate (DataTable p1, Exception e1)
             {
                // get DataTable
             }
            );

            string queryString2 = "Select firstname,count(firstname) as howmany from Person.Person group by firstname order by howmany desc";
            var dbt2 = new DbTask("aw", queryString2, MethodBase.GetCurrentMethod());
            DbWorker.GetDataTable(ref dbt2, delegate (DataTable p1, Exception e1)
            {
               // get Datatable
            }
            );

            Console.WriteLine("Finished Displaying Data Now!");

            Console.ReadKey();
        }
        
    }
}
