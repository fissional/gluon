﻿using System;
using Xamarin.Forms;

namespace gluontest
{
	public partial class CreatePost : ContentPage
	{
		public CreatePost()
		{
			InitializeComponent();
		}

		private void btnBack_Clicked(object sender, EventArgs e)
		{
			App.NavigateOut();
		}

		private async void btnCreatePost_Clicked(object sender, EventArgs e)
		{
			var newPostId = await App.Facebook.CreatePost(App.FacebookSettings.CurrentPage, editorPostBody.Text, true);

			if (newPostId != null)
			{
				// jump to post details
				App.NavigateOut();
				App.Navigate(new PostDetails(await App.Facebook.GetObject<FacebookPost>(newPostId)));
			}
			else
			{
				// display an error because something went wrong
			}
		}

		private void datePost_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName != "Date")
			{
				return;
			}

			// indicate this post will be unpublished if the publication date is after today
			labelUnpublished.IsVisible = ((DatePicker)sender).Date > DateTime.Now;
		}
	}
}
