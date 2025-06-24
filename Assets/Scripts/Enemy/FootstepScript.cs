using UnityEngine;

public class FootstepScript : MonoBehaviour
{

    private Animator animator;

    private float prevFootstep;

    [SerializeField]
    private AudioClip[] stepClips;

    [SerializeField]
    private float volume = 1.0f;

    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float footstep = this.animator.GetFloat("Footstep");

        if (Mathf.Abs(footstep) <= 0.001f)
            footstep = 0.0f;

        if ((footstep > 0.0f && prevFootstep <= 0.0f) || (footstep < 0.0f && prevFootstep >= 0.0f))
        {

            AudioClip step = stepClips[Random.Range(0, stepClips.Length)];
            Managers.audioManager.Play2DSound(step, audioType: AudioType.Sfx, volume: this.volume);
        }


        this.prevFootstep = footstep;
    }
}
