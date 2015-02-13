using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SSMM2.UI
{
	public class StartMenu
	{
		public static int StartMenuTag = Utility.Random.Next();

		public static Core.Scene GetNewStartMenuScene()
		{
			Core.Scene result = new Core.Scene();

			result.Camera.CenterCameraAroundPosition = false;

			Base.Button StartButton = new Base.Button(result, "Start");
			Base.Button QuitButton = new Base.Button(result, "Quit");

			Base.ControlOrganizer organizer = new Base.ControlOrganizer();
			organizer.Padding.Y = 30.0F;
			Viewport viewport = SquareManMan.Instance.GraphicsDevice.Viewport;
			organizer.Origin = new Vector2(viewport.Width / 2.0F, viewport.Height / 2.0F);

			organizer.AddControl(StartButton);
			organizer.AddControl(QuitButton);
			organizer.RealignControls();

			StartButton.Clicked += StartButton_Clicked;
			QuitButton.Clicked += QuitButton_Clicked;

			result.AddEntity(StartButton);
			result.AddEntity(QuitButton);

			result.LoadContent();

			return result;
		}

		static void QuitButton_Clicked(Base.Button button)
		{
			SquareManMan.Instance.Exit();
		}

		static void StartButton_Clicked(Base.Button button)
		{
			Game.Cutscenes.ImportantMessageOverlay instructions = new Game.Cutscenes.ImportantMessageOverlay();
			instructions.SceneEnded += instructions_SceneEnded;
			instructions.Text = "Use WASD/Arrow Keys to move.";
			Core.SceneManager.Instance.PopScene();
			instructions.Begin();
		}

		static void instructions_SceneEnded(Core.Cutscene scene)
		{
			Core.Scene level1Scene = new Core.Scene();
			Core.SceneBuilder.BuildScene("Content/Levels/Level 1.xml", level1Scene);
			level1Scene.LoadContent();
			Core.SceneManager.Instance.ReplaceScene(level1Scene);
		}
	}
}
