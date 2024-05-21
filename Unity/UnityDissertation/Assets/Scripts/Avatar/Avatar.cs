using System;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Represents the Avatar in the game, handling its senses, drives, and movement.
/// </summary>
public class Avatar : MonoBehaviour
{
    // Handles the senses of the avatar (touch, hearing, sight)
    private IAvatarSenses avatarSenses;
    public IAvatarSenses Senses { get { return avatarSenses; } }

    // Handles the drives (motivations or needs) of the Avatar.
    private IAvatarDrives avatarDrives;
    public IAvatarDrives Drives { get { return avatarDrives; } }

    // Handles manual movement controls for the Avatar.
    private AvatarManualMovement avatarManualMovement;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// Initializes the avatar's drives, senses, and manual movement components.
    /// </summary>
    void Awake()
    {
        avatarDrives = new AvatarDrives();
        avatarSenses = this.AddComponent<AvatarSenses>();
        avatarManualMovement = this.AddComponent<AvatarManualMovement>();
    }

    /// <summary>
    /// Start is called before the first frame update.
    /// Initializes the avatar's manual movement with its senses and animation controller.
    /// </summary>
    private void Start()
    {
        avatarManualMovement.Initialize(avatarSenses, this.AddComponent<AvatarAnimationController>());
    }

    /// <summary>
    /// Changes the forward direction of the Avatar.
    /// </summary>
    /// <param name="forward">The new forward direction as a Vector3.</param>
    public void ChangeForward(Vector3 forward)
    {
        this.transform.forward = forward;
    }

    /// <summary>
    /// Rotates the Avatar to face a target position while moving towards it.
    /// </summary>
    /// <param name="targetPosition">The position to rotate towards.</param>
    /// <param name="speed">The speed of rotation.</param>
    public void RotateToObject(Vector3 targetPosition, float speed)
    {
        this.transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(this.transform.forward, targetPosition - this.transform.position, speed * Time.deltaTime, 0.0f));
    }

    /// <summary>
    /// Moves the Avatar towards a target position.
    /// </summary>
    /// <param name="targetPosition">The position to move towards.</param>
    /// <param name="speed">The speed of movement.</param>
    public void MoveToObject(Vector3 targetPosition, float speed)
    {
        this.transform.position = Vector3.MoveTowards(this.transform.position, targetPosition, speed * Time.deltaTime);
    }

    /// <summary>
    /// Updates the Avatar's drives based on given inputs.
    /// </summary>
    /// <param name="tuple">Tuple containing boolean and two float values. If the action was modifiable, hunger effect, damage effect</param>
    /// <param name="temperature">Current temperature.</param>
    /// <param name="glare">Current glare level.</param>
    /// <param name="sound">Current sound level.</param>
    public void ActualizeDrives(Tuple<bool, float, float> tuple, float temperature, float glare, float sound)
    {
        Drives.ActualizeDrives(tuple, temperature, glare, sound);
    }

    /// <summary>
    /// Called when the Collider other enters the trigger.
    /// Updates the senses by adding the Collider.
    /// </summary>
    /// <param name="other">The Collider that entered the trigger.</param>
    private void OnTriggerEnter(Collider other)
    {
        Senses.ActualizeSensesListAdding(other);
    }

    /// <summary>
    /// Called when the Collider other exits the trigger.
    /// Updates the senses by removing the Collider.
    /// </summary>
    /// <param name="other">The Collider that exited the trigger.</param>
    private void OnTriggerExit(Collider other)
    {
        Senses.ActualizeSensesListRemoving(other);
    }
}
