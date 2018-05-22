using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TechSuperHeroesLightningData;
using System.Reflection;
using System.Data;
using System.Data.SqlClient;


namespace TestTechSuperheroesLightningData
{
    [TestClass]
    public class UnitTestConnect
    {
        [TestMethod]
        public void ConnectYes()
        {
            /// TODO: improve speed of Ping/Open Connection 327ms on my machine
            var couldConnect = DbWorker.PingConnection("aw");
            Assert.AreEqual(true, couldConnect);
        }

        [TestMethod]
        public void ConnectNo()
        {
            var couldNotConnect = DbWorker.PingConnection("awjunk");
            Assert.AreEqual(false,couldNotConnect);
        }

    }
}
