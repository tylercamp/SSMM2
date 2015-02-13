using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SSMM2.Core
{
	public abstract class KinematicEntity : Entity
	{
		public Vector2 Velocity = Vector2.Zero;
		public bool HasGravity = true;
		public bool HasFriction = true;
		public bool Solid = true;

		public bool IgnoreVelocityLimits = false;

		public Vector2 CummulativeForce = Vector2.Zero;
		private Vector2 m_CurrentForceApplicationMax = Vector2.Zero;

		private bool m_DidCollideX = false, m_DidCollideY = false;

		public KinematicEntity(Core.Scene ownerScene)
			: base(ownerScene)
		{
		}

		public void ApplyForceX(float xforce, float maxAxialMagnitude)
		{
			m_CurrentForceApplicationMax.X = Math.Max(m_CurrentForceApplicationMax.X, maxAxialMagnitude);
			CummulativeForce.X += xforce;
		}

		public void ApplyForceY(float yforce, float maxAxialMagnitude)
		{
			m_CurrentForceApplicationMax.Y = Math.Max(m_CurrentForceApplicationMax.Y, maxAxialMagnitude);
			CummulativeForce.Y += maxAxialMagnitude;
		}

		public void ApplyForce(Vector2 force, Vector2 maxPerAxisMagnitude)
		{
			m_CurrentForceApplicationMax += maxPerAxisMagnitude;
			CummulativeForce += force;
		}

		public void ApplyForce(Vector2 force)
		{
			CummulativeForce += force;
			m_CurrentForceApplicationMax = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
		}

		public bool IsGrounded()
		{
			Core.CollisionWorld collision = OwnerScene.CollisionWorld;
			return collision.EntityPlaceFree<CollisionType.Solid>(this, Position.X, Position.Y + 1.0f) != null;
		}

		public override void Update(float timeDelta)
		{
			base.Update(timeDelta);

			if (HasGravity)
				ApplyForce(Settings.Gravity, Vector2.Normalize(Settings.Gravity) * Settings.MaxVelocity);

			//Debug.DebugView.Context.DisplayText("DidCollideX: " + m_DidCollideX.ToString());
			//Debug.DebugView.Context.DisplayText("DidCollideY: " + m_DidCollideY.ToString());

			if ((CummulativeForce.X == 0.0F || Math.Sign(CummulativeForce.X) != Math.Sign(Velocity.X)) && m_DidCollideY && HasFriction)
			{
				float prevVelocity = Velocity.X;
				Velocity.X -= Settings.Friction.X * timeDelta * Math.Sign(Velocity.X);
				if (Math.Sign(prevVelocity) != Math.Sign(Velocity.X))
					Velocity.X = 0.0F;
			}

			if ((CummulativeForce.Y == 0.0F || Math.Sign(CummulativeForce.Y) != Math.Sign(Velocity.Y)) && m_DidCollideX && HasFriction)
			{
				float prevVelocity = Velocity.Y;
				Velocity.Y -= Settings.Friction.Y * timeDelta * Math.Sign(Velocity.Y);
				if (Math.Sign(prevVelocity) != Math.Sign(Velocity.Y))
					Velocity.Y = 0.0F;
			}

			Vector2 localForce = CummulativeForce * timeDelta;
			if (Math.Abs(Velocity.X) < m_CurrentForceApplicationMax.X)
			{
				Velocity.X += localForce.X;
				if (Math.Abs(Velocity.X) > m_CurrentForceApplicationMax.X)
					Velocity.X = m_CurrentForceApplicationMax.X * (float)Math.Sign(Velocity.X);
			}

			if (Math.Abs(Velocity.Y) < m_CurrentForceApplicationMax.Y)
			{
				Velocity.Y += localForce.Y;
				if (Math.Abs(Velocity.Y) > m_CurrentForceApplicationMax.Y)
					Velocity.Y = m_CurrentForceApplicationMax.Y * (float)Math.Sign(Velocity.Y);
			}

			//Debug.DebugView.Context.DisplayText("m_CurrentForceApplicationMax: " + m_CurrentForceApplicationMax);
			//Debug.DebugView.Context.DisplayText("CummulativeForce: " + CummulativeForce);
			//Debug.DebugView.Context.DisplayText("Camera Position: " + Core.Camera.Context.Position);

			CummulativeForce = Vector2.Zero;
			m_CurrentForceApplicationMax = Vector2.Zero;

			Vector2 startPosition = Position;
			Vector2 localVelocity = Velocity * timeDelta;

			if (Solid)
			{
				Core.CollisionWorld collision = OwnerScene.CollisionWorld;

				bool collidesOnX = collision.EntityPlaceFree<CollisionType.Solid>(this, startPosition.X + localVelocity.X, startPosition.Y) != null;
				bool collidesOnY = collision.EntityPlaceFree<CollisionType.Solid>(this, startPosition.X, startPosition.Y + localVelocity.Y) != null;
				bool collidesOnVector = collision.EntityPlaceFree<CollisionType.Solid>(this, startPosition + localVelocity) != null;

				if (collidesOnVector && !(collidesOnX || collidesOnY))
				{
					collision.EntityMoveSafe<CollisionType.Solid>(this, localVelocity);
					Velocity = Vector2.Zero;
				}
				else
				{
					if (collidesOnX)
					{
						collision.EntityMoveSafe<CollisionType.Solid>(this, new Vector2(localVelocity.X, 0.0F));
						Velocity.X = 0.0F;
					}
					else
					{
						Position.X += localVelocity.X;
					}

					if (collidesOnY)
					{
						collision.EntityMoveSafe<CollisionType.Solid>(this, new Vector2(0.0F, localVelocity.Y));
						Velocity.Y = 0.0F;
					}
					else
					{
						Position.Y += localVelocity.Y;
					}
				}

				m_DidCollideX = collidesOnX;
				m_DidCollideY = collidesOnY;
			}
			else
			{
				Position += Velocity * timeDelta;
			}

			if (Math.Abs(Velocity.X) < 0.1F) Velocity.X = 0.0F;
			if (Math.Abs(Velocity.Y) < 0.1F) Velocity.Y = 0.0F;

			if (Velocity.Length() > Settings.MaxVelocity && !IgnoreVelocityLimits)
			{
				Velocity.Normalize();
				Velocity *= Settings.MaxVelocity;
			}
		}
	}
}
