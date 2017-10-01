using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CameraSwitcher))]
public class Menu : MonoBehaviour {

    CameraSwitcher _cameraSwitcher;

	void Start () {
        _cameraSwitcher = GetComponent<CameraSwitcher>();
	}

    private void OnGUI()
    {
        _cameraSwitcher.Menu();
    }
}
