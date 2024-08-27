/*
* Copyright (c) 2024 Siccar (Registered co. Wallet.Services (Scotland) Ltd).
* All rights reserved.
*
* This file is part of a proprietary software product developed by Siccar.
*
* This source code is licensed under the Siccar Proprietary Limited Use License.
* Use, modification, and distribution of this software is subject to the terms
* and conditions of the license agreement. The full text of the license can be
* found in the LICENSE file or at https://github.com/siccar/SICCARV3/blob/main/LICENCE.txt.
*
* Unauthorized use, copying, modification, merger, publication, distribution,
* sublicensing, and/or sale of this software or any part thereof is strictly
* prohibited except as explicitly allowed by the license agreement.
*/

using IdentityModel.OidcClient.Browser;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;

namespace Siccar.Common.ServiceClients
{
	/// <summary>
	/// Drives the CLI sign in via a browser session
	/// </summary>
	public class SystemBrowser(int? port = null, string? path = null) : IBrowser
	{
		public int Port { get; } = port ?? GetRandomUnusedPort();
		private readonly string? _path = path;

		private static int GetRandomUnusedPort()
		{
			var listener = new TcpListener(IPAddress.Loopback, 0); 
			listener.Start();
			var port = ((IPEndPoint)listener.LocalEndpoint).Port;
			listener.Stop();
			return port;
		}

		public async Task<BrowserResult> InvokeAsync(BrowserOptions options, CancellationToken cancellationToken = default)
		{
			using var listener = new LoopbackHttpListener(Port, _path);
			OpenBrowser(options.StartUrl);

			try
			{
				var result = await listener.WaitForCallbackAsync();

				if (String.IsNullOrWhiteSpace(result))
					return new BrowserResult { ResultType = BrowserResultType.UnknownError, Error = "Empty response." };

				return new BrowserResult { Response = result, ResultType = BrowserResultType.Success };
			}
			catch (TaskCanceledException ex)
			{
				return new BrowserResult { ResultType = BrowserResultType.Timeout, Error = ex.Message };
			}
			catch (Exception ex)
			{
				return new BrowserResult { ResultType = BrowserResultType.UnknownError, Error = ex.Message };
			}
		}

		public static void OpenBrowser(string url)
		{
			try
			{
				Process.Start(url);
			}
			catch
			{
				// hack because of this: https://github.com/dotnet/corefx/issues/10361
				if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				{
					url = url.Replace("&", "^&");
					Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
				}
				else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
					Process.Start("xdg-open", url);
				else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
					Process.Start("open", url);
				else
					throw;
			}
		}
	}

	public class LoopbackHttpListener : IDisposable
	{
		const int DefaultTimeout = 60 * 5; // 5 mins (in seconds)

		readonly IWebHost _host;
		readonly TaskCompletionSource<string> _source = new();
		readonly string _url;

		public string Url => _url;

		public LoopbackHttpListener(int port, string? path = null)
		{
			path ??= String.Empty;
			if (path.StartsWith('/')) path = path[1..];

			_url = $"http://localhost:{port}/{path}";

			_host = new WebHostBuilder()
				.UseKestrel()
				.UseUrls(_url)
				.Configure(ConfigureLBH)
				
				.Build();
			_host.Start();
		}

		public void Dispose()
		{
			Task.Run(async () =>
			{
				await Task.Delay(500);
				_host.Dispose();
			});
			GC.SuppressFinalize(this);
		}

		void ConfigureLBH(IApplicationBuilder app)
		{
			app.Run(async ctx =>
			{
				if (ctx.Request.Method == "GET")
					await SetResultAsync(ctx.Request.QueryString.Value!.TrimStart('?'), ctx);
				else if (ctx.Request.Method == "POST")
				{
					if (!ctx.Request.ContentType!.Equals("application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase))
						ctx.Response.StatusCode = 415;
					else
					{
						using var sr = new StreamReader(ctx.Request.Body, Encoding.UTF8);
						var body = (await sr.ReadToEndAsync()).TrimStart('?');
						await SetResultAsync(body, ctx);
					}
				}
				else
					ctx.Response.StatusCode = 405;
			});
		}

		private async Task SetResultAsync(string value, HttpContext ctx)
		{
			try
			{
				
				// Repond with some basic content
				ctx.Response.StatusCode = 200;
				ctx.Response.ContentType = "text/html";
				
				await ctx.Response.WriteAsync("<h1>You can now return to the application.</h1>");
				await ctx.Response.Body.FlushAsync();
				_source.TrySetResult(value);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());

				ctx.Response.StatusCode = 400;
				ctx.Response.ContentType = "text/html";
				await ctx.Response.WriteAsync("<h1>Invalid request.</h1>");
				await ctx.Response.Body.FlushAsync();
			}
		}

		public Task<string> WaitForCallbackAsync(int timeoutInSeconds = DefaultTimeout)
		{
			Task.Run(async () =>
			{
				await Task.Delay(timeoutInSeconds * 1000);
				_source.TrySetCanceled();
			});
			return _source.Task;
		}
	}
}
