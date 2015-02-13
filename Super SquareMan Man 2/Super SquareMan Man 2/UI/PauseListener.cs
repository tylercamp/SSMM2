using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSMM2.UI
{
	public class PauseListener : Core.Widget
	{
		private uint m_AuthKey = Core.InputManager.ElevatedAuthKey;

		public int PauseSceneTag = Utility.Random.Next();

		public override void LoadContent()
		{
			Core.InputManager.Instance.Bind("Pause", Keys.Escape);
		}

		private void ConfigureScenePauseMenu(Core.Scene targetScene)
		{
			Base.Button resumeButton = new Base.Button(targetScene, "Resume");
			Base.Button creditsButton = new Base.Button(targetScene, "Credits");
			Base.Button quitButton = new Base.Button(targetScene, "Quit Game");
			Base.Button restartButton = new Base.Button(targetScene, "Restart Level");
			Base.Button toMenuButton = new Base.Button(targetScene, "Back to Menu");

			resumeButton.Clicked += resumeButton_Clicked;
			creditsButton.Clicked += creditsButton_Clicked;
			quitButton.Clicked += quitButton_Clicked;
			restartButton.Clicked += restartButton_Clicked;
			toMenuButton.Clicked += toMenuButton_Clicked;

			Base.ControlOrganizer organizer = new Base.ControlOrganizer();
			organizer.Padding.Y = 10.0F;

			Viewport view = SquareManMan.Instance.GraphicsDevice.Viewport;
			organizer.Origin.X = view.Width / 2.0F;
			organizer.Origin.Y = view.Height / 2.0F;

			organizer.AddControl(resumeButton);
			organizer.AddControl(creditsButton);
			organizer.AddControl(restartButton);
			organizer.AddControl(toMenuButton);
			organizer.AddControl(quitButton);

			targetScene.Camera.CenterCameraAroundPosition = false;
			targetScene.Tag = PauseSceneTag;
		}

		void creditsButton_Clicked(Base.Button button)
		{
			Core.SceneManager.Instance.PushModifier(false, false);
			Core.SceneManager.Instance.PushScene(Game.Credits.SceneGenerator.CreateCreditsScene());
		}

		public void CloseMenu()
		{
			if (Core.SceneManager.Instance.TopScene.Tag == PauseSceneTag)
				Core.SceneManager.Instance.PopScene();
		}

		void toMenuButton_Clicked(Base.Button button)
		{
			CloseMenu();

			Core.Scene newScene = StartMenu.GetNewStartMenuScene();
			Core.SceneManager.Instance.ReplaceScene(newScene);
			newScene.LoadContent();
		}

		void restartButton_Clicked(Base.Button button)
		{
			var sceneManager = Core.SceneManager.Instance;
			CloseMenu();

			Core.Scene newScene = new Core.Scene();
			Core.SceneBuilder.BuildScene(Core.SceneBuilder.LastScene, newScene);
			newScene.LoadContent();
			sceneManager.ReplaceScene(newScene);
		}

		void quitButton_Clicked(Base.Button button)
		{
			SquareManMan.Instance.Exit();
		}

		void resumeButton_Clicked(Base.Button button)
		{
			CloseMenu();
		}

		public override void PreUpdate()
		{
			base.PreUpdate();

			var sceneManager = Core.SceneManager.Instance;

			var input = Core.InputManager.Instance;

			if (!input.PassesAuthorityCheck(m_AuthKey))
				return;

			if (input.CheckPressed("Pause", m_AuthKey))
			{
				if (sceneManager.TopScene.Tag == PauseSceneTag)
				{
					sceneManager.PopScene();
				}
				else
				{
					Core.Scene newScene = new Core.Scene();
					ConfigureScenePauseMenu(newScene);
					sceneManager.PushModifier(false, true);
					sceneManager.PushScene(newScene);
					newScene.LoadContent();
				}
			}
		}
	}
}
