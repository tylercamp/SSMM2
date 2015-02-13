using Microsoft.VisualStudio.TestTools.UnitTesting;
using SSMM2.Core;
using System.Collections.Generic;

namespace SSMM_Tests
{
	[TestClass]
	public class RefreshedRandomTest
	{
		[TestMethod]
		public void RefreshedRandomGeneratesXUnique()
		{
			RefreshedRandom random = new RefreshedRandom();

			int targetUniqueItems = 10000000;
			var generatedRandoms = new SortedSet<long>();

			for (int i = 0; i < targetUniqueItems; i++)
			{
				var currentRandom = random.NextLong();

				if (generatedRandoms.Contains(currentRandom))
					Assert.Fail("Duplicate after index " + i);

				generatedRandoms.Add(currentRandom);
			}
		}
	}
}
