using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSMM2.Core
{
	/// <summary>
	/// All classes that implement IManuallyManaged are guaranteed two functions that will load and unload
	/// the object and its content immediately upon request.
	/// </summary>
	public abstract class ManuallyManaged
	{
		public abstract void Destroy();
		public abstract void LoadContent();
	}
}
