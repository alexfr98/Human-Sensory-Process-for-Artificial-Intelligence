using System;
using UnityEngine;

/// <summary>
/// Singleton class to detect user input and invoke corresponding actions.
/// </summary>
public class InputsDetector : MonoBehaviour
{
    // Singleton instance
    public static InputsDetector Instance { get; private set; }

    // Actions to be invoked on specific key presses
    public Action LHasBeenPressed;
    public Action JHasBeenPressed;
    public Action KHasBeenPressed;
    public Action IHasBeenPressed;
    public Action WHasBeenPressed;
    public Action AHasBeenPressed;
    public Action SHasBeenPressed;
    public Action DHasBeenPressed;
    public Action EHasBeenPressed;
    public Action SpaceHasBeenPressed;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// Ensures that there is only one instance of InputsDetector.
    /// </summary>
    private void Awake()
    {
        // If an instance already exists and it's not this one, destroy this object.
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    /// <summary>
    /// Update is called once per frame.
    /// Checks for specific key presses and invokes corresponding actions if assigned.
    /// </summary>
    void Update()
    {
        if (Input.GetKeyDown("i"))
        {
            IHasBeenPressed?.Invoke();
        }
        else if (Input.GetKeyDown("j"))
        {
            JHasBeenPressed?.Invoke();
        }
        else if (Input.GetKeyDown("k"))
        {
            KHasBeenPressed?.Invoke();
        }
        else if (Input.GetKeyDown("l"))
        {
            LHasBeenPressed?.Invoke();
        }
        else if (Input.GetKeyDown("w"))
        {
            WHasBeenPressed?.Invoke();
        }
        else if (Input.GetKeyDown("a"))
        {
            AHasBeenPressed?.Invoke();
        }
        else if (Input.GetKeyDown("s"))
        {
            SHasBeenPressed?.Invoke();
        }
        else if (Input.GetKeyDown("d"))
        {
            DHasBeenPressed?.Invoke();
        }
        else if (Input.GetKeyDown("e"))
        {
            EHasBeenPressed?.Invoke();
        }
        else if (Input.GetKeyDown("space"))
        {
            SpaceHasBeenPressed?.Invoke();
        }
    }
}
