using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using BelfryImages.Infrastructure;
using System.IO;
using MonoTouch.Dialog;

namespace NaiveIosUrlCache
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		UIWindow window;

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			var cachePath = Path.Combine(Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments), "urlcache");
			NSUrlCache.SharedCache = new OfflineUrlCache (cachePath);

			window = new UIWindow (UIScreen.MainScreen.Bounds);
			window.RootViewController = new MyViewController ("https://github.com/belfryimages/naive-ios-urlcache");

			window.MakeKeyAndVisible ();

			using (var alert = new UIAlertView(
				"Offline cache", "When online a Github page should open. To test the cache: force quit the app, turn on airplane mode, and restart the app. The Github page should load from the cache.", 
				null, "Ok"))
				alert.Show ();
			
			return true;
		}

		class MyViewController : UIViewController {
			readonly UIWebView _webView;

			public MyViewController(string url) {
				_webView = new UIWebView(View.Bounds);

				var request = new NSUrlRequest(new NSUrl(url));
				_webView.LoadRequest(request);

				View = _webView;
			}
		}
	}
}

