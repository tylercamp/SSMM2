using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSMM2.Core
{
	public class DynamicTime
	{
		public DynamicTime()
		{
			m_BaseTime = 0.0F;
		}

		private float m_BaseTime = 0.0F;
		private float m_Delta = 0.0F;

		public void MarkNewFrame(float time)
		{
			m_Delta = time * TimeScale;

			m_BaseTime += m_Delta;
		}

		public float Delta
		{
			get
			{
				if (m_Delta == 0.0F)
					return 1.0f / 60.0f * TimeScale;

				return m_Delta;
			}
		}

		public float TimeScale = 1.0F;

		public float Time
		{
			get
			{
				return m_BaseTime;
			}
		}

		public String CleanTime
		{
			get
			{
				int hours, minutes, seconds, milliseconds;
				float time = this.Time;

				DateTime current = new DateTime();
				current = current.AddSeconds(time);
				hours = current.Hour;
				minutes = current.Minute;
				seconds = current.Second;
				milliseconds = current.Millisecond;

				String result = "";
				if (hours != 0)
					result += hours.ToString() + "h ";
				if (minutes != 0)
					result += minutes.ToString("D2") + "m ";
				if (seconds != 0)
					result += seconds.ToString("D2") + "s ";

				result += milliseconds.ToString("D3") + "ms";

				return result;
			}
		}
	}
}
