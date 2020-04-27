using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Reflection;
using System.Windows;

namespace SQLiteWPF.Utilities
{
    /// <summary>
    /// 数据访问基础类(基于SQLite)
    /// 可以用户可以修改满足自己项目的需要。
    /// </summary>

    public class SQLiteHelper
    {
        // <summary>
        /// 数据库连接字符串 可以动态更改connectionString支持多数据库.
        /// </summary>
        private static string connectionString = "Data Source=" + @"Company.db3;";

        #region 公用方法

       
        public static bool Exists(string strSql)
        {
            object obj = GetSingle(strSql);
            int cmdresult;
            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
            {
                cmdresult = 0;
            }
            else
            {
                cmdresult = int.Parse(obj.ToString());
            }
            if (cmdresult == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public static bool Exists(string strSql, params SQLiteParameter[] cmdParms)
        {
            object obj = GetSingle(strSql, cmdParms);
            int cmdresult;
            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
            {
                cmdresult = 0;
            }
            else
            {
                cmdresult = int.Parse(obj.ToString());
            }
            if (cmdresult == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        #endregion

        #region  执行简单SQL语句

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public static int ExecuteSql(string SQLString)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (System.Data.SQLite.SQLiteException E)
                    {
                        connection.Close();
                        throw new Exception(E.Message);
                    }
                }
            }
        }

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList">多条SQL语句</param>		
        public static void ExecuteSqlTran(ArrayList SQLStringList)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                using (SQLiteCommand cmd = new SQLiteCommand
                {
                    Connection = conn
                })
                {
                    SQLiteTransaction tx = conn.BeginTransaction();
                    cmd.Transaction = tx;
                    try
                    {
                        for (int n = 0; n < SQLStringList.Count; n++)
                        {
                            string strsql = SQLStringList[n].ToString();
                            if (strsql.Trim().Length > 1)
                            {
                                cmd.CommandText = strsql;
                                cmd.ExecuteNonQuery();
                            }
                        }
                        tx.Commit();
                    }
                    catch (SQLiteException E)
                    {
                        tx.Rollback();
                        throw new Exception(E.Message);
                    }
                }
            }
        }
        /// <summary>
        /// 执行带一个存储过程参数的的SQL语句。
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <param name="content">参数内容,比如一个字段是格式复杂的文章，有特殊符号，可以通过这个方式添加</param>
        /// <returns>影响的记录数</returns>
        public static int ExecuteSql(string SQLString, string content)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(SQLString, connection))
                {
                    SQLiteParameter myParameter = new SQLiteParameter("@content", DbType.String)
                    {
                        Value = content
                    };
                    cmd.Parameters.Add(myParameter);
                    try
                    {
                        connection.Open();
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (System.Data.SQLite.SQLiteException E)
                    {
                        throw new Exception(E.Message);
                    }
                    finally
                    {
                        cmd.Dispose();
                        connection.Close();
                    }
                }
            }
        }
        /// <summary>
        /// 向数据库里插入图像格式的字段(和上面情况类似的另一种实例)
        /// </summary>
        /// <param name="strSQL">SQL语句</param>
        /// <param name="fs">图像字节,数据库的字段类型为image的情况</param>
        /// <returns>影响的记录数</returns>
        public static int ExecuteSqlInsertImg(string strSQL, byte[] fs)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(strSQL, connection))
                {
                    SQLiteParameter myParameter = new SQLiteParameter("@fs", DbType.Binary)
                    {
                        Value = fs
                    };
                    cmd.Parameters.Add(myParameter);
                    try
                    {
                        connection.Open();
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (System.Data.SQLite.SQLiteException E)
                    {
                        throw new Exception(E.Message);
                    }
                    finally
                    {
                        cmd.Dispose();
                        connection.Close();
                    }
                }
            }
        }

        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果（object）。
        /// </summary>
        /// <param name="SQLString">计算查询结果语句</param>
        /// <returns>查询结果（object）</returns>
        public static object GetSingle(string SQLString)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        object obj = cmd.ExecuteScalar();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (System.Data.SQLite.SQLiteException e)
                    {
                        connection.Close();
                        throw new Exception(e.Message);
                    }
                }
            }
        }
        /// <summary>
        /// 执行查询语句，返回SQLiteDataReader
        /// </summary>
        /// <param name="strSQL">查询语句</param>
        /// <returns>SQLiteDataReader</returns>
        public static SQLiteDataReader ExecuteReader(string strSQL)
        {
            SQLiteConnection connection = new SQLiteConnection(connectionString);
            using (SQLiteCommand cmd = new SQLiteCommand(strSQL, connection))
            {
                try
                {
                    connection.Open();
                    SQLiteDataReader myReader = cmd.ExecuteReader();
                    return myReader;
                }
                catch (System.Data.SQLite.SQLiteException e)
                {
                    throw new Exception(e.Message);
                }
            }

        }
        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataSet</returns>
        public static DataSet Query(string SQLString)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    using (SQLiteDataAdapter command = new SQLiteDataAdapter(SQLString, connection))
                    {
                        command.Fill(ds, "ds");
                    }
                }
                catch (System.Data.SQLite.SQLiteException ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
        }
        /// <summary>
        /// 执行查询语句，返回List
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataSet</returns>
        public static IList<T> GetList<T>(string SQLString)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    using (SQLiteDataAdapter command = new SQLiteDataAdapter(SQLString, connection))
                    {
                        command.Fill(ds);
                    }
                }
                catch (System.Data.SQLite.SQLiteException ex)
                {
                    throw new Exception(ex.Message);
                }
                return DataSetToList<T>(ds, 0);
            }
        }


        #endregion

        #region 执行带参数的SQL语句

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public static int ExecuteSql(string SQLString, params SQLiteParameter[] cmdParms)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                        int rows = cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        return rows;
                    }
                    catch (System.Data.SQLite.SQLiteException E)
                    {
                        throw new Exception(E.Message);
                    }
                }
            }
        }


        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList">SQL语句的哈希表（key为sql语句，value是该语句的SQLiteParameter[]）</param>
        public static void ExecuteSqlTran(Hashtable SQLStringList)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                using (SQLiteTransaction trans = conn.BeginTransaction())
                {
                    using (SQLiteCommand cmd = new SQLiteCommand())
                    {
                        try
                        {
                            //循环
                            foreach (DictionaryEntry myDE in SQLStringList)
                            {
                                string cmdText = myDE.Key.ToString();
                                SQLiteParameter[] cmdParms = (SQLiteParameter[])myDE.Value;
                                PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
                                int val = cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();

                                trans.Commit();
                            }
                        }
                        catch
                        {
                            trans.Rollback();
                            throw;
                        }
                    }
                }
            }
        }

        /// 执行多条SQL列表语句，实现数据库事务。
        /// </summary>
        /// <param name="sqlStringList">多条SQL语句</param>
        /// <param name="commandParametersList">对应SQL语句列表中每一条语句的参数集合列表</param>
        /// <param name="cmdType">命令类型(存储过程, 文本, 等等)</param>
        public static void ExecuteBatchSql(IList<string> sqlStringList, IList<SQLiteParameter[]> commandParametersList)
        {
            if (sqlStringList.Count == 0 || commandParametersList.Count == 0 || sqlStringList.Count != commandParametersList.Count)
            {
                throw new Exception("传入参数错误");
            }

            //创建SQLiteCommand执行命令语句对象
            using (SQLiteCommand cmd = new SQLiteCommand())
            {
                //使用using语句，方便using语句中声明的对象自动被Dispose
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    //打开数据库连接
                    connection.Open();
                    //SQLiteCommand执行命令语句对象添加数据库连接对象
                    cmd.Connection = connection;
                    //创建事务
                    SQLiteTransaction tx = connection.BeginTransaction();
                    //SQLiteCommand执行命令语句对象添加事务对象
                    cmd.Transaction = tx;
                    try
                    {
                        //遍历SQL语句，依次执行
                        for (int n = 0; n < sqlStringList.Count; n++)
                        {
                            try
                            {
                                //执行命令前预处理
                                PrepareCommand(cmd, connection, null, sqlStringList[n], commandParametersList[n]);

                                //执行非查询数据库操作
                                cmd.ExecuteNonQuery();

                                //清空传入的参数
                                cmd.Parameters.Clear();
                            }
                            catch (Exception e)
                            {
                                MessageBox.Show(e.ToString());
                            }

                            /*
                            string strsql = SQLStringList[n].ToString();
                            //执行数据库相应操作
                            if (strsql.Trim().Length > 1)
                            {
                                cmd.CommandText = strsql;
                                cmd.ExecuteNonQuery();
                            }
                            */
                            /*
                            //每执行500条SQL语句就做一次事务提交
                            if (n > 0 && (n % 500 == 0 || n == SQLStringList.Count - 1))
                            {
                                //事务提交
                                tx.Commit();
                                //重新开启事务
                                tx = conn.BeginTransaction();
                            }
                            */
                        }

                        //一次性提交事务
                        tx.Commit();
                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
                        tx.Rollback();
                        Console.WriteLine(e.GetType() + ":" + e.Message);
                        throw new Exception(e.Message);
                    }
                }

            }
        }


        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果（object）。
        /// </summary>
        /// <param name="SQLString">计算查询结果语句</param>
        /// <returns>查询结果（object）</returns>
        public static object GetSingle(string SQLString, params SQLiteParameter[] cmdParms)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                        object obj = cmd.ExecuteScalar();
                        cmd.Parameters.Clear();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (System.Data.SQLite.SQLiteException e)
                    {
                        throw new Exception(e.Message);
                    }
                }
            }
        }

        /// <summary>
        /// 执行查询语句，返回SQLiteDataReader
        /// </summary>
        /// <param name="strSQL">查询语句</param>
        /// <returns>SQLiteDataReader</returns>
        public static SQLiteDataReader ExecuteReader(string SQLString, params SQLiteParameter[] cmdParms)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                        SQLiteDataReader myReader = cmd.ExecuteReader();
                        cmd.Parameters.Clear();
                        return myReader;
                    }
                    catch (System.Data.SQLite.SQLiteException e)
                    {
                        throw new Exception(e.Message);
                    }
                }
            }

        }

        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataSet</returns>
        public static DataSet Query(string SQLString, params SQLiteParameter[] cmdParms)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                SQLiteCommand cmd = new SQLiteCommand();
                PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                using (SQLiteDataAdapter da = new SQLiteDataAdapter(cmd))
                {
                    using (DataSet ds = new DataSet())
                    {
                        try
                        {
                            da.Fill(ds, "ds");
                            cmd.Parameters.Clear();
                        }
                        catch (System.Data.SQLite.SQLiteException ex)
                        {
                            throw new Exception(ex.Message);
                        }
                        return ds;
                    }
                }
            }
        }
        /// <summary>
        /// 执行查询语句，返回List
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataSet</returns>
        public static IList<T> GetList<T>(string SQLString, params SQLiteParameter[] cmdParms)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                SQLiteCommand cmd = new SQLiteCommand();
                PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                using (SQLiteDataAdapter da = new SQLiteDataAdapter(cmd))
                {
                    using (DataSet ds = new DataSet())
                    {
                        try
                        {
                            da.Fill(ds);
                            cmd.Parameters.Clear();
                        }
                        catch (System.Data.SQLite.SQLiteException ex)
                        {
                            throw new Exception(ex.Message);
                        }
                        return DataSetToList<T>(ds, 0);
                    }
                }
            }
        }

        private static void PrepareCommand(SQLiteCommand cmd, SQLiteConnection conn, SQLiteTransaction trans, string cmdText, SQLiteParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null)
                cmd.Transaction = trans;
            cmd.CommandType = CommandType.Text;//cmdType;
            if (cmdParms != null)
            {
                foreach (SQLiteParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
        }

        #endregion
        /// <summary>
        /// DataSet数据转为IList集合
        /// </summary>
        /// <typeparam name="T">转换类型</typeparam>
        /// <param name="dataSet">数据源</param>
        /// <param name="tableIndex">需要转换表的索引</param>
        /// <returns></returns>
        public static IList<T> DataSetToList<T>(DataSet dataSet, int tableIndex)
        {
            //确认参数是否有效
            if (dataSet == null || dataSet.Tables.Count <= 0 || tableIndex < 0)
                return null;

            //声明DataTable对象，通过索引用来获取数据集DataSet中指定表
            DataTable dt = dataSet.Tables[tableIndex];

            //声明用于返回的集合对象
            IList<T> list = new List<T>();

            //遍历DataTable中每一行数据
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                //创建泛型对象
                T _t = Activator.CreateInstance<T>();

                //获取对象所有属性
                PropertyInfo[] propertyInfo = _t.GetType().GetProperties();

                //遍历DataTable中每一列
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    foreach (PropertyInfo info in propertyInfo)
                    {
                        //属性名称和列名相同时赋值
                        if (dt.Columns[j].ColumnName.ToUpper().Equals(info.Name.ToUpper()))
                        {
                            //判断当前列值是否为空
                            if (dt.Rows[i][j] != DBNull.Value)
                            {
                                //不为空，设置当前列值
                                try
                                {
                                    //info.SetValue(_t, dt.Rows[i][j], null);
                                    switch (info.GetMethod.ReturnParameter.ParameterType.Name)
                                    {
                                        case ("Int16"):
                                            info.SetValue(_t, Convert.ToInt16(dt.Rows[i][j]), null);
                                            break;
                                        case ("Int32"):
                                            info.SetValue(_t, Convert.ToInt32(dt.Rows[i][j]), null);
                                            break;
                                        case ("Int64"):
                                            info.SetValue(_t, Convert.ToInt64(dt.Rows[i][j]), null);
                                            break;
                                        case ("Double"):
                                            info.SetValue(_t, Convert.ToDouble(dt.Rows[i][j]), null);
                                            break;
                                        case ("Decimal"):
                                            info.SetValue(_t, Convert.ToDecimal(dt.Rows[i][j]), null);
                                            break;
                                        case ("Guid"):
                                            info.SetValue(_t, (System.Guid)(dt.Rows[i][j]), null);
                                            break;
                                        case ("String"):
                                            info.SetValue(_t, (string)(dt.Rows[i][j]), null);
                                            break;
                                        case ("Byte[]"):
                                            info.SetValue(_t, (byte[])(dt.Rows[i][j]), null);
                                            break;
                                        case ("Boolean"):
                                            info.SetValue(_t, (bool)(dt.Rows[i][j]), null);
                                            break;
                                        case ("DateTime"):
                                            info.SetValue(_t, Convert.ToDateTime(dt.Rows[i][j]), null);
                                            break;
                                        case ("Object"):
                                            info.SetValue(_t, dt.Rows[i][j], null);
                                            break;
                                        case ("Byte"):
                                            info.SetValue(_t, (byte)(dt.Rows[i][j]), null);
                                            break;
                                        case ("UInt16"):
                                            info.SetValue(_t, Convert.ToUInt16(dt.Rows[i][j]), null);
                                            break;
                                        case ("UInt32"):
                                            info.SetValue(_t, Convert.ToUInt32(dt.Rows[i][j]), null);
                                            break;
                                        case ("UInt64"):
                                            info.SetValue(_t, Convert.ToUInt64(dt.Rows[i][j]), null);
                                            break;
                                        default:
                                            throw new SystemException("Value is of unknown data type");
                                    }
                                }
                                catch (Exception e)
                                {
                                    MessageBox.Show(e.ToString());
                                }

                            }
                            else
                            {
                                //为空，设置null
                                info.SetValue(_t, null, null);
                            }
                            break;
                        }
                    }
                }
                list.Add(_t);
            }
            return list;
        }
    }
}
