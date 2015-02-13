using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSMM2.UI.Base
{
	public class ControlOrganizer
	{
		public Vector2 Origin;

		public enum AlignStyle
		{
			Left,
			Middle,
			Right,
			Top,
			Bottom
		}

		public AlignStyle VerticleAlignStyle = AlignStyle.Middle;
		public AlignStyle HorizontalAlignStyle = AlignStyle.Middle;

		public Vector2 Padding = Vector2.Zero;

		private List<Control> m_Controls = new List<Control>();

		public void AddControl(Control control)
		{
			if (!m_Controls.Contains(control))
				m_Controls.Add(control);

			control.ParentOrganizer = this;

			RealignControls();
		}

		public void RemoveControl(Control control)
		{
			control.ParentOrganizer = null;

			RealignControls();
		}

		public void RealignControls()
		{
			if (m_Controls.Count == 0)
				return;

			float totalHeight = 0.0F;

			for (int i = 0; i < m_Controls.Count; i++)
			{
				Control current = m_Controls[i];

				totalHeight += current.BoundingBox.Height;

				switch (HorizontalAlignStyle)
				{
					case AlignStyle.Left:
						current.Position.X = Origin.X;
						break;
					case AlignStyle.Middle:
						current.Position.X = Origin.X - current.BoundingBox.Width / 2.0F;
						break;
					case AlignStyle.Right:
						current.Position.X = Origin.X - current.BoundingBox.Width;
						break;
					default:
						throw new Exception("Invalid style for HorizontalAlignStyle.");
				}
			}

			totalHeight += Padding.Y * (m_Controls.Count - 1);

			float currentBasePosition;

			switch (VerticleAlignStyle)
			{
				case AlignStyle.Top:
					currentBasePosition = 0.0F;
					break;
				case AlignStyle.Middle:
					currentBasePosition = -totalHeight / 2.0F;
					break;
				case AlignStyle.Bottom:
					currentBasePosition = totalHeight;
					break;
				default:
					throw new Exception("Invalid style for VerticalAlignStyle.");
			}


			for (int i = 0; i < m_Controls.Count; i++)
			{
				Control current = m_Controls[i];

				current.Position.Y = Origin.Y + currentBasePosition;
				currentBasePosition += Padding.Y + current.BoundingBox.Height;
			}
		}
	}
}
