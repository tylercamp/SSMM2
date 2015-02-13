using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SSMM2.Core
{
	/// <summary>
	/// Simple method for rendering a group of colored primitive types. Not recommended for advanced drawing.
	/// </summary>
	public class PrimitiveBatch
	{
		GraphicsDevice m_Device;
		BasicEffect m_Effect = null;

		List<VertexPositionColor> m_TriangleVertices = new List<VertexPositionColor>();
		List<VertexPositionColor> m_LineVertices = new List<VertexPositionColor>();

		public Color ActiveColor;

		/// <summary>
		/// Creates a new primitive drawing batch that enables the rendering of individual primitive types.
		/// </summary>
		/// <param name="device">GraphicsDevice object to be used for displaying graphics.</param>
		public PrimitiveBatch(GraphicsDevice device)
		{
			m_Device = device;
			m_Effect = new BasicEffect(m_Device);
			m_Effect.VertexColorEnabled = true;
			m_Effect.Texture = null;
			m_Effect.Projection = Matrix.CreateOrthographicOffCenter(0, m_Device.Viewport.Width, m_Device.Viewport.Height, 0, 0, 1);

			ActiveColor = Color.White;
		}

		/// <summary>
		/// Draws all polygons that have been added to the batch without any transformations.
		/// Clears the batch after drawing.
		/// </summary>
		public void DrawPolygons()
		{
			DrawPolygons(Matrix.Identity);
		}

		/// <summary>
		/// Draws all polygons that have been added to the batch with the transform defined
		/// by the given matrix. Clears the batch after drawing.
		/// </summary>
		/// <param name="CameraSpaceMatrix">Matrix to transform by.</param>
		public void DrawPolygons(Matrix transformMatrix)
		{
			m_Effect.View = transformMatrix;
			m_Effect.CurrentTechnique.Passes[0].Apply();
			
			VertexBufferBinding[] previousVertexBuffers = m_Device.GetVertexBuffers();
			IndexBuffer previousIndexBuffer = m_Device.Indices;

			if (m_LineVertices.Count > 0)
			{
				VertexPositionColor[] lineBuffer = new VertexPositionColor[m_LineVertices.Count];

				int i = 0;
				foreach (VertexPositionColor v in m_LineVertices)
				{
					lineBuffer[i++] = v;
				}

				m_Device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, lineBuffer, 0, lineBuffer.Length / 2);

				m_LineVertices.Clear();
			}

			if (m_TriangleVertices.Count > 0)
			{
				VertexPositionColor[] triangleBuffer = new VertexPositionColor[m_TriangleVertices.Count];

				int i = 0;
				foreach (VertexPositionColor v in m_TriangleVertices)
				{
					triangleBuffer[i++] = v;
				}

				m_Device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, triangleBuffer, 0, triangleBuffer.Length / 3);

				m_TriangleVertices.Clear();
			}

			m_Device.SetVertexBuffers(previousVertexBuffers);
			m_Device.Indices = previousIndexBuffer;
		}

		/// <summary>
		/// Adds a line to the batch using the current active color.
		/// </summary>
		/// <param name="p1">Starting point of the line.</param>
		/// <param name="p2">Ending point of the line.</param>
		public void AddLine(Vector2 p1, Vector2 p2)
		{
			m_LineVertices.Add(new VertexPositionColor(new Vector3(p1, 0.0f), ActiveColor));
			m_LineVertices.Add(new VertexPositionColor(new Vector3(p2, 0.0f), ActiveColor));
		}

		/// <summary>
		/// Adds a rectangle to the batch using the current active color.
		/// </summary>
		/// <param name="rect">The rectangle to be drawn.</param>
		public void AddRectangle(Rectangle rect)
		{
			AddTriangle(
				new Vector2(rect.Left, rect.Top),
				new Vector2(rect.Right, rect.Top),
				new Vector2(rect.Left, rect.Bottom)
				);

			AddTriangle(
				new Vector2(rect.Right, rect.Top),
				new Vector2(rect.Right, rect.Bottom),
				new Vector2(rect.Left, rect.Bottom)
				);
		}

		/// <summary>
		/// Adds a triangle to the batch using the current active color. Beware of backface culling.
		/// </summary>
		/// <param name="p1">First point on the triangle.</param>
		/// <param name="p2">Second point on the triangle.</param>
		/// <param name="p3">Third point on the triangle.</param>
		public void AddTriangle(Vector2 p1, Vector2 p2, Vector2 p3)
		{
			m_TriangleVertices.Add(new VertexPositionColor(new Vector3(p1, 0.0f), ActiveColor));
			m_TriangleVertices.Add(new VertexPositionColor(new Vector3(p2, 0.0f), ActiveColor));
			m_TriangleVertices.Add(new VertexPositionColor(new Vector3(p3, 0.0f), ActiveColor));
		}
	}
}
