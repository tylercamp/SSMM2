using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace SSMM2.Game
{
	public class OutsideRoomBorder : Core.Entity
	{
		public OutsideRoomBorder(Core.Scene ownerScene, bool isLava)
			: base (ownerScene)
		{
			m_IsLava = isLava;
		}

		bool m_IsLava;
		Effects.ParticleSystemV1 m_LavaSystem;

		public override void LoadContent()
		{
			SetSpriteFromResource("BlankTexture");

			if (!m_IsLava)
				return;

			m_LavaSystem = new Effects.ParticleSystemV1(OwnerScene);
			m_LavaSystem.Position = Position + new Vector2(BoundingBox.Width / 2.0F, BoundingBox.Height / 2.0F);
			m_LavaSystem.Size = new Vector2(BoundingBox.Width, BoundingBox.Height);

			Effects.ParticleSystemDescriptor descriptor = new Effects.ParticleSystemDescriptor();
			descriptor.EjectaDirection = 90.0F;
			descriptor.MaxEjectaDirectionVariance = 0.0F;
			descriptor.StartSpeed = new Core.Range(5.0F, 10.0F);
			descriptor.StartParticleScale = new Core.Range(5.0F, 8.0F);
			descriptor.ParticleScaleVariance = new Core.Range(0.0F, 0.0F);
			descriptor.ParticleLife = new Core.Range(1000000.0F, 1000000.0F);
			descriptor.Interactive = false;
			//descriptor.GravityStrengthFactor = new Core.Range(0.01F, 0.05F);
			descriptor.RadialAcceleration = new Core.Range(10.0F, 20.0F);
			//descriptor.TangentialAcceleration = new Core.Range(-100.0F, 100.0F);
			descriptor.ParticleSprite = Core.ResourceManager.Instance.GetResource<Texture2D>("DefaultParticle");

			descriptor.AlphaFromAge = false;
			descriptor.ColorRanges = new Color[3];
			descriptor.ColorRanges[0] = new Color(255, 50, 30, 255);

			m_LavaSystem.Descriptor = descriptor;
			m_LavaSystem.CollisionSource = OwnerScene.CollisionWorld;

			for (int i = 0; i < 20; i++)
				m_LavaSystem.Burst();
		}

		public override void Update(float timeDelta)
		{
		}

		public override void Draw(SpriteBatch spriteBatch, Core.PrimitiveBatch primitiveBatch)
		{
		}
	}
}