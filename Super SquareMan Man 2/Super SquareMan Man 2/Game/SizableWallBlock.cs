using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace SSMM2.Game
{
	public class SizeableWallBlock : Core.Entity, CollisionType.Solid, CollisionType.Floor
	{
		public SizeableWallBlock(Core.Scene ownerScene, Vector2 position)
			: base (ownerScene)
		{
			Position = position;
		}

		public override void LoadContent()
		{
			SetSpriteFromResource("BlankTexture");
		}
	}
}
