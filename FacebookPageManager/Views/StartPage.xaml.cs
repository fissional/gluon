using System.Text;
using Xamarin.Forms;

namespace gluontest
{
	public partial class StartPage : BaseContentPage
	{
		public StartPage()
		{
			InitializeComponent();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			if (App.IsLoggedIn)
			{
				// show the UI if we've logged in
				this.buttonStack.IsVisible = true;
			}
		}

		void btnSettings_Clicked(object sender, System.EventArgs e)
		{
			App.Navigate(new PagesList());
		}
	}
}

