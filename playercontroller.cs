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
	public int tipoHabilidadCuchillo;
    public int TipoDeMovimiento;

    public bool MovimientoEnProceso = false;
    public bool EnLaHabilidad = false;
    private float tiempoColdDownHabilidad;
    public GameObject areaDeEfecto;
    
    //Melee atack
    private bool melee;
    public bool meleeMode;
    public float meleeRange;

    public GameObject cuchillo;
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

    private bool clicder = false;
   
    




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
        anim.SetBool("reverse", revers);
        anim.SetBool("meleeMode", meleeMode);
        anim.SetBool("enLaHabilidad", EnLaHabilidad);
        anim.SetBool("movimiento", MovimientoEnProceso);

		anim.SetInteger ("TipoHabilidad", TipoHabilidad);
		anim.SetInteger ("HCuchillo", tipoHabilidadCuchillo);
		anim.SetInteger ("TipoMovimiento", TipoDeMovimiento);


        BrazoAnim.SetBool("invis", itsdashing);
       

        if (meleeMode == false)
        {
            if (EnLaHabilidad == true)
            {
                BrazoAnim.SetBool("invis", EnLaHabilidad);
            }else if (itsdashing == false)
            {
                BrazoAnim.SetBool("invis", MovimientoEnProceso);
            }
            
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
        Habilidad1Cuchillo();
    }







    private void Rotado()
    {
        rotado = !rotado;
        transform.Rotate(0, 180, 0);
    }



	//	MMMMM	MMMMM	EEEEEEEE	LLLL      	EEEEEEEE	EEEEEEEE
	//	MMMMMM MMMMMM	EEEE    	LLLL      	EEEE		EEEE
	//	MMMMMMMMMMMMM	EEEEEE  	LLLL      	EEEEEE		EEEEEE
	//	MMMMM	MMMMM	EEEE    	LLLL      	EEEE		EEEE
	//	MMMMM	MMMMM	EEEEEEEE	LLLLLLLLLL	EEEEEEEE	EEEEEEEE

    int meleeTipe;
    float Mtiempo = 2;
    float tiempoPrarElSiguiemte;

    public float meleeHTiempoReferencia;
    public float tiempoDeDuracionDeLaHabilidadMelee;
    public float meleeBackDashVelocity;

    private bool backDashActivado;
    private bool puedeInvocar = true;

    public float antesDeDesplasarce;
    public float delayDespazamiento;

    private void meleeA()
    {
        

        if (Time.time >= tiempoPrarElSiguiemte && grounded == true)
        {
            anim.SetInteger("TipoMelee", meleeTipe);
            anim.SetInteger("HCuchillo", tipoHabilidadCuchillo);


            if (meleeMode == true && Input.GetButtonDown("Fire1"))
            {

                tiempoPrarElSiguiemte = Time.time + 0.5f / Mtiempo;
                meleeTipe = Random.Range(1, 4);
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
        }

        if (meleeMode == true)
        {

            //   HHH
            //  HHHH
            //   HHH
            //   HHH
            //  HHHHH

            if (tipoHabilidadCuchillo == 1 && Input.GetButtonDown("Fire2"))
            {

                EnEsperaDelBackDash();  
            }

        }
       
    }

    void EnEsperaDelBackDash()
    {
        if (Time.time >= tiempoPrarElSiguiemte && clicder == false)
        {
            tiempoDeLaHabilidad = 0.5f;
            EnLaHabilidad = true;
            clicder = true;
            backDashActivado = true;

            anim.SetTrigger("meleeHabilidad");

            meleeHTiempoReferencia = tiempoDeDuracionDeLaHabilidadMelee;
            delayDespazamiento = antesDeDesplasarce;
        }
    }

    private void Habilidad1Cuchillo()
    {
       
        if (backDashActivado == true)
        {
            if (delayDespazamiento > 0)
            {
                delayDespazamiento -= Time.deltaTime;
                canMoeve = false;

            }else if (delayDespazamiento <= 0)
            {
                if (meleeHTiempoReferencia > 0)
                {
                    meleeHTiempoReferencia -= Time.deltaTime;

                   

                    if (mira.transform.position.x < gameObject.transform.position.x)
                    {
                        rb2d.velocity = new Vector2(meleeBackDashVelocity, 0);
                    }
                    else if (mira.transform.position.x > gameObject.transform.position.x)
                    {
                        rb2d.velocity = new Vector2(-meleeBackDashVelocity, 0);
                    }


                    if (meleeHTiempoReferencia <= meleeHTiempoReferencia - delayDespazamiento && puedeInvocar == true)
                    {
                        Instantiate(cuchillo, meleePos.position, meleePos.rotation);
                        puedeInvocar = false;
                    }
                }

            }
            if (meleeHTiempoReferencia <= 0)
            {
                canMoeve = true;
                tiempoPrarElSiguiemte = Time.time + 0.6f / Mtiempo;
                backDashActivado = false;
                puedeInvocar = true;
                clicder = false;
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
            tiempoResp = 0.2f;
        }
        if (Input.GetButtonDown("Prueba") && meleeMode == true && puedeCam == true)
        {
            anim.SetTrigger("MeleeOff");
            meleeMode = false;
            brazo.GetComponent<brazocontroller>().invis = false;
            puedeCam = false;
            tiempoResp = 0.2f;
        }
    }


	//	HHHH	HHHH		AAAA		BBBBBBB		IIIIII
	//	HHHH	HHHH	   AAAAAA		BB	 BBB	 IIII
	//	HHHHHHHHHHHH	  AAAAAAAA		BBBBBB		 IIII
	//	HHHH	HHHH	 AAAA  AAAA		BB   BBB	 IIII
	//	HHHH	HHHH	AAAA	AAAA	BBBBBBB		IIIIII


    void ReduccionDeTiempo()
    {
        if (tiempoDeLaHabilidad != 0)
        {
            tiempoDeLaHabilidad -= Time.deltaTime;
        }
        if (tiempoDeLaHabilidad <= 0)
        {
            EnLaHabilidad = false;
            MovimientoEnProceso = false;
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

        if (meleeMode == false)
        {
            if (Input.GetButtonDown("Fire2") && EnLaHabilidad == false)
            {
                rb2d.gravityScale = 0;
                rb2d.velocity = new Vector2(0, 0);
                canMoeve = false;
                tiempoDeLaHabilidad = 0.6f;
                brazo.GetComponent<brazocontroller>().invis = true;
                Instantiate(areaDeEfecto, gameObject.transform.position, Quaternion.identity);
                anim.SetTrigger("SobreCarga");
                tiempoColdDownHabilidad = 2;
                EnLaHabilidad = true;
            }
        }
     
    }


    //Dasahear a todo lao
    private void AttempToDash()
    {
        itsdashing = true;
		MovimientoEnProceso = true;
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
				MovimientoEnProceso = false;
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
            MovimientoEnProceso = true;
            tiempoDeLaHabilidad = 1.1f;
            antesDeIr = 0.7f;
            
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

    private bool saltoActivado;
    public GameObject balaVer;
    private bool dobleSalto = true;
    void SaltoVertical()
    {
        if (grounded == true)
        {
            dobleSalto = false;
            saltoActivado = false;
        }

        if (antesDeIr != 0)
        {
            antesDeIr -= Time.deltaTime;
        }
        if (dobleSalto == false && grounded == false && jump == false)
        {
            if (Input.GetButtonDown("Dash") || Input.GetButtonDown("Jump"))
            {
                antesDeIr = 0.28f;
                MovimientoEnProceso = true;
                tiempoDeLaHabilidad = 0.65f;
                dobleSalto = true;
                saltoActivado = true;

                anim.SetTrigger("dobleSalto");
            }
        }
       

        if (antesDeIr <= 0 && dobleSalto == true && saltoActivado == true)
        {
            tiempoColddownMovimiento = 2;
            rb2d.AddForce(Vector2.up * 300, ForceMode2D.Impulse);
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
