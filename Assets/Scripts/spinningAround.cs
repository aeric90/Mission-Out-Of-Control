using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spinningAround : MonoBehaviour
{
    public GameObject aroundObject;
    public float speed;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        spinAround();
    }

    void spinAround() //this will make moon spin around Earth
    {
        transform.RotateAround(aroundObject.transform.position, Vector3.up, speed * Time.deltaTime);
    }
}
