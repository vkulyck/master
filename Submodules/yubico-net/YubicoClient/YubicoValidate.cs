﻿/**
 * Copyright (c) 2012, Yubico AB.  All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 *
 * * Redistributions of source code must retain the above copyright
 *   notice, this list of conditions and the following disclaimer.
 *
 * * Redistributions in binary form must reproduce the above copyright
 *   notice, this list of conditions and the following
 *   disclaimer in the documentation and/or other materials provided
 *   with the distribution.
 *
 *  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND
 *  CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES,
 *  INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
 *  MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 *  DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS
 *  BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
 *  EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
 *  TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
 *  DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
 *  ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR
 *  TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF
 *  THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF
 *  SUCH DAMAGE.
 */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Threading;

namespace Yubico
{
    public sealed class YubicoValidate
    {
        private static HttpClient _httpClient = new HttpClient
        {
            Timeout = new TimeSpan(0, 0, 0, 5) // 5 seconds
        };

        public async static Task<IYubicoResponse> ValidateAsync(IEnumerable<string> urls, string userAgent)
        {
            var tasks = new List<Task<IYubicoResponse>>();
            var tokenSource = new CancellationTokenSource();

            foreach (var url in urls)
            {
                tasks.Add(DoVerifyAsync(url, userAgent, tokenSource.Token));
            }

            while (tasks.Count != 0)
            {
                var completedTask = await Task.WhenAny(tasks.ToArray());
                tasks.Remove(completedTask);
                if (completedTask.Result != null)
                {
                    tokenSource.Cancel();
                    return completedTask.Result;
                }
            }

            return null;
        }

        private async static Task<IYubicoResponse> DoVerifyAsync(string url, string userAgent, CancellationToken token)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url)
            };
            request.Headers.Add("User-Agent", userAgent ?? "YubicoClient");

            try
            {
                var response = await _httpClient.SendAsync(request, token);

                token.ThrowIfCancellationRequested();
                if(!response.IsSuccessStatusCode)
                {
                    return null;
                }

                IYubicoResponse yubiResponse;
                var responseContent = await response.Content.ReadAsStringAsync();
                token.ThrowIfCancellationRequested();

                try
                {
                    yubiResponse = new YubicoResponse(responseContent, url);
                }
                catch (ArgumentException)
                {
                    return null;
                }

                token.ThrowIfCancellationRequested();
                if (yubiResponse.Status == YubicoResponseStatus.ReplayedRequest)
                {
                    //throw new YubicoValidationException("Replayed request, this otp & nonce combination has been seen before.");
                    return null;
                }

                return yubiResponse;
            }
            catch (TaskCanceledException)
            {
                // timeout
                return null;
            }
        }
    }
}
