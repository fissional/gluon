using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;

namespace gluontest
{
	public partial class PostsList : ContentPage
	{
		FacebookPagedCollection<FacebookPost> postsCollection;
		FacebookPost selectedPost;
		//FacebookPage currentPage;

		void btnBack_Clicked(object sender, System.EventArgs e)
		{
			App.NavigateOut();
		}

		void Handle_PostSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
		{
			selectedPost = (FacebookPost)e.SelectedItem;
		}

		public PostsList(string headerText, FacebookPage page, FacebookPagedCollection<FacebookPost> posts)
		{
			InitializeComponent();

			labelPostHeader.Text = headerText;
			//currentPage = page;
			postsCollection = posts;
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
