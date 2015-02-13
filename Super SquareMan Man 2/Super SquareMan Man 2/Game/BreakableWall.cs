using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSMM2.Game
{
	public class BreakableWall : Core.Entity, CollisionType.Solid, CollisionType.Floor
	{
		public BreakableWall(Core.Scene ownerScene)
			: base(ownerScene)
		{
		}

		public override void LoadContent()
		{
			SetSpriteFromResource("BreakableWall");
		}

		public override void Hit(Core.Entity source)
		{
			base.Hit(source);
			if (source is Bullet)
				this.Destroy();
		}

		public override void Destroy()
		{
			base.Destroy();
		}
	}
}
