using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SSMM2.Core
{
	public class CollisionMaskAABB
	{
		public static CollisionMaskAABB Empty
		{
			get
			{
				return new CollisionMaskAABB(0.0F, 0.0F, 0.0F, 0.0F);
			}
		}

		public static CollisionMaskAABB FromRectangle(Rectangle rect)
		{
			return new CollisionMaskAABB(rect);
		}

		public float X;
		public float Y;
		public float Width;
		public float Height;

		public float Left
		{
			get
			{
				return X;
			}
			set
			{
				Width += X - value;
				X = value;
			}
		}

		public float Right
		{
			get
			{
				return X + Width;
			}
			set
			{
				Width = value - X;
			}
		}

		public float Top
		{
			get
			{
				return Y;
			}
			set
			{
				Height += Y - value;
				Y = value;
			}
		}

		public float Bottom
		{
			get
			{
				return Y + Height;
			}
			set
			{
				Height = value - Y;
			}
		}

		public Rectangle XnaBounds
		{
			get
			{
				return new Rectangle((int)X, (int)Y, (int)Width, (int)Height);
			}
		}

		public Vector2 Size
		{
			get
			{
				return new Vector2(Width, Height);
			}
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;

			CollisionMaskAABB mask = obj as CollisionMaskAABB;

			if (mask == null)
				return base.Equals(obj);

			const float tolerance = 0.01F;

			return
				Math.Abs(mask.Left - Left) < tolerance		&&
				Math.Abs(mask.Right - Right) < tolerance	&&
				Math.Abs(mask.Top - Top) < tolerance		&&
				Math.Abs(mask.Bottom - Bottom) < tolerance;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public CollisionMaskAABB()
		{
			X = 0.0f; Y = 0.0f; Width = 0.0f; Height = 0.0f;
		}

		public CollisionMaskAABB(float x, float y, float width, float height)
		{
			this.X = x;
			this.Y = y;
			this.Width = width;
			this.Height = height;
		}

		public CollisionMaskAABB(Rectangle source)
		{
			X = source.X;
			Y = source.Y;
			Width = source.Width;
			Height = source.Height;
		}

		public bool Intersects(CollisionMaskAABB other)
		{
			return other.Left < this.Right && other.Right > this.Left && other.Top < this.Bottom && other.Bottom > this.Top;
		}

		public bool Intersects(Vector2 other)
		{
			return other.X < this.Right && other.X > this.Left && other.Y < this.Bottom && other.Y > this.Top;
		}

		private Vector2 GetAxisIntersectionTimes(Vector2 movingAxis, Vector2 staticAxis, float distance)
		{
			if (distance == 0.0F)
				distance = 0.00001F;

			float timeEnter = (staticAxis.X - movingAxis.Y) / distance;
			float timeLeave = (staticAxis.Y - movingAxis.X) / distance;

			if (distance < 0)
			{
				float temp = timeEnter;
				timeEnter = timeLeave;
				timeLeave = temp;
			}

			return new Vector2(timeEnter, timeLeave);
		}

		/* Returns time of entry and time of exit as the x and y components of the returned Vector2 value */
		public Vector2 GetCollisionTimes(CollisionMaskAABB other, Vector2 velocity)
		{
			Vector2 xaxisTimes = GetAxisIntersectionTimes (
				new Vector2 (X, X + Width),
				new Vector2 (other.X, other.X + other.Width),
				velocity.X
				);

			Vector2 yaxisTimes = GetAxisIntersectionTimes (
				new Vector2 (Y, Y + Height),
				new Vector2 (other.Y, other.Y + other.Height),
				velocity.Y
				);

			Vector2 result = new Vector2 (Math.Max (xaxisTimes.X, yaxisTimes.X), Math.Min (xaxisTimes.Y, yaxisTimes.Y));

			if (result.X >= result.Y)
				return Vector2.Zero;
			else
				return result;
		}
	}

	public class CollisionWorld
	{
		private SortedSet<Entity> m_Entities;

		public CollisionWorld(SortedSet<Entity> sourceEntityList)
		{
			m_Entities = sourceEntityList;
		}

		public Entity EntityPlaceFree<T>(Entity entity, float x, float y)
		{
			return EntityPlaceFree<T>(entity, new Vector2(x, y));
		}

		public Entity EntityPlaceFree<T>(Entity entity, Vector2 position)
		{
			return EntityPlaceFree(entity, position, typeof(T));
		}

		public Entity EntityPlaceFree(Entity entity, Vector2 position, Type targetType)
		{
			if (entity.BoundingBox == CollisionMaskAABB.Empty) return null;

			Vector2 startPosition = entity.Position;
			entity.Position = position;

			foreach (Entity currentEntity in m_Entities)
			{
				if (
					targetType.IsAssignableFrom (currentEntity.GetType ()) &&
					currentEntity != entity &&
					entity.BoundingBox.Intersects (currentEntity.BoundingBox)
					)
				{
					entity.Position = startPosition;
					return currentEntity;
				}
			}

			entity.Position = startPosition;
			return null;
		}

		public Entity EntityMoveSafe<T>(Entity entity, Vector2 direction)
		{
			return EntityMoveSafe(entity, direction, typeof(T));
		}


		public Entity EntityMoveSafe(Entity entity, Vector2 direction, Type targetType)
		{
			float nearestTime = 1.0F;
			Entity nearestEnemy = null;

			foreach (Entity currentEntity in m_Entities)
			{
				if (!targetType.IsAssignableFrom(currentEntity.GetType()))
					continue;

				Vector2 collisionTimes = entity.BoundingBox.GetCollisionTimes(currentEntity.BoundingBox, direction);
				if (collisionTimes.X < 0 || collisionTimes == Vector2.Zero)
					continue;

				if (nearestTime > collisionTimes.X)
				{
					nearestTime = collisionTimes.X;
					nearestEnemy = currentEntity;
				}
			}

			entity.Position += direction * nearestTime;
			return nearestEnemy;
		}

		public Entity PointFree<T>(Entity sourceEntity, Vector2 point)
		{
			foreach (Entity entity in m_Entities)
			{
				if (
					entity is T &&
					entity != sourceEntity &&
					(point.X > entity.BoundingBox.Left && point.X < entity.BoundingBox.Right) &&
					(point.Y > entity.BoundingBox.Top && point.Y < entity.BoundingBox.Bottom)
					)
					return entity;
			}

			return null;
		}
	}
}
