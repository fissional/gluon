using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace gluontest
{
	public class FacebookPagedCollection<T>
	{
		readonly List<T> m_storage;
		readonly FbRequestPaging m_paging;
		readonly string m_method;

		/// <summary>
		/// Collection of T that represents the result of a facebook API call
		/// </summary>
		public List<T> Data { get { return m_storage; } }

		/// <summary>
		/// Returns true if the result has no data, false if it does
		/// </summary>
		public bool IsEmpty => m_storage?.Count == 0;

		public FacebookPagedCollection(string data, string method)
		{
			var root = JObject.Parse(data);
			m_storage = ((JArray)root["data"]).Select((JToken arg) => arg.ToObject<T>()).ToList();
			m_paging = root["paging"]?.ToObject<FbRequestPaging>();
			m_method = method;
		}

	    FacebookPagedCollection ()
		{
			m_storage = new List<T>();
		}

		/// <summary>
		/// Get the next page of data, if any
		/// </summary>
		public async Task<FacebookPagedCollection<T>> Next()
		{
			if (String.IsNullOrWhiteSpace(m_paging.UrlNext))
				return new FacebookPagedCollection<T>();

			// request next page
			var req = new FacebookApiRequest(new Uri(m_paging.UrlNext), m_method);
			var nextData = await req.GetRawResult();
			return new FacebookPagedCollection<T>(nextData, m_method);
		}
	}
}