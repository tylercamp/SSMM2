using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;

namespace SSMM2.Game
{
	public abstract class Gun : Core.Entity
	{
		public Core.ColorScheme ColorScheme;

		private Vector2 m_Velocity = Vector2.Zero;

		public abstract String Name
		{
			get;
		}

		private GunHolder m_Owner;
		public GunHolder Owner
		{
			get
			{
				return m_Owner;
			}

			set
			{
				m_Owner = value;
				if (m_Owner == null)
					ColorScheme.CurrentScheme = Core.ColorScheme.SchemeType.Unknown;
				else
					ColorScheme.CurrentScheme = m_Owner.ColorScheme;
			}
		}

		public abstract SoundEffect ShootSound
		{
			get;
		}

		public override void LoadContent()
		{
			Depth = -200;
		}

		private int m_Ammo = 0x7FFFFFFF;
		public int Ammo
		{
			get
			{
				return m_Ammo;
			}

			set
			{
				if (value < 0) return;
				if (value > MaxCapacity) return;

				m_Ammo = value;
			}
		}

		public abstract int MaxCapacity
		{
			get;
		}

		private float m_ElapsedFireTime = -1.0F;
		public abstract float CooldownDurationSeconds
		{
			get;
		}

		public bool CanShoot
		{
			get
			{
				return m_ElapsedFireTime < 0.0f && m_Ammo > 0;
			}
		}

		public Gun(Core.Scene ownerScene, GunHolder owner)
			: base (ownerScene)
		{
			Owner = owner;
			if (Owner != null)
				Position = Owner.GunHoldingPosition;
		}

		public override void Update(float timeDelta)
		{
			base.Update(timeDelta);

			Debug.DebugView.Context.UpdateOutputSlot("Ammo", m_Ammo.ToString());

			BlendColor = ColorScheme.RelevantColor;

			if (m_ElapsedFireTime >= 0.0F)
			{
				m_ElapsedFireTime += timeDelta;
				if (m_ElapsedFireTime >= CooldownDurationSeconds)
					m_ElapsedFireTime = -1.0F;
			}

			if (m_Owner == null)
			{
				m_Velocity += Settings.Gravity * timeDelta;

				var collisionWorld = OwnerScene.CollisionWorld;
				if (null == collisionWorld.EntityMoveSafe<CollisionType.Solid>(this, Position + m_Velocity * timeDelta + new Vector2 (0.0F, Settings.GunHoverHeight)))
				{
					Position += m_Velocity * timeDelta;
				}
				else
				{
					m_Velocity = Vector2.Zero;
				}

				GunHolder possibleHolder = collisionWorld.EntityPlaceFree<GunHolder>(this, Position) as GunHolder;
				if (possibleHolder != null)
				{
					possibleHolder.AddGun(this);
				}

				return;
			}
			else
			{
				m_Velocity = Vector2.Zero;

				Vector2 vectorToTargetPosition = Owner.GunHoldingPosition - Position;

				if (vectorToTargetPosition == Vector2.Zero) return;

				Vector2 delta = vectorToTargetPosition * timeDelta * Settings.GunSnapSpeed;

				if (delta.Length() > vectorToTargetPosition.Length())
					Position = Owner.GunHoldingPosition;
				else
					Position += delta;
			}
		}

		public virtual void AimAt(Vector2 position)
		{
			Vector2 difference = Vector2.Normalize(position - this.Position);
			if (difference.X < 0)
			{
				Scale.X = -1.0F;
				SpriteRotationDegrees = -MathHelper.ToDegrees((float)Math.Atan2(difference.Y, -difference.X));
			}
			else
			{
				Scale.X = 1.0F;
				SpriteRotationDegrees = MathHelper.ToDegrees((float)Math.Atan2(difference.Y, difference.X));
			}
		}

		protected abstract void GenerateBulletToTarget(Core.Scene targetScene, Vector2 targetPosition);

		public void ShootAt(Vector2 position)
		{
			AimAt(position);

			if (!CanShoot)
				return;

			GenerateBulletToTarget(OwnerScene, position);

			ShootSound.Play();

			m_ElapsedFireTime = 0.0F;
			--m_Ammo;
		}
	}
}
