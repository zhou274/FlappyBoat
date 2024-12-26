using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject gameManager;
    private GameController gameController;
    public float accelerationY = 0.02f;
    public float jumpPower = 2.5f;
    public Transform shootPosition;
    public GameObject bulletPrefab;
    private bool isFalling = false;
    public bool isControlable = false;
    private bool isTutorialJump = true;
    private Vector3 initPosition;
    private Vector3 positionChange = new Vector3(0, 0, 0);

    // Start is called before the first frame update
    void Start()
    {
        gameController = gameManager.GetComponent<GameController>();
        initPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(isFalling) {
            positionChange.Set(positionChange.x, positionChange.y-accelerationY*Time.deltaTime, positionChange.z);
            transform.position += positionChange * Time.deltaTime;
        }

        if(isControlable) {
            if(Input.touchCount > 0) {
                if(Input.GetTouch(0).phase == TouchPhase.Began) {
                    jump();
                    shoot();
                    if(isTutorialJump) {
                        gameController.startSpawnEnemy(); 
                        isTutorialJump = false;
                        isFalling = true;
                    } 
                }                                       
            }
        }
        if(transform.position.y>0.9)
        {
            transform.position = new Vector3(transform.position.x, 0.9f, transform.position.z);
        }
        if(transform.position.y < -1) dead();
    }

    public void restart() {
        isFalling = false;
        isControlable = false;
        isTutorialJump = true;
        positionChange = new Vector3(0, 0, 0);
        transform.position = initPosition;
    }

    private void dead() {
        if(isControlable) {
            gameController.gameOver();
            isControlable = false;
            positionChange.Set(positionChange.x-0.5f, positionChange.y, positionChange.z);
        }    
    }

    private void jump() {
        positionChange.Set(positionChange.x, positionChange.y+jumpPower, positionChange.z);
    }

    private void shoot() {
        Instantiate(bulletPrefab, shootPosition.position, shootPosition.rotation);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        Enemy enemy = other.GetComponent<Enemy>();
        if(enemy != null) {
            dead();
        }
    }
}
