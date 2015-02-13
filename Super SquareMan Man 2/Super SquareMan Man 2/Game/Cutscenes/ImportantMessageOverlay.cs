using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SSMM2.Game.Cutscenes
{
	public class ImportantMessageOverlay : Core.Cutscene
	{
		public override bool IsDone
		{
			get { return Core.InputManager.Instance.GetKeyboard(Core.InputManager.DefaultAuthKey).IsKeyDown(Keys.Enter); }
		}

		public override void LoadContent()
		{
			TextFont = Core.ResourceManager.Instance.GetResource<SpriteFont>("ImportantFont");
		}

		public override void End()
		{
			base.End();
		}

		public override void Draw(SpriteBatch spriteBatch, Core.PrimitiveBatch primitiveBatch)
		{
			base.Draw(spriteBatch, primitiveBatch);

			Rectangle viewport = OwnerScene.Camera.Viewport;
			primitiveBatch.ActiveColor = OverlayColor;
			primitiveBatch.AddRectangle(viewport);
			primitiveBatch.DrawPolygons();

			spriteBatch.End();
			OwnerScene.ReconfigureDraw(spriteBatch);

			String tempString = Text + "\n(Press Enter to continue)";

			Vector2 totalSize = TextFont.MeasureString(tempString);

			String[] individualLines = tempString.Split('\n');

			float offsetSoFar = -totalSize.Y / 2.0F;

			foreach (String currentText in individualLines)
			{
				Vector2 currentTextSize = TextFont.MeasureString(currentText);
				spriteBatch.DrawString(
					TextFont,
					currentText,
					new Vector2(
						//	Cast to int to prevent filtering that would normally occur with floating-point coordinates
						(int)(-currentTextSize.X / 2.0F),
						(int)(offsetSoFar)
						),
					TextColor);

				offsetSoFar += currentTextSize.Y;
			}
		}

		public Color OverlayColor = Color.Black * 0.7F;
		public Color TextColor = Color.White;
		public SpriteFont TextFont;

		public String Text = "";
	}
}
