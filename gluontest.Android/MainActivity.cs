using Android.App;
using Android.OS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace gluontest.Android
{
	[Activity(Label = "gluontest.Android", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : FormsApplicationActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			Forms.Init(this, savedInstanceState);
			LoadApplication(new gluontest.App());
		}
	}
}

