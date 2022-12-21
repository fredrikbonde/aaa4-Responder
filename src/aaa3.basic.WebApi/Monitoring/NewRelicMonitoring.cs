using NewRelic.Api.Agent;
using System;
using System.Collections.Generic;

namespace aaa3.basic.WebApi.Monitoring
{
    /// <summary>
    /// NewRelicMonitoring class that uses NewRelic Agent API
    /// </summary>
    /// 
    public class NewRelicMonitoring : IMonitoring
    {
        private readonly ITransaction _currentTransaction;

        public NewRelicMonitoring()
        {
            IAgent agent = NewRelic.Api.Agent.NewRelic.GetAgent();
            _currentTransaction = agent.CurrentTransaction;
        }
        /// <summary>
        /// NewRelic NoticeException
        /// </summary>
        /// 
        public void NoticeException(Exception exception)
        {
            NewRelic.Api.Agent.NewRelic.NoticeError(exception);
        }

        /// <summary>
        /// NewRelic NoticeError
        /// </summary>
        /// 
        public void NoticeError(string error)
        {
            NewRelic.Api.Agent.NewRelic.NoticeError(error, new Dictionary<string, string>());
        }

        /// <summary>
        /// NewRelic NoticeError
        /// </summary>
        /// 
        public void NoticeError(string error, IDictionary<string, string> parameters)
        {
            NewRelic.Api.Agent.NewRelic.NoticeError(error, parameters);
        }

        /// <summary>
        /// NewRelic AddCustomParameter
        /// </summary>
        /// 
        public void AddCustomParameter(string name, string value)
        {
            _currentTransaction.AddCustomAttribute(name, value);
        }

        /// <summary>
        /// NewRelic AddCustomParameter
        /// </summary>
        /// 
        public void AddCustomParameter(IDictionary<string, string> values)
        {
            foreach (var @value in values)
            {
                _currentTransaction.AddCustomAttribute(@value.Key, @value.Value);
            }
        }
    }
}