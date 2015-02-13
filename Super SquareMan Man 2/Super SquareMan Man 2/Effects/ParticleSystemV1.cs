using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SSMM2.Core;
using System;

namespace SSMM2.Effects
{
	/// <summary>
	/// Particle objects are not to be added to a global scene. They are instead to be maintained within scenes
	/// owned by ParticleSystem objects.
	/// </summary>
	public class Particle : Core.KinematicEntity
	{
		ParticleSystemV1 m_Parent;
		public ParticleSystemV1 Parent
		{
			get
			{
				return m_Parent;
			}
		}

		public float Age = 0.0F;
		public float MaxLife
		{
			get;
			private set;
		}

		private Vector2 m_TargetScale;
		private Vector2 m_StartScale;
		private float m_TangentialFactor;
		private float m_RadialFactor;
        private float m_GravityStrength;

		public ParticleSystemDescriptor Descriptor
		{
			private set;
			get;
		}

		
		public Particle(Core.Scene ownerScene, ParticleSystemV1 parent, Vector2 position)
			: base (ownerScene)
		{
			m_Parent = parent;

			Position = position;

			ParticleSystemDescriptor desc = parent.Descriptor;
			MaxLife = desc.ParticleLife.Any;
			m_StartScale = new Vector2(desc.StartParticleScale.Any);
			m_TargetScale = m_StartScale + new Vector2 (desc.ParticleScaleVariance.Any);

			Descriptor = parent.Descriptor;

			//	We override with a custom implementation
			HasGravity = false;

			float startDirection = desc.EjectaDirection + (float)Utility.Random.NextDouble() * desc.MaxEjectaDirectionVariance - desc.MaxEjectaDirectionVariance / 2.0F;
			if (desc.StartSpeed.Any == 0.0F)
				HasGravity = HasGravity;
			Velocity = new Vector2(
				(float)Math.Cos(MathHelper.ToRadians(startDirection)),
				(float)-Math.Sin(MathHelper.ToRadians(startDirection))
				) * desc.StartSpeed.Any;

			m_TangentialFactor = desc.TangentialAcceleration.Any;
			m_RadialFactor = desc.RadialAcceleration.Any;
            m_GravityStrength = desc.GravityStrengthFactor.Any;

			IgnoreVelocityLimits = desc.IgnoreVelocityLimits;

			BlendColor = desc.ColorRanges[0];

			Solid = parent.Descriptor.Interactive;
		}

		public override void Update(float timeDelta)
		{
			Vector2 previousPosition = Position;

			base.Update(timeDelta);

			if (Descriptor.Interactive)
			{
				Core.Entity collidedEntity;
				if (Descriptor.CollisionMode == ParticleSystemDescriptor.CollisionDetectionMode.BoundingBox)
					collidedEntity = OwnerScene.CollisionWorld.EntityPlaceFree<CollisionType.Solid>(this, Position + m_Parent.Position);
				else
					collidedEntity = OwnerScene.CollisionWorld.PointFree<CollisionType.Solid>(this, Position + m_Parent.Position);

				if (collidedEntity != null)
				{
					switch (Descriptor.InteractionMode)
					{
						case ParticleSystemDescriptor.PhysicsMode.Bounce:
							if (Descriptor.CollisionMode == ParticleSystemDescriptor.CollisionDetectionMode.BoundingBox)
								collidedEntity = OwnerScene.CollisionWorld.EntityPlaceFree<CollisionType.Solid>(this, new Vector2(previousPosition.X, Position.Y) + m_Parent.Position);
							else
								collidedEntity = OwnerScene.CollisionWorld.PointFree<CollisionType.Solid>(this, new Vector2(previousPosition.X, Position.Y) + m_Parent.Position);

							if (collidedEntity != null)
								Velocity.Y *= -1;



							if (Descriptor.CollisionMode == ParticleSystemDescriptor.CollisionDetectionMode.BoundingBox)
								collidedEntity = OwnerScene.CollisionWorld.EntityPlaceFree<CollisionType.Solid>(this, new Vector2(Position.X, previousPosition.Y) + m_Parent.Position);
							else
								collidedEntity = OwnerScene.CollisionWorld.PointFree<CollisionType.Solid>(this, new Vector2(Position.X, previousPosition.Y) + m_Parent.Position);
							if (collidedEntity != null)
								Velocity.X *= -1;
							break;

						case ParticleSystemDescriptor.PhysicsMode.Destroy:
							this.Destroy();
							break;
					}
				}
			}

			Age += timeDelta;

			if (Age > MaxLife)
				Age = MaxLife;

			ApplyLerpParticleScale(Age / MaxLife, m_StartScale, m_TargetScale);
			ApplyRadialAcceleration(m_Parent.ClosestPointInBoundsToPoint(Position), m_RadialFactor);
			ApplyGravity(Settings.Gravity, m_GravityStrength);
			ApplyTangentialAcceleration(Vector2.Zero, m_TangentialFactor);

			if (MaxLife != 0.0F)
				ApplyColorBlending(Age / MaxLife, Descriptor.ColorRanges);

			if (Age >= MaxLife)
				this.Destroy();
		}

		public override void LoadContent()
		{
			SetSpriteFromResource(m_Parent.Descriptor.ParticleSprite);
		}

		public override void Draw(SpriteBatch spriteBatch, PrimitiveBatch primitiveBatch)
		{
			Position += m_Parent.Position;
			Color oldColor = BlendColor;
			BlendColor *= (m_Parent.BlendColor.A / 255.0F);

			DrawSelf(spriteBatch);

			BlendColor = oldColor;
			Position -= m_Parent.Position;
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
				ApplyForce(vectorToCenter * accelerationFactor, new Vector2(float.MaxValue, float.MaxValue));
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

	public class ParticleSystemV1 : Core.Entity
	{
		public ParticleSystemDescriptor Descriptor;

		Core.Scene m_ParticleScene;

		public bool IsStreaming
		{
			get;
			private set;
		}
		float m_TotalAge = 0.0F;
		uint m_TotalStreamedParticles = 0;

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

		public CollisionWorld CollisionSource
		{
			get
			{
				return m_ParticleScene.CollisionWorld;
			}

			set
			{
				m_ParticleScene.CollisionWorld = value;
			}
		}

		/// <summary>
		/// Size is centered around the particle system's position. A size of zero means a point particle system. Default
		/// is zero.
		/// </summary>
		public Vector2 Size = Vector2.Zero;

		public ParticleSystemV1(Core.Scene ownerScene)
			: base (ownerScene)
		{
			m_ParticleScene = new Core.Scene();
			m_ParticleScene.Camera = ownerScene.Camera;
		}

		public uint ParticleCount
		{
			get
			{
				return m_ParticleScene.EntityCount;
			}
		}

		public void Clear()
		{
			m_ParticleScene = new Core.Scene();
		}

		public override void Destroy()
		{
			base.Destroy();
			Clear();
		}

		public void Stream()
		{
			IsStreaming = true;
			m_TotalAge = 0.0F;
		}

		public void StopStreaming()
		{
			IsStreaming = false;
		}

		public void Burst()
		{
			int particleCount = (int)Descriptor.BurstSize.Any;
			for (int i = 0; i < particleCount; i++)
			{
				Vector2 particlePosition = Vector2.Zero;
				particlePosition.X += Utility.Random.Next(-(int)Size.X / 2, (int)Size.X / 2);
				particlePosition.Y += Utility.Random.Next(-(int)Size.Y / 2, (int)Size.Y / 2);
				(new Particle(m_ParticleScene, this, particlePosition)).Depth = Depth;
			}
		}

		public override void LoadContent()
		{
			m_ParticleScene.LoadContent();
		}

		public override void Update(float timeDelta)
		{
			base.Update(timeDelta);
			if (IsStreaming)
			{
				m_TotalAge += timeDelta;

				uint expectedTotalParticles = (uint)(m_TotalAge * Descriptor.EmitCount);
				while (m_TotalStreamedParticles < expectedTotalParticles)
				{
					Burst();
					++m_TotalStreamedParticles;
				}
			}
			m_ParticleScene.Update(timeDelta);
			m_ParticleScene.HandleDeferredOperations();
		}

		public override void Draw(SpriteBatch spriteBatch, PrimitiveBatch primitiveBatch)
		{
			GraphicsDevice graphics = SquareManMan.Instance.GraphicsDevice;

			BlendState prevBlend = graphics.BlendState;
			graphics.BlendState = Descriptor.Blend;
			m_ParticleScene.Draw(spriteBatch, primitiveBatch, false);
			graphics.BlendState = prevBlend;
		}
	}
}
