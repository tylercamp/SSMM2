using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSMM2.Game
{
	public interface GunHolder
	{
		Vector2 GunHoldingPosition
		{
			get;
		}

		Core.ColorScheme.SchemeType ColorScheme
		{
			get;
		}

		void AddGun(Gun newGun);
	}
}
