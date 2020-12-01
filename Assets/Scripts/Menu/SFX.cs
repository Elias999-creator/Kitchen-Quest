using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFX : MonoBehaviour
{
    // Start is called before the first frame update

    public AudioSource Click;

    public void PlayClick()
    {
        Click.Play();
    }
}
