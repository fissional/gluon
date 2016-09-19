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

		private void btnBack_Clicked(object sender, EventArgs e)
		{
			App.NavigateOut();
		}
	}
}
