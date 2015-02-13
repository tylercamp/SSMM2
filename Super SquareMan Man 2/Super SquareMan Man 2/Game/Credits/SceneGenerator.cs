using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSMM2.Game.Credits
{


	public class SceneGenerator
	{
		public static Core.Scene CreateCreditsScene()
		{
			Core.Scene newScene = new Core.Scene();

			new CreditsController(newScene);

			newScene.LoadContent();
			return newScene;
		}
	}
}
