using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SSMM2.UI
{
	public class GameOverMenu
	{
		public static Core.Scene CreateNewGameOverMenuScene()
		{
			Core.Scene newScene = new Core.Scene();

			Base.Text youSuck = new Base.Text(newScene, "GAME OVER, YOU SUCK");
			Base.Button iKnow = new Base.Button(newScene, "I know");

			iKnow.Clicked += iKnow_Clicked;

			Base.ControlOrganizer organizer = new Base.ControlOrganizer();
			organizer.Origin = Vector2.Zero;
			organizer.Padding.Y = 30.0F;

			organizer.AddControl(youSuck);
			organizer.AddControl(iKnow);

			newScene.LoadContent();

			return newScene;
		}

		static void iKnow_Clicked(Base.Button button)
		{
			Core.SceneManager.Instance.PopScene();
		}
	}
}
