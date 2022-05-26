using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Gamla.Logic
{
	public class RemoteResourceManager
	{
		public static readonly string BundleFolder = "RemoteContent";
		public static readonly int MaxSaveContentLenght = 40;
		
		private static readonly Dictionary<string, RemoteContent> Contents = new Dictionary<string, RemoteContent>();
		private static string _finallPathUrl;

		private static DontDestroyOnLoad _ResourceGo;

		public static int GetCount()
		{
			return Contents.Count;
		}


		public static void DestroyLocalContent()
		{
			var path = Application.persistentDataPath + "/" + BundleFolder;

			if (Directory.Exists(path)) Directory.Delete(path, true);

			Directory.CreateDirectory(path);
		}

		public static void Init()
		{
			var resourceGo = new GameObject("_GamlaResourceManagerGo");
			resourceGo.hideFlags = HideFlags.HideAndDontSave;
			_ResourceGo = resourceGo.AddComponent<DontDestroyOnLoad>();
			
			_finallPathUrl = Application.persistentDataPath + "/" + BundleFolder;
			if (!Directory.Exists(_finallPathUrl)) Directory.CreateDirectory(_finallPathUrl);
			EventManager.DelayEvent.Add("RemoteResourceManager", RemoveOld, 4000, true);
		}

		public static bool HasLocalContent(string name)
		{
			string path = _finallPathUrl + "/" + name + ".jpg";
			return Utils.CheckFileExists(path);
		}

		public static void GetContent(string url, Action<Texture2D> callbackAction, GameObject callbackOwner)
		{
			if (string.IsNullOrEmpty(url))
			{
				return;
			}
			
			var name = "ava_" + url.GetHashCode();

			name = name.ToLower();
			if (callbackOwner != null)
			{
				if (Contents.ContainsKey(name))
				{
					RemoteContent bundle = Contents[name];
					bundle.IsFree();
					bundle.AddOwner(callbackOwner, callbackAction);
					if (bundle.IsHaveContent()) callbackAction.Invoke(bundle.Get());
				}
				else
				{
					var container = new RemoteContent(name, null, callbackOwner, callbackAction);
					Contents.Add(name, container);
					Load(url, container);
				}

				if (Contents.Count > MaxSaveContentLenght)
				{
					RemoveOld();
				}
			}
		}

		public static void LoadToRawImage(RawImage avatar, string _tempUrl) //Todo: to RawImage extension
		{
			if (_tempUrl != String.Empty)
			{
				GetContent(_tempUrl, texture2D =>
				{
					if (texture2D != null)
					{
						var _temp = texture2D;
						if (avatar != null && avatar != null && avatar.gameObject != null)
							avatar.texture = _temp;
					}
				}, avatar.gameObject);
			}
		}

		private static void RemoveOld()
		{
			var toRemove = Contents.Where(pair => pair.Value.IsOldBundle())
				.Select(pair => pair.Key)
				.ToList();

			foreach (var key in toRemove)
			{
				if (Contents[key].Texture != null)
				{
					GameObject.Destroy(Contents[key].Texture);
				}

				Contents[key].Clear();
				Contents.Remove(key);
			}
		}

		private static void Load(string url, RemoteContent container)
		{
			var name = "ava_" + url.GetHashCode();
			_ResourceGo.StartCoroutine(LoadTextureCoroutine(name, url, container));
		}

		private static IEnumerator LoadTextureCoroutine(String name, String url, RemoteContent container)
		{
			string path = _finallPathUrl + "/" + name + ".jpg";

			Debug.Log("path: " + path);

			if (Utils.CheckFileExists(path))
			{
				Texture2D texture = new Texture2D(2, 2, TextureFormat.ARGB32, false);
				texture.LoadImage(Utils.ReadFile(path), true);
				texture.name = container.textureName;

				container.Texture = texture;
				container.Loaded();
			}
			else
			{
				WWW w = new WWW(url);
				yield return w;

				if (w.error != null)
				{
					container.Loaded();
				}
				else
				{
					Utils.SaveFile(_finallPathUrl + "/" + container.textureName + ".jpg", w.bytes);
					yield return null;

					Texture2D texture = new Texture2D(2, 2, TextureFormat.ARGB32, false);
					texture.LoadImage(w.bytes, true);

					texture.name = container.textureName;
					container.Texture = texture;
					container.Loaded();
				}
			}

			yield return null;
		}
	}

	public class RemoteContent
	{
		public string textureName = "";
		private readonly List<GameObject> _owners = new List<GameObject>();
		public long LastTimeUpdate;
		public Texture2D Texture;
		private readonly List<Action<Texture2D>> _callbackActions = new List<Action<Texture2D>>();

		public RemoteContent(string name, Texture2D texture, GameObject owner, Action<Texture2D> callbackAction)
		{
			textureName = name;
			Texture = texture;
			AddOwner(owner, callbackAction);
		}

		public bool IsOldBundle()
		{
			return Utils.GetPlainUnixTime - LastTimeUpdate > 10 && IsFree();
		}

		public Texture2D Get()
		{
			LastTimeUpdate = Utils.GetPlainUnixTime;
			return Texture;
		}

		public void Loaded()
		{
			foreach (var callbackAction in _callbackActions)
			{
				if (callbackAction != null) callbackAction.Invoke(Texture);
			}

			_callbackActions.Clear();
		}

		public bool IsHaveContent()
		{
			return Texture != null;
		}

		public void AddOwner(GameObject go, Action<Texture2D> callbackAction)
		{
			_owners.Add(go);
			_callbackActions.Add(callbackAction);
		}

		public bool IsFree()
		{
			var result = true;
			foreach (var gOwner in _owners)
			{
				if (gOwner != null) result = false;
			}

			_owners.RemoveAll(r => r == null);
			return result;
		}

		public void Clear()
		{
			_owners.Clear();
			_callbackActions.Clear();
		}
	}
}