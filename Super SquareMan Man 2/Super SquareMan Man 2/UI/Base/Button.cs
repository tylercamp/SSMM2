using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace SSMM2.UI.Base
{
	public class Button : Control
	{
		private void ChangeSprite(String requestedValue, out String spriteNameVariable, out Texture2D spriteObject)
		{
			spriteNameVariable = requestedValue;

			if (m_ButtonIsLoaded)
				spriteObject = Core.ResourceManager.Instance.GetResource<Texture2D>(requestedValue);
			else
				spriteObject = null;
		}

		private void ChangeFont(String requestedValue, out String fontNameVariable, out SpriteFont fontObject)
		{
			fontNameVariable = requestedValue;
			if (m_ButtonIsLoaded)
				fontObject = Core.ResourceManager.Instance.GetResource<SpriteFont>(requestedValue);
			else
				fontObject = null;
		}

		public String SpriteIdle
		{
			get { return m_SpriteIdleName; }
			set { ChangeSprite(value, out m_SpriteIdleName, out m_SpriteIdle); }
		}

		public String SpriteHover
		{
			get { return m_SpriteHoverName; }
			set { ChangeSprite(value, out m_SpriteHoverName, out m_SpriteHover); }
		}

		public String SpritePressed
		{
			get { return m_SpritePressedName; }
			set { ChangeSprite(value, out m_SpritePressedName, out m_SpritePressed); }
		}

		public String Font
		{
			get { return m_FontName; }
			set { ChangeFont(value, out m_FontName, out m_Font); }
		}

		private String m_SpriteIdleName = "UI/DefaultButtonIdle";
		private String m_SpriteHoverName = "UI/DefaultButtonHover";
		private String m_SpritePressedName = "UI/DefaultButtonPressed";
		private String m_FontName = "Debug Assets/debugfont";

		bool m_ButtonIsLoaded = false;

		private Texture2D m_SpriteIdle;
		private Texture2D m_SpriteHover;
		private Texture2D m_SpritePressed;
		private SpriteFont m_Font;

		public SpriteFont TextFont;

		public String Text = "{Default Text}";

		public bool AutoSize = true;

		private bool m_WasClicked;
		private uint m_AuthKey;

		public Button(Core.Scene ownerScene, String text, uint authKey = Core.InputManager.DefaultAuthKey)
			: base (ownerScene)
		{
			m_WasClicked = false;
			m_AuthKey = authKey;
			Text = text;
		}

		public delegate void ClickedHandler(Button button);
		public event ClickedHandler Clicked;

		public override void LoadContent()
		{
			var content = Core.ResourceManager.Instance;
			m_SpriteHover = content.GetResource<Texture2D>(m_SpriteHoverName);
			m_SpriteIdle = content.GetResource<Texture2D>(m_SpriteIdleName);
			m_SpritePressed = content.GetResource<Texture2D>(m_SpritePressedName);
			m_Font = content.GetResource<SpriteFont>(m_FontName);

			Sprite = m_SpriteIdle;

			m_ButtonIsLoaded = true;

			SpriteCenter = Vector2.Zero;
		}

		public override void Update(float timeDelta)
		{
			base.Update(timeDelta);

			MouseState currentMouse = Core.InputManager.Instance.GetMouse(m_AuthKey);
			Vector2 mousePosition;
			if (Core.InputManager.Instance.GetMousePosition(m_AuthKey, out mousePosition))
			{
				mousePosition = Vector2.Transform(mousePosition, OwnerScene.Camera.ScreenToWorldMatrix);

				if (BoundingBox.Intersects(mousePosition))
				{
					if (Core.InputManager.Instance.CheckLeftPressed(m_AuthKey))
					{
						m_WasClicked = true;
					}

					if (Core.InputManager.Instance.CheckLeftReleased(m_AuthKey) && m_WasClicked)
					{
						if (Clicked != null)
							Clicked(this);
					}
				}
			
				if (currentMouse.LeftButton == ButtonState.Released)
					m_WasClicked = false;

				if (m_WasClicked)
				{
					Sprite = m_SpritePressed;
				}
				else
				{
					if (BoundingBox.Intersects(mousePosition))
						Sprite = m_SpriteHover;
					else
						Sprite = m_SpriteIdle;
				}
			}
		}

		public override void Draw(SpriteBatch spriteBatch, Core.PrimitiveBatch primitiveBatch)
		{
			base.Draw(spriteBatch, primitiveBatch);

			Vector2 textSize = m_Font.MeasureString(Text);
			Vector2 textPosition = Position + BoundingBox.Size / 2 - textSize / 2;
			textPosition.X = (int)textPosition.X;
			textPosition.Y = (int)textPosition.Y;

			spriteBatch.DrawString(m_Font, Text, textPosition, Color.Black);
		}
	}
}
