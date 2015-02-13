using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace SSMM2.Template
{
	public class KinematicEntityTemplate : Core.KinematicEntity, CollisionType.Solid
	{
		public override void LoadContent()
		{
			SetSpriteFromResource(null);
		}
	}
}
