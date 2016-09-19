using System;
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

		private void btnCreatePost_Clicked(object sender, EventArgs e)
		{
		}
	}
}
