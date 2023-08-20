using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookToCamera : MonoBehaviour
{
    void Start()
    {
        transform.rotation = Camera.main.transform.rotation;
    }
}
