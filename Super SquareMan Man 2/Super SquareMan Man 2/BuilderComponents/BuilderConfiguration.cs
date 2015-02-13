using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSMM2.BuilderComponents
{
	public class BuilderConfiguration
	{
		public static void Apply()
		{
			Core.SceneBuilder.Components = new List<Core.BuilderComponent>();

			List<Core.BuilderComponent> components = Core.SceneBuilder.Components;

			components.Add(new PlayerBuilder());
			components.Add(new RectangleBuilder());
			components.Add(new BasicEnemyTurretBuilder());
			components.Add(new GunBuilder());
			components.Add(new PowerupBuilder());
			components.Add(new BreakableWallBuilder());
		}
	}
}
