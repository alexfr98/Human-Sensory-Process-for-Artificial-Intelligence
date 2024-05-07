using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InputsDetector : MonoBehaviour
{
    public static InputsDetector Instance { get; private set; }

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
    

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    // Update is called once per frame
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
            Debug.Log(" e has been pressed");
            EHasBeenPressed?.Invoke();

        }

        else if (Input.GetKeyDown("space"))
        {
            SpaceHasBeenPressed?.Invoke();

        }
    }
}
