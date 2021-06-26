using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;

using System.Data;

using System.Reflection;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyFavor.WebApi.Common
{
    public class DBHelper : DbContext
    {
        private IDbConnection connection = null;
        private SqlCommand command = null;
        //public DBHelper(DbContextOptions<DBHelper> options) : base(options)
        //{
        //    if (null == connection)
        //        connection = new SqlConnection("User ID=ecisuser;Password=sa123456; Initial Catalog=MyFavorRepos;Data Source=DESKTOP-913BTVJ;Connection Timeout=60;");//这个GetDbConnection需要在NuGet中添加Microsoft.AspNetCore.App

        //    if (connection.State == ConnectionState.Closed)
        //        connection.Open();
        //    if (command == null)
        //        command = connection.CreateCommand() as SqlCommand;

        //}

        public DBHelper(string connectionString)
        {

            if (null == connection)
                connection = new SqlConnection(connectionString);//这个GetDbConnection需要在NuGet中添加Microsoft.AspNetCore.App


        }

        public int ExecuteNonQuery(string SqlText, SqlParameter[] parameters = null)
        {
            try
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                command = connection.CreateCommand() as SqlCommand;
                command.CommandText = SqlText;
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }
                return command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                connection.Close();
            }
        }

        public int Insert<T>(T table)
        {
            try
            {
                command.CommandText = GetInsertSqlStr(table, command.Parameters);
                return command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable ExecSqlStr(string sql, Dictionary<string, object> Parameters = null)
        {
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            command = connection.CreateCommand() as SqlCommand;

            command.CommandText = sql;
            if (Parameters != null)
            {
                foreach (var str in Parameters.Keys)
                {
                    var value = Parameters.GetValueOrDefault(str);
                    command.Parameters.Add(
                        new SqlParameter()
                        {
                            ParameterName = "@" + str,
                            Value = value,
                            DbType = GetDbType(value.GetType())
                        }
                        );
                }
            }
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = command;
            DataSet myDataSet = new DataSet();
            da.Fill(myDataSet);
            DataTable db = myDataSet.Tables[0];
            return db;
        }


        private string GetInsertSqlStr<T>(T table, SqlParameterCollection sqlParameters)
        {
            string strSql = "insert into " + typeof(T).Name + " (";
            //获得泛型类型的公共属性
            var pros = typeof(T).GetProperties().Where(pi => !Attribute.IsDefined(pi, typeof(NotMappedAttribute))).ToArray();
            string values = "";
            foreach (PropertyInfo p in pros)
            {
                strSql += p.Name + ",";
                values += "@" + p.Name + ",";

                sqlParameters.Add(new SqlParameter()
                {
                    ParameterName = "@" + p.Name,
                    Value = p.GetValue(table),
                    DbType = GetDbType(p.PropertyType)
                });
            }
            values = values.Substring(0, values.Length - 1);
            strSql = strSql.Substring(0, strSql.Length - 1) + ") values (" + values + ")";
            return strSql;
        }

        private DbType GetDbType(Type t)
        {
            switch (Type.GetTypeCode(t))
            {
                case TypeCode.Boolean:
                    return DbType.Boolean;
                case TypeCode.Byte:
                    return DbType.Byte;
                case TypeCode.DateTime:
                    return DbType.DateTime;
                case TypeCode.Decimal:
                    return DbType.Decimal;
                case TypeCode.Double:
                    return DbType.Double;
                case TypeCode.Int16:
                    return DbType.Int16;
                case TypeCode.Int32:
                    return DbType.Int32;
                case TypeCode.Int64:
                    return DbType.Int64;
                case TypeCode.String:
                    return DbType.String;
                default:
                    return DbType.Object;
            }
        }

    }
}
