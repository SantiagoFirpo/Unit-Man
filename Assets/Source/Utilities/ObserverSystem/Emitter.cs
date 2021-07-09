using System.Collections.Generic;

namespace UnitMan.Source.Utilities.ObserverSystem
{
	public class Emitter
	{
		private readonly List<Observer> _observers;
		
		public Emitter()
		{
			_observers = new List<Observer>();
		}
		public void Attach(Observer observer)
		{
			_observers.Add(observer);
		}

		public void EmitNotification()
		{
			foreach (Observer observer in _observers)//_observers seems to be altered in this foreach, copying it with ToList() solves
														//this but generates a lot of garbage
														//happens in PelletController and TimerManager
			{
				observer.OnNotified(this);
			}
		}

		public void Detach(Observer observer)
		{
			_observers.Remove(observer);
		}
		
		~Emitter()
		{
			foreach (Observer observer in _observers)
			{
				Detach(observer);
			}
		}
	}
	
	public class Emitter<TDataContainer>
	{
		private readonly List<Observer<TDataContainer>> _observers;
		public Emitter()
		{
			_observers = new List<Observer<TDataContainer>>();
		}

		public void Attach(Observer<TDataContainer> observer)
		{
			_observers.Add(observer);
		}
		
		public void EmitNotification(TDataContainer dataContainer)
		{
			foreach (Observer<TDataContainer> observer in _observers)
			{
				observer.OnNotified(this, dataContainer);
			}
		}

		public void Detach(Observer<TDataContainer> observer)
		{
			_observers.Remove(observer);
		}
		
		~Emitter()
		{
			foreach (Observer<TDataContainer> observer in _observers)
			{
				Detach(observer);
			}
		}
	}
}