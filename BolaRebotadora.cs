using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BolaRebotadora : MonoBehaviour {

    public float speed;

    private bool rotado;
    public bool tocandoMuro;

    public float radio;

    private Vector2 Mouse, Objetivo;
    private Vector3 positionpasada;


    public LayerMask queEsT;
    public GameObject puntoCon;
    private Rigidbody2D rb2d;

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

        tocandoMuro = Physics2D.Raycast(puntoCon.transform.position, Vector2.right, radio, queEsT);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(gameObject.transform.position, new Vector2(gameObject.transform.position.x + radio, gameObject.transform.position.y));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "pared")
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x * -1, rb2d.velocity.y);
        }

        if (collision.gameObject.tag == "Ground")
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, rb2d.velocity.y * -1);
        }
    }
}
