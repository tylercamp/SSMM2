using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSMM2.Debug
{
	class LevelRestartListener : Core.Widget
	{
		public override void LoadContent()
		{
			
		}

		public override void PostDraw()
		{
			KeyboardState keyboard = Core.InputManager.Instance.GetKeyboard(Core.InputManager.MasterAuthKey);
			if (keyboard.IsKeyDown(Keys.R) && keyboard.IsKeyDown(Keys.LeftControl))
			{
				Core.SceneManager manager = Core.SceneManager.Instance;
				Core.Scene topScene = manager.TopScene;
				if (topScene.SourceType == Core.Scene.SceneSourceType.File)
				{
					Core.Scene newScene = new Core.Scene();
					Core.SceneBuilder.BuildScene(topScene.Source, newScene);
					newScene.LoadContent();
					manager.PopScene();
					manager.PushScene(newScene);
				}
			}
		}
	}
}
