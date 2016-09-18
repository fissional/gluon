using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace gluontest
{
	public class FacebookApiRequest
	{
		HttpWebRequest m_request;

		/// <summary>
		/// Create a new Facebook API request
		/// </summary>
		/// <param name="accessToken">access token which is required to call the API</param>
		/// <param name="objectName">name of the graph API object</param>
		/// <param name="edgeName">edge name</param>
		public FacebookApiRequest(string accessToken, string objectName, string edgeName, Dictionary<string, string> parameters)
			: this(accessToken, "GET", objectName, edgeName, parameters)
		{
		}

		/// <summary>
		/// Create a new Facebook API request
		/// </summary>
		/// <param name="accessToken">access token which is required to call the API</param>
		/// <param name="objectName">name of the graph API object</param>
		public FacebookApiRequest(string accessToken, string objectName)
			: this(accessToken, "GET", objectName, null, null)
		{
		}

		public FacebookApiRequest(string accessToken, string method, string objectName, string edgeName, Dictionary<string, string> parameters)
		{
			var url = new StringBuilder($"https://graph.facebook.com/{objectName}");
			if (edgeName != null)
			{
				url.Append($"/{edgeName}");
			}

			url.Append($"?access_token={accessToken}");

			if (parameters != null)
				foreach (var parameter in parameters)
				{
					url.Append($"{parameter.Key}={parameter.Value}");
				}

			m_request = WebRequest.CreateHttp(url.ToString());
			m_request.Method = method;
		}

		public FacebookApiRequest(Uri uri, string method)
		{
			m_request = WebRequest.CreateHttp(uri);
			m_request.Method = method;
		}

		public async Task<T> GetResult<T>(Func<string, T> converter)
		{
			// get the HTTP result
			var response =
				await Task<WebResponse>.Factory.FromAsync(m_request.BeginGetResponse, m_request.EndGetResponse, null);
			// read the full response stream
			var fullResult = await new StreamReader(response.GetResponseStream()).ReadToEndAsync();

			// convert result
			return converter(fullResult);
		}

		public async Task<FacebookPagedCollection<T>> GetResults<T>()
		{
			// get the HTTP result
			var response = 
				await Task<WebResponse>.Factory.FromAsync(m_request.BeginGetResponse, m_request.EndGetResponse, null);
			// read the full response stream
			var fullResult = await new StreamReader(response.GetResponseStream()).ReadToEndAsync();

			// return an enumerator over results
			return new FacebookPagedCollection<T>(fullResult, m_request.Method);
		}

		public async Task<string> GetRawResult()
		{
			// get the HTTP result
			var response =
				await Task<WebResponse>.Factory.FromAsync(m_request.BeginGetResponse, m_request.EndGetResponse, null);
			// read the full response stream
			var rawResult = await new StreamReader(response.GetResponseStream()).ReadToEndAsync();
			return rawResult;
		}
	}
}

