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
		readonly HttpWebRequest m_request;

		/// <summary>
		/// Deserialize the specified json.
		/// </summary>
		/// <param name="json">json string representing an instance of an object of type T</param>
		/// <typeparam name="T">The type to deserialize the provided JSON to</typeparam>
		static T DeserializeJson<T>(string json)
		{
			var root = JObject.Parse(json);
			return root.ToObject<T>();
		}

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

		/// <summary>
		/// Create a new Facebook API request
		/// </summary>
		/// <param name="accessToken">access token which is required to call the API</param>
		/// <param name="method">HTTP method</param>
		/// <param name="objectName">name of the graph API object</param>
		/// <param name="edgeName">edge name</param>
		/// <param name="parameters">additional Facebook API parameters</param>
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
					url.Append($"&{parameter.Key}={parameter.Value}");
				}

			m_request = WebRequest.CreateHttp(url.ToString());
			m_request.Method = method;
		}

		/// <summary>
		/// Create a new Facebook API request
		/// </summary>
		/// <param name="uri">full URI of the API request</param>
		/// <param name="method">HTTP method</param>
		public FacebookApiRequest(Uri uri, string method)
		{
			m_request = WebRequest.CreateHttp(uri);
			m_request.Method = method;
		}

		/// <summary>
		/// Gets a single Facebook object as the result of an API request
		/// </summary>
		/// <returns>object represented in the API response</returns>
		/// <typeparam name="T">Expected type of the result</typeparam>
		public async Task<T> GetResult<T>()
		{
			// get the HTTP result
			var response =
				await Task<WebResponse>.Factory.FromAsync(m_request.BeginGetResponse, m_request.EndGetResponse, null);
			// read the full response stream
			var fullResult = await new StreamReader(response.GetResponseStream()).ReadToEndAsync();

			// convert result
			return DeserializeJson<T>(fullResult);
		}

		/// <summary>
		/// Get a collection of results from an API request that returns multiple objects
		/// </summary>
		/// <returns>collection of Facebook API objects, potentially with more pages of results</returns>
		/// <typeparam name="T">Expected type of all results</typeparam>
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

		/// <summary>
		/// Get a string representing the response from an API request without processing
		/// </summary>
		/// <returns>string returned from the API request</returns>
		public async Task<string> GetRawResult()
		{
			try
			{
				// get the HTTP result
				var response =
					await Task<WebResponse>.Factory.FromAsync(m_request.BeginGetResponse, m_request.EndGetResponse, null);
				// read the full response stream
				var rawResult = await new StreamReader(response.GetResponseStream()).ReadToEndAsync();
				return rawResult;
			}
			catch (WebException e)
			{
				throw new FacebookApiException(e, await CreateFromException(e));
			}
		}

		/// <summary>
		/// Create a new error object from a thrown web exception
		/// </summary>
		/// <returns>FacebookError object with the data contained in the provided WebException</returns>
		/// <param name="ex">WebException thrown from an API call</param>
		private async static Task<FacebookError> CreateFromException(WebException ex)
		{
			var errorResponse = await new StreamReader(ex.Response.GetResponseStream()).ReadToEndAsync();
			var errorJson = JObject.Parse(errorResponse);
			var newError = errorJson["error"].ToObject<FacebookError>();
			return newError;
		}
	}
}

