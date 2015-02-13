using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SSMM2.Game.Cutscenes
{
	public class ParticlePreviewOverlay : Core.Cutscene
	{
		public override bool IsDone
		{
			get { return m_IsDone; }
		}

		private Effects.ParticleSystemV1 m_PartSystem;
		private Editors.ParticleEditor m_Editor;

		bool m_IsDone = false;

		Color m_OriginalColor;

		Effects.ParticleSystemDescriptor m_Descriptor;

		public ParticlePreviewOverlay(Effects.ParticleSystemDescriptor baseDescriptor)
		{
			m_Descriptor = baseDescriptor;
		}

		public override void Begin()
		{
			m_OriginalColor = SquareManMan.Instance.BackgroundColor;
			//SquareManMan.Instance.BackgroundColor = Color.Green;
			base.Begin();
		}

		public override void LoadContent()
		{
			TextFont = Core.ResourceManager.Instance.GetResource<SpriteFont>("ImportantFont");

			m_PartSystem = new Effects.ParticleSystemV1(OwnerScene);
			m_PartSystem.Descriptor = m_Descriptor;
			m_PartSystem.Position.X = 1280.0F / 2.0F;
			m_PartSystem.Position.Y = 720.0F / 2.0F;
			

			m_PartSystem.Stream();

			m_Editor = new Editors.ParticleEditor();
			m_Editor.Show();
			m_Editor.SyncUIToParticleSystem(m_PartSystem);
			m_Editor.FormClosing += m_Editor_FormClosing;

			Core.SceneManager.Instance.PushModifier(false, false);

			OwnerScene.Camera.Position = m_PartSystem.Position;
			m_PartSystem.CollisionSource = OwnerScene.CollisionWorld;
		}

		void m_Editor_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
		{
			m_IsDone = true;
		}

		public override void End()
		{
			OwnerScene.RemoveEntity(m_PartSystem);
			m_PartSystem = null;

			SquareManMan.Instance.BackgroundColor = m_OriginalColor;
			base.End();
		}

		public override void Draw(SpriteBatch spriteBatch, Core.PrimitiveBatch primitiveBatch)
		{
			base.Draw(spriteBatch, primitiveBatch);

			if (m_PartSystem == null) return;

			Debug.DebugView.Context.UpdateOutputSlot("Particle Count", m_PartSystem.ParticleCount.ToString());

			if (!m_IsDone)
			{
				m_Editor.SyncParticleSystemToUI(m_PartSystem);

				m_PartSystem.Draw(spriteBatch, primitiveBatch);
			}
		}

		public Color OverlayColor = Color.Black * 0.7F;
		public Color TextColor = Color.White;
		public SpriteFont TextFont;

		public String Text = "";
	}
}
