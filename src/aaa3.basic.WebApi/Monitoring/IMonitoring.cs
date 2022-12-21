using System;
using System.Collections.Generic;

namespace aaa3.basic.WebApi.Monitoring
{
    /// <summary>
    /// Interface for NewRelic Agent API
    /// </summary>
    ///
    public interface IMonitoring
    {
        /// <summary>
        /// NewRelic NoticeException
        /// </summary>
        /// 
        void NoticeException(Exception exception);

        /// <summary>
        /// NewRelic NoticeError
        /// </summary>
        /// 
        void NoticeError(string error);

        /// <summary>
        /// NewRelic NoticeError
        /// </summary>
        /// 
        void NoticeError(string error, IDictionary<string, string> parameters);

        /// <summary>
        /// NewRelic AddCustomParameter
        /// </summary>
        /// 
        void AddCustomParameter(string name, string value);

        /// <summary>
        /// NewRelic AddCustomParameter
        /// </summary>
        /// 
        void AddCustomParameter(IDictionary<string, string> value);
    }
}