using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SSMM2.BuilderComponents
{
	public class BasicEnemyTurretBuilder : Core.BuilderComponent
	{
		public String[] CorrespondingResourceNames
		{
			get
			{
				return new String[] { "Turret" };
			}
		}

		public void BuildObject(String resourceName, Core.Scene targetScene, Core.EntityConstructionData constructionData)
		{
			Game.BasicEnemyTurret turret = new Game.BasicEnemyTurret(targetScene);

			turret.Scale = constructionData.Scale;
			turret.BlendColor = constructionData.BlendColor;
			turret.SpriteRotationDegrees = MathHelper.ToDegrees (constructionData.Angle);
			turret.Position = constructionData.Position;
		}
	}
}
