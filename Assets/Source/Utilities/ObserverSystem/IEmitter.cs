using System.Collections.Generic;

namespace UnitMan.Source.Utilities.ObserverSystem
{
	public interface IEmitter<TDataContainer>
	{

		public void Attach(IObserver<TDataContainer> observer);
		
		public void EmitNotification(IEnumerable<ObserverSystem.IObserver<TDataContainer>> observers, TDataContainer dataContainer);

		public void Detach(ObserverSystem.IObserver<TDataContainer> observer);
	}
	
	public interface IEmitter
	{

		public void Attach(ObserverSystem.IObserver observer);
		
		public void EmitNotification(IEnumerable<ObserverSystem.IObserver> observers);

		public void Detach(ObserverSystem.IObserver observer);
	}
}