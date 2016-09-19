using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gluontest
{
	public class FacebookApi
	{
		// access token for the Facebook API
		private readonly string m_token;

		/// <summary>
		/// Create a new instance of the Facebook API with the provided access token
		/// </summary>
		/// <param name="token">access token that will be sent to the API with every request.</param>
		public FacebookApi(string token)
		{
			m_token = token;
		}

		/// <summary>
		/// true if the Facebook API is logged in and capable of making requests, false if not
		/// </summary>
		public bool IsLoggedIn
		{
			get { return !string.IsNullOrWhiteSpace(m_token); }
		}

		/// <summary>
		/// Get all the Facebook pages the provided user has rights to
		/// </summary>
		/// <param name="user">Facebook user object</param>
		public async Task<FacebookPagedCollection<FacebookPage>> GetPages(FacebookUser user)
		{
			var req = new FacebookApiRequest(m_token, user.Id, "accounts", null);
			var result = await req.GetResults<FacebookPage>();
			return result;
		}

		/// <summary>
		/// Gets all the posts for the provided Facebook node
		/// </summary>
		/// <param name="node">Facebook user object</param>
		public async Task<FacebookPagedCollection<FacebookPost>> GetPosts(FacebookNode node)
		{
			var req = new FacebookApiRequest(m_token, node.Id, "feed", null);
			var result = await req.GetResults<FacebookPost>();
			return result;
		}

		/// <summary>
		/// Gets all the unpublished posts for the provided Facebook node
		/// </summary>
		/// <param name="page">Facebook user object</param>
		public async Task<FacebookPagedCollection<FacebookPost>> GetUnpublishedPosts(FacebookPage page)
		{
			var req = new FacebookApiRequest(m_token, page.Id, "promotable_posts", new Dictionary<string, string> { { "is_published", "false" } });
			var result = await req.GetResults<FacebookPost>();
			return result;
		}

		/// <summary>
		/// Get the list of facebook users who have liked a post
		/// </summary>
		/// <returns>Users that like the <paramref name="post"/></returns>
		/// <param name="post">the post to collect likes for</param>
		public async Task<FacebookPagedCollection<FacebookUser>> GetLikes(FacebookPost post)
		{
			var req = new FacebookApiRequest(m_token, post.Id, "likes", null);
			var result = await req.GetResults<FacebookUser>();
			return result;
		}

		public async Task<FacebookPagedCollection<FacebookInsight>> GetPostInsight(FacebookPost post, string insightName)
		{
			var req = new FacebookApiRequest(m_token, post.Id, $"insights/{insightName}", null);
			var result = await req.GetResults<FacebookInsight>();
			return result;
		}

		public async Task<int> GetViewCount(FacebookPost post)
		{
			var insights = await GetPostInsight(post, "post_impressions_unique");
			var uniqueViews = insights.Data.Sum(i => i.Values.Sum(x =>
			{
				int val = 0;
				Int32.TryParse(x.Value, out val);
				return val;
			}));
			return uniqueViews;
		}

		/// <summary>
		/// Gets an object representing the current Facebook user
		/// </summary>
		public async Task<FacebookUser> GetMe()
		{
			var req = new FacebookApiRequest(m_token, "me");
			var result = await req.GetResult<FacebookUser>();
			return result;
		}

		/// <summary>
		/// Creates a new post on a Facebook Page
		/// </summary>
		/// <param name="page">Page on which to create the post</param>
		/// <param name="message">Body of the new post</param>
		/// <param name="isPublished">If set to <c>true</c> the post will be published and a story generated</param>
		/// <param name="publishDate">Date on which the post should be published - if before DateTime.Now, the post will be backdated</param>
		public async Task<string> CreatePost(FacebookPage page, string message, bool isPublished, DateTime? publishDate = null)
		{
			var requestParameters = new Dictionary<string, string>
			 {
				{ "message", message },
				{ "published", isPublished ? "1" : "0" },
			};
			if (publishDate.HasValue)
			{
				var dateParamName = publishDate.Value > DateTime.Now ? "scheduled_publish_time" : "backdated_time";
				requestParameters[dateParamName] = publishDate.Value.ToString();
			}
			// this operation requires a page access token for this page
			var req = new FacebookApiRequest(page.AccessToken, "POST", page.Id, "feed", requestParameters);
			var result = await req.GetRawResult();
			return result;
		}
	}
}

