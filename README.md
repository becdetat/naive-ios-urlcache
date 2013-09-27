# Naive offline cache for iOS `UIWebView` applications

Dropping this in to an iOS application will allow `UIWebView` to work offline.

It doesn't include precaching (although that could be done by bundling cached files with the app) or any smarts around cache invalidation or caching while online. In fact if the device is online it will _always_ use the online resource and update the cached item. The only thing it adds is offline capability.

The cache is assigned to the static `NSUrlCache.SharedCache` so add this to your `AppDelegate.FinishedLaunching` or similar:

	var cachePath = Path.Combine(Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments), "urlcache");
	NSUrlCache.SharedCache = new OfflineUrlCache(cachePath);

This will only affect anything that uses `NSUrl` such as `UIWebView`. So anything that is in .NET world like RestSharp won't be affected. Which is usually what you would want.

It uses [Dan Clarke's Monotouch port](https://github.com/danclarke/Reachability) of [Tony Million's Reachability](https://github.com/tonymillion/Reachability class). Originally this started as a port of [Edwin Vermeer's EVURLCache](https://github.com/evermeer/EVURLCache) which has a lot more features, but I ended up cutting out just about everything.

