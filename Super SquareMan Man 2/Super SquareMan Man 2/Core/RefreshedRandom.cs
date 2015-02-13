using System;

namespace SSMM2.Core
{
	public class RefreshedRandom
	{
		Random m_Random;
		int m_CurrentGenerationChildren = 0;

		private void ResetRandom()
		{
			int seed = (int)DateTime.Now.Ticks;
			if (m_Random != null)
				seed -= m_Random.Next();
			Console.WriteLine("RefreshedRandom.ResetRandom [seed {0}] [currentGen {1}]", seed, m_CurrentGenerationChildren);

			m_Random = new Random(seed);
			m_CurrentGenerationChildren = 0;
		}

		private void CheckRandomBounds(int newChildrenCount)
		{
			m_CurrentGenerationChildren += newChildrenCount;

			//	The period of a Random.Next is almost 100,000 items (learned the hard way), I don't know
			//		the period when it's mixed with NextDouble and NextBytes. Just
			//		use 20000 as a safe number.
			if (m_CurrentGenerationChildren > 20000)
				ResetRandom();
		}

		public RefreshedRandom()
		{
			ResetRandom();
		}

		public int Next()
		{
			CheckRandomBounds(1);
			return m_Random.Next();
		}

		public int Next(int maxValue)
		{
			CheckRandomBounds(1);
			return m_Random.Next(maxValue);
		}

		public int Next(int minValue, int maxValue)
		{
			CheckRandomBounds(1);
			return m_Random.Next(minValue, maxValue);
		}

		public long NextLong()
		{
			long highPart = ((long)Next()) << (sizeof(int) * 8);
			long lowPart = (long)Next();
			return highPart | lowPart;
		}

		public double NextDouble()
		{
			CheckRandomBounds(1);
			return m_Random.NextDouble();
		}

		public void NextBytes(byte[] buffer)
		{
			CheckRandomBounds(buffer.Length);
			m_Random.NextBytes(buffer);
		}
	}
}
