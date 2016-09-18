﻿using System;
using System.Collections.Generic;
using System.Text;

using Xamarin.Forms;

namespace gluontest
{
	public partial class PagesList : ContentPage
	{
		void btnBack_Clicked(object sender, System.EventArgs e)
		{
			App.NavigateOut();
		}

		public PagesList()
		{
			InitializeComponent();
		}

		private Button SelectedPageButton;

		private void SelectPage(Button button, FacebookPage page)
		{
			if (SelectedPageButton != null)
			{
				SelectedPageButton.Style = null;
			}

			SelectedPageButton = button;
			SelectedPageButton.Style = (Style)Application.Current.Resources["SelectedButton"];
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
				SelectedPageButton = newButton;
				SelectedPageButton.Style = (Style)Application.Current.Resources["SelectedButton"];
			}
			return newButton;
		}

		protected async override void OnAppearing()
		{
			base.OnAppearing();

			var user = await App.Facebook.GetMe();
			var cursor = await App.Facebook.GetPages(user);

			while (!cursor.IsEmpty)
			{
				foreach (var fbPage in cursor.Data)
				{
					stackPageList.Children.Add(CreatePageButton(fbPage));
				}
				cursor = await cursor.Next();
			}

			// hide the loading indicator
			labelLoading.IsVisible = false;
		}
	}
}

