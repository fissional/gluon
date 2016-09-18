using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

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
			var req = new FacebookApiRequest(m_token, page.Id, "promotable_posts", new Dictionary<string, string> { {"is_published", "false"} });
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
		/// Gets an object representing the current Facebook user
		/// </summary>
		public async Task<FacebookUser> GetMe()
		{
			var req = new FacebookApiRequest(m_token, "me");
			var result = await req.GetResult(FacebookJson.DeserializeUser);
			return result;
		}
	}
}

