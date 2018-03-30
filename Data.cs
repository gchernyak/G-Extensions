using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace G.Extensions
{
    public static class Data
    {
        /// <summary>
        /// Execute stored procedure with transaction. This method will map automatically to the specified type. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="parameters"></param>
        /// <param name="commandName"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static IEnumerable<T> ExecuteTransactional<T>(DbContext context, IEnumerable<SqlParameter> parameters,
            string commandName, int userId)
        {
            var result = new List<T>();
            using (context)
            {
                context.Database.Initialize(false);

                DbCommand contextCmd = context.Database.Connection.CreateCommand();
                contextCmd.CommandText = "[dbo].[P_USER_IDCONTEXT_UPDATE]";
                contextCmd.CommandType = CommandType.StoredProcedure;
                contextCmd.Parameters.Add(new SqlParameter("UserId", userId));

                DbCommand procCommand = context.Database.Connection.CreateCommand();
                procCommand.CommandText = commandName;
                procCommand.CommandType = CommandType.StoredProcedure;

                context.Database.Connection.Open();
                foreach (SqlParameter sqlParameter in parameters)
                {
                    procCommand.Parameters.Add(sqlParameter);
                }

                DbTransaction trans = context.Database.Connection.BeginTransaction(IsolationLevel.ReadUncommitted);
                try
                {
                    contextCmd.Transaction = trans;
                    contextCmd.ExecuteNonQuery();
                    procCommand.Transaction = trans;

                    DbDataReader reader = procCommand.ExecuteReader();

                    result = ((IObjectContextAdapter) context)
                        .ObjectContext
                        .Translate<T>(reader).ToList();

                    reader.Close();
                    trans.Commit();
                    return result;
                }
                catch (SqlException ex)
                {
                    trans.Rollback();
                    throw ex;
                }
                finally
                {
                    context.Database.Connection.Close();
                }
            }
        }

        public static IEnumerable<T> ExecuteTransactional<T>(DbContext context, string command, int userId)
        {
            var result = new List<T>();
            using (context)
            {
                context.Database.Initialize(false);

                DbCommand contextCmd = context.Database.Connection.CreateCommand();
                contextCmd.CommandText = "[dbo].[P_USER_IDCONTEXT_UPDATE]";
                contextCmd.CommandType = CommandType.StoredProcedure;
                contextCmd.Parameters.Add(new SqlParameter("UserId", userId));

                DbCommand procCommand = context.Database.Connection.CreateCommand();
                procCommand.CommandText = command;
                procCommand.CommandType = CommandType.Text;

                context.Database.Connection.Open();
               
                DbTransaction trans = context.Database.Connection.BeginTransaction(IsolationLevel.ReadUncommitted);
                try
                {
                    contextCmd.Transaction = trans;
                    contextCmd.ExecuteNonQuery();
                    procCommand.Transaction = trans;

                    DbDataReader reader = procCommand.ExecuteReader();

                    result = ((IObjectContextAdapter)context)
                        .ObjectContext
                        .Translate<T>(reader).ToList();

                    reader.Close();
                    trans.Commit();
                    return result;
                }
                catch (SqlException ex)
                {
                    trans.Rollback();
                    throw ex;
                }
                finally
                {
                    context.Database.Connection.Close();
                }
            }
        }

        /// <summary>
        /// Overload: Execute stored procedure with transaction (if applicable). This method will return the DataReader. 
        /// Note this will return and use the correct DbContext based on the Context Type (Example: VmsDataContext, OpaDataContext, MvaDataContext)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="parameters"></param>
        /// <param name="commandName"></param>
        /// <param name="userId"></param>
        /// <param name="outputPara"></param>
        /// <returns></returns>
        public static IEnumerable<IDataRecord> ExecuteTransactional(DbContext context, IEnumerable<SqlParameter> parameters, string commandName, int userId, string outputPara = "")
        {
            var contextName = context.GetType().Name;
            switch (contextName)
            {
                case "DataContext1":
                    using (context)
                    {
                        context.Database.Initialize(false);
                        var contextCmd = context.Database.Connection.CreateCommand();
                        contextCmd.CommandText = "[dbo].[P_CONTEXT]";
                        contextCmd.CommandType = CommandType.StoredProcedure;
                        contextCmd.Parameters.Add(new SqlParameter("UserId", userId));
                        var procCommand = context.Database.Connection.CreateCommand();
                        procCommand.CommandText = commandName;
                        procCommand.CommandType = CommandType.StoredProcedure;
                        if (parameters != null)
                        {
                            procCommand.Parameters.AddRange(parameters.ToArray());
                        }
                        context.Database.Connection.Open();
                        var trans = context.Database.Connection.BeginTransaction(IsolationLevel.ReadUncommitted);
                        try
                        {
                            contextCmd.Transaction = trans;
                            contextCmd.ExecuteNonQuery();

                            procCommand.Transaction = trans;
                            
                            using (var reader = procCommand.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    while (reader.Read())
                                    {
                                        yield return reader;
                                    }
                                }
                            }
                        }
                        finally
                        {
                            try
                            {
                                trans.Commit();
                            }
                            catch (InvalidOperationException)
                            {
                                // transaction was already closed or disposed... moving along...
                                // TODO: this was always happening when calling from ClipFunctions.GetClipsFromMaterial. Could not find out why...
                            }
                            context.Database.Connection.Close();
                        }
                    }
                    break;
                case "DataContext2":
                    using (context)
                    {
                        context.Database.Initialize(false);
                        var procCommand = context.Database.Connection.CreateCommand();
                        procCommand.CommandText = commandName;
                        procCommand.CommandType = CommandType.StoredProcedure;
                        if (parameters != null)
                        {
                            procCommand.Parameters.AddRange(parameters.ToArray());
                        }
                        context.Database.Connection.Open();
                        var trans = context.Database.Connection.BeginTransaction(IsolationLevel.ReadUncommitted);
                        try
                        {
                            procCommand.Transaction = trans;
                            using (DbDataReader reader = procCommand.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    while (reader.Read())
                                    {
                                        yield return reader;
                                    }
                                }
                            }
                        }
                        finally
                        {
                            trans.Commit();
                            context.Database.Connection.Close();
                        }
                    }
                    break;
                case "DataContext3":
                    using (context)
                    {
                        context.Database.Initialize(false);
                        var procCommand = context.Database.Connection.CreateCommand();
                        procCommand.CommandText = commandName;
                        procCommand.CommandType = CommandType.StoredProcedure;
                        if (parameters != null)
                        {
                            procCommand.Parameters.AddRange(parameters.ToArray());
                        }
                        context.Database.Connection.Open();
                        var trans = context.Database.Connection.BeginTransaction(IsolationLevel.ReadUncommitted);
                        try
                        {
                            procCommand.Transaction = trans;
                            using (var reader = procCommand.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    while (reader.Read())
                                    {
                                        yield return reader;
                                    }
                                }
                            }
                        }
                        finally
                        {
                            trans.Commit();
                            context.Database.Connection.Close();
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Execute stored procedure with transaction. 
        /// This method will map automatically to the specified type that is not an exact match. 
        /// The method will remove underscores and spaces as well as do a best match for each member.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="parameters"></param>
        /// <param name="commandName"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static IEnumerable<T> ExecuteAmbiguousTransactional<T>(DbContext context, IEnumerable<SqlParameter> parameters, string commandName, int? userId)
        {
            var result = new List<T>();
            using (context)
            {
                context.Database.Initialize(false);
                DbTransaction trans = null;
                DbCommand contextCmd = context.Database.Connection.CreateCommand();
                contextCmd.CommandText = "[dbo].[P_USER]";
                contextCmd.CommandType = CommandType.StoredProcedure;
                contextCmd.Parameters.Add(new SqlParameter("UserId", userId));
                

                DbCommand command = context.Database.Connection.CreateCommand();
                command.CommandText = commandName;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = (int)context.Database.CommandTimeout;

                context.Database.Connection.Open();
                foreach (SqlParameter sqlParameter in parameters)
                {
                    command.Parameters.Add(sqlParameter);
                }

                // if a null user is passed then i am assuming that we are hitting a db that doesn't have a P_USER_IDCONTEXT_UPDATE

                if (userId != null)
                {
                    trans = context.Database.Connection.BeginTransaction(IsolationLevel.ReadUncommitted);
                    contextCmd.Transaction = trans;
                    contextCmd.ExecuteNonQuery();
                    command.Transaction = trans;
                }
                try
                {
                    //execute the reader for the procedure passed
                    using (DbDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader.HasRows)
                            {
                                object instance = ConverterDataHelper.GetInstanceOfObject(typeof(T));
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    // setting up some key words
                                    var numberWordIndicators = new[] { "count", "id", "num", "number" };
                                    var currentType = reader.GetFieldType(i);
                                    string currentField = reader.GetName(i);
                                    if (currentField.Contains("_") || currentField.Contains(" "))
                                    {
                                        currentField = currentField.Replace("_", "").Replace(" ", "");
                                    }
                                    var instancePropertyNames = instance.GetType().GetProperties(BindingFlags.Public
                                                                                                 | BindingFlags.Instance
                                                                                                 |
                                                                                                 BindingFlags.IgnoreCase)
                                        .Select(p => p.Name);
                                    var partialMatchKeyValuePair = PropertyContains(instancePropertyNames, currentField);
                                    var typeInInstance = instance.GetType()
                                        .GetProperty(currentField, BindingFlags.Public
                                                                   | BindingFlags.Instance
                                                                   | BindingFlags.IgnoreCase);

                                    var value = reader.GetValue(i) == DBNull.Value ? null : reader.GetValue(i);
                                    var intValue = 0;
                                    var tryConvertionToInt = false;
                                    if (value != null)
                                    {
                                        bool isInt = int.TryParse(value.ToString(), out intValue);

                                        tryConvertionToInt = (isInt && currentType != typeof(int));
                                    }
                                    if (typeInInstance != null)
                                    {
                                        // I need to check if the type is nullable if it is i want to extract the generic base type to see if i can get a match
                                        Type genericBaseType = null;

                                        if (typeInInstance.PropertyType.IsGenericType &&
                                            typeInInstance.PropertyType.GetGenericTypeDefinition() ==
                                            typeof(Nullable<>))
                                        {
                                            genericBaseType = typeInInstance.PropertyType.GetGenericArguments()[0];
                                        }
                                        //if types match and we found a suitable property then try to insert a value
                                        if ((typeInInstance.PropertyType == currentType) ||
                                            (genericBaseType == currentType))
                                        {
                                            try
                                            {
                                                instance.GetType()
                                                    .GetProperty(currentField, BindingFlags.Public
                                                                               | BindingFlags.Instance
                                                                               | BindingFlags.IgnoreCase)
                                                    .SetValue(instance, value);
                                            }
                                            catch
                                            {
                                                // ignored
                                                // swallowing exceptions here because we may have more returning columns then there are fields in our instance.
                                            }

                                        }
                                        // if properties do not match and the current data reader column contains the word "ID" we try to convert the data row value to integer
                                        else if (typeInInstance.PropertyType != currentType &&
                                                 (numberWordIndicators.Any(w => currentField.ToLower().Contains(w))))
                                        {

                                            // checking to see if the value can be converted to integer
                                            if (tryConvertionToInt)
                                            {
                                                if (intValue > 0)
                                                {
                                                    instance.GetType()
                                                        .GetProperty(currentField, BindingFlags.Public
                                                                                   | BindingFlags.Instance
                                                                                   | BindingFlags.IgnoreCase)
                                                        .SetValue(instance, intValue);
                                                }
                                            }
                                        }
                                    }
                                    else if (partialMatchKeyValuePair.Value)
                                    {

                                        var partialMatchType = instance.GetType()
                                            .GetProperty(partialMatchKeyValuePair.Key, BindingFlags.Public
                                                                                       | BindingFlags.Instance
                                                                                       | BindingFlags.IgnoreCase)
                                            .PropertyType;
                                        if (partialMatchType == currentType)
                                        {
                                            // this is to support properties that maybe be partial matches.
                                            // example: ParentContentOwner/ContentOwner 
                                            instance.GetType()
                                                .GetProperty(partialMatchKeyValuePair.Key, BindingFlags.Public
                                                                                           | BindingFlags.Instance
                                                                                           | BindingFlags.IgnoreCase)
                                                .SetValue(instance, value);
                                        }
                                        else if (partialMatchType != currentType &&
                                                 numberWordIndicators.Any(w => currentField.ToLower().Contains(w)))
                                        {
                                            if (intValue > 0)
                                            {
                                                instance.GetType()
                                                    .GetProperty(partialMatchKeyValuePair.Key, BindingFlags.Public
                                                                                               |
                                                                                               BindingFlags.Instance
                                                                                               |
                                                                                               BindingFlags
                                                                                                   .IgnoreCase)
                                                    .SetValue(instance, intValue);
                                            }
                                        }
                                    }
                                }
                                result.Add((T)instance);
                            }
                        }
                    }
                    //return the out parameter value
                    if (command.Parameters.Count > 2)
                    {
                        parameters.First().Value = command.Parameters[0].Value;
                    }

                    if (contextCmd.Transaction != null)
                    {
                        trans.Commit();
                    }
                    return result;
                }
                catch (SqlException ex)
                {
                    trans.Rollback();
                    throw ex;
                }
                finally
                {
                    context.Database.Connection.Close();
                }
            }
        }

        private static KeyValuePair<string, bool> PropertyContains(IEnumerable<string> instanceNames,
            string currentProperty)
        {
            foreach (var instanceName in instanceNames)
            {
                var currentPropertyLength = currentProperty.Length;
                var instanceNameLenth = instanceName.Length;

                // check witch way to compare
                if (instanceNameLenth >= currentPropertyLength)
                {
                    var indexOfCurrentProperty = instanceName.IndexOf(currentProperty,
                        StringComparison.OrdinalIgnoreCase);
                    if (indexOfCurrentProperty != -1)
                    {
                        if (indexOfCurrentProperty + currentPropertyLength < instanceNameLenth &&
                            (currentProperty.IndexOf("Id", StringComparison.OrdinalIgnoreCase) > -1 &&
                             instanceName.IndexOf("Id", StringComparison.OrdinalIgnoreCase) > -1) ||
                            (currentProperty.IndexOf("Id", StringComparison.OrdinalIgnoreCase) == -1 &&
                             instanceName.IndexOf("Id", StringComparison.OrdinalIgnoreCase) == -1)
                            )
                        {
                            return new KeyValuePair<string, bool>(instanceName, true);
                        }
                    }
                }

                if (currentPropertyLength >= instanceNameLenth)
                {
                    var indexOfInstanceName = currentProperty.IndexOf(instanceName, StringComparison.OrdinalIgnoreCase);
                    if (indexOfInstanceName != -1)
                    {
                        if (currentPropertyLength >= indexOfInstanceName + instanceNameLenth &&
                            (
                                (currentProperty.IndexOf("Id", StringComparison.OrdinalIgnoreCase) > -1 &&
                                 instanceName.IndexOf("Id", StringComparison.OrdinalIgnoreCase) > -1) ||
                                (currentProperty.IndexOf("Id", StringComparison.OrdinalIgnoreCase) == -1 &&
                                 instanceName.IndexOf("Id", StringComparison.OrdinalIgnoreCase) == -1)
                                )
                            )
                        {
                            return new KeyValuePair<string, bool>(instanceName, true);
                        }
                    }
                }
            }
            return new KeyValuePair<string, bool>("false", false);
        }

        public static T TryGetValue<T>(this IDataRecord record, string fieldName, T defaultValue = default(T))
        {
            try
            {
                int index = record.GetOrdinal(fieldName);
                return !record.IsDBNull(index) ? (T) record.GetValue(index) : defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        public static void ExecuteUpdateSessionContext(DbContext context, int userId)
        {
            var contextName = context.GetType().Name;
            context.Database.Initialize(false);
            var contextCmd = context.Database.Connection.CreateCommand();
            contextCmd.CommandText = "[dbo].[P_USER]";
            contextCmd.CommandType = CommandType.StoredProcedure;
            contextCmd.Parameters.Add(new SqlParameter("UserId", userId));
            var procCommand = context.Database.Connection.CreateCommand();
            context.Database.Connection.Open();
            var trans = context.Database.Connection.BeginTransaction(IsolationLevel.ReadUncommitted);
            try
            {
                contextCmd.Transaction = trans;
                contextCmd.ExecuteNonQuery();
            }
            finally
            {
                try
                {
                    trans.Commit();
                }
                catch (InvalidOperationException)
                {
                }
            }
        }
    }
}