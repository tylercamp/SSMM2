using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSMM2.Effects
{
	class SlowedTimeEffect
	{
		public static ParticleSystemV1 Spawn(Core.Scene ownerScene)
		{
			ParticleSystemV1 result = new ParticleSystemV1(ownerScene);

			ParticleSystemDescriptor descriptor = new ParticleSystemDescriptor();

			descriptor.EmitCount = 20.0F;
			descriptor.MaxEjectaDirectionVariance = 360.0F;
			descriptor.StartSpeed = new Core.Range(1.0F);
			descriptor.StartParticleScale = new Core.Range(2.0F, 5.0F);
			descriptor.ParticleLife = new Core.Range(0.4F, 0.4F);
			descriptor.RadialAcceleration = new Core.Range(-9000.0F);

			descriptor.IgnoreVelocityLimits = true;

			descriptor.AlphaFromAge = false;

			Color [] colors = new Color [4];
			colors[0] = new Color(255, 255, 255, 0);
			colors[1] = new Color(35, 145, 255, 0);
			colors[2] = new Color(20, 150, 255, 0);
			colors[3] = new Color(0, 160, 255, 150);

			descriptor.ColorRanges = colors;

			descriptor.ParticleSprite = Core.ResourceManager.Instance.GetResource<Texture2D>("Player");

			result.Descriptor = descriptor;

			return result;
		}
	}
}
