using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSMM2.Core
{
	public class DataStore
	{
		private Hashtable m_ObjectMappings = new Hashtable ();

		public static DataStore Context = new DataStore();

		public object this[String dataName]
		{
			get
			{
				dataName = dataName.ToLower();

				if (!m_ObjectMappings.ContainsKey(dataName))
					return null;

				return m_ObjectMappings[dataName];
			}

			set
			{
				m_ObjectMappings[dataName.ToLower()] = value;
			}
		}
	}
}
