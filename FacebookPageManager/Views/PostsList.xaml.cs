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

		public PostsList(string headerText, FacebookPage page, FacebookPagedCollection<FacebookPost> posts)
		{
			InitializeComponent();

			labelPostHeader.Text = headerText;
			currentPage = page;
			postsCollection = posts;
		}

		private View CreatePostView(FacebookPost post)
		{
			return new Label
			{
				FontSize = 8,
				Text = post.Message ?? post.Story
			};
		}

		protected async override void OnAppearing()
		{
			base.OnAppearing();

			var cursor = postsCollection;

			while (!cursor.IsEmpty)
			{
				foreach (var fbPage in cursor.Data)
				{
					stackPostList.Children.Add(CreatePostView(fbPage));
				}
				cursor = await cursor.Next();
			}

			// hide the loading indicator
			labelLoading.IsVisible = false;
		}
	}
}
