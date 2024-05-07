using System;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Avatar : MonoBehaviour
{
    [NonSerialized]
    private IAvatarSenses avatarSenses;
    public IAvatarSenses Senses { get { return avatarSenses; } }
    private IAvatarDrives avatarDrives;
    public IAvatarDrives Drives { get { return avatarDrives; } }

    private AvatarManualMovement avatarManualMovement;
    // Start is called before the first frame update
    void Awake()
    {
        avatarDrives = new AvatarDrives();
        avatarSenses = this.AddComponent<AvatarSenses>();
        avatarManualMovement = this.AddComponent<AvatarManualMovement>();

    }

    private void Start()
    {
        avatarManualMovement.Initialize(avatarSenses, this.AddComponent<AvatarAnimationController>());
    }

    public void ChangeForward(Vector3 forward)
    {
        this.transform.forward = forward;
    }

    //Avatar is moving and rotate while moving
    public void RotateToObject(Vector3 targetPosition, float speed)
    {
        this.transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(this.transform.forward, targetPosition - this.transform.position, speed * Time.deltaTime, 0.0f));
    }
    public void MoveToObject(Vector3 targetPosition, float speed)
    {
        this.transform.position = Vector3.MoveTowards(this.transform.position, targetPosition, speed * Time.deltaTime);
    }

    public void ActualizeDrives(Tuple<bool, float, float> tuple, float temperature, float glare, float sound)
    {
        Drives.ActualizeDrives(tuple, temperature, glare, sound);
    }

    private void OnTriggerEnter(Collider other)
    {
        Senses.ActualizeSensesListAdding(other);
    }

    private void OnTriggerExit(Collider other)
    {
        Senses.ActualizeSensesListRemoving(other);
    }
}
