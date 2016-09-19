using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Auth;
using gluontest;

[assembly: ExportRenderer(typeof(LoginPage), typeof(gluontest.iOS.LoginPageRenderer))]
namespace gluontest.iOS
{
	public class LoginPageRenderer : PageRenderer
	{
		bool m_loginShown;
		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);

			if (!m_loginShown && !App.IsLoggedIn)
			{
				var auth = new OAuth2Authenticator(
					clientId: "1058962650819998",
					scope: string.Join("+", App.RequiredPermissions),
					authorizeUrl: new Uri("https://m.facebook.com/dialog/oauth/"),
					redirectUrl: new Uri("http://www.facebook.com/connect/login_success.html")
				);

				auth.Completed += (sender, eventArgs) =>
				{
					// We presented the UI, so it's up to us to dimiss it on iOS.
					App.SuccessfulLoginAction.Invoke();

					if (eventArgs.IsAuthenticated)
					{
						// save the access token in the application
						App.SaveToken(eventArgs.Account.Properties["access_token"]);
					}
					else {
						// user canceled - need to show login screen again (or failure)
					}
				};

				m_loginShown = true;
				PresentViewController(auth.GetUI(), true, null);
			}
		}
	}
}
