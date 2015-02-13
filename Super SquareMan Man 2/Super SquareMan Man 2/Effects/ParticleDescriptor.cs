using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SSMM2.Core;

namespace SSMM2.Effects
{
	public class ParticleSystemDescriptor
	{
		/// <summary>
		/// Initial direction of particles created in degrees. Default is 0.
		/// </summary>
		public float EjectaDirection = 0.0F;

		/// <summary>
		/// Variation of ejecta direction upon creation in degrees; a value of 360 effects a particle system that
		/// will initially span in all directions, while 0 will effect a particle system that will only fire in
		/// a single direction. Default is 0.
		/// </summary>
		public float MaxEjectaDirectionVariance = 0.0F;

		/// <summary>
		/// The initial speed of the particle upon creation, in pixels per second. Default of 50.
		/// </summary>
		public Range StartSpeed = new Range(50.0F);

		/// <summary>
		/// The range of initial scale values for each particle. This single scale applies to both the X and Y
		/// scale factors. Default is 1.
		/// </summary>
		public Range StartParticleScale = new Range(1.0F);

		/// <summary>
		/// Particle scales LERP as they age between StartParticleScale.Any and StartParticleScale.Any + ParticleScaleVariance.Any. Default
		/// is Value = 0.
		/// </summary>
		public Range ParticleScaleVariance = new Range(0.0F);

		/// <summary>
		/// The minimum and maximum amount of time that a particle may exist. Time in seconds. Default
		/// is Value = 5.
		/// </summary>
		public Range ParticleLife = new Range(5.0F);

		/// <summary>
		/// Acceleration of the particle towards the particle system's position. The first value selected
		/// from the range is used for the entirety of the particle's lifetime. Default is 0.
		/// </summary>
		public Range RadialAcceleration = new Range(0.0F);

		/// <summary>
		/// Acceleration of the particle tangent to the radial acceleration. A single value is chosen from
		/// the range when the particle is created and is used for the particle's lifetime. Default is 0.
		/// </summary>
		public Range TangentialAcceleration = new Range(0.0F);

		/// <summary>
		/// The factor by which Settings.Gravity is applied. A single value is selected from the range
		/// and is used for the entirety of the particle's lifetime. Default is 0 (no gravity).
		/// </summary>
		public Range GravityStrengthFactor = new Range(0.0F);

		/// <summary>
		/// The system will automatically LERP the blend color of each particle between the colors defined
		/// in ColorRanges based on its current age and the maximum lifetime of the particle. Default is a
		/// single white color (white for its entire lifetime).
		/// </summary>
		public Color[] ColorRanges = { new Color(255, 255, 255, 255) };

		/// <summary>
		/// When true, the particle can react to its collision environment. Default is false. When true,
		/// all particles are assumed to have a spherical collision mask with a diameter of (width + height)/2.
		/// </summary>
		public bool Interactive = false;

		/// <summary>
		/// Whether or not the particles generated should be influenced by the Settings.MaxVelocity variable.
		/// </summary>
		public bool IgnoreVelocityLimits = false;



		public enum PhysicsMode
		{
			Bounce,
			Destroy
		}

		/// <summary>
		/// How a particle should respond upon colliding with a solid object
		/// </summary>
		public PhysicsMode InteractionMode = PhysicsMode.Bounce;

		public enum CollisionDetectionMode
		{
			BoundingBox,
			CenterOfParticle
		}

		/// <summary>
		/// What metric is used to determine if there is a collision
		/// </summary>
		public CollisionDetectionMode CollisionMode = CollisionDetectionMode.CenterOfParticle;

		// TODO: Implement
		/// <summary>
		/// The range of factors that will be used to calculate a particle's bounce velocity based on its current velocity.
		/// Valid only if Interactive is true. Default is Value = 0.5.
		/// </summary>
		public Range BounceFactor = new Range(0.5F);

		/// <summary>
		/// The type of blending to be used for the particles within this system.
		/// </summary>
		public BlendState Blend = BlendState.Additive;

		/// <summary>
		/// The amount of particles that will be created when the system is invoked. Default
		/// of Value = 15.
		/// </summary>
		public Range BurstSize = new Range(15.0f);

		/// <summary>
		/// The sprite to be used for every particle within the system. Default value of null.
		/// </summary>
		public Texture2D ParticleSprite = null;

		/// <summary>
		/// Number of particles emitted per second by the system
		/// </summary>
		public float EmitCount = 20.0F;

		public bool AlphaFromAge = true;
	}
}
