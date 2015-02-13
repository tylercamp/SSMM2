using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SSMM2.Game.Credits
{
	public class CreditsController : Core.Entity
	{
		public String PrimaryText = "MADE BY TYLER CAMP";
		public String SubText = "(YEAH)";

		private SpriteFont m_MainFont = null;
		private SpriteFont m_SubFont = null;

		private Vector3 m_MainTextPosition = new Vector3(0.0F, 0.0F, 5.0F);
		private Vector3 m_SubTextPosition = new Vector3(-50.0F, 50.0F, 5.0F);

		private float m_ZoomSpeed = 0.2F;

		private uint m_AuthKey = Core.InputManager.ElevatedAuthKey + 100;

		public CreditsController(Core.Scene scene)
			: base(scene)
		{
		}

		public override void LoadContent()
		{
			m_MainFont = Core.ResourceManager.Instance.GetResource<SpriteFont>("CreditsMainFont");
			m_SubFont = Core.ResourceManager.Instance.GetResource<SpriteFont>("CreditsSubFont");

			Core.InputManager.Instance.Lock(m_AuthKey);
		}

		public override void Update(float timeDelta)
		{
			base.Update(timeDelta);

			if (Core.InputManager.Instance.GetKeyboard(m_AuthKey).IsKeyDown(Keys.Escape))
			{
				Core.InputManager.Instance.Unlock(m_AuthKey);
				Core.SceneManager.Instance.PopScene();
			}

			if (m_MainTextPosition.Z > 1.0F)
			{
				m_MainTextPosition.Z -= m_ZoomSpeed;
				m_SubTextPosition.Z -= m_ZoomSpeed;
			}

			if (m_MainTextPosition.Z < 1.0F)
			{
				m_MainTextPosition.Z = 1.0F;
				m_SubTextPosition.Z = 1.0F;
			}
		}

		public override void Draw(SpriteBatch spriteBatch, Core.PrimitiveBatch primitiveBatch)
		{
			Vector2 projectedMainTextPosition = new Vector2(m_MainTextPosition.X, m_MainTextPosition.Y) / m_MainTextPosition.Z;
			Vector2 projectedSubTextPosition = new Vector2(m_SubTextPosition.X, m_SubTextPosition.Y) / m_SubTextPosition.Z;

			float mainTextScale = 1.0F / m_SubTextPosition.Z;
			float subTextScale = 1.0F / m_SubTextPosition.Z;

			Vector2 mainTextSize = m_MainFont.MeasureString(PrimaryText) * mainTextScale;
			Vector2 subTextSize = m_SubFont.MeasureString(SubText) * subTextScale;

			float textAngle = 30.0F;

			Color textColor = Color.White;

			spriteBatch.DrawString(
				m_MainFont,
				PrimaryText,
				projectedMainTextPosition,
				textColor,
				MathHelper.ToRadians(textAngle),
				mainTextSize / 2.0F,
				mainTextScale,
				SpriteEffects.None,
				0.0F);

			spriteBatch.DrawString(
				m_SubFont,
				SubText,
				projectedSubTextPosition,
				textColor,
				MathHelper.ToRadians(textAngle),
				subTextSize / 2.0F,
				subTextScale,
				SpriteEffects.None,
				0.0F);
		}
	}
}
