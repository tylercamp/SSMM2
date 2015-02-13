using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSMM2.Game.Guns
{
	public class Shotgun : Gun
	{
		public override int MaxCapacity
		{
			get
			{
				return 10;
			}
		}

		public override float CooldownDurationSeconds
		{
			get { return 0.9F; }
		}

		public override string Name
		{
			get { return "Shotgun"; }
		}

		public override void LoadContent()
		{
			base.LoadContent();

			SetSpriteFromResource("Shotgun");
			SpriteCenter.X = 4.0f;
			SpriteCenter.Y = 6.0f;
		}

		public Shotgun(Core.Scene ownerScene, GunHolder owner)
			: base(ownerScene, owner)
		{
			ColorScheme.BadColor = Color.White;
			ColorScheme.GoodColor = Color.Red;
			ColorScheme.UnknownColor = Color.Yellow;
		}

		public float SpreadDegrees = 20.0F;

		public override SoundEffect ShootSound
		{
			get { return Core.ResourceManager.Instance.GetResource<SoundEffect>("Audio/shotgun"); }
		}

		protected override void GenerateBulletToTarget(Core.Scene targetScene, Vector2 targetPosition)
		{
			for (int i = 0; i < 8; i++)
			{
				Vector2 currentNormal = Vector2.Normalize(targetPosition - this.Position);
				float angle = MathHelper.ToDegrees((float)Math.Atan2(currentNormal.Y, currentNormal.X));
				angle += (float)Utility.Random.NextDouble() * SpreadDegrees - SpreadDegrees / 2.0F;
				currentNormal.X = (float)Math.Cos(MathHelper.ToRadians(angle));
				currentNormal.Y = (float)Math.Sin(MathHelper.ToRadians(angle));

				new ShotgunBullet(
					OwnerScene,
					this.Owner is Core.Entity ? this.Owner as Core.Entity : null,
					this.Position,
					currentNormal
					);
			}
		}
	}
}
