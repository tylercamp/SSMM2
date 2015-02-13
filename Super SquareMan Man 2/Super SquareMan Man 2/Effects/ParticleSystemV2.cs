using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SSMM2.Core;
using System;
using System.Collections.Generic;

namespace SSMM2.Effects
{
	struct ParticleV2
	{
		ParticleSystemV2 m_Parent;
		public ParticleSystemV2 Parent
		{
			get
			{
				return m_Parent;
			}
		}

		public float Age;
		public float MaxLife;

		private Vector2 m_TargetScale;
		private Vector2 m_StartScale;
		private float m_TangentialFactor;
		private float m_RadialFactor;
        private float m_GravityStrength;

		public Vector2 Position;
		public Vector2 Velocity;
		public Color BlendColor;
		public Vector2 Scale;

		public ParticleSystemDescriptor Descriptor;

		public void ApplyForce(Vector2 force)
		{
			Velocity += force;
		}
		
		public ParticleV2(ParticleSystemV2 parent, Vector2 position)
		{
			m_Parent = parent;

			Age = 0.0F;
			Position = position;
			Scale = Vector2.One;

			ParticleSystemDescriptor desc = parent.Descriptor;
			MaxLife = desc.ParticleLife.Any;
			m_StartScale = new Vector2(desc.StartParticleScale.Any);
			m_TargetScale = m_StartScale + new Vector2 (desc.ParticleScaleVariance.Any);

			Descriptor = parent.Descriptor;

			float startDirection = desc.EjectaDirection + (float)Utility.Random.NextDouble() * desc.MaxEjectaDirectionVariance - desc.MaxEjectaDirectionVariance / 2.0F;

			Velocity = new Vector2(
				(float)Math.Cos(MathHelper.ToRadians(startDirection)),
				(float)-Math.Sin(MathHelper.ToRadians(startDirection))
				) * desc.StartSpeed.Any;

			m_TangentialFactor = desc.TangentialAcceleration.Any;
			m_RadialFactor = desc.RadialAcceleration.Any;
            m_GravityStrength = desc.GravityStrengthFactor.Any;

			BlendColor = desc.ColorRanges[0];
		}

		public void Update(float timeDelta)
		{
			Vector2 previousPosition = Position;

			ApplyLerpParticleScale(Age / MaxLife, m_StartScale, m_TargetScale);
			ApplyRadialAcceleration(m_Parent.ClosestPointInBoundsToPoint(Position), m_RadialFactor);
			ApplyGravity(Settings.Gravity, m_GravityStrength);
			ApplyTangentialAcceleration(Vector2.Zero, m_TangentialFactor);
			ApplyColorBlending(Age / MaxLife, Descriptor.ColorRanges);
		}

		public void Draw(SpriteBatch spriteBatch, PrimitiveBatch primitiveBatch)
		{
			spriteBatch.Draw(
				Descriptor.ParticleSprite,
				Position + m_Parent.Position, null,
				BlendColor * (m_Parent.BlendColor.A / 255.0F),
				0.0F, new Vector2(Descriptor.ParticleSprite.Width / 2.0F, Descriptor.ParticleSprite.Height / 2.0F),
				Vector2.One,
				SpriteEffects.None,
				0.0F
				);
		}

		#region Particle Behaviors
		void ApplyLerpParticleScale(float ageNormal, Vector2 startScale, Vector2 endScale)
		{
			Scale = Vector2.Lerp(startScale, endScale, ageNormal);
		}

		void ApplyRadialAcceleration(Vector2 centerPosition, float accelerationFactor)
		{
			if (accelerationFactor == 0.0F)
				return;

			Vector2 vectorToCenter = centerPosition - Position;
			if (vectorToCenter != Vector2.Zero)
			{
				vectorToCenter.Normalize();
				ApplyForce(vectorToCenter * accelerationFactor);
			}
		}

		void ApplyGravity(Vector2 gravity, float strengthFactor)
		{
			ApplyForce(gravity * strengthFactor);
		}

		void ApplyTangentialAcceleration(Vector2 centerPosition, float accelerationFactor)
		{
			Vector2 vectorToCenter = centerPosition - Position;

			if (vectorToCenter == Vector2.Zero)
				return;

			Vector2 perpVector = new Vector2(vectorToCenter.Y, -vectorToCenter.X);
			perpVector.Normalize();
			ApplyForce(perpVector * accelerationFactor);
		}

		void ApplyColorBlending(float ageNormal, Color[] colorRange)
		{
			int firstIndex = (int)Math.Floor(ageNormal * (colorRange.Length - 1));
			int lastIndex = (int)Math.Ceiling(ageNormal * (colorRange.Length - 1));
			
			BlendColor = Color.Lerp(colorRange[firstIndex], colorRange[lastIndex], ((ageNormal * colorRange.Length) - firstIndex) / lastIndex);

			float newAlpha = BlendColor.A / 255.0F;
			if (Descriptor.AlphaFromAge)
				newAlpha *= (1.0F - ageNormal);
			BlendColor.A = 255;
			BlendColor *= newAlpha;
		}
		#endregion
	}

	public class ParticleSystemV2 : Core.Entity
	{
		public ParticleSystemDescriptor Descriptor;

		List<ParticleV2> m_Particles;

		internal Vector2 ClosestPointInBoundsToPoint(Vector2 point)
		{
			Vector2 result = point;
			if (result.X < -Size.X / 2.0F)
				result.X = -Size.X / 2.0F;
			if (result.X > Size.X / 2.0F)
				result.X = Size.X / 2.0F;
			if (result.Y < -Size.Y / 2.0F)
				result.Y = Size.Y / 2.0F;
			if (result.Y > Size.Y / 2.0F)
				result.Y = Size.Y / 2.0F;

			return result;
		}

		/// <summary>
		/// Size is centered around the particle system's position. A size of zero means a point particle system. Default
		/// is zero.
		/// </summary>
		public Vector2 Size = Vector2.Zero;

		public ParticleSystemV2(Core.Scene ownerScene, int maxParticles)
			: base (ownerScene)
		{
			m_Particles = new List<ParticleV2>(maxParticles);
		}

		public void Clear()
		{
			throw new NotImplementedException();
		}

		public override void Destroy()
		{
			base.Destroy();
			throw new NotImplementedException();
		}

		public void Burst()
		{
			int burstSize = (int)Descriptor.BurstSize.Any;
			for (int i = 0; i < burstSize; i++)
			{
				m_Particles.Add(new ParticleV2(this, Position));
			}
		}

		public override void LoadContent()
		{
			/*throw new NotImplementedException();*/
		}

		public override void Update(float timeDelta)
		{
			base.Update(timeDelta);

			for (int i = 0; i < m_Particles.Count; i++)
			{
				m_Particles[i].Update(timeDelta);
			}
		}

		public override void Draw(SpriteBatch spriteBatch, PrimitiveBatch primitiveBatch)
		{
			GraphicsDevice graphics = SquareManMan.Instance.GraphicsDevice;

			BlendState prevBlend = graphics.BlendState;
			graphics.BlendState = Descriptor.Blend;
			
			for (int i = 0; i < m_Particles.Count; i++)
			{
				m_Particles[i].Draw(spriteBatch, primitiveBatch);
			}
			
			graphics.BlendState = prevBlend;
		}
	}
}
