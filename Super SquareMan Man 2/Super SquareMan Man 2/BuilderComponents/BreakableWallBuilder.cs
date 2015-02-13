using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSMM2.BuilderComponents
{
	public class BreakableWallBuilder : Core.BuilderComponent
	{
		public String[] CorrespondingResourceNames
		{
			get
			{
				return new String[] { "BreakableWall" };
			}
		}

		public void BuildObject(String resourceName, Core.Scene targetScene, Core.EntityConstructionData constructionData)
		{
			Game.BreakableWall wall = new Game.BreakableWall(targetScene);
			wall.Position = constructionData.Position;
			wall.BlendColor = constructionData.BlendColor;
			wall.Scale = constructionData.Scale;
		}
	}
}
