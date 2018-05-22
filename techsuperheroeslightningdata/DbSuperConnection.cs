using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace TechSuperHeroesLightningData
{
    public class DbSuperConnection
    {
        public string Connect
        {
            get { return ConnectionString; }
        }

        private string ConnectionString { set; get; }
        private string PrefixConnectionString { set; get; }

        public DbSuperConnection(string pConnectionName)
        {
            PrefixConnectionString = "";
            DetectServer();
            ConnectionString = PrefixConnectionString + pConnectionName;
        }

        public void DetectServer()
        {
            PrefixConnectionString = "";
        }
    }
}
