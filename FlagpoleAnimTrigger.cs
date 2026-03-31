using UnityEngine;
using Photon.Pun; // Required for network syncing

public class FlagpoleTrigger : MonoBehaviourPun
{
    [Header("Animation Settings")]
    public string animationTriggerName = "GoalFinish"; // The name of the trigger in your Animator
    
    [Header("Detection")]
    public string targetTag = "Flagpole";

    // This detects 2D collisions (Standard for NSMBvs mods)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. Check if we hit the Flagpole
        if (collision.CompareTag(targetTag))
        {
            // 2. Only trigger if this is the LOCAL player (prevents everyone playing the animation)
            if (photonView.IsMine)
            {
                TriggerGoalAnimation();
            }
        }
    }

    private void TriggerGoalAnimation()
    {
        Animator anim = GetComponent<Animator>();
        
        if (anim != null)
        {
            // Reset potential overlapping states and play the win animation
            anim.SetTrigger(animationTriggerName);
            
            Debug.Log("Flagpole touched! Playing goal animation.");
        }
        
        // Optional: Disable player movement here so they don't run past the pole
        // var movement = GetComponent<PlayerController>(); 
        // if (movement != null) movement.enabled = false;
    }
}