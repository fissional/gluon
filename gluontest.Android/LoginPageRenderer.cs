using System;
using Android.App;
using Xamarin.Auth;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using gluontest;

[assembly: ExportRenderer(typeof(LoginPage), typeof(gluontest.Android.LoginPageRenderer))]
namespace gluontest.Android
{
	public class LoginPageRenderer : PageRenderer
	{
		bool m_loginShown;
		protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
		{
			base.OnElementChanged(e);

			var activity = this.Context as Activity;

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

				activity.StartActivity(auth.GetUI(activity));
				m_loginShown = true;
			}
		}
	}
}
