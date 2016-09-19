﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace gluontest
{
	public class FacebookNode
	{
		/// <summary>
		/// Page ID
		/// </summary>
		[JsonProperty("id")]
		public string Id { get; set; }
	}

	public class FacebookPage : FacebookNode
	{
		/// <summary>
		/// Name of the facebook user
		/// </summary>
		[JsonProperty("name")]
		public string Name { get; set; }

		/// <summary>
		/// Page category
		/// </summary>
		[JsonProperty("category")]
		public string Category { get; set; }

		/// <summary>
		/// Access token needed to access this page
		/// </summary>
		[JsonProperty("access_token")]
		public string AccessToken { get; set; }

		/// <summary>
		/// Permissions the current user has to this page
		/// </summary>
		[JsonProperty("perms")]
		public string[] Permissions { get; set; }
	}

	public class FacebookPost : FacebookNode, INotifyPropertyChanged
	{
		/// <summary>
		/// Body of the Facebook post. Either <see cref="Message"/> or <see cref="Story"/>, whichever is not null.
		/// </summary>
		public string Body => Message ?? Story;

		public async Task LoadInsights()
		{
			this.InsightViewCount = $"{await App.Facebook.GetViewCount(this)} views";
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(nameof(InsightViewCount)));
		}

		public string InsightViewCount { get; private set; } = "# views...";

		/// <summary>
		/// Story of the post
		/// </summary>
		[JsonProperty("story")]
		public string Story { get; set; }

		/// <summary>
		/// Message of the post
		/// </summary>
		[JsonProperty("message")]
		public string Message { get; set; }

		/// <summary>
		/// Time the post was created
		/// </summary>
		[JsonProperty("created_time")]
		public string CreatedTime { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;
	}

	public class FacebookUser : FacebookNode
	{
		/// <summary>
		/// Name of the facebook user
		/// </summary>
		[JsonProperty("name")]
		public string Name { get; set; }

		public FacebookUser()
		{
		}
		public FacebookUser(string name, string id)
		{
			this.Name = name;
			this.Id = id;
		}
	}

	public class FacebookInsightValue
	{
		/// <summary>
		/// Value of the insight
		/// </summary>
		[JsonProperty("value")]
		public string Value { get; set; }
	}

	public class FacebookInsight : FacebookNode
	{
		/// <summary>
		/// Name of the facebook user
		/// </summary>
		[JsonProperty("name")]
		public string Name { get; set; }

		/// <summary>
		/// Period over which the insight was measured
		/// </summary>
		[JsonProperty("period")]
		public string Period { get; set; }

		/// <summary>
		/// Insight title
		/// </summary>
		[JsonProperty("title")]
		public string Title { get; set; }

		/// <summary>
		/// Insight description
		/// </summary>
		[JsonProperty("description")]
		public string Description { get; set; }

		/// <summary>
		/// Value collection for this insight
		/// </summary>
		[JsonProperty("values")]
		public List<FacebookInsightValue> Values { get; set; }
	}

	public class FbRequestPaging
	{
		public FbRequestPagingCursors cursors { get; set; }

		[JsonProperty("previous")]
		public string UrlPrevious { get; set; }

		[JsonProperty("next")]
		public string UrlNext { get; set; }
	}

	public class FbRequestPagingCursors
	{
		[JsonProperty("before")]
		public string Before { get; set; }
		[JsonProperty("after")]
		public string After { get; set; }
	}
}