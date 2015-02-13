using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SSMM2.Core
{
	public struct ColorScheme
	{
		public enum SchemeType
		{
			Good,
			Bad,
			Unknown
		}

		public SchemeType CurrentScheme;

		public Color RelevantColor
		{
			get
			{
				switch (CurrentScheme)
				{
					case (SchemeType.Good):	return GoodColor;
					case (SchemeType.Bad): return BadColor;

					default: return UnknownColor;
				}
			}
		}

		public Color GoodColor;
		public Color BadColor;
		public Color UnknownColor;
	}
}
