using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSMM2.Game
{
	class RespawnPoint : Core.Entity, CollisionType.RespawnArea
	{
		public RespawnPoint(Core.Scene ownerScene)
			: base (ownerScene)
		{
		}

		public override void LoadContent()
		{
			Depth = -50;
			BlendColor = new Color(77, 77, 77, 77);

			SetSpriteFromResource("BlankTexture");
		}
	}
}
