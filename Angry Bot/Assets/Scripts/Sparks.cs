using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sparks : MonoBehaviour
{
    private void Start()
    {
        Destroy(gameObject, 0.2f);
    }
}
