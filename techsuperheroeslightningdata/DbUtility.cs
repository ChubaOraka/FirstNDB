using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechSuperHeroesLightningData
{
    class DbUtility
    {
        public static string DelimitText(string p1)
        {
            if (p1 == null) return " null ";
            else return "'" + p1.ToString().Replace("'", "''") + "'";
        }

        public static string DelimitNonText(long? p1)
        {
            if (p1 == null) return " null ";
            else return p1.ToString();
        }

        public static string DelimitObject(object p1)
        {
            if (p1 == null) return " null ";
            else return "'" + p1.ToString().Replace("'", "''") + "'";
        }

        /// <summary>
        /// Replaces all instances of double single quotes ('') with one single quote (')
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string SingleQuoteFromDatabase(string input)
        {
            if (input != null)
            {
                return input.Replace("''", "'");
            }
            else
            {
                return input;
            }
        }

        /// <summary>
        /// Removes all instances of single quotes (') from a string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string RemoveSingleQuotesFromString(string input)
        {
            if (input != null)
            {
                return input.Replace("'", "");
            }
            else
            {
                return input;
            }
        }

        /// <summary>
        /// Strips out HTML tags by removing everthing between "<" and ">", and the brackets
        /// </summary>
        /// <remarks>
        /// Note that if the string uses "<" for something other than a tag (for example, in a mathematical way),
        /// everything after it will be removed
        /// </remarks>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string StripHTMLTags(string input)
        {
            StringBuilder sb = new StringBuilder();
            bool isInsideTag = false;

            char[] stringAsBits = input.ToCharArray();

            foreach (char bit in stringAsBits)
            {
                if (bit.Equals('<'))
                {
                    isInsideTag = true;
                }
                else if (bit.Equals('>'))
                {
                    isInsideTag = false;
                }
                else
                {
                    if (!isInsideTag)
                    {
                        sb.Append(bit);
                    }
                }
            }

            return sb.ToString();
        }

        public static bool IsAdHocSql(string pSql)
        {
            if (pSql.ToLower().StartsWith("insert into")) return true;
            if (pSql.ToLower().StartsWith("select ")) return true;
            if (pSql.ToLower().StartsWith("delete from ")) return true;
            if (pSql.ToLower().StartsWith("update")) return true;

            // TODO: this hack goes away when we provide direct Sproc calling
            if (pSql.ToLower().Contains("'")) return true;
            if (pSql.ToLower().Contains("get_")) return true;
            return false;
        }
    }
}
