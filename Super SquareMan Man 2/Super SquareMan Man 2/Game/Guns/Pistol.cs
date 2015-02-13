using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSMM2.Game.Guns
{
	public class Pistol : Gun
	{
		public override int MaxCapacity
		{
			get
			{
				return 25;
			}
		}

		public override float CooldownDurationSeconds
		{
			get { return 0.4F; }
		}

		public override string Name
		{
			get { return "Pistol"; }
		}

		public override void LoadContent()
		{
			base.LoadContent();

			SetSpriteFromResource("Pistol");
			SpriteCenter.X = 4.0f;
		}

		public Pistol(Core.Scene ownerScene, GunHolder owner)
			: base(ownerScene, owner)
		{
			ColorScheme.BadColor = Color.White;
			ColorScheme.GoodColor = Color.Red;
			ColorScheme.UnknownColor = Color.Yellow;
		}

		public override SoundEffect ShootSound
		{
			get { return Core.ResourceManager.Instance.GetResource<SoundEffect>("Audio/genericshoot"); }
		}

		protected override void GenerateBulletToTarget(Core.Scene targetScene, Vector2 targetPosition)
		{
			new PistolBullet(
				OwnerScene,
				(this.Owner is Core.Entity ? this.Owner as Core.Entity : null),
				this.Position,
				Vector2.Normalize(targetPosition - this.Position)
				);
		}
	}
}
