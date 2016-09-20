using System;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;

namespace gluontest
{
	public partial class PostsList : ContentPage
	{
		FacebookPagedCollection<FacebookPost> m_postsCollection;

		public PostsList(string headerText, FacebookPagedCollection<FacebookPost> posts)
		{
			InitializeComponent();

			labelPostHeader.Text = headerText;
			m_postsCollection = posts;
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

			App.Navigate(new PostDetails((FacebookPost)listPosts.SelectedItem));
		}

		private void listPosts_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
		{
			buttonPostDetails.IsEnabled = sender != null;
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
			if (listPosts.ItemsSource != null)
			{
				return;
			}

			var cursor = m_postsCollection;
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
