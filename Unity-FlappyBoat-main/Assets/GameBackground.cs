using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBackground : MonoBehaviour
{
    // Start is called before the first frame update
    public float moveSpeed;
    private Vector3 positionChange;
    private float removePositionX;
    private float addPositionX;
    SpriteRenderer spriteRenderer;
    private bool isCloned = false;

    void Start()
    {
        positionChange = new Vector3(-moveSpeed, 0, 0);
        spriteRenderer = GetComponent<SpriteRenderer>();
        removePositionX = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).x - spriteRenderer.bounds.size.x;
        addPositionX = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width/2, 0, 0)).x;
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += positionChange * Time.deltaTime;
        
        if(transform.position.x < addPositionX  && !isCloned) {
            Instantiate(gameObject, transform.position + new Vector3(spriteRenderer.bounds.size.x, 0, 0), transform.rotation);
            isCloned = true;
        }

        if(transform.position.x < removePositionX) {
            Destroy(gameObject);
        }       
    }
}
