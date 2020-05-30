using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BolaRebotadora : MonoBehaviour {

    public float speed;
    public float tiempoDeVida;

    private bool rotado;
    private bool puedeCambiar = true;
    private bool revoto = false;
    private int elemento;

    public bool tocandoMuro;

    public float radio;

    private Vector2 Mouse, Objetivo;
    private Vector3 positionpasada;


    public LayerMask queEsPared;
    public LayerMask queEsPiso;
    private Animator anim;
    private Rigidbody2D rb2d;

    private ParticleSystem fuego;
    private ParticleSystem hielo;
    public GameObject contenedorF;
    public GameObject contenedorH;

    private void Awake()
    {
        rb2d = gameObject.GetComponent<Rigidbody2D>();
        anim = gameObject.GetComponent<Animator>();

        hielo = contenedorH.GetComponent<ParticleSystem>();
        fuego = contenedorF.GetComponent<ParticleSystem>();
    }

    // Use this for initialization
    void Start () {

        rb2d.velocity = rb2d.velocity = transform.right * speed;
        positionpasada = transform.position;

    }
	
	// Update is called once per frame
	void Update () {

        anim.SetFloat("ele", elemento);

        if (positionpasada != transform.position)
        {
            transform.right = transform.position - positionpasada;
            positionpasada = transform.position;
        }

        Destroy(gameObject, tiempoDeVida);
    }

    private bool tocandoPiso;
    private void FixedUpdate()
    {
        tocandoMuro = Physics2D.OverlapCircle(gameObject.transform.position, radio, queEsPared);
        tocandoPiso = Physics2D.OverlapCircle(gameObject.transform.position, radio, queEsPiso);

        if (revoto == true && puedeCambiar == true)
        {
            elemento = Random.Range(1, 4);
            revoto = false;
            puedeCambiar = false;
        }

        if (elemento == 1)
        {
            fuego.Play();
        }
        if (elemento == 2)
        {
            hielo.Play();
        }



        if (tocandoPiso == true && rb2d.velocity.x > rb2d.velocity.y)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, (rb2d.velocity.y * -1));
            revoto = true;
        }

        if (tocandoMuro == true && rb2d.velocity.y > rb2d.velocity.x)
        {
            rb2d.velocity = new Vector2((rb2d.velocity.x * -1), rb2d.velocity.y);
            revoto = true;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(gameObject.transform.position, new Vector2(gameObject.transform.position.x + radio, gameObject.transform.position.y));
    }
}
