using System;
using System.Data;
using System.Data.Common;
using System.Reflection;

namespace G.Extensions
{
    public static class ConverterDataHelper
    {
        public static DataTable CreateDataTable(PropertyInfo[] properties)

        {
            DataTable dt = new DataTable();
            DataColumn dc = null;
            foreach (PropertyInfo pi in properties)
            {
                dc = new DataColumn();
                dc.ColumnName = pi.Name;
                dc.DataType = Nullable.GetUnderlyingType(pi.PropertyType) ?? pi.PropertyType;
                dt.Columns.Add(dc);
            }
            return dt;
        }

        public static void FillData(PropertyInfo[] properties, DataTable dt, Object o)
        {
            DataRow dr = dt.NewRow();
            foreach (PropertyInfo pi in properties)
            {
                dr[pi.Name] = pi.GetValue(o, null);
            }
            dt.Rows.Add(dr);
        }

        public static PropertyCollection GetPropertyCollection(DbDataReader reader)
        {
            var props = new PropertyCollection();
            // getting properties and cleaning up any underscores
            while (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    if (!props.ContainsKey(reader.GetName(i).Replace("_", "")))
                    {
                        props.Add(reader.GetName(i).Replace("_", ""), reader.GetDataTypeName(i));
                    }
                }
            }
            return props;
        }

        public static object GetInstanceOfObject(Type type)
        {
            var instance = Activator.CreateInstance(type);
            return instance;
        }
    }
}