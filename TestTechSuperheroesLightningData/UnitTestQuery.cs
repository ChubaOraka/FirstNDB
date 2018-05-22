using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TechSuperHeroesLightningData;
using System.Reflection;
using System.Data;
using System.Data.SqlClient;

namespace TestTechSuperheroesLightningData
{
    [TestClass]
    public class UnitTestQuery
    {
        [TestMethod]
        public void TestDataTable()
        {
            string queryString = "Select top 20 * from Person.Person";
            var dbt = new DbTask("aw", queryString, MethodBase.GetCurrentMethod());
            DbWorker.GetDataTable(ref dbt, delegate (DataTable p1, Exception e1)
            {
                Assert.AreEqual(20, p1.Rows.Count);
            }
            );
        }

        [TestMethod]
        public void TestDataReader()
        {
            string queryString = "Select top 2 * from Person.Person";
            var dbt = new DbTask("aw", queryString, MethodBase.GetCurrentMethod());
            DbWorker.GetDataReader(ref dbt, delegate (SqlDataReader p1, Exception e1)
            {
                Assert.AreEqual(true, p1.HasRows);
                int i = 0;
                while (p1.Read())
                {
                    i++;
                }
                Assert.AreEqual(2, i);
            }
            );
        }

        [TestMethod]
        public void TestScalar()
        {
            string queryString = "Select top 1 FirstName from Person.Person";
            var dbt = new DbTask("aw", queryString, MethodBase.GetCurrentMethod());
            string blah=DbWorker.ExecuteScalar<string>(ref dbt);
            Assert.IsInstanceOfType(blah,typeof(string)); 
        }
    }
}
