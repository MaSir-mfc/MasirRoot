using Masir.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masir
{
    /// <summary>
    /// 数据库操作基类，提供数据库操作的基层封装方法
    /// </summary>
    public abstract class MaDataHelper : IDisposable
    {

        #region 事务处理

        /// <summary>
        /// 是否使用事务
        /// </summary>
        protected bool m_ifUseTransaction;
        /// <summary>
        /// 是否使用事务
        /// </summary>
        public bool IfUseTransaction
        {
            get { return m_ifUseTransaction; }
            set { m_ifUseTransaction = value; }
        }

        /// <summary>
        /// 是否开始事务
        /// </summary>
        protected bool m_isBeginTransaction;
        /// <summary>
        /// 是否开始事务
        /// </summary>
        public bool IsBeginTransaction
        {
            get { return m_isBeginTransaction; }
            set { m_isBeginTransaction = value; }
        }

        private IDbTransaction m_dataTransaction;
        /// <summary>
        /// 获得当前事务
        /// </summary>
        protected IDbTransaction GetNonceTransaction()
        {
            if (m_ifUseTransaction)
            {
                if (m_dataTransaction == null)
                {
                    m_isBeginTransaction = true;
                    m_dataTransaction = m_dataConnection.BeginTransaction();
                }
                return m_dataTransaction;
            }
            else
            {
                throw new Exception("没有启动事务");
            }

        }

        /// <summary>
        /// 事务提交
        /// </summary>
        public virtual void Commit()
        {
            if (m_ifUseTransaction)
            {
                m_dataTransaction.Commit();
            }
            else
            {
                throw new Exception("没有启动事务，无法执行事务提交");
            }
        }

        /// <summary>
        /// 事务回滚
        /// </summary>
        public virtual void Rollback()
        {
            if (m_ifUseTransaction)
            {
                if (m_dataTransaction != null)
                {
                    m_dataTransaction.Rollback();
                }
            }
            else
            {
                throw new Exception("没有启动事务，无法执行事务提交");
            }
        }

        #endregion

        #region 数据库对象

        /// <summary>
        /// 设置数据库链接字符串
        /// </summary>
        protected string m_connString;

        /// <summary>
        /// 设置数据库链接字符串
        /// </summary>
        public string ConnString
        {
            set { m_connString = value; }
        }

        /// <summary>
        /// 获得数据库链接对象
        /// </summary>
        protected IDbConnection m_dataConnection;

        /// <summary>
        /// 获得数据库链接对象
        /// </summary>
        public IDbConnection DataConnection
        {
            get { return m_dataConnection; }
        }

        /// <summary>
        /// 创建一个连接对象
        /// </summary>
        public IDbCommand DataCommand
        {
            get { return m_dataConnection.CreateCommand(); }
        }

        /// <summary>
        /// 获取或设置存储过程需要使用的参数值，参数名带@
        /// </summary>
        protected Hashtable m_spFileValue;

        /// <summary>
        /// 获取或设置存储过程需要使用的参数值，参数名带@
        /// </summary>
        public Hashtable SpFileValue
        {
            get { return m_spFileValue; }
            set { m_spFileValue = value; }
        }

        #endregion

        /// <summary>
        /// 开启数据库链接
        /// </summary>
        protected virtual void Open()
        {
            if (m_dataConnection.State == ConnectionState.Closed)
            {
                m_dataConnection.ConnectionString = m_connString;
                m_dataConnection.Open();
            }
        }

        #region 执行SQL语句

        /// <summary>
        /// 执行SQL语句并返回受影响的行数
        /// </summary>
        /// <param name="sqlStr"></param>
        /// <returns></returns>
        public virtual int SqlExecute(string sqlStr)
        {
            Open();
            using (IDbCommand cmd = m_dataConnection.CreateCommand())
            {
                if (m_ifUseTransaction)
                {
                    cmd.Transaction = GetNonceTransaction();
                }
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sqlStr;
                return cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 执行SQL语句，并返回查询所返回的结果集中第一行的第一列。忽略其他列或行
        /// </summary>
        /// <param name="sqlStr">SQL语句</param>
        /// <returns></returns>
        public virtual object SqlScalar(string sqlStr)
        {
            Open();
            using (IDbCommand cmd = m_dataConnection.CreateCommand())
            {
                if (m_ifUseTransaction)
                {
                    cmd.Transaction = GetNonceTransaction();
                }
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sqlStr;
                return cmd.ExecuteScalar();
            }
        }

        /// <summary>
        /// 执行SQL语句，获得数据表
        /// </summary>
        /// <param name="sqlStr">SQL语句</param>
        /// <returns></returns>
        public virtual DataTable SqlGetDataTable(string sqlStr)
        {
            Open();
            using (IDbCommand cmd = m_dataConnection.CreateCommand())
            {
                if (m_ifUseTransaction)
                {
                    cmd.Transaction = GetNonceTransaction();
                }
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sqlStr;
                IDataReader dr = cmd.ExecuteReader();
                DataTable tempDT = new DataTable("tempDT1");
                tempDT.Load(dr, LoadOption.Upsert);
                return tempDT;
            }
        }

        /// <summary>
        /// 执行SQL语句，返回只进结果集流的读取方法
        /// </summary>
        /// <param name="sqlStr">SQL语句</param>
        /// <returns></returns>
        public virtual IDataReader SqlGetDataReader(string sqlStr)
        {
            Open();
            using (IDbCommand cmd = m_dataConnection.CreateCommand())
            {
                if (m_ifUseTransaction)
                {
                    cmd.Transaction = GetNonceTransaction();
                }
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sqlStr;
                return cmd.ExecuteReader();
            }
        }

        /// <summary>
        /// 执行SQL语句，返回只进结果集流的读取方法
        /// </summary>
        /// <param name="sqlStr"></param>
        /// <param name="behavior"></param>
        /// <returns></returns>
        public virtual IDataReader SqlGetDataReader(string sqlStr, CommandBehavior behavior)
        {
            Open();
            using (IDbCommand cmd = m_dataConnection.CreateCommand())
            {
                if (m_ifUseTransaction)
                {
                    cmd.Transaction = GetNonceTransaction();
                }
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sqlStr;
                return cmd.ExecuteReader(behavior);
            }
        }
        #endregion

        #region 执行带参数的SQL语句

        /// <summary>
        /// 执行带参数的SQL语句
        /// </summary>
        /// <param name="sqlStr">SQL语句</param>
        /// <param name="parameters">SQL语句中的参数，参数名带@</param>
        /// <returns></returns>
        public virtual int SqlExecute(string sqlStr, Hashtable parameters)
        {
            throw new Exception("该数据库操作类没有实现该方法");
        }
        /// <summary>
        /// 执行带参数的SQL语句
        /// </summary>
        /// <param name="sqlStr"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual System.Data.DataTable SqlGetDataTable(string sqlStr, Hashtable parameters)
        {
            throw new Exception("该数据库操作类没有实现该方法");
        }
        /// <summary>
        /// 执行带参数的SQL语句
        /// </summary>
        /// <param name="sqlStr"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual object SqlScalar(string sqlStr, Hashtable parameters)
        {
            throw new Exception("该数据库操作类没有实现该方法");
        }

        #endregion

        #region 执行存储过程

        /// <summary>
        /// 执行存储过程，返回受影响的行数。
        /// </summary>
        /// <param name="spName"></param>
        /// <returns></returns>
        public abstract int SpExecute(string spName);

        /// <summary>
        /// 执行存储过程，并返回查询所返回的结果集中第一行的第一列。忽略其他列或行。
        /// </summary>
        /// <param name="spName"></param>
        /// <returns></returns>
        public abstract object SpScalar(string spName);

        /// <summary>
        /// 执行存储过程，获得数据表(适用返回一张表的情况)
        /// </summary>
        /// <param name="spName"></param>
        /// <returns></returns>
        public abstract System.Data.DataTable SpGetDataTable(string spName);

        /// <summary>
        /// 执行存储过程，获得DataSet（适用返回多张表的情况）
        /// </summary>
        /// <param name="spName"></param>
        /// <returns></returns>
        public abstract System.Data.DataSet SpGetDataSet(string spName);


        /// <summary>
        /// 执行存储过程，返回只进结果集流的读取方法。
        /// </summary>
        /// <param name="spName"></param>
        /// <returns></returns>
        public abstract System.Data.IDataReader SpGetDataReader(string spName);

        /// <summary>
        /// 执行存储过程，获得返回值
        /// </summary>
        /// <param name="spName"></param>
        /// <returns></returns>
        public abstract int SpGetReturnValue(string spName);

        /// <summary>
        /// 执行存储过程，获得返回值
        /// </summary>
        /// <param name="spName">存储过程名称</param>
        /// <param name="returnKey">返回值的键名</param>
        /// <param name="returnValue">返回值的键值表</param>
        /// <returns></returns>
        public abstract int SpGetReturnValue(string spName, string[] returnKey, ref System.Collections.Hashtable returnValue);

        /// <summary>
        ///  执行存储过程，获得返回值
        /// </summary>
        /// <param name="spName">存储过程名称</param>
        /// <param name="returnKey">返回值的键名</param>
        /// <param name="returnValue">返回值的键值表</param>
        /// <param name="returnTable">返回的数据表</param>
        /// <returns></returns>
        public abstract int SpGetReturnValue(string spName, string[] returnKey, ref System.Collections.Hashtable returnValue, ref System.Data.DataTable returnTable);

        #endregion

        #region 提取数据操作类的工厂方法

        /// <summary>
        /// 获得数据操作类
        /// </summary>
        /// <param name="name">数据库名称，与Database.config中配置的一致</param>
        /// <returns></returns>
        public static MaDataHelper GetDataHelper(string name)
        {
            DataBaseInfo _connStrInfo = DataBaseConfig.Instance.GetDataBaseInfoByCache(name);
            return GetDataHelper(_connStrInfo);
        }
        /// <summary>
        /// 获得数据库操作类
        /// </summary>
        /// <param name="database">数据库信息</param>
        /// <returns></returns>
        public static MaDataHelper GetDataHelper(DataBaseInfo database)
        {
            return GetDataHelper(database.ConnType, database.ConnString);
        }

        /// <summary>
        /// 获得数据库操作类
        /// </summary>
        /// <param name="type">数据库类型</param>
        /// <param name="conn">连接字符</param>
        /// <returns></returns>
        public static MaDataHelper GetDataHelper(DataBaseType type, string conn)
        {
            switch (type)
            {
                case DataBaseType.Sql:
                    {
                        return new SqlDataHelper(conn);
                    }
                case DataBaseType.Oracle:
                    break;
                case DataBaseType.Access:
                    break;
                case DataBaseType.MySql:
                    break;
                case DataBaseType.Db:
                    break;
                default:
                    {
                        return new SqlDataHelper(conn);
                    }

            }
            return new SqlDataHelper(conn);
        }

        #endregion


        #region IDisposable成员

        bool disposed = false;
        /// <summary>
        /// 接口实现
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 初始化数据库连接状态
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (!disposing)
                    return;

                if (m_dataConnection != null)
                {
                    if (m_dataConnection.State != ConnectionState.Closed)
                    {
                        m_dataConnection.Close();
                    }
                    m_dataConnection.Dispose();
                    m_dataConnection = null;
                    m_dataTransaction = null;
                    m_connString = null;
                    m_spFileValue = null;
                }
                disposed = true;
            }
        }

        #endregion
    }
}
