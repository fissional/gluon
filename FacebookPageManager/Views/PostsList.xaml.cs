using System;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;

namespace gluontest
{
	public partial class PostsList : ContentPage
	{
		FacebookPagedCollection<FacebookPost> postsCollection;

		public PostsList(string headerText, FacebookPagedCollection<FacebookPost> posts)
		{
			InitializeComponent();

			labelPostHeader.Text = headerText;
			postsCollection = posts;
		}

		private void btnBack_Clicked(object sender, EventArgs e)
		{
			App.NavigateOut();
		}

		private void btnDetails_Clicked(object sender, EventArgs e)
		{
			if (listPosts.SelectedItem == null)
			{
				return;
			}
		}

		private View CreatePostView(FacebookPost post)
		{
			return new Label
			{
				FontSize = 10,
				Text = post.Message ?? post.Story
			};
		}

		protected async override void OnAppearing()
		{
			base.OnAppearing();

			var cursor = postsCollection;
			var allPosts = new ObservableCollection<FacebookPost>();
			listPosts.ItemsSource = allPosts;
			
			while (!cursor.IsEmpty)
			{
				foreach (var fbPost in cursor.Data)
				{
					allPosts.Add(fbPost);
					await fbPost.LoadInsights();
				}
				cursor = await cursor.Next();
			}

			// hide the loading indicator or update to empty indicator
			if (!allPosts.Any())
			{
				labelEmpty.IsVisible = true;
			}
		}
	}
}
