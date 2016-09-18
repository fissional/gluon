using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace gluontest
{
	public partial class PostsList : ContentPage
	{
		FacebookPagedCollection<FacebookPost> postsCollection;
		FacebookPage currentPage;

		void btnBack_Clicked(object sender, System.EventArgs e)
		{
			App.NavigateOut();
		}

		public PostsList(FacebookPage page, FacebookPagedCollection<FacebookPost> posts)
		{
			InitializeComponent();
			currentPage = page;
			postsCollection = posts;
		}

		private View CreatePostView(FacebookPost post)
		{
			return new Label
			{
				Text = post.Message ?? post.Story
			};
		}

		protected async override void OnAppearing()
		{
			base.OnAppearing();

			var cursor = postsCollection;
			int currentIndex = 0;
			while (!cursor.IsEmpty)
			{
				foreach (var fbPage in cursor.Data)
				{
					stackPostList.Children.Insert(currentIndex, CreatePostView(fbPage));
					currentIndex = 1;
				}
				cursor = await cursor.Next();
			}

			// hide the loading indicator
			labelLoading.IsVisible = false;
		}
	}
}
