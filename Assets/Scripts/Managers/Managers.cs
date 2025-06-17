using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(PlayerManager))]
[RequireComponent(typeof(InventoryManager))]
public class Managers : MonoBehaviour {

    /*
        General manager module that has to control the lifecyrcle and keep track of all the other managers
        All the scripts in the game that need to access a specific manager will request it to this module
    */

    public static PlayerManager Player {get; private set;}
    public static InventoryManager Inventory {get; private set;}

    public static InventoryUIManager InventoryUI {get; private set;}

    private List<IGameManager> _startSequence;

    void Awake() {
        Player = GetComponent<PlayerManager>();
        Inventory = GetComponent<InventoryManager>();
        InventoryUI = GetComponent<InventoryUIManager>();

        _startSequence = new List<IGameManager>
        {
            Player,
            Inventory,
            InventoryUI
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
