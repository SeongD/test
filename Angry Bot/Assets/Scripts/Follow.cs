using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public Transform target;
    public float distance;
    public float height;
    public float speed;

    private Vector3 pos;

    private void Update()
    {
        pos = new Vector3(
            target.position.x,
            height,
            target.position.z - distance);

        transform.position = Vector3.Lerp(
            transform.position,
            pos,
            speed * Time.deltaTime);
    }
}
