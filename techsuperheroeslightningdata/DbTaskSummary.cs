using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TechSuperHeroesLightningData
{
    public struct DbTaskSummary
    {
        public Guid EndToEndTaskKey;
        public string EndToEndTaskDescription;
        public string Message;
        public long MilliSecondsElapsed;
        public DataSet BlackBox;
        public string ExtendedInfo;
        public int RowCount;
        public int ColCount;
        public int CellCount;
        public MethodBase MthdBs;
        public string MthdBsPretty;
        public string QueryRaw;
        public string QueryPrettyFormatted;
        public TypeOfQuery QueryType;
        public string QueryAsExecutableText;
        public bool ActivityLogCall;
        public string BlackBoxAsText;
        public string TimingPretty;
        public Exception RawExceptionQuery;
        public Exception RawExceptionDelegate;
        public string ExceptionPretty;
    }
}
