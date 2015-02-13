using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSMM2.Core
{
	public class SceneManager
	{
		private List<SceneManagerMember> m_Scenes = new List<SceneManagerMember>();
		private bool m_IsProcessing = false;

		/// <summary>
		/// Used to maintain an operation upon the scene list. SceneListOperations are added to the List of operations and handled
		/// upon a call to HandleDeferredOperations. This system is used to permit any convoluted set of adds/removals/replacements
		/// that could happen within a game loop.
		/// </summary>
		private class SceneListOperation
		{
			public enum Op
			{
				Pop,
				Push,
				Replace,
				Clear
			}

			public Op operation;
			public SceneManagerMember referenceScene;

			public SceneListOperation(Op operation, SceneManagerMember referenceScene)
			{
				this.operation = operation;
				this.referenceScene = referenceScene;
			}
		}

		private List<SceneListOperation> m_DeferredSceneOperations = new List<SceneListOperation>();

		private LinkedList<Widget> m_Widgets = new LinkedList<Widget>();
		private List<WidgetListOperation> m_DeferredWidgetOperations = new List<WidgetListOperation>();

		private class WidgetListOperation
		{
			public enum Op
			{
				Add,
				Remove
			}

			public Op operation;
			public Widget relevantWidget;

			public WidgetListOperation(Widget relevantWidget, Op operation)
			{
				this.operation = operation;
				this.relevantWidget = relevantWidget;
			}
		}

		public Scene TopScene
		{
			get
			{
				if (m_Scenes.Count == 0)
					return null;

				return m_Scenes[m_Scenes.Count - 1] as Scene;
			}
		}

		private void ImmediateAddWidget(Widget widget)
		{
			widget.m_WidgetLinkPosition = m_Widgets.AddLast(widget);
		}

		private void ImmediateRemoveWidget(Widget widget)
		{
			if (widget.m_WidgetLinkPosition == null)
				return;

			m_Widgets.Remove(widget.m_WidgetLinkPosition);
			widget.m_WidgetLinkPosition = null;
		}

		public static SceneManager Instance;

		/// <summary>
		/// Used for referencing/traversing/manipulating the Scenes contained within the SceneManager. Changes effected upon the List provided by Scenes is not
		/// reflected in the internally-held List. Manipulation of the list itself must be done by PushScene() and PopScene().
		/// </summary>
		public List<Scene> Scenes
		{
			get
			{
				List<Scene> scenes = new List<Scene>();

				for (int i = 0; i < m_Scenes.Count; i++)
				{
					if (m_Scenes[i] is Scene)
						scenes.Add(m_Scenes[i] as Scene);
				}

				return scenes;
			}
		}

		public void ClearScenes()
		{
			if (m_IsProcessing)
				m_DeferredSceneOperations.Add(new SceneListOperation(SceneListOperation.Op.Clear, null));
			else
				ImmediateClearScenes();
		}

		/// <summary>
		/// Adds the given widget to the scene. If the scene is being processed, the addition is marked as a deferred operation and will
		/// be completed once HandleDeferredOperations is called outside of scene's processing loop.
		/// </summary>
		/// <param name="widget">Widget to be added to the scene.</param>
		/// <returns>The same widget that was passed as a parameter.</returns>
		public Widget AddWidget(Widget widget)
		{
			if (m_IsProcessing)
				m_DeferredWidgetOperations.Add(new WidgetListOperation(widget, WidgetListOperation.Op.Add));
			else
				ImmediateAddWidget(widget);

			return widget;
		}

		/// <summary>
		/// Removes the given widget from the scene and calls its Release() method. If the scene is being processed, the removal is marked
		/// as a deferred operation and will be completed once HandleDeferredOperations is called outside of the scene's processing loop.
		/// </summary>
		/// <param name="widget">Widget to be removed.</param>
		public void RemoveWidget(Widget widget)
		{
			if (m_IsProcessing)
				m_DeferredWidgetOperations.Add(new WidgetListOperation(widget, WidgetListOperation.Op.Remove));
			else
				ImmediateRemoveWidget(widget);
		}

		public void PushModifier(bool updateEnabled, bool drawEnabled)
		{
			if (m_IsProcessing)
				m_DeferredSceneOperations.Add(new SceneListOperation(SceneListOperation.Op.Push, new SceneStackModifier(updateEnabled, drawEnabled)));
			else
				ImmediatePushModifier(new SceneStackModifier(updateEnabled, drawEnabled));
		}

		private void ImmediatePushModifier(SceneStackModifier modifier)
		{
			m_Scenes.Add(modifier);
		}

		/// <summary>
		/// Applies all deferred operations to the Scenes list.
		/// </summary>
		public void HandleDeferredOperations()
		{
			foreach (SceneListOperation op in m_DeferredSceneOperations)
			{
				switch (op.operation)
				{
					case (SceneListOperation.Op.Push):
						{
							if (op.referenceScene is Scene)
								ImmediatePushScene(op.referenceScene as Scene);
							else
								ImmediatePushModifier(op.referenceScene as SceneStackModifier);
							break;
						}

					case (SceneListOperation.Op.Pop):
						{
							ImmediatePopScene();
							break;
						}

					case (SceneListOperation.Op.Replace):
						{
							ImmediateReplaceScene(op.referenceScene as Scene);
							break;
						}

					case (SceneListOperation.Op.Clear):
						{
							ImmediateClearScenes();
							break;
						}
				}
			}

			m_DeferredSceneOperations.Clear();

			foreach (WidgetListOperation op in m_DeferredWidgetOperations)
			{
				switch (op.operation)
				{
					case (WidgetListOperation.Op.Add):
						{
							ImmediateAddWidget(op.relevantWidget);
							break;
						}
					case (WidgetListOperation.Op.Remove):
						{
							ImmediateRemoveWidget(op.relevantWidget);
							break;
						}
				}
			}

			foreach (SceneManagerMember scene in m_Scenes)
			{
				if (scene is Scene)
					(scene as Scene).HandleDeferredOperations();
			}
		}

		/// <summary>
		/// Adds a requested operation for the scene at the top of the list to be removed. This operation should be completed by the end of a game loop,
		/// or upon the invokation of HandleDeferredOperations(). If the Scenes are not currently being processed, this operation occurs immediately.
		/// </summary>
		public void PopScene()
		{
			if (m_IsProcessing)
				m_DeferredSceneOperations.Add(new SceneListOperation(SceneListOperation.Op.Pop, null));
			else
				ImmediatePopScene();
		}

		/// <summary>
		/// Adds a requested operation for the given scene to be added to the top of the list. This operation should be the completed by the end of a game loop,
		/// or upon the invokation of HandleDeferredOperations(). If the Scenes are not currently being processed, this operation occurs immediately.
		/// </summary>
		/// <param name="newScene">Scene to be added to the top of the list.</param>
		public void PushScene(Scene newScene)
		{
			if (m_IsProcessing)
				m_DeferredSceneOperations.Add(new SceneListOperation(SceneListOperation.Op.Push, newScene));
			else
				ImmediatePushScene(newScene);
		}

		/// <summary>
		/// Adds a requested operation for the scene at the top of the list to be replaced with the given scene. If there is no scene in the list, the new
		/// scene is simply added. This operation should be completed by the end of a game loop, or upon the invokation of HandleDeferredOperations().
		/// If the Scenes are not currently being processed, this operation occurs immediately.
		/// </summary>
		/// <param name="newScene">The scene to replace the scene at the top of the stack.</param>
		public void ReplaceScene(Scene newScene)
		{
			if (m_IsProcessing)
				m_DeferredSceneOperations.Add(new SceneListOperation(SceneListOperation.Op.Replace, newScene));
			else
				ImmediateReplaceScene(newScene);
		}

		private void ImmediatePopScene()
		{
			if (m_Scenes.Count == 0)
				return;

			Scene newScene = null;
			Scene oldScene = m_Scenes.Last() as Scene;
			if (m_Scenes.Count > 1)
				newScene = m_Scenes.ElementAt(m_Scenes.Count - 2) as Scene;


			if (OnWillModifySceneList != null)
				OnWillModifySceneList(oldScene, newScene);

			m_Scenes.RemoveAt(m_Scenes.Count - 1);

			while (m_Scenes.Count > 0 && m_Scenes.Last() is SceneStackModifier)
				m_Scenes.RemoveAt(m_Scenes.Count - 1);

			if (OnSceneListModified != null)
				OnSceneListModified(oldScene, newScene);
		}

		private void ImmediatePushScene(Scene newScene)
		{
			Scene old = null;
			if (m_Scenes.Count > 0)
				old = m_Scenes.Last() as Scene;

			if (OnWillModifySceneList != null)
				OnWillModifySceneList(old, newScene);

			m_Scenes.Add(newScene);

			if (OnSceneListModified != null)
				OnSceneListModified(old, newScene);
		}

		private void ImmediateReplaceScene(Scene newScene)
		{
			Scene old = null;
			if (m_Scenes.Count > 0)
			{
				old = m_Scenes.Last() as Scene;
				if (OnWillModifySceneList != null)
					OnWillModifySceneList(old, newScene);
				m_Scenes.RemoveAt(m_Scenes.Count - 1);
			}
			else
			{
				if (OnWillModifySceneList != null)
					OnWillModifySceneList(null, newScene);
			}

			m_Scenes.Add(newScene);

			if (OnSceneListModified != null)
				OnSceneListModified(old, newScene);
		}


		private void ImmediateClearScenes()
		{
			m_Scenes.Clear();
		}

		public delegate void SceneListModifiedCallback(Scene oldScene, Scene newScene);
		public event SceneListModifiedCallback OnWillModifySceneList;
		public event SceneListModifiedCallback OnSceneListModified;

		private SceneStackModifier GetSceneModifier(int index)
		{
			SceneStackModifier compositeModifier = new SceneStackModifier(true, true);

			for (int i = m_Scenes.Count - 1; i >= index; i--)
			{
				SceneManagerMember currentMember = m_Scenes[i];
				if (currentMember is Scene)
					continue;

				SceneStackModifier currentModifier = m_Scenes[i] as SceneStackModifier;
				if (!currentModifier.ScenesAreDrawable)
					compositeModifier.ScenesAreDrawable = false;
				if (!currentModifier.ScenesAreUpdateable)
					compositeModifier.ScenesAreUpdateable = false;
			}

			return compositeModifier;
		}

		/// <summary>
		/// Renders all Scenes in the list, from top to bottom.
		/// </summary>
		/// <param name="spriteBatch">Pre-initialized SpriteBatch object to be used for rendering.</param>
		public void DrawScenes(SpriteBatch spriteBatch, PrimitiveBatch primitiveBatch)
		{
			m_IsProcessing = true;

			foreach (Widget widget in m_Widgets)
			{
				widget.PreDraw();
			}

			for (int i = 0; i < m_Scenes.Count; i++)
			{
				if (m_Scenes[i] is SceneStackModifier)
					continue;

				SceneStackModifier modifier = GetSceneModifier(i);
				bool canDrawCurrentScene = modifier.ScenesAreDrawable;
				if (canDrawCurrentScene)
					(m_Scenes[i] as Scene).Draw(spriteBatch, primitiveBatch);
			}

			foreach (Widget widget in m_Widgets)
			{
				widget.PostDraw();
			}

			m_IsProcessing = false;
		}

		/// <summary>
		/// Updates all Scenes in the list, from top to bottom.
		/// </summary>
		public void UpdateScenes(float timeDelta)
		{
			m_IsProcessing = true;

			foreach (Widget widget in m_Widgets)
			{
				widget.PreUpdate();
			}

			for (int i = 0; i < m_Scenes.Count; i++)
			{
				if (m_Scenes[i] is SceneStackModifier)
					continue;

				SceneStackModifier modifier = GetSceneModifier(i);
				bool canUpdateCurrentScene = modifier.ScenesAreUpdateable;
				if (canUpdateCurrentScene)
					(m_Scenes[i] as Scene).Update(timeDelta);
			}

			foreach (Widget widget in m_Widgets)
			{
				widget.PostUpdate();
			}

			m_IsProcessing = false;
		}
	}
}
