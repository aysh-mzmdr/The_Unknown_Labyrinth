using UnityEngine;

public class WalkingAudioSync : StateMachineBehaviour
{
    public AudioClip walkingClip;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AudioSource src = animator.GetComponentInParent<AudioSource>();
        if (src == null || walkingClip == null) return;
        src.clip = walkingClip;
        src.loop = true;
        src.Play();
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AudioSource src = animator.GetComponentInParent<AudioSource>();
        if (src == null) return;
        src.Stop();
    }
}
