using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class AvatarManualMovement : MonoBehaviour
{
    private bool isMoving = false;
    private AvatarAnimationController animationController;
    private IAvatarSenses avatarSenses;
    public void Initialize(IAvatarSenses avatarSenses, AvatarAnimationController animationController)
    {
        InputsDetector.Instance.JHasBeenPressed += RotateLeft;
        InputsDetector.Instance.KHasBeenPressed += RotateDown;
        InputsDetector.Instance.LHasBeenPressed += RotateRight;
        InputsDetector.Instance.IHasBeenPressed += RotateUp;

        InputsDetector.Instance.WHasBeenPressed += MoveUp;
        InputsDetector.Instance.AHasBeenPressed += MoveLeft;
        InputsDetector.Instance.SHasBeenPressed += MoveDown;
        InputsDetector.Instance.DHasBeenPressed += MoveRight;

        InputsDetector.Instance.EHasBeenPressed += Eat;
        
        this.animationController = animationController;
        this.avatarSenses = avatarSenses;
    }


    private void MoveRight()
    {
        Vector3 direction = new Vector3(1.0f, 0.0f, 0.0f);
        ExecuteMovement(direction);
    }

    private void MoveDown()
    {
        Vector3 direction = new Vector3(0.0f, 0.0f, -1.0f);
        ExecuteMovement(direction);
    }

    private void MoveLeft()
    {
        Vector3 direction = new Vector3(-1.0f, 0.0f, 0.0f);
        ExecuteMovement(direction);
    }

    private void MoveUp()
    {
        Vector3 direction = new Vector3(0.0f, 0.0f, 1.0f);
        ExecuteMovement(direction);
    }

    private void RotateUp()
    {
        ChangeForward(new Vector3(0.0f, 0.0f, 1.0f));
    }
    private void RotateDown()
    {
        ChangeForward(new Vector3(0.0f, 0.0f, -1.0f));
    }
    private void RotateLeft()
    {
        ChangeForward(new Vector3(-1.0f, 0.0f, 0.0f));
    }
    private void RotateRight()
    {
        ChangeForward(new Vector3(1.0f, 0.0f, 0.0f));
    }

    private void Eat()
    {
        Debug.Log(" eat been tried");
        if (avatarSenses.SightList.Count > 0)
        {
            int counter = 0;
            bool found = false;
            while(counter < avatarSenses.SightList.Count && !found)
            {
                IWorldObject worldObject = avatarSenses.SightList[counter];
                // Maybe would be better to have always a variable saving what we have in front. (Would it be realistc?)
                if(new Vector2(worldObject.gameObject.transform.position.x, worldObject.gameObject.transform.position.z) == new Vector2(this.transform.position.x, this.transform.position.z) + new Vector2(this.transform.forward.x, this.transform.forward.z))
                {
                    found = true;
                    string objectType = avatarSenses.SightList[counter].ObjectType;
                    if (objectType == "Food")
                    {
                        WorldClass.Instance.ResetObject(worldObject);
                        Debug.Log("The avatar can eat. Index " + counter);
                        StartCoroutine(animationController.PlayWholePickingUpAnimation());
                    }
                    else
                    {
                        StartCoroutine(animationController.PlayWholeDismissAnimation());
                        Debug.LogError("The avatar can't eat. Object type is: " + objectType);
                    }
                }
                else
                {
                    counter++;
                }

            }
        }
        else
        {
            Debug.LogError("The avatar can't eat, nothing in his view. Remember to press the space bar to actualize the drives");
        }

        
    }


    private void ExecuteMovement(Vector3 direction)
    {
        if (!isMoving)
        {
            isMoving = true;
            if (!PathIsBlocked(direction))
            {
                
                StartCoroutine(MoveAndRotateTowardsTarget(direction));

            }
            else
            {
                StartCoroutine(RotateTowardsTarget(direction));
            }
        }

    }

    IEnumerator RotateTowardsTarget(Vector3 direction)
    {
        Quaternion targetRotation = Quaternion.LookRotation(direction); // Desired orientation
        isMoving = true;
        float rotationSpeed = 50.0f; // Speed at which the avatar rotates

        while (!IsFacingTarget(targetRotation))
        {
            // Rotate towards the target direction
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            animationController.SetWalkingAnimation();
            yield return null; // Wait for the next frame
        }
        this.transform.rotation = targetRotation;
        isMoving = false; // Movement finished
        StartCoroutine(animationController.PlayWholeIdlenimation());

    }

    private bool IsFacingTarget(Quaternion targetRotation)
    {
        // Determine if the object is facing the target direction within a small threshold
        float angleToTarget = Quaternion.Angle(transform.rotation, targetRotation);
        return angleToTarget < 1.0f; // Considered "facing" if within 1 degree of the target, adjust threshold as needed
    }

    IEnumerator MoveAndRotateTowardsTarget(Vector3 direction)
    {
        Vector3 targetPosition = this.transform.position + direction;
        isMoving = true;
        float speed = 1.0f; // Speed at which the avatar moves
        float rotationSpeed = 5.0f; // Speed at which the avatar rotates

        while (transform.position != targetPosition)
        {
            // Move towards the target position
            MoveToObject(targetPosition, speed);
            RotateToObject(targetPosition, rotationSpeed);
            animationController.SetWalkingAnimation();
            yield return null; // Wait for the next frame
        }
        isMoving = false; // Movement finished
        StartCoroutine(animationController.PlayWholeIdlenimation());
    }

    private bool PathIsBlocked(Vector3 offset)
    {
        Vector3 center = this.transform.position + offset; // Center of the sphere, adjust with offset if needed
        float radius = 0.3f; // Radius of the sphere
        LayerMask layerMask = LayerMask.GetMask("ObstacleLayer"); // Only check against objects in "ObstacleLayer"
        bool isBlocked = Physics.CheckSphere(center, radius, layerMask, QueryTriggerInteraction.Ignore);
        return isBlocked;
    }

    private void ChangeForward(Vector3 forward)
    {
        this.transform.forward = forward;
    }
    //Avatar is moving and rotate while moving
    private void RotateToObject(Vector3 targetPosition, float speed)
    {
        this.transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(this.transform.forward, targetPosition - this.transform.position, speed * Time.deltaTime, 0.0f));
    }
    private void MoveToObject(Vector3 targetPosition, float speed)
    {
        this.transform.position = Vector3.MoveTowards(this.transform.position, targetPosition, speed * Time.deltaTime);
    }

}
