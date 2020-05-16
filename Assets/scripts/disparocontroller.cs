using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class disparocontroller : MonoBehaviour {

    public float bulletSpeed = 12f;
    public float bulletlife = 2f;
    public float bulletDamage = 1.90f;
    Rigidbody2D bullet;
    private Vector2 Mouse, Objetivo;
    private Vector3 positionpasada;

    private void Awake()
    {
        bullet = GetComponent<Rigidbody2D>();

        
    }

    // Use this for initialization
    void Start () {
        bullet.velocity = bullet.velocity = transform.right * bulletSpeed;
        positionpasada = transform.position;

    }
	
	// Update is called once per frame
	void Update () {

        bulletDamage += 0.30f;

        if (positionpasada != transform.position)
        {
            transform.right = transform.position - positionpasada;
            positionpasada = transform.position;
        }
    

        Destroy(gameObject, bulletlife);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Ground")
        {
            Destroy(gameObject, 0.3f);
            gameObject.GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 0f, 0f);
        }
        if (col.gameObject.tag == "enemy")
        {
            Destroy(gameObject, 0.3f);
            gameObject.GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 0f, 0f);
        }
    }
}
