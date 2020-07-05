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
    public bool itsdashing;
    public float dashTime;
    public float dashSpeed;
    public float distanceBetweenImage;
    private float dashCooldown = 0;
    private float dashtimeLeft;
    private float lastImageXpos;
    private float lastDash = -100f;
    private bool canDash = false;

    public float radioDeDenegacion;

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
    private bool puedeRotar;
    public bool canMoeve = true;
    public bool canJump = true;
    private bool flyJump;
    public bool noAquiNo = false;  

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
        jumpS = GetComponent<AudioSource>();

        BrazoAnim = brazo.GetComponent<Animator>();

        puedeRotar = true;
        canDash = true;
        llamado = true;
        tiempoColdDownHabilidad = 0;
        gravedadVariable = -9.8f;

    }






    // Update is called once per frame
    void Update() {
        //animador
        anim.SetFloat("Speed", Mathf.Abs(rb2d.velocity.x));
        anim.SetBool("Grounded", grounded);
        anim.SetBool("reverse", revers);
        anim.SetBool("meleeMode", meleeMode);
        anim.SetBool("enLaHabilidad", EnLaHabilidad);
        anim.SetBool("HMovimiento", MovimientoEnProceso);
        anim.SetBool("RoketP", golpeCohete);

        anim.SetInteger ("TipoHabilidad", TipoHabilidad);
		anim.SetInteger ("HCuchillo", tipoHabilidadCuchillo);
		anim.SetInteger ("TipoMovimiento", TipoDeMovimiento);

        
            
        grounded = Physics2D.OverlapCircle(pisoT.position, areaPiso, whatIsGround);

        noAquiNo = Physics2D.OverlapCircle(mira.position, radioDeDenegacion, whatIsGround);



        //Mouse
		mira.position = Camera.main.ScreenToWorldPoint(new Vector3(
            Input.mousePosition.x,
            Input.mousePosition.y,
            10)
            );

        ComprovadoresDeInputs();
        CambioDeModo();
        MeleeA();
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
            if (puedeRotar == true)
            {

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


        impulsor = Input.GetButton("Fire2");


        CheckDash();
        Habilidad1Cuchillo();
        GolpeImpulsor();
        contenedor.transform.position = gameObject.transform.position;
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

    public float tiempoTotalDelGolpe = 0;
    public float referenciaDelGolpe;
    public float velocidadDelGolpe;
    public int mirarA;

    private bool backDashActivado;
    private bool puedeInvocar = true;

    public float antesDeDesplasarce;
    public float delayDespazamiento;

    private void MeleeA()
    {
        anim.SetInteger("TipoMelee", meleeTipe);
        anim.SetInteger("HCuchillo", tipoHabilidadCuchillo);

        if (Time.time >= tiempoPrarElSiguiemte && grounded == true)
        {
            


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

            if (tipoHabilidadCuchillo == 1 && Input.GetButtonDown("Fire2") && MovimientoEnProceso == false)
            {
                EnEsperaDelBackDash();  
            }

            //  D
            //  O
            //  S

            if (tipoHabilidadCuchillo == 2 && MovimientoEnProceso == false)
            {
                Impulsor();
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

    public bool llamado = true;
    private bool impulsor;

    private bool speedPunch;
    void Impulsor()
    {
        
        puedeRotar = true;
        if (impulsor == true && tiempoColdDownHabilidad <= 0 && MovimientoEnProceso == false)
        {
            
            if (llamado == true)
            {
                anim.SetTrigger("meleeHabilidad");
                llamado = false;
            }
            rb2d.velocity = new Vector2(0, rb2d.velocity.y);
            canJump = false;
            canMoeve = false;
            EnLaHabilidad = true;

            tiempoDeLaHabilidad = tiempoTotalDelGolpe;

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

            if (mira.transform.position.x > gameObject.transform.position.x)
            {
                mirarA = 1;
            }
            else if (mira.transform.position.x <= gameObject.transform.position.x)
            {
                mirarA = -1;
            }

            if (tiempoTotalDelGolpe <= 0.6f)
            {
                tiempoTotalDelGolpe += Time.deltaTime;
            }
        }

        if (Input.GetButtonUp("Fire2") && tiempoTotalDelGolpe != 0.0000)
        {
            if (llamado == false)
            {
                
                llamado = true;
            }
            
            if (tiempoTotalDelGolpe < 0.2f)
            {
                tiempoTotalDelGolpe = 0.2f;
            }
            speedPunch = true;
            golpeCohete = true;
            referenciaDelGolpe = tiempoTotalDelGolpe;
            clicder = true;

           
        }
       
    }

    private bool golpeCohete;
    void GolpeImpulsor()
    {
        
        if (speedPunch == true)
        {

            if (referenciaDelGolpe > 0)
            {
                EnLaHabilidad = true;
                puedeRotar = false;
                rb2d.velocity = new Vector2(velocidadDelGolpe * mirarA, rb2d.velocity.y);
                referenciaDelGolpe -= Time.deltaTime;
                tiempoColdDownHabilidad = tiempoTotalDelGolpe * 4;

            }

            if (referenciaDelGolpe <= 0)
            {
                golpeCohete = false;
                puedeRotar = true;
                canJump = true;
                canMoeve = true;
                clicder = false;
                speedPunch = false;
                EnLaHabilidad = false;
               
                tiempoTotalDelGolpe = 0;
            }
        }
       
    }



    float tiempoResp;
    bool puedeCam = true;
    private void CambioDeModo()
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
        if (itsdashing == false)
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
        
    }
        
		

    private float tiempoColddownMovimiento;
    private void SeleccionDeHabilidad()
    {
        ReduccionDeTiempo();

        if (tiempoColdDownHabilidad > 0)
        {
            tiempoColdDownHabilidad -= Time.deltaTime;
        }

        if (tiempoColddownMovimiento > 0)
        {
            tiempoColddownMovimiento -= Time.deltaTime;
        }

        

        if (TipoHabilidad == 1 && tiempoColdDownHabilidad <= 0 && MovimientoEnProceso == false)
        {
            SobreCarga();
        }

        if (TipoDeMovimiento == 1 && tiempoColddownMovimiento <= 0 && EnLaHabilidad == false)
        {
            if (Input.GetButtonDown("Dash") && canDash == true)
            {
                AttempToDash();
            }
        }else if (TipoDeMovimiento == 2 && tiempoColddownMovimiento <= 0)
        {
            Teleportacion();
        }else if (TipoDeMovimiento == 3 && tiempoColddownMovimiento <= 0 && EnLaHabilidad == false)
        {
            SaltoVertical();
        }else if (TipoDeMovimiento == 4 && tiempoColddownMovimiento <= 0 && EnLaHabilidad == false)
        {
            Aceleracion();
        }else if (TipoDeMovimiento == 5 && tiempoColddownMovimiento <= 0 && EnLaHabilidad == false)
        {
            InvercionDeGravedad();
        }
    }

    //Sobre Cargaw
    private float tiempoDeLaHabilidad;
    private void SobreCarga()
    {

        if (meleeMode == false)
        {
            if (Input.GetButtonDown("Fire2") && EnLaHabilidad == false && tiempoDeLaHabilidad <= 0)
            {
                rb2d.gravityScale = 0;
                rb2d.velocity = new Vector2(0, 0);
                canMoeve = false;
                tiempoDeLaHabilidad = 0.5f;
                brazo.GetComponent<brazocontroller>().invis = true;
                Instantiate(areaDeEfecto, gameObject.transform.position, Quaternion.identity);
                tiempoColdDownHabilidad = 2;
                EnLaHabilidad = true;
            }
        }
     
    }


    //Dasahear a todo lao
    private void AttempToDash()
    {
        itsdashing = true;
        canDash = false;
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
                    Instantiate(trypref, transform.position, gameObject.transform.rotation);
                    
                }
            }

            if (dashtimeLeft <= 0)
            {
                tiempoColddownMovimiento = 1f;
                MovimientoEnProceso = false;
                canDash = true;
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
        if (EnLaHabilidad == false)
        {
            if (Vector3.Distance(gameObject.transform.position, mira.position) <= 18 && Input.GetButtonDown("Dash") && inicio == false && noAquiNo == false)
            {
                mouseP = mira.position;

                MovimientoEnProceso = true;
                canMoeve = false;
                tiempoDeLaHabilidad = 1.1f;
                antesDeIr = 0.7f;

                inicio = true;
            }
            if (antesDeIr > 0)
            {
                rb2d.gravityScale = 0;
                rb2d.velocity = new Vector2(0, 0);
                antesDeIr -= Time.deltaTime;
                rb2d.velocity = new Vector2(0, 0);
            }
            if (antesDeIr <= 0 && inicio == true)
            {
                rb2d.gravityScale = 4;
                gameObject.transform.position = mouseP;
                inicio = false;
                tiempoColddownMovimiento = 3;
            }
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

            }
        }

        if(grounded == true)
        {
            MovimientoEnProceso = false;
            saltoActivado = false;
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


    private bool cambioGravedad;
    private float tiempoParaCAmbiar;
    private float gravedadVariable;

    void InvercionDeGravedad()
    {
        

        if (Input.GetButtonDown("Dash") && cambioGravedad == false && Time.time >= tiempoParaCAmbiar)
        {
            rb2d.gravityScale = 0;
            Physics2D.gravity = new Vector2(0, 9);
            puwerJump = puwerJump * -1;
            gameObject.transform.Rotate(180, gameObject.transform.rotation.y, 0);
            cambioGravedad = !cambioGravedad;
            tiempoParaCAmbiar = Time.time + 0.1f;
        }
        if (Input.GetButtonDown("Dash") && cambioGravedad == true && Time.time >= tiempoParaCAmbiar)
        {
            rb2d.gravityScale = 0;
            Physics2D.gravity = new Vector2(0, -9);
            puwerJump = puwerJump * -1;
            gameObject.transform.Rotate(180, gameObject.transform.rotation.y, 0);
            cambioGravedad = !cambioGravedad;
            tiempoParaCAmbiar = Time.time + 0.1f;
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
        Gizmos.DrawWireSphere(pisoT.position,  areaPiso);
        Gizmos.DrawWireSphere(mira.position, radioDeDenegacion);
    }
}
