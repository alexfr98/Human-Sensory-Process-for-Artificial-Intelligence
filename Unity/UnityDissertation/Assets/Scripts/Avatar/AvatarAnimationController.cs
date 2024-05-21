using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Avatar))]
public class AvatarAnimationController : MonoBehaviour
{
    // Reference to the Animator component
    private Animator anim;

    /// <summary>
    /// Start is called before the first frame update.
    /// Initializes the Animator component.
    /// </summary>
    void Start()
    {
        anim = this.GetComponent<Animator>();
    }

    /// <summary>
    /// Plays the idle animation by gradually setting the Speed parameter to 0.
    /// </summary>
    /// <returns>IEnumerator for coroutine.</returns>
    public IEnumerator PlayWholeIdleAnimation()
    {
        while (anim.GetFloat("Speed") > 0.01f)
        {
            anim.SetFloat("Speed", 0.0f, 0.01f, Time.deltaTime);
            yield return null;
        }
    }

    /// <summary>
    /// Plays the dismiss animation by gradually setting the Speed parameter to 2.
    /// </summary>
    /// <returns>IEnumerator for coroutine.</returns>
    public IEnumerator PlayWholeDismissAnimation()
    {
        while (anim.GetFloat("Speed") < 1.99f)
        {
            anim.SetFloat("Speed", 2.0f, 0.01f, Time.deltaTime);
            yield return null;
        }

        // Wait for the animation to complete
        yield return new WaitForSeconds(1.5f);

        StartCoroutine(PlayWholeIdleAnimation());
    }

    /// <summary>
    /// Plays the picking up animation by gradually setting the Speed parameter to 1.
    /// </summary>
    /// <returns>IEnumerator for coroutine.</returns>
    public IEnumerator PlayWholePickingUpAnimation()
    {
        while (anim.GetFloat("Speed") < 1.0f || anim.GetFloat("Speed") > 1.0f)
        {
            anim.SetFloat("Speed", 1.0f, 0.01f, Time.deltaTime);
            yield return null;
        }

        // Wait for the animation to complete
        yield return new WaitForSeconds(1.5f);

        StartCoroutine(PlayWholeIdleAnimation());
    }

    /// <summary>
    /// Sets the walking animation by setting the Speed parameter to 0.5.
    /// </summary>
    public void SetWalkingAnimation()
    {
        anim.SetFloat("Speed", 0.5f, 0.01f, Time.deltaTime);
    }
}
