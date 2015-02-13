using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SSMM2.Debug
{
	public class CommandlineConfig
	{
		/// <summary>
		/// Registers various Commandline procedures
		/// </summary>
		public static void Apply()
		{
			Commandline cmd = Commandline.Context;
			
			Core.SignalDispatcher.Instance.AddListener("Game.TimeScale", delegate(object data)
			{
				Core.DynamicTime time = Core.SceneManager.Instance.Scenes.Last().Time;

				if (data != null)
				{
					time.TimeScale = float.Parse(data as String);
				}

				return time.TimeScale;
			});

			Core.SignalDispatcher.Instance.AddListener("Signals", delegate(object data)
			{
				List<String> signals = Core.SignalDispatcher.Instance.AllSignalNames;

				cmd.WriteLine("All registered signals:");

				foreach (String signal in signals)
				{
					cmd.WriteLine("-- " + signal);
				}

				return null;
			});

			Core.SignalDispatcher.Instance.AddListener("Exit", delegate(object data)
			{
				SquareManMan.Instance.Exit();

				return null;
			});

			#region Editors
			Core.SignalDispatcher.Instance.AddListener("PartEdit", delegate(object data)
			{
				Effects.ParticleSystemDescriptor descriptor = new Effects.ParticleSystemDescriptor();
				descriptor.ParticleSprite = Core.ResourceManager.Instance.GetResource<Texture2D>("DefaultParticle");
				Game.Cutscenes.ParticlePreviewOverlay particlePreview = new Game.Cutscenes.ParticlePreviewOverlay(descriptor);
				particlePreview.Begin();

				return null;
			});
			#endregion

			#region Commandline Debug
			Core.SignalDispatcher.Instance.AddListener("CLS", delegate(object o)
			{
				cmd.Clear();
				return null;
			});
			#endregion
		}
	}
}
