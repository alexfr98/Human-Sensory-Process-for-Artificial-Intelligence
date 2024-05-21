using System.Collections;
using UnityEngine;

/// <summary>
/// Handles the manual movement and rotation of the Avatar, including interactions such as eating.
/// </summary>
public class AvatarManualMovement : MonoBehaviour
{
    // Indicates whether the Avatar is currently moving.
    private bool isMoving = false;

    // Reference to the Avatar's animation controller.
    private AvatarAnimationController animationController;

    // Reference to the Avatar's senses.
    private IAvatarSenses avatarSenses;

    /// <summary>
    /// Initializes the manual movement by setting up input detection and references.
    /// </summary>
    /// <param name="avatarSenses">The Avatar's senses.</param>
    /// <param name="animationController">The Avatar's animation controller.</param>
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

    // Moves the Avatar to the right.
    private void MoveRight()
    {
        Vector3 direction = new Vector3(1.0f, 0.0f, 0.0f);
        ExecuteMovement(direction);
    }

    // Moves the Avatar downwards.
    private void MoveDown()
    {
        Vector3 direction = new Vector3(0.0f, 0.0f, -1.0f);
        ExecuteMovement(direction);
    }

    // Moves the Avatar to the left.
    private void MoveLeft()
    {
        Vector3 direction = new Vector3(-1.0f, 0.0f, 0.0f);
        ExecuteMovement(direction);
    }

    // Moves the Avatar upwards.
    private void MoveUp()
    {
        Vector3 direction = new Vector3(0.0f, 0.0f, 1.0f);
        ExecuteMovement(direction);
    }

    // Rotates the Avatar to face upwards.
    private void RotateUp()
    {
        ChangeForward(new Vector3(0.0f, 0.0f, 1.0f));
    }

    // Rotates the Avatar to face downwards.
    private void RotateDown()
    {
        ChangeForward(new Vector3(0.0f, 0.0f, -1.0f));
    }

    // Rotates the Avatar to face left.
    private void RotateLeft()
    {
        ChangeForward(new Vector3(-1.0f, 0.0f, 0.0f));
    }

    // Rotates the Avatar to face right.
    private void RotateRight()
    {
        ChangeForward(new Vector3(1.0f, 0.0f, 0.0f));
    }

    /// <summary>
    /// Makes the Avatar attempt to eat an object in front of it.
    /// </summary>
    private void Eat()
    {
        Debug.Log(" eat been tried");
        if (avatarSenses.SightList.Count > 0)
        {
            int counter = 0;
            bool found = false;
            while (counter < avatarSenses.SightList.Count && !found)
            {
                IWorldObject worldObject = avatarSenses.SightList[counter];
                if (new Vector2(worldObject.gameObject.transform.position.x, worldObject.gameObject.transform.position.z) ==
                    new Vector2(this.transform.position.x, this.transform.position.z) + new Vector2(this.transform.forward.x, this.transform.forward.z))
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

    /// <summary>
    /// Executes movement in the specified direction if the path is not blocked.
    /// </summary>
    /// <param name="direction">The direction to move.</param>
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

    /// <summary>
    /// Rotates the Avatar towards the target direction.
    /// </summary>
    /// <param name="direction">The direction to rotate towards.</param>
    private IEnumerator RotateTowardsTarget(Vector3 direction)
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
        StartCoroutine(animationController.PlayWholeIdleAnimation());
    }

    /// <summary>
    /// Checks if the Avatar is facing the target direction.
    /// </summary>
    /// <param name="targetRotation">The target rotation.</param>
    /// <returns>True if the Avatar is facing the target direction; otherwise, false.</returns>
    private bool IsFacingTarget(Quaternion targetRotation)
    {
        // Determine if the object is facing the target direction within a small threshold
        float angleToTarget = Quaternion.Angle(transform.rotation, targetRotation);
        return angleToTarget < 1.0f; // Considered "facing" if within 1 degree of the target, adjust threshold as needed
    }

    /// <summary>
    /// Moves and rotates the Avatar towards the target position.
    /// </summary>
    /// <param name="direction">The direction to move and rotate towards.</param>
    private IEnumerator MoveAndRotateTowardsTarget(Vector3 direction)
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
        StartCoroutine(animationController.PlayWholeIdleAnimation());
    }

    /// <summary>
    /// Checks if the path in the specified direction is blocked.
    /// </summary>
    /// <param name="offset">The direction to check.</param>
    /// <returns>True if the path is blocked; otherwise, false.</returns>
    private bool PathIsBlocked(Vector3 offset)
    {
        Vector3 center = this.transform.position + offset; // Center of the sphere, adjust with offset if needed
        float radius = 0.3f; // Radius of the sphere
        LayerMask layerMask = LayerMask.GetMask("ObstacleLayer"); // Only check against objects in "ObstacleLayer"
        bool isBlocked = Physics.CheckSphere(center, radius, layerMask, QueryTriggerInteraction.Ignore);
        return isBlocked;
    }

    /// <summary>
    /// Changes the forward direction of the Avatar.
    /// </summary>
    /// <param name="forward">The new forward direction.</param>
    private void ChangeForward(Vector3 forward)
    {
        this.transform.forward = forward;
    }

    /// <summary>
    /// Rotates the Avatar to face the target position while moving.
    /// </summary>
    /// <param name="targetPosition">The position to rotate towards.</param>
    /// <param name="speed">The speed of rotation.</param>
    private void RotateToObject(Vector3 targetPosition, float speed)
    {
        this.transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(this.transform.forward, targetPosition - this.transform.position, speed * Time.deltaTime, 0.0f));
    }

    /// <summary>
    /// Moves the Avatar towards the target position.
    /// </summary>
    /// <param name="targetPosition">The position to move towards.</param>
    /// <param name="speed">The speed of movement.</param>
    private void MoveToObject(Vector3 targetPosition, float speed)
    {
        this.transform.position = Vector3.MoveTowards(this.transform.position, targetPosition, speed * Time.deltaTime);
    }
}


