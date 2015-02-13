using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSMM2.UI
{
	class GameWonMenu
	{
		public static Core.Scene CreateNewGameWonMenuScene()
		{
			Core.Scene newScene = new Core.Scene();

			Base.Text youWon = new Base.Text(newScene, "You Won (I guess?)");
			Base.Button okCool = new Base.Button(newScene, "Ok cool");

			okCool.Clicked += okCool_Clicked;

			Base.ControlOrganizer organizer = new Base.ControlOrganizer();
			organizer.Origin = Vector2.Zero;
			organizer.Padding.Y = 30.0F;

			organizer.AddControl(youWon);
			organizer.AddControl(okCool);

			newScene.LoadContent();

			return newScene;
		}

		static void okCool_Clicked(Base.Button button)
		{
			Core.Scene targetScene = StartMenu.GetNewStartMenuScene();

			Core.SceneManager.Instance.ReplaceScene(targetScene);
		}
	}
}
