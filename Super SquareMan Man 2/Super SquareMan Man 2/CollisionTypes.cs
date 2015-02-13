using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSMM2.CollisionType
{
	interface Solid { }

	interface Player : Solid { }

	interface Floor : Solid { }

	interface Enemy : Solid { }

	interface Powerup
	{
		String Name { get; }
		String Instructions { get; }

		void Apply(Game.Player target);
	}

	interface RespawnArea
	{
	}
}
