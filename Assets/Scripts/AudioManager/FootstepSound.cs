using UnityEngine;

public class FootstepSound : MonoBehaviour
{

    private Animator animator;

    private float prevFootstep;


    [SerializeField]
    private AudioPlayer audioPlayer;

    

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
            audioPlayer.PlayAudio();
        }


        this.prevFootstep = footstep;
    }
}
