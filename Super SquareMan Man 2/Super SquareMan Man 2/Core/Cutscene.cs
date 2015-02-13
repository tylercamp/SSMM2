using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace SSMM2.Core
{
	public abstract class Cutscene : Entity
	{
		public virtual void Begin()
		{
			SceneManager.Instance.PushScene(RawScene);
		}

		public Scene RawScene
		{
			get
			{
				Scene newScene = new Scene();
				newScene.AddEntity(this);
				newScene.HandleDeferredOperations();
				newScene.LoadContent();
				return newScene;
			}
		}

		public delegate void SceneEndedHandler(Cutscene scene);
		public event SceneEndedHandler SceneEnded;

		public Cutscene()
			: base (null)
		{
		}

		public virtual void End()
		{
			SceneManager.Instance.PopScene();
			if (SceneEnded != null)
				SceneEnded(this);
		}

		public override void Update(float timeDelta)
		{
			base.Update(timeDelta);

			if (IsDone)
				End();
		}

		public abstract bool IsDone
		{
			get;
		}
	}
}
