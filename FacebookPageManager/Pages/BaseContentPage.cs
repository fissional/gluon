using Xamarin.Forms;

namespace gluontest
{
	public class BaseContentPage : ContentPage
	{
		protected override void OnAppearing()
		{
			base.OnAppearing();

			if (!App.IsLoggedIn && (Navigation.ModalStack.Count == 0 || !(Navigation.ModalStack[0] is LoginPage)))
			{ 
				Navigation.PushModalAsync(new LoginPage());
			}
		}
	}
}