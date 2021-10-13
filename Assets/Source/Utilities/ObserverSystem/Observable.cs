using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnitMan.Source.Utilities.ObserverSystem
{
	[Serializable]
	public class Observable
	{
		[SerializeField]
		private List<Observer> observers;
		
		public Observable()
		{
			observers = new List<Observer>();
		}
		public void Attach(Observer observer)
		{
			observers.Add(observer);
		}

		public void EmitNotification()
		{
			foreach (Observer observer in observers.ToArray())//observers seem to be altered in this foreach, copying it with ToList() solves
														//this but generates a lot of garbage
														//happens in PelletController and TimerManager
			{
				observer.OnNotified();
			}
		}

		public void Detach(Observer observer)
		{
			observers.Remove(observer);
		}
		
		~Observable()
		{
			foreach (Observer observer in observers.ToArray()) //intentionally copying collection to avoid unintended
																//modifications
			{
				Detach(observer);
			}
		}
	}
	[Serializable]

	
	public class Observable<TDataContainer>
	{
		[SerializeField]
		private List<Observer<TDataContainer>> observers;
		public Observable()
		{
			observers = new List<Observer<TDataContainer>>();
		}

		public void Attach(Observer<TDataContainer> observer)
		{
			observers.Add(observer);
		}
		
		public void EmitNotification(TDataContainer dataContainer)
		{
			foreach (Observer<TDataContainer> observer in observers)
			{
				observer.OnNotified(dataContainer);
			}
		}

		public void Detach(Observer<TDataContainer> observer)
		{
			observers.Remove(observer);
		}
		
		~Observable()
		{
			foreach (Observer<TDataContainer> observer in observers.ToArray())
			{
				Detach(observer);
			}
		}
	}
}