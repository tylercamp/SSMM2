using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SSMM2.Core
{
	/// <summary>
	/// Base class that provides functionality for automated processing by a Scene.
	/// </summary>
	public abstract class Entity : ManuallyManaged, IComparable
	{
		public Texture2D Sprite;

		public Color BlendColor = Color.White;

		public float SpriteRotationDegrees = 0.0F;

		public Vector2 SpriteCenter = Vector2.Zero;

		public Vector2 Position = Vector2.Zero;

		public Vector2 Scale = Vector2.One;

		public override bool Equals(object obj)
		{
			if (obj is Entity)
				return (obj as Entity).m_Depth == m_Depth && (obj as Entity).m_SubDepth == m_SubDepth;
			else
				return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return (int)m_Depth ^ (int)m_SubDepth;
		}

		public Entity(Core.Scene ownerScene)
		{
			OwnerScene = ownerScene;
			if (OwnerScene != null)
			{
				OwnerScene.AddEntity(this);
			}

			Depth = 0;
		}

		public virtual CollisionMaskAABB BoundingBox
		{
			get
			{
				if (Sprite == null)
					return CollisionMaskAABB.Empty;

				return new CollisionMaskAABB(
					Position.X - SpriteCenter.X * Scale.X,
					Position.Y - SpriteCenter.Y * Scale.Y,
					Sprite.Width * Scale.X,
					Sprite.Height * Scale.Y
					);
			}
		}

		public void DrawSelf(SpriteBatch spriteBatch)
		{
			if (Sprite == null) return;

			if (!OwnerScene.Camera.IsInCameraFrustum(BoundingBox.XnaBounds))
				return;

			spriteBatch.Draw(Sprite, Position, null, BlendColor, MathHelper.ToRadians(SpriteRotationDegrees), SpriteCenter, Scale, SpriteEffects.None, 0.0F);
		}

		protected void SetSpriteFromResource(String resourceName)
		{
			SetSpriteFromResource(Core.ResourceManager.Instance.GetResource<Texture2D>(resourceName));
		}

		protected void SetSpriteFromResource(Texture2D resource)
		{
			Sprite = resource;
			SpriteCenter = new Vector2(Sprite.Width / 2, Sprite.Height / 2);
		}

		//  Entities are stored in a SortedSet, which works well with automatically sorting Entities by Depth, but it breaks on 
		//  Entities that have the same Depth. In the case of the same Depth, we use subDepth which is nearly guaranteed to be
		//  different for each entity on the same depth.
		internal long m_SubDepth = Utility.Random.NextLong();

		internal long m_Depth = 0;
		internal long m_NewDepth = 0;

		internal bool m_IsDead = true;
		internal bool m_IsLoaded = false;

		/// <summary>
		/// Determines the order in which an entity is processed in relation to other Entities. An entity with a higher Depth will
		/// be processed before one with a lower Depth. Changing the Depth causes ordering to be reconfigured at the end of a game
		/// loop.
		/// </summary>
		public long Depth
		{
			get
			{
				return m_Depth;
			}

			set
			{
				//	m_Depth will be updated to m_NewDepth once deferred operations are carried out by the containing scene.
				m_NewDepth = value;

				if (OwnerScene != null)
					OwnerScene.PostEntityDepthChange(this);
			}
		}

		/// <summary>
		/// IComparable CompareTo implementation. Compares based on Depth and subDepth properties.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public int CompareTo(object obj)
		{
			if (obj == null)
				return 1;

			Entity comparable = obj as Entity;
			if (comparable.Depth == Depth)
			{
				return comparable.m_SubDepth.CompareTo(m_SubDepth);
			}
			else
			{
				return comparable.Depth.CompareTo(Depth); // Backwards so that higher values are processed first
			}
		}

		public Scene OwnerScene
		{
			get;
			internal set;
		}

		public virtual void Update(float timeDelta)
		{
		}

		public virtual void Draw(SpriteBatch spriteBatch, PrimitiveBatch primitiveBatch)
		{
			DrawSelf(spriteBatch);
		}

		public virtual void Hit(Entity source)
		{
		}

		/// <summary>
		/// Removes the entity from its owner scene, unlinks/removes all collision delegates, and unloads all resources contained by the entity.
		/// </summary>
		public override void Destroy()
		{
			if (OwnerScene != null)
			{
				m_IsDead = true;
				OwnerScene.RemoveEntity(this);
			}
		}
	}
}
