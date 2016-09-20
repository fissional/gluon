using Newtonsoft.Json;

namespace gluontest
{
	public class FacebookAttachment
	{
		[JsonProperty("type")]
		public string Type { get; set; }
		
		[JsonProperty("media")]
		public FacebookMediaItem Media { get; set; }	
	}

	public class FacebookMediaItem
	{
		[JsonProperty("image")]
		public FacebookImage Image { get; set; }
	}
	
	public class FacebookImage
	{
		[JsonProperty("height")]
		public string Height { get; set; }

		[JsonProperty("width")]
		public string Width { get; set; }

		[JsonProperty("src")]
		public string ImageUrl { get; set; }
	}
}
