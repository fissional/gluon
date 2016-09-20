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
		}

		private void btnBack_Clicked(object sender, EventArgs e)
		{
			App.NavigateOut();
		}
	}
}
