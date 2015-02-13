using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSMM2.UI.Base
{
	public abstract class Control : Core.Entity
	{
		Core.CollisionMaskAABB m_PreviousMask;

		public ControlOrganizer ParentOrganizer
		{
			get;
			set;
		}

		public Control(Core.Scene ownerScene)
			: base (ownerScene)
		{
		}

		public override void Update(float timeDelta)
		{
			base.Update(timeDelta);

			var ctx = Core.SceneManager.Instance;

			//	Realign on bounding box resize
			if (!BoundingBox.Equals(m_PreviousMask) && ParentOrganizer != null)
				ParentOrganizer.RealignControls();

			m_PreviousMask = BoundingBox;
		}
	}
}
