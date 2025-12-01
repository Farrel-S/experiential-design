using UnityEngine;

public class LandingScript : MonoBehaviour
{
    public Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator.Play("Rotate");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
