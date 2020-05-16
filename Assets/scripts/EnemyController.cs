using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {


    public float maxSpeed = 1f;
    public float direccion = 1f;

    public float enemySpeed;
    public float ristriccionDeVelocidad;
    public float rangeTounch;
    public float rangeToToachWall;

    private bool LeftTounch;
    private bool RightTouch;
    private bool comprovanteAereo;

    public LayerMask layer;
    public Transform wallComprove;
    public Transform leftTounch;
    public Transform rightTouch;
    private Rigidbody2D enemy;
    private Animator anim;

    private void Awake()
    {

    }

    // Use this for initialization
    void Start () {
        enemy = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
		
	}

    private void FixedUpdate()
    {
        //Movimiento del enemigo
        enemy.velocity = new Vector2(enemySpeed * direccion, enemy.velocity.y);
        //Comprovante de la direccion
        if (LeftTounch && comprovanteAereo)
        {
            direccion = 1;
        }else if (RightTouch && comprovanteAereo)
        {
            direccion = -1;
        }
        //Comprovante del salto del enemigo
        if (LeftTounch && !comprovanteAereo)
        {
            enemy.AddForce(Vector2.up * (enemySpeed - ristriccionDeVelocidad), ForceMode2D.Impulse);
        }
        else if (RightTouch && !comprovanteAereo)
        {
            enemy.AddForce(Vector2.up * (enemySpeed - ristriccionDeVelocidad), ForceMode2D.Impulse);
        }

        //Mirar hacia la direccion correcta
        if (direccion == 1)
        {
            gameObject.GetComponent<SpriteRenderer>().flipX = true;
        }else if (direccion == -1)
        {
            gameObject.GetComponent<SpriteRenderer>().flipX = false;
        }
                
    }

    // Update is called once per frame
    void Update()
    {
        LeftTounch = Physics2D.OverlapCircle(leftTounch.position, rangeTounch, layer);
        RightTouch = Physics2D.OverlapCircle(rightTouch.position, rangeTounch, layer);
        comprovanteAereo = Physics2D.Raycast(wallComprove.position, Vector2.right, rangeToToachWall, layer);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(wallComprove.position, new Vector3(wallComprove.position.x + rangeToToachWall, 
            wallComprove.position.y, 
            wallComprove.position.z));
        Gizmos.DrawSphere(leftTounch.position,rangeTounch);
        Gizmos.DrawSphere(rightTouch.position, rangeTounch);
    }
}