using UnityEngine;
using System.Collections;
using System.Collections.Generic;


// could be useful, but not in this moment
public class PlayerManager : MonoBehaviour, IGameManager {

    /*
        Manager that takes into account all the stats related to the player
        possible examples: health, number of kills (?), player speed (normal or slow for stealth)
    */

    public ManagerStatus status {get; private set;}

    public void Startup() {
        Debug.Log("Player manager starting ...");
        status = ManagerStatus.Started;
    }
}
