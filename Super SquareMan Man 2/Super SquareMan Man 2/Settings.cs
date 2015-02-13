using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SSMM2
{
	public class Settings
	{
		public static Vector2 Gravity = new Vector2(0.0f, 1500.0f);

		public static float MaxVelocity = 1000.0f;

		public static float GunSnapSpeed = 40.0f;
		public static float GunHoverHeight = 15.0F;

		public static Vector2 Friction = new Vector2(3000.0F, 1000.0F);
	}
}
