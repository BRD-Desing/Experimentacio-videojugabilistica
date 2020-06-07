using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlasmaScript : MonoBehaviour {

    public float speed;
    public float lifeTime;

    private Vector2 Mouse, Objetivo;
    private Vector3 positionpasada;

    Rigidbody2D rb2d;

    private void Awake()
    {
        rb2d = gameObject.GetComponent<Rigidbody2D>();
    }

        // Use this for initialization
        void Start () {

        rb2d.velocity = rb2d.velocity = transform.right * speed;
        positionpasada = transform.position;

    }
	
	// Update is called once per frame
	void Update () {

        if (positionpasada != transform.position)
        {
            transform.right = transform.position - positionpasada;
            positionpasada = transform.position;
        }

        Destroy(gameObject, lifeTime);
    }
}
