using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace gluontest
{
	public partial class PostDetails : ContentPage
	{
		FacebookPost m_post;

		public PostDetails(FacebookPost post)
		{
			InitializeComponent();
			m_post = post;
			this.BindingContext = m_post;
		}

		protected async override void OnAppearing()
		{
			base.OnAppearing();
			await m_post.LoadLikes();

			var attachments = await App.Facebook.GetPostAttachments(m_post);
			if (attachments.Data.Count > 0 && attachments.Data[0].Media?.Image?.ImageUrl != null)
			{
				// just display a single image here
				imageAttach.Source = ImageSource.FromUri(new Uri(attachments.Data[0].Media.Image.ImageUrl));
			}
		}

		private void btnBack_Clicked(object sender, EventArgs e)
		{
			App.NavigateOut();
		}
	}
}
