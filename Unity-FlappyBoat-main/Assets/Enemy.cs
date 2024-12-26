using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed;

    // Update is called once per frame
    void Update()
    {
        transform.position -= new Vector3(speed, 0, 0) * Time.deltaTime;
        if(transform.position.x < Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).x - 1f) dead();
    }

    public void dead() {
        Destroy(gameObject);
    }
}
