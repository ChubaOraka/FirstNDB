using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace TechSuperHeroesLightningData
{
    public static class DBMaria
    {
        public static string PrettyDataTable(DataTable pDtbl, DataTable Schema = null)
        {
            string textLongCell = "<long text, length=xxxxxxx chars>";
            string textNullDataTable = "<DataTable is null>";
            string textEmptyDataTable = "<DataTable has 0 rows";

            int columnNumber = 0;
            int maxColumnSizeToDisplay = 75;
            if(pDtbl==null) return textNullDataTable;
            if (pDtbl.Rows.Count == 0) return textEmptyDataTable;

            // calculate column length based on data
            var colLengthMax = new Dictionary<int, int>();
            foreach (DataRow dr in pDtbl.Rows)
            {
                columnNumber = 0;
                foreach (DataColumn c in pDtbl.Columns)
                {
                    object objCell = dr[c.ColumnName];
                    string strCell = dr[c.ColumnName].ToString();
                    var lengthOfCell = strCell.Length;
                    if (objCell is DBNull && lengthOfCell<8)
                    {
                        lengthOfCell = 8;
                    }
                    if (!colLengthMax.ContainsKey(columnNumber))
                    {
                        colLengthMax.Add(columnNumber, lengthOfCell);
                    }
                    else
                    {
                        if (lengthOfCell > colLengthMax[columnNumber])
                        {
                            colLengthMax[columnNumber] = lengthOfCell;
                            if (lengthOfCell > maxColumnSizeToDisplay)
                            {
                                colLengthMax[columnNumber] = textLongCell.Length+4;
                            }
                        }
                    }
                    columnNumber++;
                }
            }

            // This code looks at column name and re-sizes if name is larger than data length
            columnNumber = 0;
            foreach (DataColumn c in pDtbl.Columns)
            {
                if (c.ColumnName.Length > colLengthMax[columnNumber])
                {
                    colLengthMax[columnNumber] = c.ColumnName.Length+4;
                }

                columnNumber++;
            }

            // This code looks at Data and re-sizes column sizes based on data

            // now put column headings into StringBuilder
            var sbColumnHeadings=new StringBuilder();
            columnNumber = 0;
            foreach (DataColumn c in pDtbl.Columns)
            {
                sbColumnHeadings.AppendFormat("{0} | ", c.ColumnName.PadRight(colLengthMax[columnNumber]));
                columnNumber++;
            }
            sbColumnHeadings.AppendLine();

            // now we are ready to throw data into string
            var sbDataTable = new StringBuilder();
            long lengthOfData = 0;
                foreach (DataRow dr in pDtbl.Rows)
                {
                    columnNumber = 0;
                    foreach (DataColumn c in pDtbl.Columns)
                    {
                        object tempCell = dr[c.ColumnName];
                        string strCell = tempCell.ToString();
                        int thisCellLength = strCell.Trim().Length;
                        if (thisCellLength > maxColumnSizeToDisplay)
                        {
                            strCell = "<long text, length=" + thisCellLength + " chars>";
                            thisCellLength = textLongCell.Length + 4;
                        }
 
                        if (tempCell is DBNull)
                        {
                            strCell = "<null>";
                            lengthOfData = 1;
                        }
                        lengthOfData += thisCellLength;

                        // pad Column to length
                        sbDataTable.AppendFormat("{0} | ", strCell.PadRight(colLengthMax[columnNumber]));
                        columnNumber++;
                    }
                    sbDataTable.AppendLine();
                       
                    //Debug.WriteLine(sbDataTable.ToString());
                }
            columnNumber = 0;

            // now we can build the Data Display since column lengths are set





            // now put it all together
            string extraInfo = string.Format("<data display> rows={0}; cols={1}; CellCount={2}; Length={3} ", pDtbl.Rows.Count,
                pDtbl.Columns.Count, pDtbl.Rows.Count * pDtbl.Columns.Count, lengthOfData);
            return extraInfo + Environment.NewLine + sbColumnHeadings.ToString() +  sbDataTable.ToString();
        }

        public static string PrettyDBDeets(DbTaskSummary pDBDeets)
        {
            StringBuilder sbDeets = new StringBuilder();
            sbDeets.AppendLine(pDBDeets.TimingPretty);
            sbDeets.AppendLine(pDBDeets.QueryAsExecutableText);
            sbDeets.AppendLine(pDBDeets.ExceptionPretty);
            sbDeets.AppendLine(pDBDeets.ExtendedInfo);
            sbDeets.AppendLine(pDBDeets.BlackBoxAsText);
            return sbDeets.ToString();
        }

        public static string PrettyException(params Exception[] pExc)
        {
            StringBuilder sbExc = new StringBuilder();
            foreach (Exception excTemp in pExc)
            {
                if (excTemp != null)
                {
                    sbExc.Append(OnePrettyException(excTemp));
                    Exception inExc = null;
                    inExc = excTemp.InnerException;
                    while (inExc != null)
                    {
                        sbExc.AppendLine("InnerException");
                        sbExc.AppendLine(OnePrettyException(inExc));
                        inExc = inExc.InnerException;
                    }
                }
            }
            string errorMsg = sbExc.ToString();
            return errorMsg;
        }

        public static string OnePrettyException(Exception pExc)
        {
            StringBuilder sbExc = new StringBuilder();
            if (pExc != null)
            {
                sbExc.AppendLine("Exception.GetType()=" + pExc.GetType().ToString());
                sbExc.AppendLine("Exception.Message=" + pExc.Message);
                sbExc.AppendLine("Exception.StackTrace=" + System.Environment.NewLine + pExc.StackTrace);
            }
            return sbExc.ToString();
        }

        public static string PrettyMethodBase(MethodBase pMB)
        {
            if (pMB.ReflectedType == null) return "<caller unknown>";
            return pMB.ReflectedType.Name + "." + pMB.Name;
        }

        public static string PrettySQL(string pSQL)
        {
           // TODO make SqlPrettyFunction
            StringBuilder sbQuery = new StringBuilder();
            sbQuery.Append(pSQL);
            //sbQuery.Replace(", ", "," + System.Environment.NewLine + "   "); // this gets split isnull(u.lastname,'')+', '+isnull(u.firstname,'') AS Coordinator_Name,
            sbQuery.Replace(",   ", "," + System.Environment.NewLine + "   ");
            sbQuery.Replace("FROM", System.Environment.NewLine + "          FROM");
            sbQuery.Replace("LEFT OUTER JOIN", System.Environment.NewLine + "          LEFT OUTER JOIN");
            sbQuery.Replace("WHERE", System.Environment.NewLine + "          WHERE");
            return sbQuery.ToString();

        }

        public static string PrettyDBtTiming(DbTask pDbt)
        {
            return string.Format("{0} took {1}ms. Extended Info={2}", pDbt.DbQueryType.ToString(),
                pDbt.SW.ElapsedMilliseconds, pDbt.DebugExtendedInfo);
        }
    }

}
