using System.Collections;
using UnityEngine;

public class RepeatSound : MonoBehaviour
{

    public AudioClip audioClip;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(this.LoopSound());
    }


    IEnumerator LoopSound()
    {
        while (true)
        {
            Managers.audioManager.PlayClipAtPoint(this.audioClip, this.transform.position, 1.0f, AudioType.SoundFx);
            yield return new WaitForSeconds(1.0f);
        }
    }
}
