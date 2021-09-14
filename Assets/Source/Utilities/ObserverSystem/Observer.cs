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
		public void OnNotified()
		{
			_onNotified.Invoke();
		}
	}
	public class Observer<TDataContainer>
	{
		private readonly Action<TDataContainer> _onNotified;

		public Observer(Action< TDataContainer> onNotified)
		{
			_onNotified = onNotified;
		}

		public void OnNotified(TDataContainer dataContainer)
		{
			_onNotified.Invoke(dataContainer);
		}
	}
	
}