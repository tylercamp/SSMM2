using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;
using System.IO;
using Microsoft.Xna.Framework.Audio;

namespace SSMM2.Core
{
	public class ResourceManager
	{
		public static ResourceManager Instance;

		private ContentManager m_ContentManager;
		public ResourceManager (ContentManager contentManager)
		{
			m_ContentManager = contentManager;
		}

		private Hashtable m_CachedContent = new Hashtable();

		public T GetResource<T>(String contentName)
			where T : class
		{
			String translatedName = TranslateContentName<T>(contentName);
			if (!m_CachedContent.ContainsKey(translatedName))
			{
				try
				{
					PreCacheResource<T>(contentName);
				}
				catch (Exception e)
				{
					Debug.Commandline.Context.WriteLine("Warning: GetResource call failed for resource \"" + contentName + "\"");

					if (typeof(T) == typeof(Texture2D))
						return ForceGetResource<T>("NFTexture");
					if (typeof(T) == typeof(SpriteFont))
						return ForceGetResource<T>("NFFont");
					if (typeof(T) == typeof(SoundEffect))
						return ForceGetResource<T>("NFSound");

					throw new Exception("Could not pre-cache the given resource for access. " + e.Message);
				}
			}

			return m_CachedContent[translatedName] as T;
		}

		public void PreCacheResource<T> (String contentName)
			where T : class
		{
			String translatedName = TranslateContentName<T>(contentName);
			if (m_CachedContent.ContainsKey(translatedName)) return;

			m_CachedContent[translatedName] = m_ContentManager.Load<T>(PathToLocal (contentName));
		}

		private T ForceGetResource<T>(String contentName)
			where T : class
		{
			PreCacheResource<T>(contentName);
			return m_CachedContent[TranslateContentName<T>(contentName)] as T;
		}

		private String PathToLocal(String resourcePath)
		{
			String localPath;
			if (!Path.IsPathRooted(resourcePath))
			{
				localPath = resourcePath;
			}
			else
			{
				String searchString = "Content";

				localPath = resourcePath.Substring(resourcePath.IndexOf(searchString) + searchString.Length + 1);
			}

			return localPath;
		}

		private String TranslateContentName<T>(String contentName)
			where T : class
		{
			return (PathToLocal (contentName) + typeof(T).ToString()).ToLower();
		}
	}
}
