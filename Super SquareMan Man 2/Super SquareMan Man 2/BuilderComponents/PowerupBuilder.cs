using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSMM2.BuilderComponents
{
	class PowerupBuilder : Core.BuilderComponent
	{
		public String[] CorrespondingResourceNames
		{
			get
			{
				return new String[] { "JumpUpgrade", "TimeUpgrade" };
			}
		}

		public void BuildObject(String resourceName, Core.Scene targetScene, Core.EntityConstructionData constructionData)
		{
			switch (resourceName)
			{
				case ("JumpUpgrade"):
					{
						Game.JumpUpgrade newUpgrade = new Game.JumpUpgrade(targetScene);
						newUpgrade.Position = constructionData.Position;
						break;
					}

				case ("TimeUpgrade"):
					{
						Game.TimeUpgrade newUpgrade = new Game.TimeUpgrade(targetScene);
						newUpgrade.Position = constructionData.Position;
						break;
					}
			}
		}
	}
}
