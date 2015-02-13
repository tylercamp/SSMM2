using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSMM2.BuilderComponents
{
	class GunBuilder : Core.BuilderComponent
	{
		public String[] CorrespondingResourceNames
		{
			get
			{
				return new String[] { "Pistol", "Shotgun" };
			}
		}

		public void BuildObject(String resourceName, Core.Scene targetScene, Core.EntityConstructionData constructionData)
		{
            var something = this.CorrespondingResourceNames;

			Game.Gun gun;
			switch (resourceName)
			{
				case ("Pistol"):
					gun = new Game.Guns.Pistol(targetScene, null);
					break;
				case ("Shotgun"):
					gun = new Game.Guns.Shotgun(targetScene, null);
					break;
				default:
					throw new NotImplementedException();
			}

			gun.BlendColor = constructionData.BlendColor;
			gun.SpriteRotationDegrees = constructionData.Angle;
			gun.Scale = constructionData.Scale;
			gun.Position = constructionData.Position;
		}
	}
}
