using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SSMM2.Core
{
	public class EntityConstructionData
	{
		public Color BlendColor;
		public Vector2 Position;
		public Vector2 Scale;
		public float Angle;
		public String Tag;

		public Dictionary<String, object> CustomProperties;
	}

	public interface BuilderComponent
	{
		String [] CorrespondingResourceNames
		{
			get;
		}

		void BuildObject(String resourceName, Scene targetScene, EntityConstructionData constructionData);
	}

	public class SceneBuilder
	{
		public static String LastScene
		{
			get;
			private set;
		}

		public static List<BuilderComponent> Components;

		public static void BuildScene(String sourceFile, Scene targetScene)
		{
			LastScene = sourceFile;

			if (!File.Exists(sourceFile))
				throw new Exception("Unable to find level file.");

			List<EntityConstructionData> levelData = SceneParsers.Gleed2d.GenerateSceneMembers(sourceFile);

			foreach (EntityConstructionData entityData in levelData)
			{
				foreach (BuilderComponent builder in Components)
				{
					foreach (String relevantAsset in builder.CorrespondingResourceNames)
					{
						if (relevantAsset == entityData.Tag)
						{
							builder.BuildObject(entityData.Tag, targetScene, entityData);
						}
					}
				}
			}

			targetScene.Source = sourceFile;
			targetScene.SourceType = Scene.SceneSourceType.File;
		}
	}
}
