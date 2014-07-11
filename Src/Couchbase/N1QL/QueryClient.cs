﻿using System;
using System.Net.Http;
using Common.Logging;
using Couchbase.Views;

namespace Couchbase.N1QL
{
    /// <summary>
    /// A <see cref="IViewClient"/> implementation for executing N1QL queries against a Couchbase Server.
    /// </summary>
    public class QueryClient : IQueryClient
    {
        private readonly static ILog Log = LogManager.GetCurrentClassLogger();

        public QueryClient(HttpClient httpClient, IDataMapper dataMapper)
        {
            HttpClient = httpClient;
            DataMapper = dataMapper;
        }

        /// <summary>
        /// Executes an ad-hoc N1QL query against a Couchbase Server.
        /// </summary>
        /// <typeparam name="T">The Type to cast the resulting rows to.</typeparam>
        /// <param name="server">The <see cref="Uri"/> of the server.</param>
        /// <param name="query">A string containing a N1QL query.</param>
        /// <returns>An <see cref="IQueryResult{T}"/> implementation representing the results of the query.</returns>
        public IQueryResult<T> Query<T>(Uri server, string query)
        {
            IQueryResult<T> queryResult = new QueryResult<T>();

            var content = new StringContent(query);
            var postTask = HttpClient.PostAsync(server, content);
            try
            {
                postTask.Wait();
                var postResult = postTask.Result;

                var readTask = postResult.Content.ReadAsStreamAsync();
                readTask.Wait();

                queryResult = DataMapper.Map<QueryResult<T>>(readTask.Result);
            }
            catch (AggregateException ae)
            {
                ae.Flatten().Handle(e =>
                {
                    Log.Error(e);
                    return true;
                });
            }
            return queryResult;
        }

        /// <summary>
        /// The <see cref="IDataMapper"/> to use for mapping the output stream to a Type.
        /// </summary>
        public IDataMapper DataMapper { get; set; }

        /// <summary>
        /// The <see cref="HttpClient"/> to use for the HTTP POST to the Server.
        /// </summary>
        public HttpClient HttpClient { get; set; }
    }
}

#region [ License information          ]

/* ************************************************************
 *
 *    @author Couchbase <info@couchbase.com>
 *    @copyright 2014 Couchbase, Inc.
 *
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *
 *        http://www.apache.org/licenses/LICENSE-2.0
 *
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 *
 * ************************************************************/

#endregion