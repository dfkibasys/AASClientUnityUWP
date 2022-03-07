/*******************************************************************************
* Copyright (c) 2020, 2021 Robert Bosch GmbH
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the Eclipse Public License 2.0 which is available at
* http://www.eclipse.org/legal/epl-2.0
*
* SPDX-License-Identifier: EPL-2.0
*******************************************************************************/
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace BaSyx.Utils.Client.Http
{
    public class SimpleHttpClientTimeoutHandler : DelegatingHandler
    {
        public TimeSpan DefaultTimeout { get; set; } = TimeSpan.FromSeconds(100);

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            using (CancellationTokenSource cts = GetCancellationTokenSource(request, cancellationToken))
            {
                try
                {

                    return await base.SendAsync(request, cts?.Token ?? cancellationToken);

                }
                catch (OperationCanceledException e) when (!cancellationToken.IsCancellationRequested)
                {
                    Console.WriteLine(e.Message);
                    throw new TimeoutException();
                }
                catch (Exception e)
                {
                    Console.Write(e.Message);
                    return null;
                }
            }
        }

        private CancellationTokenSource GetCancellationTokenSource(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            TimeSpan timeout = request.GetTimeout() ?? DefaultTimeout;
            if (timeout == Timeout.InfiniteTimeSpan)
            {
                return null;
            }
            else
            {
                CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                cts.CancelAfter(timeout);
                return cts;
            }
        }
    }
}
