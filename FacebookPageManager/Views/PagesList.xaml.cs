using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace gluontest
{
	public partial class PagesList : ContentPage
	{
		private Button m_selectedPageButton;

		public PagesList()
		{
			InitializeComponent();
		}

		private void btnBack_Clicked(object sender, System.EventArgs e)
		{
			App.NavigateOut();
		}

		private void SelectPage(Button button, FacebookPage page)
		{
			if (button.Equals(m_selectedPageButton))
			{
				App.NavigateOut();
			}
			if (m_selectedPageButton != null)
			{
				m_selectedPageButton.Style = null;
			}

			m_selectedPageButton = button;
			m_selectedPageButton.Style = (Style)Application.Current.Resources["SelectedButton"];
			App.FacebookSettings.CurrentPage = page;
		}

		private View CreatePageButton(FacebookPage page)
		{
			var newButton = new Button();
			newButton.Text = page.Name;
			newButton.BorderWidth = 1;
			newButton.Clicked += (sender, e) => SelectPage((Button)sender, page);
			if (App.FacebookSettings.CurrentPage != null && App.FacebookSettings.CurrentPage.Id == page.Id)
			{
				m_selectedPageButton = newButton;
				m_selectedPageButton.Style = (Style)Application.Current.Resources["SelectedButton"];
			}
			return newButton;
		}

		protected async override void OnAppearing()
		{
			base.OnAppearing();

			var user = await App.Facebook.GetMe();
			var cursor = await App.Facebook.GetPages(user);
			var anyDataFound = cursor.Data.Any();

			while (!cursor.IsEmpty)
			{
				foreach (var fbPage in cursor.Data)
				{
					stackPageList.Children.Add(CreatePageButton(fbPage));
				}
				cursor = await cursor.Next();
			}

			// hide the loading indicator or update to empty indicator
			if (anyDataFound)
			{
				labelLoading.IsVisible = false;
			}
			else
			{
				labelLoading.Text = "No pages found :(";
			}
		}
	}
}

