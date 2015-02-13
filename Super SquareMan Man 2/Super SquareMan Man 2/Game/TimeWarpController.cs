using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSMM2.Game
{
	class TimeWarpController : Core.Entity
	{
		public static TimeWarpController Instance
		{
			get;
			private set;
		}

		public static void Spawn(Core.Scene targetScene)
		{
			Instance = new TimeWarpController(targetScene);
		}



		public const float ScaleChangeSpeed = 1.8F;

		public float CurrentTimeScale
		{
			get;
			private set;
		}

		public float TargetTimeScale;

		private Effects.ParticleSystemV1 m_TimeScaleSystem;

		public TimeWarpController(Core.Scene owner)
			: base (owner)
		{
			Depth = -100000;

			CurrentTimeScale = 1.0F;
			TargetTimeScale = 1.0F;

			m_TimeScaleSystem = Effects.SlowedTimeEffect.Spawn(owner);

			m_TimeScaleSystem.Stream();
		}

		public override void LoadContent()
		{
		}

		public override void Destroy()
		{
			base.Destroy();

			Instance = null;
		}

		public override void Update(float timeDelta)
		{
			base.Update(timeDelta);

			int prevSign = Math.Sign(TargetTimeScale - CurrentTimeScale);
			CurrentTimeScale += (prevSign * ScaleChangeSpeed) * (timeDelta / OwnerScene.Time.TimeScale);

			if (Math.Sign(TargetTimeScale - CurrentTimeScale) != prevSign)
				CurrentTimeScale = TargetTimeScale;

            //  The magic
			OwnerScene.Time.TimeScale = CurrentTimeScale;

			m_TimeScaleSystem.BlendColor.A = (byte)Math.Min(255, (255 * (1.0F - CurrentTimeScale) * 2.0F));
			m_TimeScaleSystem.Position = Vector2.Transform(
				new Vector2(OwnerScene.Camera.Viewport.Width / 2.0F, OwnerScene.Camera.Viewport.Height / 2.0F),
				OwnerScene.Camera.ScreenToWorldMatrix
				);

			Debug.DebugView.Context.UpdateOutputSlot("TimeScale", CurrentTimeScale.ToString());
		}
	}
}
