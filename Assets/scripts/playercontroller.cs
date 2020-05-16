using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playercontroller : MonoBehaviour { 
    //movimiento en X
    public float speed;
    private float Xmov;
    public float movimientoAereo;
    private float multiplicador = 0.95f;
    private bool rotado = true;

   
    
    
    //Movimiento en V
    public float puwerJump;
    //Saltar desde la pared
    public float wallJumpForce;

   
    
    
    
    //Melee atack
    private bool melee;
    private bool meleeMode;
    public float meleeRange;
    public Transform meleePos;
    public LayerMask enemiSet;

  
    
    
    //Dash Setting
    private bool itsdashing;
    public float dashTime;
    public float dashSpeed;
    public float distanceBetweenImage;
    private float dashCooldown = 0;
    private float dashtimeLeft;
    private float lastImageXpos;
    private float lastDash = -100f;
    private bool canDash = false;

    public float direccion = 1;





    //Inputs o entradas
    private bool saltoComprovante = false;
    private bool dashComprovante = false;
    private bool disparoPrincipalComprovante = false;
    private bool disparoSecundarioCopmprovante = false;

    private bool golpeMeleeComprovante = false;

    
    
    //otros Boleanos
    public bool grounded;
    public bool trans = false;
    public bool jump;
    public bool revers;
    public bool canMoeve = true;
    public bool canJump = true;
    private bool flyJump;

    
    
    
    //deslizandose en una pared
    private bool isTouchingWallright;
    private bool isTouchingWallLeft;
   
    
    
    //comprovadores
    public Transform WallCheckright;
    public Transform WallCheckLeft;
   
    
    
    //Otras varas de la misma guea
    private bool isSlidingWall;
    public float wallSlidingS;
    public float WallCheckDistance;




    public Transform mira;
    public Transform brazo;



    //intento disparo
    public GameObject trypref;



    //Animador del brazo
    Animator invis;



    //Componentes
    Animator anim;






    //Controladores de animacion
    public AnimatorControllerParameter DashAnim;

    Rigidbody2D rb2d;
    SpriteRenderer sprite;
    CapsuleCollider2D collid;
    AudioSource jumpS;
    public LayerMask whatIsGround;





    // Use this for initialization
    void Start() {
        anim = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        collid = GetComponent<CapsuleCollider2D>();
        jumpS = GetComponent<AudioSource>();

        invis = brazo.GetComponent<Animator>();


    }






    // Update is called once per frame
    void Update() {
        //animador
        anim.SetFloat("Speed", Mathf.Abs(rb2d.velocity.x));
        anim.SetBool("Grounded", grounded);
        anim.SetBool("dash", itsdashing);
        anim.SetBool("reverse", revers);
        anim.SetBool("WallSliding", isSlidingWall);
        anim.SetBool("meleeMode", meleeMode);


        invis.SetBool("invis", itsdashing);


        //Mouse
		mira.position = Camera.main.ScreenToWorldPoint(new Vector3(
            Input.mousePosition.x,
            Input.mousePosition.y,
            10)
            );

        ComprovadoresDeInputs();
        cambioDeModo();
    }







    private void FixedUpdate()
    {
        if (rb2d.velocity.y > wallJumpForce)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, wallJumpForce);
        }
        //Se puede mover???
        if (canMoeve)
        {
            Xmov = Input.GetAxisRaw("Horizontal");
            if (Xmov != 0)
            {
                direccion = Xmov;
            }
            if (grounded)
            {
                //MOvimiento en X
                rb2d.velocity = new Vector2(Xmov * speed, rb2d.velocity.y);
            }else if (!grounded && !isSlidingWall && Xmov != 0)
            {
                //movimiento aereo
                Vector2 ForceToAdd = new Vector2(movimientoAereo * Xmov, 0);
                rb2d.AddForce(ForceToAdd);
                if (Mathf.Abs(rb2d.velocity.x)>speed)
                {
                    rb2d.velocity = new Vector2(speed * Xmov, rb2d.velocity.y);
                }
            }else if (!grounded && !isSlidingWall && Xmov == 0)
            {
                rb2d.velocity = new Vector2(rb2d.velocity.x * multiplicador, rb2d.velocity.y);
            }
                
           

            //Mira
                if (mira.transform.position.x < gameObject.transform.position.x)
                {
                     if (rotado)
                     {
                         Rotado();
                     }
                  
                }
                else if (mira.transform.position.x > gameObject.transform.position.x)
                {
                    if (!rotado)
                    {
                         Rotado();
                    }
                }

            if (mira.position.x < gameObject.transform.position.x && Xmov > 0 ||
                mira.position.x > gameObject.transform.position.x && Xmov < 0)
            {
                revers = true;
            }
            else if(Xmov == 0)
            {
                revers = false;
            }else
            {
                revers = false;
            }
        }

        CheckDash();

        //Dash
        if (dashCooldown != 0)
        {
            dashCooldown -= Time.deltaTime;
        }

        if (dashCooldown <= 0)
        {
            canDash = true;
            if (Input.GetButtonDown("Dash"))
            {
                AttempToDash();
            }
        }

        if (canJump)
        {
            //Salto
            if (saltoComprovante && grounded)
            {
                jump = true;
                jumpS.Play();
            }
            if (jump)
            {
                rb2d.AddForce(Vector2.up * puwerJump, ForceMode2D.Impulse);
                jump = false;
                flyJump = false;
            }
        }
        if (golpeMeleeComprovante == true)
        {
            meleeA();
            canMoeve = false;
        }
        
    }

    private void meleeA()
    {
        melee = true;
        anim.SetTrigger("melee");
        Collider2D[] hitenemies = Physics2D.OverlapCircleAll(meleePos.position, 
            meleeRange, 
            enemiSet);
        foreach (Collider2D enemy in hitenemies)
        {
            Debug.Log("Le pegaste a " + enemy.name);
        }
     }


    private void Rotado()
    {
        rotado = !rotado;
        transform.Rotate(0, 180, 0);
    }





    private void cambioDeModo()
    {
        if (Input.GetButton("Prueba"))
        {
            anim.SetTrigger("MeleeMode");
            meleeMode = true;
        }
    }





    //Dasahear a todo lao
    private void AttempToDash()
    {
        itsdashing = true;
        dashtimeLeft = dashTime;
        lastDash = Time.time;
    }





    private void CheckDash()
    {
        if (itsdashing)
        {
            if (dashtimeLeft > 0)
            {
                canMoeve = false;
                canJump = false;
                rb2d.velocity = new Vector2(dashSpeed * direccion, 0);
                dashtimeLeft -= Time.deltaTime;

                if (Mathf.Abs(transform.position.x - lastImageXpos) > distanceBetweenImage)
                {
                    trypref.GetComponent<SpriteRenderer>().flipX = sprite.flipX;
                    Instantiate(trypref, transform.position, Quaternion.identity);
                    
                }
            }

            if (dashtimeLeft <= 0)
            {
                dashCooldown = 1f;
                canDash = false;
                itsdashing = false;
                canMoeve = true;
                canJump = true;
            }
        }
    }






    private void OnTriggerEnter2D(Collider2D col)
    {
         if (col.gameObject.tag == "Ground")
        {
            grounded = true;
        }
    }




    private void OnTriggerExit2D(Collider2D col)
    {
         if (col.gameObject.tag == "Ground")
        {
            grounded = false;
        }
    }





    private void ComprovadoresDeInputs()
    {
        if (Input.GetButtonDown("Jump"))
        {
            saltoComprovante = true;
        }
        else if (Input.GetButtonUp("Jump"))
        {
            saltoComprovante = false;
        }

        if (Input.GetButtonDown("Fire3"))
        {
            golpeMeleeComprovante = true;
        }else if (Input.GetButtonUp("Fire3"))
        {
            golpeMeleeComprovante = false;
        }
    }




    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(gameObject.transform.position, mira.position);
        Gizmos.DrawWireSphere(meleePos.position, meleeRange);
    }
}
