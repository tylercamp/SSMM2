using Microsoft.Xna.Framework;
using System;

namespace SSMM2.Core
{
	/// <summary>
	/// Maintains information regarding Camera orientation and Viewport.
	/// TODO: Paths, smooth transformations.
	/// </summary>
    public class Camera
    {
		private Rectangle m_Viewport;

		public Camera()
		{
			m_Viewport = SquareManMan.Instance.GraphicsDevice.Viewport.Bounds;
		}

		public Camera(Rectangle viewport)
		{
			m_Viewport = viewport;
		}

		public bool CenterCameraAroundPosition = true;

		// XTODO: Revision, does the Camera class *really* need access to the Viewport view the GraphicsDeviceManager?
		//	should be revised once we actually get to more complex Scenes/multiplayer/split-screen.
		//
		// YES: While the Camera will no longer have the responsibility of maintaining Viewport/backbuffer size, it should
		//	have direct access to the GraphicsDeviceManager so that it does not maintain an out-of-date Viewport
		//	(That is, if the Viewport's size *were* to change.)
		//
		// TODO: How should split-screen be handled with a Camera?

		/// <summary>
		/// Size of the Viewport that this Camera deals with.
		/// </summary>
		public Rectangle Viewport
		{
			get
			{
				return m_Viewport;
			}
		}



		/* Camera-Space Manipulation */
		/* (Uses lazy evaluation) */

		/// <summary>
		/// Whenever a member is changed, m_WasChanged is set to true so that the CameraSpaceMatrix property can recalculate
		/// the cache. Initially set to false so that the cache will be set at the first request.
		/// </summary>
		private bool m_WasChanged = true;

		/// <summary>
		/// The matrix that currently applies to our transformation. This is updated upon each change to Position, Scale,
		/// and property, and is returned by the CameraSpaceMatrix property.
		/// </summary>
		private Matrix m_MatrixCache;



		/// <summary>
		/// Position of the Camera's center. Transformations are applied as translation-Rotation-translation-Scale. Second
		/// translation is for centering.
		/// </summary>
		public Vector2 Position
		{
			get
			{
				return m_Position;
			}

			set
			{
				m_Position = value;
				m_WasChanged = true;
			}
		}
		private Vector2 m_Position = Vector2.Zero;

		/// <summary>
		/// Scale of the Camera's view space. Transformations are applied as translation-Rotation-translation-Scale. Second
		/// translation is for centering.
		/// </summary>
		public Vector2 Scale
		{
			get
			{
				return m_Scale;
			}
			set
			{
				m_Scale = value;
				m_WasChanged = true;
			}
		}
		private Vector2 m_Scale = Vector2.One;

		/// <summary>
		/// Rotation of the Camera's view space, in degrees. Transformations are applied as translation-Rotation-translation-Scale.
		/// Second translation is for centering.
		/// </summary>
		public float Rotation
		{
			get
			{
				return m_Rotation;
			}
			set
			{
				m_Rotation = value;
				m_WasChanged = true;
			}
		}
		private float m_Rotation = 0.0F;

		/// <summary>
		/// A transformation matrix containing the transform that defines Camera space. Generally passed to SpriteBatch.Begin
		/// for Camera transformations. Also use this if you want to get the screen-space Position of a point. Transformations
		/// are applied as translation-Rotation-translation-Scale. Second translation is for centering.
		/// </summary>
		public Matrix CameraSpaceMatrix
		{
			get
			{
				if (m_WasChanged)
				{
					Vector2 drawPosition = m_Position + m_JitterOffset;

					m_MatrixCache =
						Matrix.CreateTranslation(-drawPosition.X, -drawPosition.Y, 0.0F) *

						Matrix.CreateRotationZ(MathHelper.ToRadians(-m_Rotation)) *

						//	Divide the translation values by the scaling values due to the offset for the "center" of the Camera
						//		changing as Scale changes.
						(CenterCameraAroundPosition ? Matrix.CreateTranslation((Viewport.Width / 2) / m_Scale.X, (Viewport.Height / 2) / m_Scale.Y, 0.0F) : Matrix.Identity) *

						Matrix.CreateScale(m_Scale.X, m_Scale.Y, 0.0F);

					m_WasChanged = false;
				}

				return m_MatrixCache;
			}
		}

		/// <summary>
		/// Matrix that can be multiplied against to transform a vector from screen space to world space, i.e. selecting
		/// an object in Camera space using the mouse.
		/// TODO: Implement using lazy evaluation
		/// </summary>
		public Matrix ScreenToWorldMatrix
		{
			get
			{
				Vector2 drawPosition = m_Position + m_JitterOffset;

				return
					Matrix.CreateScale(1 / m_Scale.X, 1 / m_Scale.Y, 0.0F) *

					(CenterCameraAroundPosition ? Matrix.CreateTranslation((-Viewport.Width / 2) / m_Scale.X, (-Viewport.Height / 2) / m_Scale.Y, 0.0F) : Matrix.Identity) *

					Matrix.CreateRotationZ(MathHelper.ToRadians(m_Rotation)) *

					Matrix.CreateTranslation(drawPosition.X, drawPosition.Y, 0.0F);
			}
		}



		/// <summary>
		/// The minimum distance that the Position being followed will be from the center from the view. Must be manually
		/// applied via BorderedAbsoluteFollow(). Note that this is in screen coordinates, not world coordinates.
		/// TODO: Would having this in world coordinates provide a better experience than screen coordinates?
		/// </summary>
		public Vector2 FollowBorder = new Vector2 (200, 200);

		/// <summary>
		/// Positions the Camera such that the given Position is within the view frustum, taking into account the
		/// Value of the Camera::FollowBorder property.
		/// </summary>
		/// <param name="Position">Position to be followed.</param>
		public void BorderedAbsoluteFollow(Vector2 position)
		{
			position = Vector2.Transform(position, CameraSpaceMatrix);
			Vector2 newPosition = Vector2.Transform(this.Position, CameraSpaceMatrix);

			if (Math.Abs(position.X - newPosition.X) > FollowBorder.X)
				newPosition.X = position.X - Math.Sign (position.X - newPosition.X) * FollowBorder.X;
			if (Math.Abs(position.Y - newPosition.Y) > FollowBorder.Y)
				newPosition.Y = position.Y - Math.Sign(position.Y - newPosition.Y) * FollowBorder.Y;

			this.Position = Vector2.Transform(newPosition, ScreenToWorldMatrix);
		}



		/// <summary>
		/// Checks the given polygon against the frustum of the view as defined by the Camera's space and the Viewport.
		/// </summary>
		/// <param name="Position">The Xna.Framework.Vector2 Position to be checked against.</param>
		/// <returns>Whether or not the polygon is contained in or intersects the view frustum.</returns>
		public bool IsInCameraFrustum(Vector2 position)
		{
			Vector2 transformedPosition = Vector2.Transform(position, CameraSpaceMatrix);
			return
				transformedPosition.X >= 0 &&
				transformedPosition.Y >= 0 &&
				transformedPosition.X <= Viewport.Width &&
				transformedPosition.Y <= Viewport.Height;
		}

		public void Update(float timeDelta)
		{
			var random = Utility.Random;
			m_JitterOffset = new Vector2((float)(random.NextDouble() * Jitter.X * 2.0) - Jitter.X, (float)(random.NextDouble() * Jitter.Y * 2.0) - Jitter.Y);

			Vector2 oldJitter = Jitter;

			Jitter.X -= JitterNormalizationFactor * timeDelta;
			Jitter.Y -= JitterNormalizationFactor * timeDelta;

			if (Math.Sign(Jitter.X) != Math.Sign(oldJitter.X))
				Jitter.X = 0.0F;
			if (Math.Sign(Jitter.Y) != Math.Sign(oldJitter.Y))
				Jitter.Y = 0.0F;
		}

		public Vector2 Jitter = Vector2.Zero;
		private Vector2 m_JitterOffset = Vector2.Zero;
		public float JitterNormalizationFactor = 3.0F;

		/// <summary>
		/// Checks the given polygon against the frustum of the view as defined by the Camera's space and the Viewport.
		/// TODO: Test
		/// </summary>
		/// <param name="rect">The Xna.Framework.Rectangle to be checked against.</param>
		/// <returns>Whether or not the polygon is contained in or intersects the view frustum.</returns>
		public bool IsInCameraFrustum(Rectangle rect)
		{

			/*
			// TODO: Possible optimization - if Camera space doesn't include a Rotation
			//	then we can remove the need for a transformation matrix and vectors.
			//	Transformation becomes much simpler without the Rotation, and even more
			//	simple as other space components become normalized.
			
			// The algorithm takes the 4 points for the corners of the rect, transforms
			//	them individually, creates a rectangle that contains all 4 points, and
			//	then does an intersection/contains check between the transformed rectangle
			//	and the view frustum. Inefficient, but it works for now.
			Vector2 tl, tr, bl, br;
			tl = Vector2.Transform(new Vector2(rect.X, rect.Y), CameraSpaceMatrix);
			tr = Vector2.Transform(new Vector2(rect.X + rect.Width, rect.Y), CameraSpaceMatrix);
			bl = Vector2.Transform(new Vector2(rect.X, rect.Y + rect.Height), CameraSpaceMatrix);
			br = Vector2.Transform(new Vector2(rect.X + rect.Width, rect.Y + rect.Height), CameraSpaceMatrix);

			Rectangle transformedRectangle = Rectangle.Empty;
			//	Sucks that C# doesn't have variadic functions
			transformedRectangle.X = (int)Math.Min(Math.Min(Math.Min(tl.X, tr.X), bl.X), br.X);
			transformedRectangle.Y = (int)Math.Min(Math.Min(Math.Min(tl.Y, tr.Y), bl.Y), br.Y);
			transformedRectangle.Width = transformedRectangle.X - (int)Math.Max(Math.Max(Math.Max(tl.X, tr.X), bl.X), br.X);
			transformedRectangle.Height = transformedRectangle.Y - (int)Math.Max(Math.Max(Math.Max(tl.Y, tr.Y), bl.Y), br.Y);
			 */

			Vector2 positionA, positionB;
			positionA = Vector2.Transform(new Vector2(rect.X, rect.Y), CameraSpaceMatrix);
			positionB = Vector2.Transform(new Vector2(rect.X + rect.Width, rect.Y + rect.Height), CameraSpaceMatrix);

			Rectangle transformedRectangle
				= new Rectangle(
					(int)positionA.X,
					(int)positionA.Y,
					(int)(positionB.X - positionA.X),
					(int)(positionB.Y - positionA.Y)
					);

			return CollisionMaskAABB.FromRectangle(transformedRectangle).Intersects(CollisionMaskAABB.FromRectangle(Viewport));
		}
	}
}
