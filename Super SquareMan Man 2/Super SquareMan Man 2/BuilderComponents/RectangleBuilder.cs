using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSMM2.BuilderComponents
{
	public class RectangleBuilder : Core.BuilderComponent
	{
		public String[] CorrespondingResourceNames
		{
			get
			{
				return new String[] { "$Rectangle" };
			}
		}

		public void BuildObject(String resourceName, Core.Scene targetScene, Core.EntityConstructionData constructionData)
		{
			if (constructionData.BlendColor == new Color(200, 66, 66, 200))
			{
				bool isLava =
					constructionData.CustomProperties.ContainsKey("IsLava") &&
					(constructionData.CustomProperties["IsLava"] as bool ?).Value;

				Game.OutsideRoomBorder border = new Game.OutsideRoomBorder(targetScene, isLava);
				border.Position = constructionData.Position;
				border.Scale = constructionData.Scale;
				return;
			}

			if (constructionData.BlendColor == new Color(0, 0, 128, 255))
			{
				String targetLevel = constructionData.CustomProperties["TargetLevel"] as String;

				Game.LevelChangeObject levelChange = new Game.LevelChangeObject(targetScene);
				levelChange.Position = constructionData.Position;
				levelChange.Scale = constructionData.Scale;
				levelChange.BlendColor = constructionData.BlendColor;
				levelChange.TargetLevel = targetLevel;
				return;
			}

			if (constructionData.BlendColor == new Color(77, 77, 77, 77))
			{
				Game.RespawnPoint respawnPoint = new Game.RespawnPoint(targetScene);
				respawnPoint.Position = constructionData.Position;
				respawnPoint.Scale = constructionData.Scale;
				return;
			}

			Game.SizeableWallBlock wall = new Game.SizeableWallBlock(targetScene, constructionData.Position);

			wall.Scale = constructionData.Scale;
			wall.BlendColor = constructionData.BlendColor;
			wall.SpriteRotationDegrees = constructionData.Angle;
		}
	}
}
