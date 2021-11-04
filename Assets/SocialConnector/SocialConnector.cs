using UnityEngine;

#if UNITY_IPHONE

using System.Runtime.InteropServices;

#endif

namespace SocialConnector
{
	public class SocialConnector
	{
		#if UNITY_IPHONE
		[DllImport ("__Internal")]
		private static extern void SocialConnector_Share (string text, string url, string textureUrl);


		#elif UNITY_ANDROID
		private static AndroidJavaObject clazz = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
		private static AndroidJavaObject activity = clazz.GetStatic<AndroidJavaObject> ("currentActivity");
		#endif

		#if UNITY_IPHONE
		
		private static void _Share (string text, string url, string textureUrl)
		{
			SocialConnector_Share (text, url, textureUrl);
		}


		#elif UNITY_ANDROID
		
		private static void _Share (string text, string url, string textureUrl)
		{
			using (var intent = new AndroidJavaObject ("android.content.Intent")) {
				intent.Call<AndroidJavaObject> ("setAction", "android.intent.action.SEND");
				intent.Call<AndroidJavaObject> ("setType", string.IsNullOrEmpty (textureUrl) ? "text/plain" : "image/png");

				if (!string.IsNullOrEmpty (url))
					text += "\t" + url;
				if (!string.IsNullOrEmpty (text))
					intent.Call<AndroidJavaObject> ("putExtra", "android.intent.extra.TEXT", text);

				if (!string.IsNullOrEmpty (textureUrl)) {

					var versionClazz = new AndroidJavaClass("android.os.Build$VERSION");
    				var apiLevel = versionClazz.GetStatic<int>("SDK_INT");
					AndroidJavaObject uri;
					if(24 <= apiLevel) {
						var context = activity.Call<AndroidJavaObject> ("getApplicationContext");
						var fileProvider = new AndroidJavaClass("android.support.v4.content.FileProvider");
						var file = new AndroidJavaObject ("java.io.File", textureUrl);
						uri = fileProvider.CallStatic<AndroidJavaObject>("getUriForFile", context, Application.identifier + ".fileprovider", file);
					} else {
						var uriClazz = new AndroidJavaClass ("android.net.Uri");
						var file = new AndroidJavaObject ("java.io.File", textureUrl);
						uri = uriClazz.CallStatic<AndroidJavaObject> ("fromFile", file);
					}
					intent.Call<AndroidJavaObject> ("putExtra", "android.intent.extra.STREAM", uri);
				}
				var chooser = intent.CallStatic<AndroidJavaObject> ("createChooser", intent, "");
				chooser.Call<AndroidJavaObject> ("putExtra", "android.intent.extra.EXTRA_INITIAL_INTENTS", intent);
				activity.Call ("startActivity", chooser);
			}
		}
		#endif

		public static void Share (string text)
		{
			Share (text, null, null);
		}

		public static void Share (string text, string url)
		{
			Share (text, url, null);
		}

		public static void Share (string text, string url, string textureUrl)
		{
#if UNITY_ANDROID || UNITY_IPHONE
			_Share (text, url, textureUrl);
#endif
		}
	}
}