namespace UnitMan.Source.Utilities.ObserverSystem
{
	public interface IObserver<TDataContainer>
	{
		public void OnObserved(IEmitter<TDataContainer> source, TDataContainer dataContainer);
	}
	
	public interface IObserver
	{
		public void OnObserved(IEmitter source);
	}
}