using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSMM2.Core
{
	public class FpsTracker
	{
		private int m_CurrentFrames = 0;
		private long m_StartTime = 0;

		public static FpsTracker Instance;

		public int Value
		{
			get;
			private set;
		}

		public FpsTracker()
		{
			m_StartTime = DateTime.Now.Ticks;

			Value = 60;
		}

		public void MarkNewFrame()
		{
			if ((DateTime.Now.Ticks - m_StartTime) / 10000000.0F >= 1.0F)
			{
				Value = m_CurrentFrames;
				m_CurrentFrames = 0;
				m_StartTime = DateTime.Now.Ticks;
			}

			Debug.DebugView.Context.UpdateOutputSlot("FPS", Value.ToString());

			m_CurrentFrames++;
		}
	}
}
