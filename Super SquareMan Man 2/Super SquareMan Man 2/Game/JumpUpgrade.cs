using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSMM2.Game
{
	class JumpUpgrade : Core.Entity, CollisionType.Powerup
	{
		public JumpUpgrade(Core.Scene ownerScene)
			: base (ownerScene)
		{
		}

		public override void LoadContent()
		{
			SetSpriteFromResource("Powerups/JumpUpgrade");
		}

		public String Name
		{
			get
			{
				return "Multi-Jump";
			}
		}

		public String Instructions
		{
			get
			{
				return "";
			}
		}

		public void Apply(Game.Player target)
		{
			target.TotalJumpsAllowed++;
		}
	}
}
