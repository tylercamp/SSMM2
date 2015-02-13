using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace SSMM2.Core.SceneParsers
{
	class Gleed2d
	{
		public static List<EntityConstructionData> GenerateSceneMembers(String sourceFile)
		{
			XmlDocument levelData = new XmlDocument();
			levelData.Load(sourceFile);

			var result = new List<EntityConstructionData>();

			XmlNodeList levelComponents = levelData.GetElementsByTagName("Item");
			foreach (XmlNode node in levelComponents)
			{
				XmlAttribute nodeType = node.Attributes["xsi:type"];
				if (
					nodeType != null &&
					nodeType.Value != "TextureItem" &&
					nodeType.Value != "RectangleItem"
					)
					throw new Exception("Unrecognized resource type " + nodeType.Value);



				EntityConstructionData entityData = new EntityConstructionData();

				var customPropertiesNode = node["CustomProperties"];
				ParseProperties(customPropertiesNode, entityData);

				String componentType = nodeType.Value;
				switch (componentType)
				{
					case ("TextureItem"):
						{
							ParseTexture(node, entityData);
							break;
						}

					case ("RectangleItem"):
						{
							ParseRectangle(node, entityData);
							break;
						}

					default:
						{
							entityData.Tag = "";
							break;
						}
				}

				result.Add(entityData);
			}

			return result;
		}

		private static void ParseRectangle(XmlNode node, EntityConstructionData target)
		{
			target.Angle = 0.0F;

			target.BlendColor = new Color();
			target.BlendColor.R = byte.Parse(node["FillColor"]["R"].InnerText);
			target.BlendColor.G = byte.Parse(node["FillColor"]["G"].InnerText);
			target.BlendColor.B = byte.Parse(node["FillColor"]["B"].InnerText);
			target.BlendColor.A = byte.Parse(node["FillColor"]["A"].InnerText);


			target.Position = new Vector2(
				float.Parse(node["Position"]["X"].InnerText),
				float.Parse(node["Position"]["Y"].InnerText)
				);

			target.Scale = new Vector2(
				float.Parse(node["Width"].InnerText),
				float.Parse(node["Height"].InnerText)
				);

			target.Tag = "$Rectangle";
		}

		private static void ParseTexture(XmlNode node, EntityConstructionData target)
		{
			target.Angle = float.Parse(node["Rotation"].InnerText);

			target.BlendColor = new Color();
			target.BlendColor.R = byte.Parse(node["TintColor"]["R"].InnerText);
			target.BlendColor.G = byte.Parse(node["TintColor"]["G"].InnerText);
			target.BlendColor.B = byte.Parse(node["TintColor"]["B"].InnerText);
			target.BlendColor.A = byte.Parse(node["TintColor"]["A"].InnerText);

			target.Scale = new Vector2(
				float.Parse(node["Scale"]["X"].InnerText),
				float.Parse(node["Scale"]["Y"].InnerText)
				);

			target.Position = new Vector2(
				float.Parse(node["Position"]["X"].InnerText),
				float.Parse(node["Position"]["Y"].InnerText)
				);

			target.Tag = Path.GetFileName(node["asset_name"].InnerText);
			//	TODO: Should this be pre-cached here?
			ResourceManager.Instance.PreCacheResource<Texture2D>(node["asset_name"].InnerText);
		}

		private static void ParseProperties(XmlNode propertiesNode, EntityConstructionData target)
		{
			target.CustomProperties = new Dictionary<String, object>();
			foreach (XmlNode currentProperty in propertiesNode)
			{
				String propertyName = currentProperty.Attributes["Name"].InnerText;
				String propertyType = currentProperty.Attributes["Type"].InnerText;

				switch (propertyType.ToLower())
				{
					case "bool":
						{
							target.CustomProperties[propertyName] = new Nullable<bool>(bool.Parse(currentProperty["boolean"].InnerText));
							break;
						}

					case "string":
						{
							target.CustomProperties[propertyName] = currentProperty["string"].InnerText;
							break;
						}

					default:
						throw new Exception("Unable to process unknown attribute type " + propertyType);
				}
			}
		}
	}
}
