namespace gluontest
{
	public partial class StartPage : BaseContentPage
	{
		public StartPage()
		{
			InitializeComponent();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			// show the Facebook controls, if logged in
			if (App.IsLoggedIn)
			{
				if (App.FacebookSettings.CurrentPage != null)
				{
					labelPageHeader.Text = App.FacebookSettings.CurrentPage.Name;
					stackPageActions.IsVisible = true;
				}

				// show the UI if we've logged in
				this.buttonStack.IsVisible = true;
			}
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
