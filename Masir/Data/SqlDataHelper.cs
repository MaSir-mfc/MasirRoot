using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masir.Data
{
    /// <summary>
    /// 提供sql数据库操作方法
    /// </summary>
    public class SqlDataHelper : MaDataHelper
    {
        #region 构造函数

        /// <summary>
        /// 默认无参构造
        /// </summary>
        public SqlDataHelper()
        {
            m_dataConnection = new SqlConnection();
            m_ifUseTransaction = false;
            m_spFileValue = new Hashtable();
        }
        /// <summary>
        /// 是否启动事务的构造
        /// </summary>
        /// <param name="ifUseTransaction"></param>
        public SqlDataHelper(bool ifUseTransaction)
        {
            m_dataConnection = new SqlConnection();
            m_ifUseTransaction = ifUseTransaction;
            m_spFileValue = new Hashtable();
        }
        /// <summary>
        /// 更改数据库链接字符串的构造
        /// </summary>
        /// <param name="connStr"></param>
        public SqlDataHelper(string connStr)
        {
            m_connString = connStr;
            m_dataConnection = new SqlConnection(connStr);
            m_ifUseTransaction = false;
            m_spFileValue = new Hashtable();
        }
        /// <summary>
        /// 更改数据库链接字符串并设置事务的构造
        /// </summary>
        /// <param name="connStr"></param>
        /// <param name="ifUseTransaction"></param>
        public SqlDataHelper(string connStr, bool ifUseTransaction)
        {
            m_connString = connStr;
            m_dataConnection = new SqlConnection(connStr);
            m_ifUseTransaction = ifUseTransaction;
            m_spFileValue = new Hashtable();
        }

        #endregion

        /// <summary>
        /// 从数据库中获得对象包含的参数名称（数据表、视图的数据列名称、存储过程参数）
        /// </summary>
        /// <param name="objectName">对象名称</param>
        /// <returns></returns>
        public List<string> GetDataObjectParameter(string objectName)
        {
            List<string> parameterList = new List<string>();
            string sql = @"SELECT syscolumns.name AS parameter
		FROM sysobjects, syscolumns
		WHERE sysobjects.id = syscolumns.id
		AND sysobjects.name = '" + objectName + @"'
		ORDER BY colid";

            DataTable _dt = SqlGetDataTable(sql);
            foreach (DataRow item in _dt.Rows)
            {
                parameterList.Add(item[0].ToString());
            }
            return parameterList;
        }

        #region 执行带参数的SQL语句
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlStr"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public override int SqlExecute(string sqlStr, Hashtable parameters)
        {
            Open();
            using (SqlCommand cmd = m_dataConnection.CreateCommand() as SqlCommand)
            {
                if (m_ifUseTransaction)
                {
                    cmd.Transaction = GetNonceTransaction() as SqlTransaction;
                }
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sqlStr;

                foreach (string item in parameters.Keys)
                {
                    cmd.Parameters.Add(new SqlParameter(item,parameters[item]));
                }
                return cmd.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlStr"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public override object SqlScalar(string sqlStr, Hashtable parameters)
        {
            Open();
            using (SqlCommand cmd = m_dataConnection.CreateCommand() as SqlCommand)
            {
                if (m_ifUseTransaction)
                {
                    cmd.Transaction = GetNonceTransaction() as SqlTransaction;
                }
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sqlStr;

                foreach (string item in parameters.Keys)
                {
                    cmd.Parameters.Add(new SqlParameter(item, parameters[item]));
                }
                return cmd.ExecuteScalar();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlStr"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public override DataTable SqlGetDataTable(string sqlStr, Hashtable parameters)
        {
            Open();
            using (SqlCommand cmd = m_dataConnection.CreateCommand() as SqlCommand)
            {
                if (m_ifUseTransaction)
                {
                    cmd.Transaction = GetNonceTransaction() as SqlTransaction;
                }
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sqlStr;

                foreach (string item in parameters.Keys)
                {
                    cmd.Parameters.Add(new SqlParameter(item, parameters[item]));
                }
                IDataReader dr = cmd.ExecuteReader();
                DataTable tempDT = new DataTable("tempDT1");
                tempDT.Load(dr,LoadOption.Upsert);
                return tempDT;
            }
        }

        #endregion

        #region 执行存储过程
        /// <summary>
        /// 
        /// </summary>
        /// <param name="spName"></param>
        /// <returns></returns>
        public override int SpExecute(string spName)
        {
            Open();
            using (SqlCommand cmd = m_dataConnection.CreateCommand() as SqlCommand)
            {
                if (m_ifUseTransaction)
                {
                    cmd.Transaction = GetNonceTransaction() as SqlTransaction;
                }
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = spName;

                foreach (string item in GetDataObjectParameter(spName))
                {
                    cmd.Parameters.Add(new SqlParameter(item, m_spFileValue[item]));
                }
                return cmd.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="spName"></param>
        /// <returns></returns>
        public override object SpScalar(string spName)
        {
            Open();
            using (SqlCommand cmd = m_dataConnection.CreateCommand() as SqlCommand)
            {
                if (m_ifUseTransaction)
                {
                    cmd.Transaction = GetNonceTransaction() as SqlTransaction;
                }
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = spName;

                foreach (string item in GetDataObjectParameter(spName))
                {
                    cmd.Parameters.Add(new SqlParameter(item, m_spFileValue[item]));
                }
                return cmd.ExecuteScalar();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spName"></param>
        /// <returns></returns>
        public override IDataReader SpGetDataReader(string spName)
        {
            Open();
            using (SqlCommand cmd = m_dataConnection.CreateCommand() as SqlCommand)
            {
                if (m_ifUseTransaction)
                {
                    cmd.Transaction = GetNonceTransaction() as SqlTransaction;
                }
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = spName;

                foreach (string item in GetDataObjectParameter(spName))
                {
                    cmd.Parameters.Add(new SqlParameter(item, m_spFileValue[item]));
                }
                return cmd.ExecuteReader();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spName"></param>
        /// <returns></returns>
        public override DataTable SpGetDataTable(string spName)
        {
            Open();
            using (SqlCommand cmd = m_dataConnection.CreateCommand() as SqlCommand)
            {
                if (m_ifUseTransaction)
                {
                    cmd.Transaction = GetNonceTransaction() as SqlTransaction;
                }
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = spName;

                foreach (string item in GetDataObjectParameter(spName))
                {
                    cmd.Parameters.Add(new SqlParameter(item, m_spFileValue[item]));
                }
                IDataReader dr = cmd.ExecuteReader();
                DataTable tempDT = new DataTable("tempDT1");
                tempDT.Load(dr,LoadOption.Upsert);
                return tempDT;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spName"></param>
        /// <returns></returns>
        public override DataSet SpGetDataSet(string spName)
        {
            Open();
            using (SqlCommand cmd = m_dataConnection.CreateCommand() as SqlCommand)
            {
                if (m_ifUseTransaction)
                {
                    cmd.Transaction = GetNonceTransaction() as SqlTransaction;
                }
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = spName;

                foreach (string item in GetDataObjectParameter(spName))
                {
                    cmd.Parameters.Add(new SqlParameter(item, m_spFileValue[item]));
                }
                DataSet _ds = new DataSet();
                SqlDataAdapter _a = new SqlDataAdapter(cmd);
                _a.Fill(_ds);
                return _ds;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spName"></param>
        /// <returns></returns>
        public override int SpGetReturnValue(string spName)
        {
            Open();
            using (SqlCommand cmd = m_dataConnection.CreateCommand() as SqlCommand)
            {
                if (m_ifUseTransaction)
                {
                    cmd.Transaction = GetNonceTransaction() as SqlTransaction;
                }
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = spName;

                foreach (string item in GetDataObjectParameter(spName))
                {
                    cmd.Parameters.Add(new SqlParameter(item, m_spFileValue[item]));
                }
                SqlParameter _rtnval = cmd.Parameters.Add("ReturnValue", SqlDbType.Int);
                _rtnval.Direction = ParameterDirection.ReturnValue;
                cmd.ExecuteNonQuery();
                return (int)cmd.Parameters["ReturnValue"].Value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="spName"></param>
        /// <param name="returnKey"></param>
        /// <param name="returnValue"></param>
        /// <returns></returns>
        public override int SpGetReturnValue(string spName, string[] returnKey, ref Hashtable returnValue)
        {
            Open();
            using (SqlCommand cmd = m_dataConnection.CreateCommand() as SqlCommand)
            {
                if (m_ifUseTransaction)
                {
                    cmd.Transaction = GetNonceTransaction() as SqlTransaction;
                }
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = spName;

                foreach (string item in GetDataObjectParameter(spName))
                {
                    cmd.Parameters.Add(new SqlParameter(item, m_spFileValue[item]));
                }
                //添加存储过程返回参数
                SqlParameter _rtnval = cmd.Parameters.Add("ReturnValue", SqlDbType.Int);
                _rtnval.Direction = ParameterDirection.ReturnValue;

                //添加存储过程返回参数
                foreach (string item in returnKey)
                {
                    SqlParameter _p = new SqlParameter(item,SqlDbType.VarChar,500);
                    _p.Direction = ParameterDirection.Output;
                    cmd.Parameters[item] = _p;
                }
                cmd.ExecuteNonQuery();
                foreach (string item in returnKey)
                {
                    returnValue[item] = cmd.Parameters[item].Value;
                }
                return (int)cmd.Parameters["ReturnValue"].Value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spName"></param>
        /// <param name="returnKey"></param>
        /// <param name="returnValue"></param>
        /// <param name="returnTable"></param>
        /// <returns></returns>
        public override int SpGetReturnValue(string spName, string[] returnKey, ref Hashtable returnValue, ref DataTable returnTable)
        {
            Open();
            using (SqlCommand cmd = m_dataConnection.CreateCommand() as SqlCommand)
            {
                if (m_ifUseTransaction)
                {
                    cmd.Transaction = GetNonceTransaction() as SqlTransaction;
                }
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = spName;

                foreach (string item in GetDataObjectParameter(spName))
                {
                    cmd.Parameters.Add(new SqlParameter(item, m_spFileValue[item]));
                }
                //添加存储过程返回参数
                SqlParameter _rtnval = cmd.Parameters.Add("ReturnValue", SqlDbType.Int);
                _rtnval.Direction = ParameterDirection.ReturnValue;

                //添加存储过程返回参数
                foreach (string item in returnKey)
                {
                    SqlParameter _p = new SqlParameter(item, SqlDbType.VarChar, 500);
                    _p.Direction = ParameterDirection.Output;
                    cmd.Parameters[item] = _p;
                }

                //执行存储过程，填充表
                IDataReader dr = cmd.ExecuteReader();
                returnTable.Load(dr,LoadOption.Upsert);

                foreach (string item in returnKey)
                {
                    returnValue[item] = cmd.Parameters[item].Value;
                }
                return (int)cmd.Parameters["ReturnValue"].Value;
            }
        }

        #endregion
    }
}
