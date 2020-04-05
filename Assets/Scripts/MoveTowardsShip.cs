using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTowardsShip : MonoBehaviour
{
    public static MoveTowardsShip moveTowardsShipInstance;

    public Coroutine planet;
    private bool isMoving = true;
    private Vector3 end;

    public void Awake()
    {
        if (moveTowardsShipInstance == null) { moveTowardsShipInstance = this; }
        end = new Vector3(955.0f, 485.0f, 4000.0f);
    }

    public void StartPlanetMovement()
    {
        planet = StartCoroutine(MovePlanet());
    }

    IEnumerator MovePlanet()
    {
        float speed = 56.666666667f;

        while (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, end, Time.deltaTime * speed);

            float diff = Mathf.Abs(transform.position.sqrMagnitude - end.sqrMagnitude);

            if (diff < 5.0)
            {
                isMoving = false;
                yield break;
            }
            yield return null;
        }
        StopCoroutine(planet);
    }
}
