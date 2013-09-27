/* 
Copyright (c) 2013 Ben Scott - https://github.com/belfryimages/naive-ios-urlcache
Licensed under the Apache License 2.0
See https://github.com/belfryimages/naive-ios-urlcache/blob/master/LICENSE for details
*/

using System;
using MonoTouch.Foundation;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace BelfryImages.Infrastructure
{
    public class OfflineUrlCache : NSUrlCache
    {
        readonly string _urlCachePath;
        readonly Action<string> _log;

		public OfflineUrlCache(string urlCachePath,
                             Action<string> log = null) {
            _urlCachePath = urlCachePath;
            _log = log ?? new Action<string>((x) => Console.WriteLine(x));

            if (!Directory.Exists(urlCachePath)) {
                Directory.CreateDirectory (urlCachePath);
            }
        }

        public override NSCachedUrlResponse CachedResponseForRequest(NSUrlRequest request) {
            _log (string.Format("OfflineUrlCache:CachedResponseForRequest: Cache request: {0}", request));

            // Check for reachability
            if (Reachability.Reachability.ReachabilityForInternet().CurrentStatus != Reachability.ReachabilityStatus.NotReachable) {
                // This isn't an optimising cache, if we are connected use the intarwebz
                _log ("OfflineUrlCache:CachedResponseForRequest: Network is available");
                return null;
            }

            _log ("OfflineUrlCache:CachedResponseForRequest: Network is NOT available");

            var storagePath = GetStoragePath (request);
            _log (string.Format("OfflineUrlCache:CachedResponseForRequest: Storage path: {0}", storagePath));

            if (File.Exists (storagePath)) {
                // Return cached response
                var content = NSData.FromFile (storagePath);
                _log (string.Format("OfflineUrlCache:CachedResponseForRequest: Returning cached response ({0} bytes)", content.Length));
                var response = new NSUrlResponse (request.Url, "text/html", (int)content.Length, null);
                return new NSCachedUrlResponse (response, content);
            }

            _log("OfflineUrlCache:CachedResponseForRequest: Request was not found in the cache");

            return null;
        }

        string GetStoragePath(NSUrlRequest request) {
            // The filename is the MD5 hash of the URL with dashes removed:

            var url = request.Url.AbsoluteString;
            var bytes = new UTF8Encoding ().GetBytes (url);
            var provider = new MD5CryptoServiceProvider ();
            var hashBytes = provider.ComputeHash (bytes);
            var hash = BitConverter.ToString (hashBytes);
            var filename = hash.Replace ("-", "");

            return Path.Combine (_urlCachePath, filename);
        }

        public override void StoreCachedResponse (NSCachedUrlResponse cachedResponse, NSUrlRequest forRequest) {
            _log (string.Format("OfflineUrlCache:StoreCachedResponse: For request: {0}", forRequest));

            var storagePath = GetStoragePath (forRequest);
            _log (string.Format("OfflineUrlCache:StoreCachedResponse: Storing to: {0}", storagePath));

            NSError error = null;
            if (!cachedResponse.Data.Save (storagePath, true, out error)) {
                _log (string.Format("OfflineUrlCache:StoreCachedResponse: Couldn't write to cache:  {0}", error));
            }
        }
    }
}
