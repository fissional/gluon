using System;
using Xamarin.Forms;

namespace gluontest
{
	public partial class CreatePost : ContentPage
	{
		private DateTime SelectedDate
		{
			get
			{
				return switchSpecifyDate.IsToggled ? datePost.Date + timePost.Time : DateTime.Now;
			}
		}

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
			try
			{
				var newPostId = await App.Facebook.CreatePost(
					App.FacebookSettings.CurrentPage,
					editorPostBody.Text,
					switchPublished.IsToggled, switchSpecifyDate.IsToggled ? (DateTime?)SelectedDate : null);

				if (newPostId != null)
				{
					// jump to post details
					App.NavigateOut();
					App.Navigate(new PostDetails(await App.Facebook.GetObject<FacebookPost>(newPostId)));
				}
				else
				{
					// display an error because something went wrong
					await DisplayAlert("Post Not Created", "The post was not created due to an unknown error. Please try again.", "OK");
				}
			}
			catch (FacebookApiException fx)
			{
				await DisplayAlert(fx.Error.ErrorUserTitle ?? "Error Creating Post", fx.Error.ErrorUserMessage ?? fx.Error.Message, "OK");
			}
		}

		private void datePost_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName != "Date" && e.PropertyName != "Time")
			{
				return;
			}

			// indicate this post will be unpublished if the publication date is after today
			labelUnpublished.IsVisible = SelectedDate > DateTime.Now;
		}

		private void switchDate_Toggled(object sender, ToggledEventArgs e)
		{
			datePost.IsVisible = e.Value;
			timePost.IsVisible = e.Value;
			labelImmediate.IsVisible = !e.Value;
		}

		private void switchPublish_Toggled(object sender, ToggledEventArgs e)
		{
			if (switchSpecifyDate == null)
			{
				// this can happen before the date switch is constructed
				return;
			}

			// if this post is not published, disable the date picker
			switchSpecifyDate.IsEnabled = e.Value;
			if (!e.Value)
			{
				switchSpecifyDate.IsToggled = false;
			}
		}
	}
}
