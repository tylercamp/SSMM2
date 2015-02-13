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
	public class BasicEnemyTurret : Core.Entity, GunHolder, CollisionType.Enemy
	{
		Gun m_Gun;

		public Core.ColorScheme.SchemeType ColorScheme
		{
			get
			{
				return Core.ColorScheme.SchemeType.Bad;
			}
		}

		public Vector2 GunHoldingPosition
		{
			get
			{
				return Position;
			}
		}

		public BasicEnemyTurret(Core.Scene ownerScene)
			: base(ownerScene)
		{
		}

		public void AddGun(Gun newGun)
		{

		}

		int m_Health = 5;
		public int Health
		{
			get
			{
				return m_Health;
			}

			set
			{
				m_Health = value;
				if (m_Health <= 0)
					Destroy();
			}
		}

		public override void Hit(Core.Entity source)
		{
			base.Hit(source);

			Bullet bullet = source as Bullet;
			if (bullet != null)
				Health -= bullet.Damage;
		}

		public override void LoadContent()
		{
			SetSpriteFromResource("Turret");

			Depth = -500;

			m_Gun = new Guns.Pistol(OwnerScene, this);
			OwnerScene.AddEntity(m_Gun);
		}

		public override void Update(float timeDelta)
		{
			base.Update(timeDelta);

			Player player = Core.DataStore.Context["Player"] as Player;
			if (player != null)
			{
				Vector2 turretToPlayerVector = player.Position - Position;
				turretToPlayerVector.Normalize();

				Vector2 orientationVector = new Vector2(
					(float)Math.Cos(MathHelper.ToRadians(SpriteRotationDegrees)),
					(float)Math.Sin(MathHelper.ToRadians(SpriteRotationDegrees))
					);

				if (
					Vector2.Distance(Position, player.Position) < 400.0F &&
					Vector2.Dot(turretToPlayerVector, orientationVector) > 0.0F
					)
					m_Gun.ShootAt(player.Position);
			}
		}

		public override void Destroy()
		{
			OwnerScene.Camera.Jitter = new Vector2(2.0F, 2.0F);

			if (m_Gun != null)
				m_Gun.Destroy();

			Core.ResourceManager.Instance.GetResource<SoundEffect>("Audio/explosion").Play();

			base.Destroy();
		}
	}
}
