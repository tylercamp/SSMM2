using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SSMM2
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class SquareManMan : Microsoft.Xna.Framework.Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		Core.PrimitiveBatch primitiveBatch;

		public Color BackgroundColor = Color.Black;

		public static SquareManMan Instance
		{
			get;
			private set;
		}

		public SquareManMan()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";

			graphics.PreferredBackBufferWidth = 1280;
			graphics.PreferredBackBufferHeight = 720;
			//graphics.IsFullScreen = true;

			IsMouseVisible = true;

			Instance = this;

			IsFixedTimeStep = false;

			Core.FpsTracker.Instance = new Core.FpsTracker();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);
			primitiveBatch = new Core.PrimitiveBatch(GraphicsDevice);

			// TODO: use this.Content to load your game content here
			Core.SceneManager.Instance = new Core.SceneManager();
			Core.ResourceManager.Instance = new Core.ResourceManager(Content);
			Core.InputManager.Instance = new Core.InputManager();
			Core.SignalDispatcher.Instance = new Core.SignalDispatcher();

			BuilderComponents.BuilderConfiguration.Apply();

			Debug.Commandline commandline = new Debug.Commandline();
			Debug.Commandline.Context = commandline;
			commandline.LoadContent();
			Core.SceneManager.Instance.AddWidget(commandline);

			Debug.CommandlineConfig.Apply();

			Debug.DebugView debugView = new Debug.DebugView();
			Debug.DebugView.Context = debugView;
			debugView.LoadContent();
			Core.SceneManager.Instance.AddWidget(debugView);


			Core.Scene startScene = UI.StartMenu.GetNewStartMenuScene();
			//startScene.LoadContent();
			Core.SceneManager.Instance.PushScene(startScene);

			UI.PauseListener pause = new UI.PauseListener();
			Core.SceneManager.Instance.AddWidget(pause);
			pause.LoadContent();

			Debug.LevelRestartListener levelRestart = new Debug.LevelRestartListener();
			Core.SceneManager.Instance.AddWidget(levelRestart);

		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// all content.
		/// </summary>
		protected override void UnloadContent()
		{
			// TODO: Unload any non ContentManager content here
	}	

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			Core.Timer timer = new Core.Timer(null);

			var input = Core.InputManager.Instance;
			KeyboardState keyboard = input.GetKeyboard(Core.InputManager.MasterAuthKey);
			if (keyboard.IsKeyDown(Keys.LeftShift) && keyboard.IsKeyDown(Keys.Escape))
				this.Exit();

			Core.InputManager.Instance.UpdateInput();

			Core.SceneManager.Instance.HandleDeferredOperations();
			Core.SceneManager.Instance.UpdateScenes((float)gameTime.ElapsedGameTime.TotalSeconds);

			base.Update(gameTime);

			Debug.DebugView.Context.UpdateOutputSlot("Update Processing Time", timer.ElapsedMilliseconds.ToString(), "ms");
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			Core.Timer timer = new Core.Timer(null);

			Core.FpsTracker.Instance.MarkNewFrame();
			GraphicsDevice.Clear(BackgroundColor);

			Core.SceneManager.Instance.DrawScenes(spriteBatch, primitiveBatch);

			base.Draw(gameTime);

			Debug.DebugView.Context.UpdateOutputSlot("Draw Processing Time", timer.ElapsedMilliseconds.ToString(), "ms");
		}
	}
}
