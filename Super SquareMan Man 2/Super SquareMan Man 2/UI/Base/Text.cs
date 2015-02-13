using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace SSMM2.UI.Base
{
	public class Text : Control
	{
		public String Value = "{Default Text}";
		public String FontName
		{
			get { return m_FontName; }
			set
			{
				m_FontName = value;
				if (m_FontIsLoaded)
					m_Font = Core.ResourceManager.Instance.GetResource<SpriteFont>(m_FontName);
			}
		}

		public bool CenterText = false;
		
		private bool m_FontIsLoaded = false;

		private String m_FontName = "";
		private SpriteFont m_Font;

		public Color TextColor = Color.White;

		public override Core.CollisionMaskAABB BoundingBox
		{
			get
			{
				if (m_Font == null)
					return new Core.CollisionMaskAABB(Rectangle.Empty);

				Vector2 textSize = m_Font.MeasureString(Value);
				return new Core.CollisionMaskAABB(Position.Y, Position.Y, textSize.X, textSize.Y);
			}
		}

		public Text(Core.Scene ownerScene, String text)
			: base (ownerScene)
		{
			Value = text;
		}

		public override void LoadContent()
		{
			m_Font = Core.ResourceManager.Instance.GetResource<SpriteFont>(m_FontName);

			m_FontIsLoaded = true;
		}

		public override void Draw(SpriteBatch spriteBatch, Core.PrimitiveBatch primitiveBatch)
		{
			base.Draw(spriteBatch, primitiveBatch);

			Vector2 drawPosition = Position;
			if (CenterText)
				drawPosition += BoundingBox.Size / 2.0F;

			drawPosition.X = (int)drawPosition.X;
			drawPosition.Y = (int)drawPosition.Y;

			spriteBatch.DrawString(m_Font, Value, drawPosition, TextColor);
		}
	}
}
