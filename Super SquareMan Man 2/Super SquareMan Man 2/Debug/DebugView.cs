using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SSMM2.Debug
{
	/// <summary>
	/// Toggleable view for runtime debug variables.
	/// </summary>
	public class DebugView : Core.Widget
	{
		#region Internal Members
		private Core.PrimitiveBatch m_PrimBatch;
		private SpriteFont m_DebugFont;
		private String m_Text = "";
		private SpriteBatch m_DrawBatch;

		private Dictionary<String, String> m_OutputSlots = new Dictionary<String, String>();
		private Dictionary<String, String> m_OutputSuffixes = new Dictionary<String, String>();

		public override void LoadContent()
		{
			m_DebugFont = Core.ResourceManager.Instance.GetResource<SpriteFont>("debug assets/debugfont");
			m_PrimBatch = new Core.PrimitiveBatch (SquareManMan.Instance.GraphicsDevice);
			m_PrimBatch.ActiveColor = Color.Black * 0.6F;
			m_DrawBatch = new SpriteBatch(SquareManMan.Instance.GraphicsDevice);
		}

		public override void PostDraw()
		{
			if (!Enabled)
				return;

			m_Text = "=== DEBUG ===" + m_Text;

			m_Text += "\n\n=== OUTPUT SLOTS ===";
			foreach (KeyValuePair<String, String> pair in m_OutputSlots)
			{
				m_Text += "\n" + pair.Key + ": " + pair.Value + m_OutputSuffixes[pair.Key];
			}

			Rectangle viewArea = SquareManMan.Instance.GraphicsDevice.Viewport.Bounds;
			ViewWidth = (int)Math.Max(m_DebugFont.MeasureString(m_Text).X + 20.0F, ViewWidth);
			viewArea.X = viewArea.Width - ViewWidth;
			viewArea.Width = ViewWidth;

			m_PrimBatch.AddRectangle(viewArea);
			m_PrimBatch.DrawPolygons();

			m_DrawBatch.Begin();
			m_DrawBatch.DrawString(m_DebugFont, m_Text, new Vector2(viewArea.X + 5.0F, 5.0F), Color.White);
			m_DrawBatch.End();

			m_Text = "";
		}
		#endregion

		public static DebugView Context;

		/// <summary>
		/// Area in which the view is displayed.
		/// </summary>
		public int ViewWidth;

		/// <summary>
		/// Whether or not the DebugView object should be displayed. Has no effect on methods that
		/// are used to display information; the information will simply not be displayed.
		/// </summary>
		public bool Enabled = true;

		public void UpdateOutputSlot(String slotName, String text)
		{
			m_OutputSlots[slotName] = text;
			if (!m_OutputSuffixes.ContainsKey(slotName))
				m_OutputSuffixes[slotName] = "";
		}

		public void UpdateOutputSlot(String slotName, String text, String suffix)
		{
			UpdateOutputSlot(slotName, text);
			m_OutputSuffixes[slotName] = suffix;
		}

		/// <summary>
		/// Adds text to be displayed by the DebugView object for that frame.
		/// </summary>
		/// <param name="text"></param>
		public void DisplayText(String text)
		{
			m_Text += "\n" + text;
		}
	}
}
