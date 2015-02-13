using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSMM2.Game
{
	class TimeUpgrade : Core.Entity, CollisionType.Powerup
	{
		public TimeUpgrade(Core.Scene ownerScene)
			: base(ownerScene)
		{
		}

		public override void LoadContent()
		{
			SetSpriteFromResource("Powerups/TimeUpgrade");
		}

		public String Name
		{
			get
			{
				return "Time Mod";
			}
		}

		public String Instructions
		{
			get
			{
				return "Press E to activate";
			}
		}

		public void Apply(Game.Player target)
		{
			target.CanSlowTime = true;
		}
	}
}
