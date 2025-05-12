using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Managers : MonoBehaviour {

    /*
        General manager module that has to control the lifecycle and keep track of all the other managers
        All the scripts in the game that need to access a specific manager will request it to this module
    */

    private List<IGameManager> _startSequence;

    void Awake() {
        

        _startSequence = new List<IGameManager>
        {
            
        };

        StartCoroutine(StartupManagers());
    }

    private IEnumerator StartupManagers() {
        foreach (IGameManager manager in _startSequence) {
            manager.Startup();
        }

        yield return null;

        int numModules = _startSequence.Count;
        int numReady = 0;

        while (numReady < numModules) {
            int lastReady = numReady;
            numReady = 0;

            foreach (IGameManager manager in _startSequence) {
                if (manager.status == ManagerStatus.Started) {
                    numReady++;
                }
            }
            if (numReady > lastReady) {
                Debug.Log("Progress: " + numReady + "/" + numModules);
            }
            yield return null;
        }
        Debug.Log("All managers started up");
    }
}