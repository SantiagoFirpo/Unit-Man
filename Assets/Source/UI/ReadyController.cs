using UnitMan.Source.Management;
using UnitMan.Source.Management.Session;
using UnitMan.Source.Utilities.ObserverSystem;
using UnityEngine;

namespace UnitMan.Source.UI
{
    public class ReadyController : MonoBehaviour
    {

	    private Observer _resetObserver;
	    private void OnEnable()
	    {
		    _resetObserver = new Observer(DisableLabel);
		    SessionManagerSingle.Instance.resetEmitter.Attach(_resetObserver);
	    }

	    private void DisableLabel()
	    {
		    gameObject.SetActive(false);
	    }

	    private void OnDisable()
	    {
		    SessionManagerSingle.Instance.resetEmitter.Detach(_resetObserver);
	    }
    }
    }

