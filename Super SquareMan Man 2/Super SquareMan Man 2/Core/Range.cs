using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SSMM2.Core
{
	public struct Range
	{
		public float Max;
		public float Min;

		public Range(float min, float max)
		{
			Min = min;
			Max = max;
		}

		public Range(float value)
		{
			Min = value;
			Max = value;
		}

		/// <summary>
		/// A random value within the bounds of [Min, Max].
		/// </summary>
		public float Any
		{
			get
			{
				return (float)Utility.Random.NextDouble() * (Max - Min) + Min;
			}
		}

		/// <summary>
		/// Shorthand for assigning both Max and Min to the same value.
		/// </summary>
		public float Value
		{
			set
			{
				Max = value;
				Min = value;
			}
		}
	}

	public struct Vector2Range
	{
		public Vector2 Max;
		public Vector2 Min;

		public Vector2Range(Vector2 min, Vector2 max)
		{
			Min = min;
			Max = max;
		}

		public Vector2Range(Vector2 value)
		{
			Min = value;
			Max = value;
		}

		/// <summary>
		/// A vector within the bounds of [Min, Max].
		/// </summary>
		public Vector2 Any
		{
			get
			{
				return new Vector2((float)Utility.Random.NextDouble() * (Max.X - Min.X) + Min.X, (float)Utility.Random.NextDouble() * (Max.Y - Min.Y) + Min.Y);
			}
		}

		/// <summary>
		/// Shorthand for assigning both Max and Min to the same value.
		/// </summary>
		public Vector2 Value
		{
			set
			{
				Max = value;
				Min = value;
			}
		}
	}
}
