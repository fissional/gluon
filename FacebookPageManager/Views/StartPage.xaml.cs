using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace gluontest
{
	public partial class StartPage : BaseContentPage
	{
		public StartPage()
		{
			InitializeComponent();
		}

		IList<FacebookPage> m_userPages;
		protected async override void OnAppearing()
		{
			base.OnAppearing();

			// show the Facebook controls, if logged in
			if (App.IsLoggedIn)
			{
				if (App.FacebookSettings.CurrentPage != null)
				{
					// enable the actions UI if a page is selected
					SetActionState(true);
				}
				else
				{
					await InitPages();
					SetActionState(false);
				}
			}
		}

		private async Task InitPages()
		{
			pickerPosts.Items.Clear();
			pickerPosts.Items.Add("Click to select page");
			pickerPosts.SelectedIndex = 0;

			var pagedPageColleciton = await App.Facebook.GetPages(await App.Facebook.GetMe());
			m_userPages = await pagedPageColleciton.LoadAllPages();
			foreach (var page in m_userPages)
			{
				pickerPosts.Items.Add(page.Name);
			}

			m_userPages.Insert(0, null);
		}

		private void SetActionState(bool isEnabled)
		{
			foreach (var child in stackPageActions.Children)
			{
				if (child is Button)
				{
					child.IsEnabled = isEnabled;
				}
			}
		}

		private void pickerPosts_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (pickerPosts.SelectedIndex > 0)
				App.FacebookSettings.CurrentPage = m_userPages[pickerPosts.SelectedIndex];
			else
				App.FacebookSettings.CurrentPage = null;
			SetActionState(App.FacebookSettings.CurrentPage != null);
		}

		private void btnSettings_Clicked(object sender, System.EventArgs e)
		{
			App.Navigate(new PagesList());
		}

		private async void PagePosts_Clicked(object sender, System.EventArgs e)
		{
			var posts = await App.Facebook.GetPosts(App.FacebookSettings.CurrentPage);
			App.Navigate(new PostsList("Published Posts:", posts));
		}

		private async void UnpublishedPosts_Clicked(object sender, System.EventArgs e)
		{
			var posts = await App.Facebook.GetUnpublishedPosts(App.FacebookSettings.CurrentPage);
			App.Navigate(new PostsList("Unpublished Posts:", posts));
		}

		private void CreatePost_Clicked(object sender, System.EventArgs e)
		{
			App.Navigate(new CreatePost());
		}
	}
}
