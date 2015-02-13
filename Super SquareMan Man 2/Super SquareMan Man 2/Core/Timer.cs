using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSMM2.Core
{
	public class Timer
	{
		Core.DynamicTime m_TimeSource;
		DateTime m_StartTime;

		public Timer(Core.DynamicTime source)
		{
			m_TimeSource = source;

			ResetStartTime();
		}

		public void ResetStartTime()
		{
			m_StartTime = CurrentTime;
		}

		private DateTime CurrentTime
		{
			get
			{
				if (m_TimeSource == null)
				{
					return DateTime.Now;
				}
				else
				{
					var time = new DateTime();
					time.AddSeconds(m_TimeSource.Time);
					return time;
				}
			}
		}

		public int ElapsedMilliseconds
		{
			get
			{
				return (int)(CurrentTime - m_StartTime).TotalMilliseconds;
			}
		}

		public float ElapsedTime
		{
			get
			{
				return (float)(CurrentTime - m_StartTime).TotalSeconds;
			}
		}
	}
}
