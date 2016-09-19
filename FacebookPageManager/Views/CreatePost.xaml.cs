using System;
using Xamarin.Forms;

namespace gluontest
{
	public partial class CreatePost : ContentPage
	{
		DateTime previousDate = DateTime.Now;
		private DateTime SelectedDate
		{
			get
			{
				return datePost.Date + timePost.Time;
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
			previousDate = SelectedDate;
		}

		private void switchDate_Toggled(object sender, ToggledEventArgs e)
		{
			datePost.IsEnabled = e.Value;
			timePost.IsEnabled = e.Value;

			if (e.Value)
			{
				// if the date pickers become enabled, display the previously selected date
				previousDate = SelectedDate;
				datePost.Date = previousDate.Date;
				timePost.Time = previousDate - previousDate.Date;
			}
			else
			{
				// when they are disabled, show the current date/time
				datePost.Date = DateTime.Now.Date;
				timePost.Time = DateTime.Now - datePost.Date;
			}
		}

		private void switchPublish_Toggled(object sender, ToggledEventArgs e)
		{
			if (switchSpecifyDate == null)
			{
				return;
			}

			// if this post is not published, disable the date picker
			if (!e.Value)
			{
				switchSpecifyDate.IsToggled = false;
				datePost.Date = previousDate.Date;
				timePost.Time = previousDate - previousDate.Date;
			}
		}
	}
}
