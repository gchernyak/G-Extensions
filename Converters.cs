using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace G.Extensions
{
    public static class Converters
    {
        /// <summary>
        /// Converts DateTime to Nullable DateTime
        /// </summary>
        /// <param name="dt">DateTime</param>
        /// <returns>DateTime?</returns>
        public static DateTime? Nullify(this DateTime dt)
        {
            DateTime? _nullDate = dt;
            return _nullDate;
        }

        /// <summary>
        /// Converts Integer to Nullable Integer
        /// </summary>
        /// <param name="num">int</param>
        /// <returns>int?</returns>
        public static int? Nullify(this int num)
        {
            int? _num = num;
            return _num;
        }

        /// <summary>
        /// Converts Nullable bool to bool
        /// </summary>
        /// <param name="b">bool</param>
        /// <returns>bool?</returns>
        public static bool? Nullify(this bool b)
        {
            bool? _b = b;
            return _b;
        }

        /// <summary>
        /// Parse int and change to decimal
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static decimal ToDecimal(this int d)
        {
            decimal _result;
            decimal.TryParse(d.ToString(), out _result);
            return _result;
        }
        /// <summary>
        /// Parse nullable int and change to decimal
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static decimal ToDecimal(this int? d)
        {
            decimal _result;
            decimal.TryParse(d.ToString(), out _result);
            return _result;
        }

        /// <summary>
        /// Parse string to int
        /// </summary>
        /// <param name="s">string</param>
        /// <returns>Integer</returns>
        public static int ToInt(this string s)
        {
            int _result;
            int.TryParse(s, out _result);
            return _result;
        }
        /// <summary>
        /// Parse decimal to int
        /// </summary>
        /// <param name="s">string</param>
        /// <returns>Integer</returns>
        public static int ToInt(this decimal s)
        {
            int _result;
            int.TryParse(s.ToString(), out _result);
            return _result;
        }
        /// <summary>
        /// Convert string to decimal
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static decimal ToDecimal(this string s)
        {
            decimal _result;
            decimal.TryParse(s.ToString(), out _result);
            return _result;
        }

        /// <summary>
        /// Covert XmlDocument to XDocument
        /// </summary>
        /// <param name="xmlDocument"></param>
        /// <returns></returns>
        public static XDocument ToXDocument(this XmlDocument xmlDocument)
        {
            using (var nodeReader = new XmlNodeReader(xmlDocument))
            {
                nodeReader.MoveToContent();
                return XDocument.Load(nodeReader);
            }
        }

        /// <summary>
        /// Convert XDocument to XmlDocument
        /// </summary>
        /// <param name="xDocument"></param>
        /// <returns></returns>
        public static XmlDocument ToXmlDocument(this XDocument xDocument)
        {
            var xmlDocument = new XmlDocument();
            using (var xmlReader = xDocument.CreateReader())
            {
                xmlDocument.Load(xmlReader);
            }
            return xmlDocument;
        }

        /// <summary>
        /// Converts Nullable DateTime to DateTime (MinValue if Null)
        /// </summary>
        /// <param name="dt">Nullable DateTime</param>
        /// <returns>DateTime</returns>
        public static DateTime UnNullify(this DateTime? dt)
        {
            return DateTime.MinValue;
        }

        /// <summary>
        /// Converts Nullable Interger to Integer
        /// </summary>
        /// <param name="num">int? (assigns 0)</param>
        /// <returns>int</returns>
        public static int UnNullify(this int? num)
        {
            return num ?? 0;
        }

        /// <summary>
        /// Converts bool to Nullable bool
        /// </summary>
        /// <param name="b">Nullable bool</param>
        /// <returns>bool?</returns>
        public static bool UnNullify(this bool? b)
        {
            return b ?? false;
        }

        /// <summary>
        /// Convert int array to data table (single column)
        /// </summary>
        /// <param name="a">Array</param>
        /// <param name="cn">column name</param>
        /// <returns></returns>
        public static DataTable ToIntDataTable(this int[] a, string cn)
        {
            var dt = new DataTable();
            dt.Columns.Add(cn, typeof(int));
            foreach (var i in a)
            {
                DataRow r = dt.NewRow();
                dt.Rows.Add(i);
            }
            return dt;
        }

        /// <summary>
        /// Convert string array to data table (single column)
        /// </summary>
        /// <param name="a">Array</param>
        /// <param name="cn">Column name</param>
        /// <returns></returns>
        public static DataTable ToIntDataTable(this string[] a, string cn)
        {
            var dt = new DataTable();
            dt.Columns.Add(cn, typeof(int));
            foreach (var i in a)
            {
                DataRow r = dt.NewRow();
                dt.Rows.Add(i.ToInt());
            }
            return dt;
        }

        /// <summary>
        /// Convert string array to data table (single column)
        /// </summary>
        /// <param name="a">Array</param>
        /// <param name="cn">Column name</param>
        /// <returns></returns>
        public static DataTable ToStringDataTable(this string[] a, string cn)
        {
            var dt = new DataTable();
            dt.Columns.Add(cn, typeof(string));
            foreach (var i in a)
            {
                DataRow r = dt.NewRow();
                dt.Rows.Add(i.ToString());
            }
            return dt;
        }
        public static DataTable ToDataTable<T>(this IEnumerable<T> list)
        {
            Type type = typeof(T);
            var properties = type.GetProperties();
            DataTable dataTable = new DataTable();
            foreach (PropertyInfo info in properties)
            {
                dataTable.Columns.Add(new DataColumn(info.Name, Nullable.GetUnderlyingType(info.PropertyType) ?? info.PropertyType));
            }
            foreach (T entity in list)
            {
                object[] values = new object[properties.Length];
                for (int i = 0; i < properties.Length; i++)
                {
                    values[i] = properties[i].GetValue(entity);
                }
                dataTable.Rows.Add(values);
            }
            return dataTable;
        }
        /// <summary>
        /// Convert object array to datatable
        /// </summary>
        /// <param name="array">object array</param>
        /// <returns></returns>
        public static DataTable ConvertToDataTable(this object[] array)
        {
            PropertyInfo[] properties = array.GetType().GetElementType().GetProperties();
            DataTable dt = ConverterDataHelper.CreateDataTable(properties);
            if (array.Length != 0)
            {
                foreach (object o in array)
                    ConverterDataHelper.FillData(properties, dt, o);
            }
            return dt;
        }

        /// <summary>
        /// Convert an object to datatable
        /// </summary>
        /// <param name="o">object</param>
        /// <returns></returns>
        public static DataTable ConvertToDataTable(this object o)
        {
            PropertyInfo[] properties = o.GetType().GetProperties();
            DataTable dt = ConverterDataHelper.CreateDataTable(properties);
            ConverterDataHelper.FillData(properties, dt, o);
            return dt;
        }

        /// <summary>
        /// Convert an object to data table
        /// </summary>
        /// <param name="values">List objects</param>
        /// <param name="groupSize">size</param>
        /// <param name="maxCount">max</param>
        /// <returns>List of Lists</returns>
        public static List<List<T>> SplitList<T>(IEnumerable<T> values, int groupSize, int? maxCount = null)
        {
            var result = new List<List<T>>();
            // Quick and special scenario
            if (values.Count() <= groupSize)
            {
                result.Add(values.ToList());
            }
            else
            {
                List<T> valueList = values.ToList();
                int startIndex = 0;
                int count = valueList.Count;

                while (startIndex < count && (!maxCount.HasValue || (maxCount.HasValue && startIndex < maxCount)))
                {
                    int elementCount = (startIndex + groupSize > count) ? count - startIndex : groupSize;
                    result.Add(valueList.GetRange(startIndex, elementCount));
                    startIndex += elementCount;
                }
            }
            return result;
        }

        public static T DeserializeJson<T>(string value)
        {
            var result = JsonConvert.DeserializeObject<T>(value);
            return result;
        }

        public static IEnumerable<T> DeserializeXml<T>(string value)
        {
            var serializer = new XmlSerializer(typeof(T));
            StringReader rdr = new StringReader(value);
            return (IEnumerable<T>)serializer.Deserialize(rdr);
        }
    }
}
