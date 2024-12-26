using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 0.5f;
    
    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(speed, 0, 0) * Time.deltaTime;

        if(transform.position.x > Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x) dead();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        Enemy enemy = other.GetComponent<Enemy>();
        if(enemy != null) {
            enemy.dead();
            GameObject.FindObjectOfType<Score>().GetComponent<Score>().addScore(150);
            dead();    
        }
    }

    public void dead() {
        Destroy(gameObject);
    }
}
