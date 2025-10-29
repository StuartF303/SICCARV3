// SPDX-License-Identifier: MIT
// Copyleft 2026 Sorcha Project Contributors

using System.Text.Json;
using System.Runtime.Serialization;
#nullable enable

namespace Siccar.Common.ServiceClients
{
	/// <summary>
	/// Very simple used to cache AuthN/Z Tokens to persist between subsequent executions 
	/// </summary>
	public class AuthCache
	{
		// Auth Cache Storage
		private readonly string SiccarHomeDir = string.Empty;
		private readonly string AuthCacheFile = string.Empty;
		private readonly DirectoryInfo directory;
		private readonly string fullPath = string.Empty;

		[DataMember]
		public string? AccessToken { get; set; }
		[DataMember]
		public string? IdentityToken { get; set; }
		[DataMember]
		public string? RefreshToken { get; set; }
		[IgnoreDataMember]
		public bool IsLoaded { get; set; }

		public AuthCache(string homeDir = "", string fileName = "auth.cache")
		{
			string homePath = (Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX) ?
				Environment.GetEnvironmentVariable("HOME")! : Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%")!;

			// Setup and checkfile
			SiccarHomeDir = Path.Combine(string.IsNullOrWhiteSpace(homeDir) ? homePath : homeDir, ".siccar");
			AuthCacheFile = fileName;
			fullPath = Path.Combine(SiccarHomeDir, AuthCacheFile);
			directory = Directory.CreateDirectory(SiccarHomeDir);
			IsLoaded = Read(fileName).Result;
		}
		public async Task<bool> Read(string fileName)
		{
			if (directory.EnumerateFiles(fileName).Any())
			{
				var jtext = await File.ReadAllTextAsync(fullPath);
				JsonDocument doc = JsonDocument.Parse(jtext);
				JsonElement root = doc.RootElement;
				AccessToken = root.GetProperty("accessToken").ToString();
				IdentityToken = root.GetProperty("identityToken").ToString();
				RefreshToken = root.GetProperty("refreshToken").ToString();
				return true;
			}
			return false;
		}
		public async Task<bool> Write()
		{
			string json = JsonSerializer.Serialize(this, new JsonSerializerOptions(JsonSerializerDefaults.Web));
			await File.WriteAllTextAsync(fullPath, json);
			return true;
		}
	}
}
