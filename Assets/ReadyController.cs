using UnitMan.Source.Management;
using UnityEngine;

namespace UnitMan
{
    public class ReadyController : MonoBehaviour
    {
	    private void OnEnable()
	    {
		    SessionManagerSingle.OnReset += DisableLabel;
	    }

	    private void DisableLabel()
	    {
		    gameObject.SetActive(false);
	    }

	    private void OnDisable()
	    {
		    SessionManagerSingle.OnReset -= DisableLabel;
	    }
    }
    }

