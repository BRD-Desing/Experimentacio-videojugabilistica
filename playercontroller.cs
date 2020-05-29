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


    //Chunches para las habilidades
    public int TipoHabilidad;
    public int TipoDeMovimiento;
    public bool EnLaHabilidad = false;
    private float tiempoColdDownHabilidad;
    public GameObject areaDeEfecto;
    
    //Melee atack
    private bool melee;
    public bool meleeMode;
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
   
    




    public Transform mira;
    public Transform brazo;
    //intento disparo
    public GameObject trypref;
    //Animador del brazo
    Animator BrazoAnim;
    //Componentes
    Animator anim;

    Rigidbody2D rb2d;
    SpriteRenderer sprite;
    CapsuleCollider2D collid;
    AudioSource jumpS;

    public Transform pisoT;
    public float areaPiso;
    public LayerMask whatIsGround;



    public GameObject contenedor;



    // Use this for initialization
    void Start() {
        anim = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        collid = GetComponent<CapsuleCollider2D>();
        jumpS = GetComponent<AudioSource>();

        BrazoAnim = brazo.GetComponent<Animator>();


    }






    // Update is called once per frame
    void Update() {
        //animador
        anim.SetFloat("Speed", Mathf.Abs(rb2d.velocity.x));
        anim.SetBool("Grounded", grounded);
        anim.SetBool("dash", itsdashing);
        anim.SetBool("reverse", revers);
        anim.SetBool("meleeMode", meleeMode);
        anim.SetBool("enLaHabilidad", EnLaHabilidad);


        BrazoAnim.SetBool("invis", itsdashing);

        if (meleeMode == false)
        {
            BrazoAnim.SetBool("invis", EnLaHabilidad);
        }
        else
        {
            BrazoAnim.SetBool("invis", meleeMode);
        }
            
        grounded = Physics2D.OverlapCircle(pisoT.position, areaPiso, whatIsGround);

        brazo.GetComponent<brazocontroller>().invis = EnLaHabilidad;
        brazo.GetComponent<brazocontroller>().meleeMo = meleeMode;

        if (EnLaHabilidad == false)
        {
            if (meleeMode == true)
            {
                brazo.GetComponent<brazocontroller>().invis = true;
            }
            else {
                brazo.GetComponent<brazocontroller>().invis = false;
            }
        }

        //Mouse
		mira.position = Camera.main.ScreenToWorldPoint(new Vector3(
            Input.mousePosition.x,
            Input.mousePosition.y,
            10)
            );

        ComprovadoresDeInputs();
        cambioDeModo();
        meleeA();
        SeleccionDeHabilidad();
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
            }else if (!grounded && Xmov != 0)
            {
                //movimiento aereo
                Vector2 ForceToAdd = new Vector2(movimientoAereo * Xmov, 0);
                rb2d.AddForce(ForceToAdd);
                if (Mathf.Abs(rb2d.velocity.x)>speed)
                {
                    rb2d.velocity = new Vector2(speed * Xmov, rb2d.velocity.y);
                }
            }else if (!grounded && Xmov == 0)
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


        //Salto
        if (canJump)
        {
           
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





        CheckDash();
    }

    private void Rotado()
    {
        rotado = !rotado;
        transform.Rotate(0, 180, 0);
    }



    float meleeTipe;
    float Mtiempo = 2;
    float tiempoPrarElSiguiemte;

    private void meleeA()
    {
        if (grounded == false)
        {
            meleePos.position = new Vector2(gameObject.transform.position.x + 0.09f,
                gameObject.transform.position.y + 0.92f);
        }
        else
        {
            meleePos.position = new Vector2(gameObject.transform.position.x + 0.69f,
                gameObject.transform.position.y + -0.39f);
        }

        if (Time.time >= tiempoPrarElSiguiemte)
        {
            if (meleeMode == true && Input.GetButtonDown("Fire1"))
            {
                anim.SetTrigger("melee");
                tiempoPrarElSiguiemte = Time.time + 0.5f / Mtiempo;
                meleeTipe = Random.Range(1, 4);
                melee = true;
                if (grounded == true)
                {
                    if (meleeTipe < 2)
                    {
                        anim.SetTrigger("1");
                    }
                    else
                    {
                        anim.SetTrigger("2");
                    }
                }
               


                Collider2D[] hitenemies = Physics2D.OverlapCircleAll(meleePos.position,
                    meleeRange,
                    enemiSet);
                foreach (Collider2D enemy in hitenemies)
                {
                    Debug.Log("Le pegaste a " + enemy.name);
                }
            }
        }
     }




    float tiempoResp;
    bool puedeCam = true;
    private void cambioDeModo()
    {
        if (tiempoResp > 0)
        {
            tiempoResp -= Time.deltaTime;
        }
        else
        {
            puedeCam = true;
        }
        if (Input.GetButton("Prueba") && meleeMode == false && puedeCam == true)
        {
            anim.SetTrigger("MeleeMode");
            meleeMode = true;
            brazo.GetComponent<brazocontroller>().invis = true;
            puedeCam = false;
            tiempoResp = 0.5f;
        }
        if (Input.GetButtonDown("Prueba") && meleeMode == true && puedeCam == true)
        {
            anim.SetTrigger("MeleeOff");
            meleeMode = false;
            brazo.GetComponent<brazocontroller>().invis = false;
            puedeCam = false;
            tiempoResp = 0.5f;
        }
    }


    void ReduccionDeTiempo()
    {
        if (tiempoDeLaHabilidad != 0)
        {
            tiempoDeLaHabilidad -= Time.deltaTime;
        }
        if (tiempoDeLaHabilidad <= 0)
        {
            EnLaHabilidad = false;
            
            rb2d.gravityScale = 4;
            canMoeve = true;
        }
    }

    




    private float tiempoColddownMovimiento;
    private void SeleccionDeHabilidad()
    {
        if (tiempoColdDownHabilidad != 0)
        {
            tiempoColdDownHabilidad -= Time.deltaTime;
        }
        if (tiempoColddownMovimiento != 0)
        {
            tiempoColddownMovimiento -= Time.deltaTime;
        }

        ReduccionDeTiempo();

        if (TipoHabilidad == 1 && tiempoColdDownHabilidad <= 0)
        {
            SobreCarga();
        }

        if (TipoDeMovimiento == 1 && tiempoColddownMovimiento <= 0)
        {
            canDash = true;
            if (Input.GetButtonDown("Dash"))
            {
                AttempToDash();
            }
        }else if (TipoDeMovimiento == 2 && tiempoColddownMovimiento <= 0)
        {
            Teleportacion();
        }else if (TipoDeMovimiento == 3 && tiempoColddownMovimiento <= 0)
        {
            SaltoVertical();
        }else if (TipoDeMovimiento == 4 && tiempoColddownMovimiento <= 0)
        {
            Aceleracion();
        }
    }

    //Sobre Carga
    private float tiempoDeLaHabilidad;
    private void SobreCarga()
    {
        

       if (Input.GetButtonDown("Fire2") && EnLaHabilidad == false)
        {
            rb2d.gravityScale = 0;
            rb2d.velocity = new Vector2(0,0);
            canMoeve = false;
            tiempoDeLaHabilidad = 0.6f;
            brazo.GetComponent<brazocontroller>().invis = true;
            Instantiate(areaDeEfecto, gameObject.transform.position, Quaternion.identity);
            anim.SetTrigger("SobreCarga");
            tiempoColdDownHabilidad = 2;
            EnLaHabilidad = true;
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
                tiempoColddownMovimiento = 1f;
                canDash = false;
                itsdashing = false;
                canMoeve = true;
                canJump = true;
            }
        }
    }


    //Teleporte
    private float antesDeIr;
    private bool inicio = false;
    Vector2 mouseP;
    private void Teleportacion()
    {
        if (Input.GetButtonDown("Dash") && inicio == false) {
            mouseP = mira.position;
            EnLaHabilidad = true;
            tiempoDeLaHabilidad = 1.1f;
            antesDeIr = 0.7f;
            anim.SetTrigger("teleport");
            inicio = true;
        }
        if (antesDeIr > 0)
        {
            antesDeIr -= Time.deltaTime;
            rb2d.velocity = new Vector2(0, 0);
        }
        if (Vector3.Distance(gameObject.transform.position, mira.position) <= 15 && antesDeIr <= 0 && inicio == true)
        {
            gameObject.transform.position = mouseP;
            inicio = false;
            tiempoColddownMovimiento = 3;
        }
    }

    public GameObject balaVer;
    private bool dobleSalto = true;
    void SaltoVertical()
    {
        if (grounded == true)
        {
            dobleSalto = false;
        }

        if (antesDeIr != 0)
        {
            antesDeIr -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Dash") || Input.GetButtonDown("Jump") && dobleSalto == false && grounded == false && jump == false)
        {
            antesDeIr = 0.2f;
            EnLaHabilidad = true;
            tiempoDeLaHabilidad = 0.65f;
            dobleSalto = true;
            anim.SetTrigger("dobleSalto");
        }

        if (antesDeIr <= 0 && dobleSalto == true)
        {
            tiempoColddownMovimiento = 2;
            rb2d.AddForce(Vector2.up * 200, ForceMode2D.Impulse);
            Instantiate(balaVer, new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - 2), Quaternion.identity);
            dobleSalto = false;
            
        }
    }



    private bool acelerao = false;
    private float nitroTime;
    void Aceleracion()
    {
        if (nitroTime != 0)
        {
            nitroTime -= Time.deltaTime;
        }
        if (nitroTime <= 0)
        {
            acelerao = false;
        }

        if (Input.GetButtonDown("Dash") && acelerao == false)
        {
            acelerao = true;
            nitroTime = 10;
            tiempoColddownMovimiento = 2;
            speed = 15;
            puwerJump = 60;
        }

        if (nitroTime <= 0)
        {
            speed = 8;
            puwerJump = 20;
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
    }




    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(gameObject.transform.position, mira.position);
        Gizmos.DrawWireSphere(meleePos.position, meleeRange);
        Gizmos.DrawWireSphere(pisoT.position, areaPiso);
    }
}
