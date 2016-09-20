using System;

namespace gluontest
{
	/// <summary>
	/// Exception wrapper for Facebook API errors
	/// </summary>
	public class FacebookApiException : Exception
	{
		/// <summary>
		/// Facebook error object
		/// </summary>
		public FacebookError Error { get; }
		
		public FacebookApiException (Exception innerException, FacebookError errorObject) 
			: base(errorObject.ErrorUserMessage, innerException)
		{
			Error = errorObject;
		}
	}
}

