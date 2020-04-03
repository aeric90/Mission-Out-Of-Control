using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moonSpinningAround : MonoBehaviour
{
    public GameObject earth;
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
        transform.RotateAround(earth.transform.position, Vector3.back, speed * Time.deltaTime);
    }
}
