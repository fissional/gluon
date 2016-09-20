using Xamarin.Forms;
using System;

namespace gluontest
{
	public class AppFacebookSettings
	{
		/// <summary>
		/// Saved authorization token for the Facebook API
		/// </summary>
		// TODO: this should be stored somewhere in persistent memory so the app can load it on launch
		public readonly string Token = null;

		/// <summary>
		/// The Facebook Page the app is currently managing
		/// </summary>
		public FacebookPage CurrentPage;
	}

	public partial class App : Application
	{
		/// <summary>
		/// Facebook API permissions required to run this application
		/// </summary>
		public static readonly string[] RequiredPermissions = new string[] 
		{
			"manage_pages",
			"publish_pages",
			"read_insights",
		};
		
		public static readonly AppFacebookSettings FacebookSettings = new AppFacebookSettings();

		public App()
		{
			InitializeComponent();
			Facebook = new FacebookApi(FacebookSettings.Token);
			MainPage = GetMainPage();
		}


		static NavigationPage _NavPage;

		public static Page GetMainPage()
		{
			var profilePage = new StartPage();
			_NavPage = new NavigationPage(profilePage);
			return _NavPage;
		}

		public static void Navigate(Page page)
		{
			_NavPage.Navigation.PushModalAsync(page);
		}

		public static void NavigateOut()
		{
			_NavPage.Navigation.PopModalAsync();
		}

		/// <summary>
		/// Returns true if the application can make facebook API requests, false if not
		/// </summary>
		public static bool IsLoggedIn { get { return Facebook?.IsLoggedIn ?? false; } }

		/// <summary>
		/// Application's facebook API instance
		/// </summary>
		public static FacebookApi Facebook { get; private set; }

		/// <summary>
		/// Save a facebook API access token
		/// </summary>
		public static void SaveToken(string token)
		{
			Facebook = new FacebookApi(token);
			OnLoggedIn?.Invoke();
		}

		public delegate void LoggedInHandler();
		public static event LoggedInHandler OnLoggedIn;

		public static Action SuccessfulLoginAction
		{
			get
			{
				return new Action(() =>
				{
					_NavPage.Navigation.PopModalAsync();
				});
			}
		}
	}
}

