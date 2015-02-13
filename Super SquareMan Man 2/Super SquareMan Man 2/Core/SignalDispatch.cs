using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace SSMM2.Core
{
	public class SignalDispatcher
	{
		private Hashtable m_SignalBindings = new Hashtable();

		public static SignalDispatcher Instance;

		public delegate object OnSignalCallback(object data);

		/// <summary>
		/// Maintains data regarding the signal and location of the listener within the dispatcher.
		/// </summary>
		public struct SignalCallbackReference
		{
			internal String m_Key;
			internal LinkedListNode<OnSignalCallback> m_Node;

			internal SignalCallbackReference (String key, LinkedListNode <OnSignalCallback> node)
			{
				m_Key = key;
				m_Node = node;
			}
		}

		/// <summary>
		/// Attempt to remove human error in signal naming (i.e. signAl vs signal) by converting
		/// to all lowercase and removing all spacing when adding/checking a signal/callback.
		/// </summary>
		/// <param name="signalName"></param>
		/// <returns></returns>
		private String TranslateSignalName(String signalName)
		{
			signalName = signalName.ToLower();
			signalName = signalName.Replace (" ", "");

			return signalName;
		}

		public List<String> AllSignalNames
		{
			get
			{
				List<String> results = new List<String>();
				foreach (String key in m_SignalBindings.Keys)
				{
					results.Add(key);
				}

				return results;
			}
		}

		/// <summary>
		/// Adds a listener to the given signal with the given callback.
		/// </summary>
		/// <param name="signalName">Name of the signal to listen for.</param>
		/// <param name="callback">Callback to be used upon the signal's firing.</param>
		/// <returns>A SignalCallbackReference object to be used for removing the listener from the SignalDispatcher object.</returns>
		public SignalCallbackReference AddListener(String signalName, OnSignalCallback callback)
		{
			signalName = TranslateSignalName(signalName);

			if (!m_SignalBindings.ContainsKey(signalName))
				this.AddSignal(signalName);

			SignalCallbackReference reference = new SignalCallbackReference(
				signalName,
				((LinkedList<OnSignalCallback>)m_SignalBindings[signalName]).AddLast(callback)
				);

			return reference;
		}

		/// <summary>
		/// Registers the given signal with the dispatcher.
		/// </summary>
		/// <param name="signalName">Name to be used to reference the signal.</param>
		public void AddSignal(String signalName)
		{
			m_SignalBindings.Add(TranslateSignalName(signalName), new LinkedList<OnSignalCallback>());
		}

		/// <summary>
		/// Fires all callbacks associated with the given signal.
		/// </summary>
		/// <param name="signalName">Signal to be fired.</param>
		/// <param name="data">Data to be passed to the listeners of the signal. Can be null.</param>
		public object[] FireSignal(String signalName, object data)
		{
			signalName = TranslateSignalName(signalName);

			if (!m_SignalBindings.ContainsKey(signalName))
				return null;

			LinkedList<OnSignalCallback> callbacks = (LinkedList<OnSignalCallback>)m_SignalBindings[signalName];

			object[] results = new object[callbacks.Count];

			for (int i = 0; i < callbacks.Count; i++)
			{
				results[i] = callbacks.ElementAt(i).Invoke(data);
			}

			return results;
		}

		/// <summary>
		/// Removes the given callback from the SignalDispatcher.
		/// </summary>
		/// <param name="callbackReference">SignalCallbackReference object returned by a call to SignalDispatcher::AddListener().</param>
		public void RemoveListener(SignalCallbackReference callbackReference)
		{
			if (!m_SignalBindings.ContainsKey(callbackReference.m_Key))
				throw new Exception("Invalid listener; could not find a signal entry that matches the SignalCallbackReference.");

			((LinkedList<OnSignalCallback>)m_SignalBindings[callbackReference.m_Key]).Remove(callbackReference.m_Node);
		}
	}
}
