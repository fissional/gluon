using System;
using Newtonsoft.Json.Linq;

namespace gluontest
{
	public static class FacebookJson
	{
		public static FacebookUser DeserializeUser(string json)
		{
			var root = JObject.Parse(json);
			FacebookUser user = new FacebookUser(root["name"].Value<string>(), root["id"].Value<string>());
			return user;
		}
	}
}

