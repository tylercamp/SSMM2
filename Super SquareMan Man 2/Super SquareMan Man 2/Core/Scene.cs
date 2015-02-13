using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace SSMM2.Core
{
	/*
	 * TODO: Stack process modifiers, i.e.:
	 * 
	 * SCENE E (Scene E processes normally, D is paused, the rest are stopped entirely)s
	 * PAUSE MODIFIER (Scene D is paused, but the rest are still stopped)
	 * SCENE D
	 * STOP-ALL MODIFIER (Scenes A, B, and C are stopped from drawing/updating)
	 * SCENE C
	 * PAUSE MODIFIER (Scenes A and B are paused)
	 * SCENE B
	 * SCENE A
	 * 
	 */

	public class Scene : SceneManagerMember
	{
		private bool m_IsProcessing = false;

		private SortedSet<Entity> m_Entities = new SortedSet<Entity>();

		private List<Entity> m_DeferredDepthChangeEntities = new List<Entity>();
		private List<SceneOperation> m_DeferredSceneOperations = new List<SceneOperation>();

		public int Tag = 0;

		public enum SceneSourceType
		{
			Unknown,
			File
		}

		/// <summary>
		/// The type of source that created the scene.
		/// </summary>
		public SceneSourceType SourceType = SceneSourceType.Unknown;

		/// <summary>
		/// An identifier for the source of the scene.
		/// </summary>
		public String Source = null;

		private class SceneOperation
		{
			public SceneOperation(Entity entity, Op operation)
			{
				this.relevantEntity = entity;
				this.operation = operation;
			}

			public Entity relevantEntity;

			public enum Op
			{
				Remove,
				Add
			}
			public Op operation;
		}

		internal void PostEntityDepthChange(Entity entity)
		{
			m_DeferredDepthChangeEntities.Add(entity);
		}

		private void ImmediateAddEntity(Entity entity)
		{
			entity.m_IsDead = false;
			entity.OwnerScene = this;
			m_Entities.Add(entity);

			if (m_CanImmediateLoadContent)
				entity.LoadContent();
		}

		private void ImmediateRemoveEntity(Entity entity)
		{
			entity.OwnerScene = null;
			m_Entities.Remove(entity);
		}

		/// <summary>
		/// A copy of the Entities collection stored internally. Use sparingly.
		/// </summary>
		public SortedSet<Entity> Entities
		{
			get
			{
				return new SortedSet<Entity>(m_Entities);
			}
		}

		/// <summary>
		/// Camera used by the Entities within the scene. Must be set manually.
		/// </summary>
		public Camera Camera = null;

		/// <summary>
		/// Collision world that applies primarily to the Entities within this scene.
		/// </summary>
		public CollisionWorld CollisionWorld;

		public DynamicTime Time = new DynamicTime();

		public uint EntityCount
		{
			get
			{
				return (uint)m_Entities.Count;
			}
		}

		/// <summary>
		/// Determines whether or not the scene object will draw Entities upon request.
		/// </summary>
		public bool ComponentDrawEnabled = true;

		/// <summary>
		/// Determines whether or not the scene object will update Entities upon request.
		/// </summary>
		public bool ComponentUpdateEnabled = true;

		private bool m_CanImmediateLoadContent = false;

		public Scene()
		{
			Camera = new Camera();
			CollisionWorld = new CollisionWorld(m_Entities);
		}

		public void LoadContent()
		{
			m_CanImmediateLoadContent = true;
			m_IsProcessing = true;
			foreach (Entity entity in m_Entities)
			{
				entity.LoadContent();
			}
			m_IsProcessing = false;
			HandleDeferredOperations();
		}

		/// <summary>
		/// Adds the given entity to the scene. If the scene is being processed, the addition is marked as a deferred operation and will
		/// be completed once HandleDeferredOperations is called outside of scene's processing loop.
		/// </summary>
		/// <param name="entity">Entity to be added to the scene.</param>
		/// <returns>The same entity that was passed as a parameter.</returns>
		public Entity AddEntity(Entity entity)
		{
			entity.OwnerScene = this;

			m_DeferredSceneOperations.Add(new SceneOperation(entity, SceneOperation.Op.Add));
			//if (m_IsProcessing)
			//	m_DeferredSceneOperations.Add(new SceneOperation(entity, SceneOperation.Op.Add));
			//else
			//	ImmediateAddEntity(entity);

			return entity;
		}

		/// <summary>
		/// Removes the given entity from the scene and calls its Release() method. If the scene is being processed, the removal is marked
		/// as a deferred operation and will be completed once HandleDeferredOperations is called outside of the scene's processing loop.
		/// </summary>
		/// <param name="entity">Entity to be removed.</param>
		public void RemoveEntity(Entity entity)
		{
			if (m_IsProcessing)
				m_DeferredSceneOperations.Add(new SceneOperation(entity, SceneOperation.Op.Remove));
			else
				ImmediateRemoveEntity(entity);
		}

		/// <summary>
		/// Applies the scene's Camera's properties and calls the Draw methods of all contained Entities.
		/// </summary>
		/// <param name="spriteBatch">The SpriteBatch object to be used for all drawing operations.</param>
		public void Draw(SpriteBatch spriteBatch, PrimitiveBatch primitiveBatch, bool beginNewBatch = true)
		{
			if (!ComponentDrawEnabled)
				return;

			m_IsProcessing = true;

			if (beginNewBatch)
				spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Camera.CameraSpaceMatrix);

			foreach (Entity entity in m_Entities)
			{
				entity.Draw(spriteBatch, primitiveBatch);
			}

			if (beginNewBatch)
				spriteBatch.End();

			m_IsProcessing = false;
		}

		public void ReconfigureDraw(SpriteBatch spriteBatch)
		{
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Camera.CameraSpaceMatrix);
		}

		public void Update(float timeDelta)
		{
			if (!ComponentUpdateEnabled)
				return;

			m_IsProcessing = true;

			Time.MarkNewFrame(timeDelta);

			Camera.Update(timeDelta);

			foreach (Entity entity in m_Entities)
			{
				if (!entity.m_IsDead)
					entity.Update(Time.Delta);
			}

			m_IsProcessing = false;
		}

		public void HandleDeferredOperations()
		{
			while (m_DeferredSceneOperations.Count != 0)
			{
				var operationsCopy = new List<SceneOperation>(m_DeferredSceneOperations);
				m_DeferredSceneOperations.Clear();

				foreach (SceneOperation op in operationsCopy)
				{
					switch (op.operation)
					{
						case (SceneOperation.Op.Add):
							{
								ImmediateAddEntity(op.relevantEntity);
								break;
							}

						case (SceneOperation.Op.Remove):
							{
								ImmediateRemoveEntity(op.relevantEntity);
								break;
							}
					}
				}
			}

			foreach (Entity entity in m_DeferredDepthChangeEntities)
			{
				if (!m_Entities.Contains(entity))
					throw new Exception("Impossible depth change; unable to operate on entity that does not exist within the current Scene.");

				m_Entities.Remove(entity);
				entity.m_Depth = entity.m_NewDepth;
				entity.m_SubDepth = Utility.Random.NextLong();
				m_Entities.Add(entity);
			}

			m_DeferredDepthChangeEntities.Clear();
			m_DeferredSceneOperations.Clear();
		}
	}
}
