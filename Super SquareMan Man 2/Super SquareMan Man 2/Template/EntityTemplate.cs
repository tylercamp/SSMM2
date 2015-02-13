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
	public class EntityTemplate : Core.Entity
	{
		public override void LoadContent()
		{
			SetSpriteFromResource(null);
		}
	}
}
