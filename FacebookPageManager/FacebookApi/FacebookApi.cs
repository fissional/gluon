﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

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
		/// Get a single object with the provided ID
		/// </summary>
		/// <returns>object representing the Facebook node requested</returns>
		/// <param name="nodeId">Node ID</param>
		/// <typeparam name="T">Type of the Node</typeparam>
		public async Task<T> GetObject<T>(string nodeId)
		{
			var req = new FacebookApiRequest(m_token, nodeId);
			var result = await req.GetResult<T>();
			return result;
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
			var req = new FacebookApiRequest(m_token, page.Id, "promotable_posts", new Dictionary<string, string> { 
				{ "is_published", "false" }, 
				{ "fields", "scheduled_publish_time,message,id,created_time,is_published" } 
			});
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

		/// <summary>
		/// Gets a specific insight for a Facebook post
		/// </summary>
		/// <returns>An insight object, potentially paged, with the requested data</returns>
		/// <param name="post">Facebook post for which to retrieve <paramref name="insightName"/></param>
		/// <param name="insightName">API name of the insight to retrieve</param>
		public async Task<FacebookPagedCollection<FacebookInsight>> GetPostInsight(FacebookPost post, string insightName)
		{
			var req = new FacebookApiRequest(m_token, post.Id, $"insights/{insightName}", null);
			var result = await req.GetResults<FacebookInsight>();
			return result;
		}

		/// <summary>
		/// Gets all the attachments on a Facebook post
		/// </summary>
		/// <returns>The post attachments.</returns>
		/// <param name="post">Facebook post</param>
		public async Task<FacebookPagedCollection<FacebookAttachment>> GetPostAttachments(FacebookPost post)
		{
			var req = new FacebookApiRequest(m_token, post.Id, $"attachments", null);
			var result = await req.GetResults<FacebookAttachment>();
			return result;
		}

		/// <summary>
		/// Get the number of people this post has reached. Page posts only
		/// </summary>
		/// <param name="post">Facebook post for which to get reach</param>
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
				if (publishDate.Value > DateTime.Now)
				{
					requestParameters["scheduled_publish_time"] = 
						(publishDate.Value - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds.ToString();
					requestParameters["published"] = "0";
				}
				else
				{
					requestParameters["backdated_time"] = publishDate.Value.ToString("yyyy-MM-ddTHH:mm:ssZ");
				}
			}
			// this operation requires a page access token for this page
			var req = new FacebookApiRequest(page.AccessToken, "POST", page.Id, "feed", requestParameters);
			var result = await req.GetRawResult();

			var jsonRoot = JObject.Parse(result);
			if (jsonRoot["id"] != null)
				return jsonRoot["id"].Value<string>();
			return null;
		}
	}
}

