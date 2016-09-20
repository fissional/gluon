using System;
using System.Collections.Generic;
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

		/// <summary>
		/// Load insight data for this post
		/// </summary>
		public async Task LoadInsights()
		{
			this.InsightViewCount = $"{await App.Facebook.GetViewCount(this)} views";
			FirePropertyChanged(nameof(InsightViewCount));
		}

		/// <summary>
		/// Load the likes for this post
		/// </summary>
		/// <returns>The likes.</returns>
		public async Task LoadLikes()
		{
			var allLikes = await App.Facebook.GetLikes(this);
			this.Likes = await allLikes.LoadAllPages();

			FirePropertyChanged(nameof(Likes));
			FirePropertyChanged(nameof(LikeCount));
		}

		/// <summary>
		/// Number of people this post has reached (page posts only)
		/// </summary>
		public string InsightViewCount { get; private set; } = "# views...";

		/// <summary>
		/// Number of people that have liked this post
		/// </summary>
		public string LikeCount
		{
			get
			{
				if (Likes == null)
					return "# of likes...";
				return $"{Likes.Count} likes";
			}
		}
		private IList<FacebookUser> Likes;

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

		[JsonProperty("scheduled_publish_time")]
		public string ScheduledPublishTime { get; set; }

		private string m_postTime;
		/// <summary>
		/// Either the scheduled post time, or created time of the post
		/// </summary>
		public string PostTime
		{
			get
			{
				if (m_postTime == null)
				{
					int secondsAfterEpoch = 0;
					if (string.IsNullOrWhiteSpace(ScheduledPublishTime) || !Int32.TryParse(ScheduledPublishTime, out secondsAfterEpoch))
					{
						// use post time which isn't a unix date
						m_postTime = DateTime.Parse(CreatedTime).ToString("yyyy/MM/dd HH:mm");
					}
					else
					{
						// convert from unix date
						m_postTime = (new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(secondsAfterEpoch)).ToString("yyyy/MM/dd HH:mm");
					}
				}
				return m_postTime;
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		private void FirePropertyChanged(string propertyName)
		{
			// safe way to deal w/ this event getting changed while we fire it
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}
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

	/// <summary>
	/// Error object returned from failed API calls
	/// </summary>
	public class FacebookError
	{
		/// <summary>
		/// Error type
		/// </summary>
		[JsonProperty("type")]
		public string Type { get; set; }

		/// <summary>
		/// General error message
		/// </summary>
		[JsonProperty("message")]
		public string Message { get; set; }

		/// <summary>
		/// Error code
		/// </summary>
		[JsonProperty("code")]
		public int Code { get; set; }

		/// <summary>
		/// Error subcode
		/// </summary>
		[JsonProperty("error_subcode")]
		public int SubCode { get; set; }

		/// <summary>
		/// presumably this means it might work if you try it again
		/// </summary>
		[JsonProperty("is_transient")]
		public bool IsTransient { get; set; }

		/// <summary>
		/// Error title specific to the API action
		/// </summary>
		[JsonProperty("error_user_title")]
		public string ErrorUserTitle { get; set; }

		/// <summary>
		/// Error message specific to the API action
		/// </summary>
		[JsonProperty("error_user_msg")]
		public string ErrorUserMessage { get; set; }
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