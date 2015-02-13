using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SSMM2.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SSMM2.Effects
{
	public class ParticleSystemV3 : Core.Entity
	{
		public ParticleSystemDescriptor Descriptor;

		List<Vector2>
			m_Positions,
			m_Velocities,
			m_Scales,
			m_TargetScales,
			m_StartScales;

		List<Color>
			m_BlendColors;

		List<float>
			m_Ages,
			m_MaxLives,
			m_TangentialFactors,
			m_RadialFactors,
			m_GravityStrengths;

		List<bool>
			m_EnabledParticles;
			

		public int ParticleCount
		{
			get
			{
				return m_EnabledParticles.Count(delegate(bool enabled) { return enabled; });
			}
		}

		public int MaxParticles
		{
			get { return m_EnabledParticles.Capacity; }
		}
			

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

		void FillList<T>(List<T> list, T value)
		{
			for (int i = 0; i < list.Capacity; i++)
			{
				list.Add(value);
			}
		}

		public ParticleSystemV3(Core.Scene ownerScene, int maxParticles)
			: base (ownerScene)
		{
			m_Positions		= new List<Vector2>(maxParticles); FillList<Vector2>(m_Positions,		new Vector2());
			m_Velocities	= new List<Vector2>(maxParticles); FillList<Vector2>(m_Velocities,		new Vector2());
			m_Scales		= new List<Vector2>(maxParticles); FillList<Vector2>(m_Scales,			new Vector2());
			m_TargetScales	= new List<Vector2>(maxParticles); FillList<Vector2>(m_TargetScales,	new Vector2());
			m_StartScales	= new List<Vector2>(maxParticles); FillList<Vector2>(m_StartScales,		new Vector2());

			m_BlendColors	= new List<Color>(maxParticles); FillList<Color>(m_BlendColors, new Color());

			m_Ages				= new List<float>(maxParticles); FillList<float>(m_Ages, 0.0F);
			m_MaxLives			= new List<float>(maxParticles); FillList<float>(m_MaxLives, 0.0F);
			m_TangentialFactors	= new List<float>(maxParticles); FillList<float>(m_TangentialFactors, 0.0F);
			m_RadialFactors		= new List<float>(maxParticles); FillList<float>(m_RadialFactors, 0.0F);
			m_GravityStrengths	= new List<float>(maxParticles); FillList<float>(m_GravityStrengths, 0.0F);

			m_EnabledParticles	= new List<bool>(maxParticles); FillList<bool>(m_EnabledParticles, false);
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

		private void FillNewParticle(ParticleSystemDescriptor descriptor, int index)
		{
			m_Ages[index] = 0.0F;
			m_Positions[index] = Position;
			m_Scales[index] = Vector2.One;
			m_MaxLives[index] = descriptor.ParticleLife.Any;
			m_StartScales[index] = new Vector2(descriptor.StartParticleScale.Any);
			m_TargetScales[index] = m_StartScales[index] + new Vector2(descriptor.ParticleScaleVariance.Any);
			m_TangentialFactors[index] = descriptor.TangentialAcceleration.Any;
			m_RadialFactors[index] = descriptor.RadialAcceleration.Any;
			m_GravityStrengths[index] = descriptor.GravityStrengthFactor.Any;
			m_BlendColors[index] = descriptor.ColorRanges[0];



			float startDirection =
				descriptor.EjectaDirection + (float)Utility.Random.NextDouble() * descriptor.MaxEjectaDirectionVariance
				- descriptor.MaxEjectaDirectionVariance / 2.0F;

			m_Velocities[index] = new Vector2(
				(float)Math.Cos(MathHelper.ToRadians(startDirection)),
				(float)-Math.Sin(MathHelper.ToRadians(startDirection))
				) * descriptor.StartSpeed.Any;

			m_EnabledParticles[index] = true;
		}

		public void Burst()
		{
			int burstSize = (int)Descriptor.BurstSize.Any;

			for (int i = 0; i < burstSize; i++)
			{
				FillNewParticle(Descriptor, i);
			}
		}

		public override void LoadContent()
		{
			/*throw new NotImplementedException();*/
		}

		public override void Update(float timeDelta)
		{
			base.Update(timeDelta);

			int particleCount = ParticleCount;

			/* ApplyLerpParticleScale */
			for (int i = 0; i < particleCount; i++)
				m_Scales[i] = Vector2.Lerp(m_StartScales[i], m_TargetScales[i], m_Ages[i] / m_MaxLives[i]);

			/* ApplyRadialAcceleration */
			for (int i = 0; i < particleCount; i++)
			{
				float factor = m_RadialFactors[i];
				if (factor == 0.0F) continue;

				Vector2 vectorToCenter = ClosestPointInBoundsToPoint(m_Positions[i]) - m_Positions[i];
				if (vectorToCenter != Vector2.Zero)
				{
					vectorToCenter.Normalize();
					m_Velocities[i] += vectorToCenter * factor * timeDelta;
				}
			}

			/* ApplyGravity */
			Vector2 gravity = Settings.Gravity;
			for (int i = 0; i < particleCount; i++)
			{
				m_Velocities[i] += gravity * m_GravityStrengths[i] * timeDelta;
			}

			/* ApplyTangentialAcceleration */
			for (int i = 0; i < particleCount; i++)
			{
				Vector2 vectorToCenter = Vector2.Zero - m_Positions[i];

				if (vectorToCenter == Vector2.Zero)
					continue;

				Vector2 perpVector = new Vector2(vectorToCenter.Y, -vectorToCenter.X);
				perpVector.Normalize();
				m_Velocities[i] += perpVector * m_TangentialFactors[i] * timeDelta;
			}

			/* ApplyColorBlending */
			for (int i = 0; i < particleCount; i++)
			{
				float ageNormal = m_Ages[i] / m_MaxLives[i];
				Color[] colorRange = Descriptor.ColorRanges;

				int firstIndex = (int)Math.Floor(ageNormal * (colorRange.Length - 1));
				int lastIndex = (int)Math.Ceiling(ageNormal * (colorRange.Length - 1));

				Color newColor = Color.Lerp(colorRange[firstIndex], colorRange[lastIndex], ((ageNormal * colorRange.Length) - firstIndex) / lastIndex);

				float newAlpha = BlendColor.A / 255.0F;
				if (Descriptor.AlphaFromAge)
					newAlpha *= (1.0F - ageNormal);
				newColor.A = 255;
				newColor *= newAlpha;

				m_BlendColors[i] = newColor;
			}

			/* ApplyVelocity */
			for (int i = 0; i < particleCount; i++)
			{
				m_Positions[i] += m_Velocities[i] * timeDelta;
			}
		}

		public override void Draw(SpriteBatch spriteBatch, PrimitiveBatch primitiveBatch)
		{
			GraphicsDevice graphics = SquareManMan.Instance.GraphicsDevice;

			BlendState prevBlend = graphics.BlendState;
			graphics.BlendState = Descriptor.Blend;

			Vector2 spriteCenter = new Vector2(
				Descriptor.ParticleSprite.Width / 2.0F,
				Descriptor.ParticleSprite.Height / 2.0F
				);

			int particleCount = MaxParticles;
			for (int i = 0; i < particleCount; i++)
			{
				if (!m_EnabledParticles[i])
					continue;

				spriteBatch.Draw(
					Descriptor.ParticleSprite,
					Position + m_Positions[i], null,
					m_BlendColors[i] * (BlendColor.A / 255.0F),
					0.0F, spriteCenter,
					Vector2.One,
					SpriteEffects.None,
					0.0F
					);
			}
			
			graphics.BlendState = prevBlend;
		}
	}
}
