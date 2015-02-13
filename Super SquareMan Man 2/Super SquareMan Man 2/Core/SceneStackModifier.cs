using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSMM2.Core
{
	public class SceneStackModifier : SceneManagerMember
	{
		public SceneStackModifier(bool scenesAreUpdateable, bool scenesAreDrawable)
		{
			ScenesAreUpdateable = scenesAreUpdateable;
			ScenesAreDrawable = scenesAreDrawable;
		}

		public bool ScenesAreUpdateable = true;
		public bool ScenesAreDrawable = true;
	}
}
