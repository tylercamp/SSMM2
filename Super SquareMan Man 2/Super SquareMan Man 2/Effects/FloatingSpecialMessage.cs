using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace SSMM2.Effects
{
	class FloatingSpecialMessage : Core.Entity
	{
		protected SpriteFont m_DrawFont;

		public float StartScale = 0.25F;
		public float MaxScale = 1.25F;
		public float MinScale = 0.75F;

		private float m_TargetScale;

		public float LifeLength = 3.0F;
		public float Age
		{
			get;
			private set;
		}

		public String Text;

		public FloatingSpecialMessage(Core.Scene ownerScene)
			: base (ownerScene)
		{
		}

		private float m_TransitionDuration = 0.4F;
		private float m_PreviousScale;
		private float m_TransitionNormal;

		private enum TransitionMode
		{
			Increasing,
			Decreasing
		}

		TransitionMode m_Mode = TransitionMode.Increasing;

		public static FloatingSpecialMessage Spawn(Core.Scene targetScene, Vector2 position, String text)
		{
			FloatingSpecialMessage newMessage = new FloatingSpecialMessage(targetScene);
			newMessage.Position = position;
			newMessage.Text = text;
			return newMessage;
		}

		public override void LoadContent()
		{
			m_DrawFont = Core.ResourceManager.Instance.GetResource<SpriteFont>("SpecialMessageFont");

			Depth = -1000;
			Age = 0.0F;

			m_TargetScale = MaxScale;
			m_PreviousScale = StartScale;
			m_TransitionNormal = 0.0F;
			Scale = new Vector2(StartScale);

			SpriteRotationDegrees = Utility.Random.Next(-45, 45);

			BlendColor = Color.Green * 0.8F;
		}

		public override void Update(float timeDelta)
		{
			base.Update(timeDelta);

			if (Age >= LifeLength)
				this.Destroy();

			Age += timeDelta;

			m_TransitionNormal += timeDelta / m_TransitionDuration;
			if (m_TransitionNormal > 1.0F)
				m_TransitionNormal = 1.0F;

			Scale = Vector2.Lerp(new Vector2(m_PreviousScale), new Vector2(m_TargetScale), m_TransitionNormal);
			if (m_TransitionNormal >= 1.0F)
			{
				m_PreviousScale = m_TargetScale;
				m_TransitionNormal = 0.0F;

				switch (m_Mode)
				{
					case (TransitionMode.Increasing):
						m_Mode = TransitionMode.Decreasing;
						m_TargetScale = MinScale;
						break;

					case (TransitionMode.Decreasing):
						m_Mode = TransitionMode.Increasing;
						m_TargetScale = MaxScale;
						break;
				}
			}
		}

		public override void Draw(SpriteBatch spriteBatch, Core.PrimitiveBatch primitiveBatch)
		{
			Vector2 stringSize = m_DrawFont.MeasureString(Text);
			spriteBatch.DrawString(m_DrawFont, Text, Position, BlendColor, MathHelper.ToRadians(SpriteRotationDegrees), stringSize / 2.0F, Scale, SpriteEffects.None, 0.0F);
		}
	}
}
