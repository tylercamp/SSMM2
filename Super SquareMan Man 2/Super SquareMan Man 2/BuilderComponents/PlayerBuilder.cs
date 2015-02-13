using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSMM2.BuilderComponents
{
	class PlayerBuilder : Core.BuilderComponent
	{
		public String[] CorrespondingResourceNames
		{
			get 
			{
				return new String[] { "Player" };
			}
		}

		public void BuildObject(String resourceName, Core.Scene targetScene, Core.EntityConstructionData constructionData)
		{
			Game.Player player = new Game.Player(targetScene);

			player.Position = constructionData.Position;
			player.BlendColor = constructionData.BlendColor;
		}
	}
}
