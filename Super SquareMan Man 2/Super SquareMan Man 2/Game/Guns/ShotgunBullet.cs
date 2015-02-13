using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSMM2.Game.Guns
{
	class ShotgunBulletShell : BulletShell
	{
		public ShotgunBulletShell(Core.Scene ownerScene, Vector2 startPosition, Vector2 bulletDirection)
			: base(ownerScene, startPosition, bulletDirection)
		{
		}

		public override void LoadContent()
		{
			base.LoadContent();
			SetSpriteFromResource("Bullets/GenericShell");
		}
	}

	public class ShotgunBullet : Bullet
	{
		public override int Damage
		{
			get { return 8; }
		}

		public override float Speed
		{
			get { return 1500.0f; }
		}

		public ShotgunBullet(Core.Scene ownerScene, Core.Entity original, Vector2 position, Vector2 directionNormal)
			: base(ownerScene, original, position, directionNormal)
		{

		}

		public override void LoadContent()
		{
			SetSpriteFromResource("Bullets/GenericBullet");
			OwnerScene.AddEntity(new ShotgunBulletShell(OwnerScene, Position, DirectionNormal));
		}
	}
}
