using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSMM2.Core
{
	/// <summary>
	/// A supplementary type that exists outside of any scene, they instead belong to the SceneManager. Widgets
	/// can be processed before and after both the Draw and Update stages.
	/// </summary>
	public abstract class Widget : ManuallyManaged
	{
		//	Needed to remove the widget upon release.
		internal LinkedListNode<Widget> m_WidgetLinkPosition = null;

		/// <summary>
		/// Removes the widget from the SceneManager and unloads all resources contained by the widget.
		/// </summary>
		public override void Destroy()
		{
			SceneManager.Instance.RemoveWidget(this);
		}

		public virtual void PreUpdate()
		{
		}

		public virtual void PostUpdate()
		{
		}

		public virtual void PreDraw()
		{
		}

		public virtual void PostDraw()
		{
		}
	}
}
