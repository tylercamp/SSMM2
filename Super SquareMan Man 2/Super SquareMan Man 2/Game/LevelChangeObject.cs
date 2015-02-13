using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSMM2.Game
{
	class LevelChangeObject : Core.Entity
	{
		public String TargetLevel;

		public LevelChangeObject(Core.Scene ownerScene)
			: base(ownerScene)
		{
		}

		public override void LoadContent()
		{
			SetSpriteFromResource("BlankTexture");
		}

		public override void Update(float timeDelta)
		{
			base.Update(timeDelta);

			if (null != OwnerScene.CollisionWorld.EntityPlaceFree<CollisionType.Player>(this, Position))
			{
				switch (TargetLevel)
				{
					case "$GameWin":
						{
							Core.SceneManager.Instance.ReplaceScene(UI.GameWonMenu.CreateNewGameWonMenuScene());
							break;
						}

					default:
						{
							Core.Scene newScene = new Core.Scene();
							Core.SceneBuilder.BuildScene("Content/Levels/" + TargetLevel, newScene);

							newScene.LoadContent();

							Core.SceneManager.Instance.ReplaceScene(newScene);
							break;
						}
				}
			}
		}
	}
}
