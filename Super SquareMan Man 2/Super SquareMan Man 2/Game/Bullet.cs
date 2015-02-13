using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSMM2.Game
{
	public abstract class BulletShell : Core.KinematicEntity
	{
		private float m_AgeSeconds;

		protected virtual float ShellSpeed
		{
			get
			{
				return 300.0F;
			}
		}

		public BulletShell(Core.Scene ownerScene, Vector2 startPosition, Vector2 bulletDirection)
			: base (ownerScene)
		{
			Solid = false;

			Position = startPosition;

			Velocity = new Vector2(-bulletDirection.X, -bulletDirection.Y);
			float angle = (float)Math.Atan2(Velocity.Y, Velocity.X);
			angle += MathHelper.ToRadians(60.0F) * ((float)Utility.Random.NextDouble() * 2.0F - 1.0F);
			Velocity.X = (float)Math.Cos(angle);
			Velocity.Y = (float)Math.Sin(angle);
			Velocity.Normalize();
			Velocity *= ShellSpeed;
		}

		public override void LoadContent()
		{
			Depth = -150;
		}

		public override void Update(float timeDelta)
		{
			base.Update(timeDelta);

			m_AgeSeconds += timeDelta;
			if (m_AgeSeconds >= 5.0F)
			{
				this.Destroy();
				return;
			}

			var collision = OwnerScene.CollisionWorld;
			if (collision.EntityPlaceFree<CollisionType.Floor>(this, Position) != null)
			{
				this.Destroy();
			}
		}
	}

	public abstract class Bullet : Core.Entity
	{
		private float m_AgeSeconds;

		protected virtual float m_MaxAgeSeconds
		{
			get { return 5.0F; }
		}

		public Vector2 DirectionNormal;

		public Core.Entity Source
		{
			get;
			private set;
		}

		public abstract float Speed
		{
			get;
		}

		public abstract int Damage
		{
			get;
		}

		public Core.Entity OriginatingEntity
		{
			get;
			private set;
		}

		public Bullet(Core.Scene ownerScene, Core.Entity originatingEntity, Vector2 position, Vector2 directionNormal)
			: base (ownerScene)
		{
			Position = position;
			directionNormal.Normalize();
			DirectionNormal = directionNormal;

			OriginatingEntity = originatingEntity;

			m_AgeSeconds = 0.0F;
		}

		public override void Update(float timeDelta)
		{
			base.Update(timeDelta);

			if (m_AgeSeconds > m_MaxAgeSeconds)
			{
				this.Destroy();
				return;
			}

			m_AgeSeconds += timeDelta;

			Core.Entity hitEntity = OwnerScene.CollisionWorld.EntityMoveSafe<CollisionType.Solid>(this, DirectionNormal * Speed * timeDelta);
			if (hitEntity != null && !OriginatingEntity.Equals(hitEntity))
			{
				hitEntity.Hit(this);
				this.Destroy();
			}
		}
	}
}
