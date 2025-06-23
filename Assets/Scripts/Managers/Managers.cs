using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(PointerManager)), RequireComponent(typeof(TalkManager)),RequireComponent(typeof(PlayerManager)), RequireComponent(typeof(RequireComponent)), RequireComponent(typeof(GameManager)), RequireComponent(typeof(AudioManager))]
public class Managers : MonoBehaviour
{

    /*
        General manager module that has to control the lifecycle and keep track of all the other managers
        All the scripts in the game that need to access a specific manager will request it to this module
    */

    public static TalkManager Talk;
    public static PointerManager pointerManager;
    public static PlayerManager playerManager;

    public static AudioManager audioManager;
    public static InventoryManager Inventory;
    public static GameManager game;

    private List<IGameManager> _startSequence;
    
    void OnEnable()
    {

        pointerManager = this.GetComponent<PointerManager>();
        playerManager = this.GetComponent<PlayerManager>();
        audioManager = this.GetComponent<AudioManager>();
        Inventory = this.GetComponent<InventoryManager>();
        game = this.GetComponent<GameManager>();
        Talk = this.GetComponent<TalkManager>();


        _startSequence = new List<IGameManager>
        {
            pointerManager,
            playerManager,
            audioManager,
            Inventory,
            game,
            Talk
        };

        StopAllCoroutines();
        StartCoroutine(StartupManagers());
    }

     private IEnumerator StartupManagers()
    {
        foreach (IGameManager manager in _startSequence)
        {
            manager.Startup();
        }

        yield return null;

        int numModules = _startSequence.Count;
        int numReady = 0;

        while (numReady < numModules)
        {
            int lastReady = numReady;
            numReady = 0;

            foreach (IGameManager manager in _startSequence)
            {
                if (manager.status == ManagerStatus.Started)
                {
                    numReady++;
                }
            }
            if (numReady > lastReady)
            {
                Debug.Log("Progress: " + numReady + "/" + numModules);
            }
            yield return null;
        }
        Debug.Log("All managers started up");
    }
}