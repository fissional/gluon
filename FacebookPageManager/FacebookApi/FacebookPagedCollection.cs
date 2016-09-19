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
		List<T> m_allData;
		List<T> m_storage;
		FbRequestPaging m_paging;
		string m_method;

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
			Initialize(data, method);
		}

		private void Initialize(string data, string method)
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
		/// Get the next page of data, if any. Mutates the current collection
		/// </summary>
		public async Task<FacebookPagedCollection<T>> Next()
		{
			if (String.IsNullOrWhiteSpace(m_paging.UrlNext))
				return new FacebookPagedCollection<T>();

			// request next page
			var req = new FacebookApiRequest(new Uri(m_paging.UrlNext), m_method);
			var nextData = await req.GetRawResult();
			this.Initialize(nextData, m_method);
			return this;
		}

		/// <summary>
		/// Load all pages of data for this collection
		/// </summary>
		/// <returns>The all pages.</returns>
		public async Task<IList<T>> LoadAllPages()
		{
			if (m_allData == null)
			{
				m_allData = new List<T>();
			}

			while (!IsEmpty)
			{
				m_allData.AddRange(Data);
				await Next();
			}

			return m_allData;
		}
	}
}