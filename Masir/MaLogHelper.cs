using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Masir
{
    /// <summary>
    /// 日志处理帮助类
    /// </summary>
    public static class TxLogHelper
    {
        /// <summary>
        /// 记录日志对象
        /// 对于独立类，
        /// 请使用引用Txooo命名空间后，使用this.TxLogDebug()记录日志信息
        /// </summary>
        public static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static log4net.Appender.IAppender m_allAppender;
        static log4net.Appender.IAppender m_errorAppender;
        static log4net.Appender.IAppender m_infoAppender;
        static log4net.Appender.IAppender m_debugAppender;
        static log4net.Appender.IAppender m_warnAppender;
        static log4net.Appender.IAppender m_fatalAppender;
        static DateTime m_initTime;

        static string m_defaultConfigFile;

        static TxLogHelper()
        {
            m_initTime = DateTime.Now;

            m_defaultConfigFile = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Tx.config";

            //监控配置文件信息
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo(m_defaultConfigFile));

            //读取日志配置文件路径
            m_allAppender = GetFileAppender("G_All", log4net.Core.Level.Off, log4net.Core.Level.All);
            m_errorAppender = GetFileAppender("G_Error", log4net.Core.Level.Error, log4net.Core.Level.Error);
            m_infoAppender = GetFileAppender("G_Info", log4net.Core.Level.Info, log4net.Core.Level.Info);
            m_debugAppender = GetFileAppender("G_Debug", log4net.Core.Level.Debug, log4net.Core.Level.Debug);
            m_warnAppender = GetFileAppender("G_Warn", log4net.Core.Level.Warn, log4net.Core.Level.Warn);
            m_fatalAppender = GetFileAppender("G_Fatal", log4net.Core.Level.Fatal, log4net.Core.Level.Fatal);

        }

        #region 日志文件保存路径

        static string GetLogSavePath()
        {
            if (System.Runtime.Caching.MemoryCache.Default.Get("TX_LOG_SAVE_PATH") != null)
            {
                return System.Runtime.Caching.MemoryCache.Default.Get("TX_LOG_SAVE_PATH").ToString();
            }
            else if (System.IO.File.Exists(m_defaultConfigFile))
            {
                //读取XML信息
                XmlDocument _reader = new XmlDocument();
                _reader.Load(m_defaultConfigFile);
                foreach (XmlNode item in _reader.DocumentElement.ChildNodes)
                {
                    if (item is XmlElement)
                    {
                        if (item.Name == "log4net" && item.Attributes["path"] != null)
                        {
                            if (!string.IsNullOrEmpty(item.Attributes["path"].Value))
                            {
                                List<string> _filePaths = new List<string>();
                                _filePaths.Add(m_defaultConfigFile);
                                CacheItemPolicy _cachePolicy = new CacheItemPolicy();
                                _cachePolicy.ChangeMonitors.Add(new HostFileChangeMonitor(_filePaths));

                                string _savePath = item.Attributes["path"].Value + "\\" + m_initTime.ToString("yyyyMMddHHmmss") + "\\";

                                System.Runtime.Caching.MemoryCache.Default.Set("TX_LOG_SAVE_PATH", _savePath, _cachePolicy);
                                return _savePath;
                            }
                        }
                    }
                }
            }
            return "../_txLog/";
        }

        #endregion

        #region 获取一个文件记录FileAppender

        static log4net.Appender.RollingFileAppender GetFileAppender(string name, log4net.Core.Level max, log4net.Core.Level min)
        {
            ///设置过滤器
            log4net.Filter.LevelRangeFilter _levfilter = new log4net.Filter.LevelRangeFilter();
            _levfilter.LevelMax = max;
            _levfilter.LevelMin = min;
            _levfilter.ActivateOptions();

            //设计记录格式 
            log4net.Layout.PatternLayout _layout = new log4net.Layout.PatternLayout("%date [%thread] %-5level - %message%newline");

            //Appender1  
            log4net.Appender.RollingFileAppender _appender = new log4net.Appender.RollingFileAppender();
            //设置本Appander的名称
            _appender.Name = m_initTime.ToString("HHmmss") + "." + name + ".Appender";
            _appender.File = GetLogSavePath() + name + ".log";
            //否追加到文件
            _appender.AppendToFile = true;
            _appender.MaxSizeRollBackups = 10;
            //日志文件名是否为静态
            _appender.StaticLogFileName = true;
            //_appender.DatePattern = "";
            //表示是否立即输出到文件，布尔型的。
            //_appender.ImmediateFlush
            //SecurityContext : 比较少应用，对日志进行加密只类的，使用SecurityContextProvider转换。(对日志的保密要求比较高的时候应该可以应用上吧，Log4考虑的还挺周全)
            //LockingModel : 文件锁类型，RollingFileAppender本身不是线程安全的，如果在程序中没有进行线程安全的限制，可以在这里进行配置，确保写入时的安全。有两中类型：FileAppender.ExclusiveLock 和 FileAppender.MinimalLock

            _appender.RollingStyle = log4net.Appender.RollingFileAppender.RollingMode.Date;
            //_appender.LockingModel = new log4net.Appender.FileAppender.MinimalLock();

            _appender.Layout = _layout;
            _appender.AddFilter(_levfilter);
            _appender.ActivateOptions();

            return _appender;
        }

        #endregion

        #region 提取ILog

        static object m_lockObj = new object();

        static log4net.ILog GetLogImpl(string name)
        {
            log4net.Core.LogImpl _log = log4net.LogManager.GetLogger(name) as log4net.Core.LogImpl;
            if (_log != null)
            {
                log4net.Repository.Hierarchy.Logger _logimpl = _log.Logger as log4net.Repository.Hierarchy.Logger;
                if (_logimpl != null)
                {
                    log4net.Repository.Hierarchy.Logger _logger = (log4net.Repository.Hierarchy.Logger)_logimpl;
                    if (_logger.Appenders.Count == 0)
                    {
                        lock (m_lockObj)
                        {
                            if (_logger.Appenders.Count == 0)
                            {
                                _logger.AddAppender(GetFileAppender(name, log4net.Core.Level.Off, log4net.Core.Level.All));
                                _logger.AddAppender(m_allAppender);
                                _logger.AddAppender(m_errorAppender);
                                _logger.AddAppender(m_infoAppender);
                                _logger.AddAppender(m_debugAppender);
                                _logger.AddAppender(m_warnAppender);
                                _logger.AddAppender(m_fatalAppender);

                                //log4net.Repository.Hierarchy.Hierarchy hier = (log4net.Repository.Hierarchy.Hierarchy)log4net.LogManager.GetAllRepositories()[0];
                                //hier.GetAppen
                                //log4net.Config.XmlConfigurator.Configure(//.BasicConfigurator.Configure(_appender);
                            }
                        }
                    }
                }
            }
            //缓存
            //m_logger.Set(name, _log, m_cachePolicy);
            return _log;
        }

        #endregion

        #region 获得日志记录类

        /// <summary>
        /// 获得日记记录助手，请使用类型的完整名称防止错误
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static log4net.ILog GetLogger(string name)
        {
            return GetLogImpl(name);
        }

        /// <summary>
        /// 获得当前类型的日记记录助手
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static log4net.ILog GetLogger(Type type)
        {
            //  >2.5,170[1W]
            //return log4net.LogManager.GetLogger(type.FullName);

            //  >2.5,270[1W]
            return GetLogImpl(type.FullName);

            //  >2.0,230[1W]
            //return m_logger.Get<log4net.ILog>(type.FullName, GetLogImpl, m_cachePolicy);
        }

        #endregion

        #region 记录日志  <!--级别，OFF、Fatal、ERROR、WARN、INFO、DEBUG、ALL-->

        public static void TxLogFatal(this object obj, string messages)
        {
            GetLogger(obj.GetType()).Fatal(messages);
        }

        public static void TxLogFatal(this object obj, string messages, Exception ex)
        {
            GetLogger(obj.GetType()).Fatal(messages, ex);
        }

        public static void TxLogError(this object obj, string messages)
        {
            GetLogger(obj.GetType()).Error(messages);
        }

        public static void TxLogError(this object obj, string messages, Exception ex)
        {
            GetLogger(obj.GetType()).Error(messages, ex);
        }

        public static void TxLogWarn(this object obj, string messages)
        {
            GetLogger(obj.GetType()).Warn(messages);
        }

        public static void TxLogWarn(this object obj, string messages, Exception ex)
        {
            GetLogger(obj.GetType()).Warn(messages, ex);
        }

        public static void TxLogInfo(this object obj, string messages)
        {
            GetLogger(obj.GetType()).Info(messages);
        }

        public static void TxLogInfo(this object obj, string messages, Exception ex)
        {
            GetLogger(obj.GetType()).Info(messages, ex);
        }

        public static void TxLogDebug(this object obj, string messages)
        {
            GetLogger(obj.GetType()).Debug(messages);
        }

        public static void TxLogDebug(this object obj, string messages, Exception ex)
        {
            GetLogger(obj.GetType()).Debug(messages, ex);
        }


        #endregion

    }
}
