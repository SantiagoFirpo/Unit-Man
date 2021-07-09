using System;

namespace UnitMan.Source.Utilities.ObserverSystem
{
	public class Observer
	{
		private readonly Action _onNotified;
		public Observer(Action onNotified)
		{
			_onNotified = onNotified;
		}
		public void OnNotified(Emitter source)
		{
			_onNotified.Invoke();
		}
	}
	public class Observer<TDataContainer>
	{
		private readonly Action<Emitter<TDataContainer>, TDataContainer> _onNotified;

		public Observer(Action<Emitter<TDataContainer>, TDataContainer> onNotified)
		{
			_onNotified = onNotified;
		}

		public void OnNotified(Emitter<TDataContainer> source, TDataContainer dataContainer)
		{
			_onNotified.Invoke(source, dataContainer);
		}
	}
	
}