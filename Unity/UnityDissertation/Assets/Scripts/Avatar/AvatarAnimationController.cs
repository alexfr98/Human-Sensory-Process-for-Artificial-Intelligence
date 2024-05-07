using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Avatar))]
public class AvatarAnimationController : MonoBehaviour
{

    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = this.GetComponent<Animator>();
    }

    public IEnumerator PlayWholeIdlenimation()
    {
        while (anim.GetFloat("Speed") > 0.01f)
        {
            anim.SetFloat("Speed", 0.0f, 0.01f, Time.deltaTime);
            yield return null;
        }
    }


    public IEnumerator PlayWholeDismissAnimation()
    {
        while (anim.GetFloat("Speed") < 1.99)
        {
            anim.SetFloat("Speed", 2.0f, 0.01f, Time.deltaTime);
            yield return null;
        }

        // Wait for the animation to complete
        // Assuming the animation length is known, for example, 2 seconds
        yield return new WaitForSeconds(1.5f);

        StartCoroutine(PlayWholeIdlenimation());
    }

    public IEnumerator PlayWholePickingUpAnimation()
    {
        while (anim.GetFloat("Speed") < 1.0 || anim.GetFloat("Speed") > 1.0)
        {
            anim.SetFloat("Speed", 1.0f, 0.01f, Time.deltaTime);
            yield return null;
        }

        // Wait for the animation to complete
        // Assuming the animation length is known, for example, 2 seconds
        yield return new WaitForSeconds(1.5f);

        StartCoroutine(PlayWholeIdlenimation());
    }

    public void SetWalkingAnimation()
    {
        anim.SetFloat("Speed", 0.5f, 0.01f, Time.deltaTime);
    }

}
