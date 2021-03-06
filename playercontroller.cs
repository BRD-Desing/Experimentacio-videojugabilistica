﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playercontroller : MonoBehaviour {
    public bool gamePad;

    public float vida;
    public Text vidaEnPantalla;

    //movimiento en X
    public float speed;
    private float Xmov;
    public float movimientoAereo;
    private float multiplicador = 0.95f;
    private bool rotado = true;

    public float tiempoDeEsperaAntesDeDisparar;

    //Movimiento en V
    public float puwerJump;
    //Saltar desde la pared
    public float wallJumpForce;

    //Chunches para las habilidades
    public int tipoHabilidadCuchillo;

    public int tipoHabilidadSecundaria1;
    public int tipoHabilidadSecundaria2;
    public int TipoDeMovimiento;
    public int TipoUlti;

    private bool llamado = true;
    private bool impulsor;
    private bool escudoActivo;
    public float vidaEscudo;
    public float vidaMáximaEscudo;

    public bool MovimientoEnProceso = false;
    public bool EnLaHabilidad = false;
    public bool habilidadSecundaria;
    private float tiempoColdDownHabilidad;
    public GameObject areaDeEfecto;

    //Melee atack
    private bool melee;
    public bool meleeMode;
    public float meleeRange;

    public GameObject cuchillo;
    public GameObject estrellaTeleport;
    public Transform meleePos;
    public LayerMask enemiSet;

    //Dash Setting
    public bool itsdashing;
    public float dashTime;
    public float dashSpeed;
    public float distanceBetweenImage;
    private float dashtimeLeft;
    private float lastImageXpos;
    private float lastDash = -100f;
    private bool canDash = false;

    public float radioDeDenegacion;

    public float direccion = 1;

    //Inputs o entradas
    private bool saltoComprovante = false;

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

    public float sensibilidadDelMouse;
    private float miraX;
    private float miraY;
    public Transform mira;
    public Transform brazo;
    public GameObject brazoContainer;
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
    void Start () {
        anim = GetComponent<Animator> ();
        rb2d = GetComponent<Rigidbody2D> ();
        sprite = GetComponent<SpriteRenderer> ();
        jumpS = GetComponent<AudioSource> ();

        BrazoAnim = brazo.GetComponent<Animator> ();

        puedeRotar = true;
        canDash = true;
        tiempoColdDownHabilidad = 0;
        antesDeLaEntrada = true;

        mira.position = Input.mousePosition;
        llamado = true;
        resetteo = false;

    }

    // Update is called once per frame
    void Update () {
        gamePad = brazoContainer.GetComponent<brazocontroller> ().GamePad;

        vidaEnPantalla.text = vida.ToString ();
        //animador
        anim.SetFloat ("Speed", Mathf.Abs (rb2d.velocity.x));
        anim.SetBool ("Grounded", grounded);
        anim.SetBool ("reverse", revers);
        anim.SetBool ("enLaHabilidad", EnLaHabilidad);
        anim.SetBool ("enLaUlti", EnLaUlti);
        anim.SetBool ("HMovimiento", MovimientoEnProceso);
        anim.SetBool ("RoketP", speedPunch);

        anim.SetInteger ("TipoUlti", TipoUlti);
        anim.SetInteger ("TipoMovimiento", TipoDeMovimiento);

        grounded = Physics2D.OverlapCircle (pisoT.position, areaPiso, whatIsGround);

        noAquiNo = Physics2D.OverlapCircle (mira.position, radioDeDenegacion, whatIsGround);

        miraX += Input.GetAxis ("Mouse X") * sensibilidadDelMouse;
        miraY += Input.GetAxis ("Mouse Y") * sensibilidadDelMouse;

        meleeMode = brazoContainer.GetComponent<brazocontroller> ().meleeMo;

        //Mouse

        if (gamePad == true) {
            mira.position = Camera.main.ScreenToWorldPoint (new Vector3 (
                miraX,
                miraY * -1,
                10));
        } else if (gamePad == false) {
            mira.position = Camera.main.ScreenToWorldPoint (new Vector3 (
                Input.mousePosition.x,
                Input.mousePosition.y,
                10));
        }

        mira.position = new Vector2 (Mathf.Clamp (mira.position.x, gameObject.transform.position.x + -11.25f, gameObject.transform.position.x + 11.25f),
            Mathf.Clamp (mira.position.y, gameObject.transform.position.y + -7, gameObject.transform.position.y + 7));

        ComprovadoresDeInputs ();
        MeleeA ();
        SeleccionDeHabilidad ();
        TiempoDeUltimates ();
    }

    private void FixedUpdate () {
        if (rb2d.velocity.y > wallJumpForce) {
            rb2d.velocity = new Vector2 (rb2d.velocity.x, wallJumpForce);
        }
        //Se puede mover???
        if (canMoeve) {
            Xmov = Input.GetAxisRaw ("Horizontal");
            if (Xmov != 0) {
                direccion = Xmov;
            }
            if (grounded) {
                //MOvimiento en X
                rb2d.velocity = new Vector2 (Xmov * speed, rb2d.velocity.y);
            } else if (!grounded && Xmov != 0) {
                //movimiento aereo
                Vector2 ForceToAdd = new Vector2 (movimientoAereo * Xmov, 0);
                rb2d.AddForce (ForceToAdd);
                if (Mathf.Abs (rb2d.velocity.x) > speed) {
                    rb2d.velocity = new Vector2 (speed * Xmov, rb2d.velocity.y);
                }
            } else if (!grounded && Xmov == 0) {
                rb2d.velocity = new Vector2 (rb2d.velocity.x * multiplicador, rb2d.velocity.y);
            }

            //Mira
            if (puedeRotar == true) {

                if (mira.transform.position.x < gameObject.transform.position.x) {
                    if (rotado) {
                        Rotado ();
                    }

                } else if (mira.transform.position.x > gameObject.transform.position.x) {
                    if (!rotado) {
                        Rotado ();
                    }
                }
            }

            if (mira.position.x < gameObject.transform.position.x && Xmov > 0 ||
                mira.position.x > gameObject.transform.position.x && Xmov < 0) {
                revers = true;
            } else if (Xmov == 0) {
                revers = false;
            } else {
                revers = false;
            }
        }

        //Salto
        if (canJump) {

            if (saltoComprovante && grounded) {
                jump = true;
                jumpS.Play ();
            }
            if (jump) {
                rb2d.AddForce (Vector2.up * puwerJump, ForceMode2D.Impulse);
                jump = false;
                flyJump = false;
            }
        }

        CheckDash ();
        Habilidad1Cuchillo ();
        HabilidadesSecundarias ();
        contenedor.transform.position = gameObject.transform.position;

        if (vida <= 0) {
            Destroy (gameObject);
            canJump = false;
            canMoeve = false;
        }
    }

    private void Rotado () {
        rotado = !rotado;
        transform.Rotate (0, 180, 0);
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
    private bool puedeInvocar;
    private bool animDeHabilidad = true;
    private bool ReposoEscudo;

    public float antesDeDesplasarce;
    public float delayDespazamiento;

    private float tiempoAntesDaño;
    private bool antesDeLaEntrada;

    private void MeleeA () {

        if (Time.time >= tiempoPrarElSiguiemte && grounded == true) {

            if (meleeMode == true && Input.GetButtonDown ("Fire1")) {
                if (antesDeLaEntrada == true) {
                    tiempoAntesDaño = Time.time + 0.2f;
                    antesDeLaEntrada = false;
                }
                tiempoPrarElSiguiemte = Time.time + 1f / Mtiempo;
                meleeTipe = Random.Range (1, 4);
                melee = true;

                anim.SetTrigger ("melee");

                Collider2D[] hitenemies = Physics2D.OverlapCircleAll (meleePos.position,
                    meleeRange,
                    enemiSet);
                if (tiempoAntesDaño <= Time.time) {
                    foreach (Collider2D enemy in hitenemies) {
                        enemy.GetComponent<EnemyController> ().Damage (32);
                        antesDeLaEntrada = true;
                    }
                }
            }
        }

    }

    //	HHHH	HHHH		AAAA		BBBBBBB		IIIIII       MMMMMM
    //	HHHH	HHHH	   AAAAAA		BB	 BBB	 IIII       MMM  MMM
    //	HHHHHHHHHHHH	  AAAAAAAA		BBBBBB		 IIII           MMM
    //	HHHH	HHHH	 AAAA  AAAA		BB   BBB	 IIII         MMM
    //	HHHH	HHHH	AAAA	AAAA	BBBBBBB		IIIIII      MMMMMMMM
    void EnEsperaDelBackDash () {
        if (Time.time >= tiempoPrarElSiguiemte && clicder == false) {
            tiempoDeLaHabilidad = 0.5f;
            EnLaHabilidad = true;
            clicder = true;
            backDashActivado = true;

            if (puedeInvocar == false) {
                anim.SetTrigger ("BackD");
                BrazoAnim.SetTrigger("invisible");
                puedeInvocar = true;
            }

            meleeHTiempoReferencia = tiempoDeDuracionDeLaHabilidadMelee;
            delayDespazamiento = antesDeDesplasarce;
        }
    }

    private void Habilidad1Cuchillo () {

        if (backDashActivado == true) {
            if (delayDespazamiento > 0) {
                delayDespazamiento -= Time.deltaTime;
                canMoeve = false;
            } else if (delayDespazamiento <= 0) {
                if (meleeHTiempoReferencia > 0) {
                    meleeHTiempoReferencia -= Time.deltaTime;

                    if (mira.transform.position.x < gameObject.transform.position.x) {
                        rb2d.velocity = new Vector2 (meleeBackDashVelocity, 0);
                    } else if (mira.transform.position.x > gameObject.transform.position.x) {
                        rb2d.velocity = new Vector2 (-meleeBackDashVelocity, 0);
                    }

                    if (meleeHTiempoReferencia <= meleeHTiempoReferencia - delayDespazamiento && puedeInvocar == true) {
                        Instantiate (cuchillo, meleePos.position, meleePos.rotation);
                        puedeInvocar = false;
                    }
                }

            }

            if (meleeHTiempoReferencia <= 0) {
                BrazoAnim.SetTrigger ("CambioDeArma");
                canMoeve = true;
                EnLaHabilidad = false;
                tiempoPrarElSiguiemte = Time.time + 0.6f / Mtiempo;
                tiempoDeEsperaAntesDeDisparar = Time.time + 1;
                backDashActivado = false;
                clicder = false;
            }
        }
    }

    private bool speedPunch;
    private bool resetteo;

    private bool entraEnElReseteoDelGolpe;

    void GolpeImpulsor () {
        rb2d.velocity = new Vector2 (velocidadDelGolpe * mirarA, rb2d.velocity.y);
        Collider2D[] punchEnemies = Physics2D.OverlapCircleAll (meleePos.position, meleeRange, enemiSet);

        foreach (Collider2D enemyP in punchEnemies) {
            enemyP.GetComponent<EnemyController> ().Damage (tiempoTotalDelGolpe * 100);
            enemyP.GetComponent<EnemyController> ().Empujon (tiempoTotalDelGolpe * mirarA * 60, 7);
            resetteo = true;
        }

    }

    private void Escudo () {
        anim.SetBool ("Escudo", escudoActivo);
        if (vidaEscudo > 0) {
            if (escudoActivo == true) {
                if (animDeHabilidad == true) {
                    anim.SetTrigger ("Escudo_Entrada");
                    BrazoAnim.SetTrigger("invisible");
                    animDeHabilidad = false;
                }
                canJump = false;
                EnLaHabilidad = true;
                speed = 4;
            }
        } else if (vidaEscudo <= 0) {
            ReposoEscudo = true;
            escudoActivo = false;
            canJump = true;
        }

        if (vidaEscudo >= 10) {
            ReposoEscudo = false;
        }

        if (escudoActivo == false) {
            if (vidaEscudo <= vidaMáximaEscudo) {
                vidaEscudo += 0.25f;
            }
            speed = 8;
            canJump = true;
            if (animDeHabilidad == false) {
                resetteo = true;
                animDeHabilidad = true;
            }
        }
    }

    //	HHHH	HHHH		AAAA		BBBBBBB		IIIIII
    //	HHHH	HHHH	   AAAAAA		BB	 BBB	 IIII
    //	HHHHHHHHHHHH	  AAAAAAAA		BBBBBB		 IIII
    //	HHHH	HHHH	 AAAA  AAAA		BB   BBB	 IIII
    //	HHHH	HHHH	AAAA	AAAA	BBBBBBB		IIIIII

    private bool puedeAccederAlArma;

    void HabilidadesSecundarias () {
        if (resetteo == true) {
            puedeRotar = true;
            tiempoColdDownHabilidad = Time.time + tiempoTotalDelGolpe * 10;
            canJump = true;
            canMoeve = true;
            speedPunch = false;
            EnLaHabilidad = false;
            BrazoAnim.SetTrigger ("CambioDeArma");
            referenciaDelGolpe = 0;
            resetteo = false;
        }

        if (MovimientoEnProceso == false) {

            switch (tipoHabilidadSecundaria1) {
                case 1:
                    if (EnLaHabilidad == false) {
                        if (Input.GetButtonDown ("Habilidad 1")) {
                            EnEsperaDelBackDash ();
                        }
                    }
                    break;
                case 2:
                    impulsor = Input.GetButton ("Habilidad 1");
                    if (tiempoColdDownHabilidad <= Time.time) {
                        if (escudoActivo == false) {
                            if (speedPunch == false) {
                                if (impulsor == true) {
                                    puedeRotar = true;
                                    EnLaHabilidad = true;
                                    canJump = false;
                                    canMoeve = false;
                                    if (llamado == true) {
                                        anim.SetTrigger ("Golpe");
                                        BrazoAnim.SetTrigger("invisible");
                                        llamado = false;
                                    }
                                    if (mira.transform.position.x < gameObject.transform.position.x) {
                                        if (rotado) {
                                            Rotado ();
                                        }

                                    } else if (mira.transform.position.x > gameObject.transform.position.x) {
                                        if (!rotado) {
                                            Rotado ();
                                        }
                                    }
                                    if (mira.transform.position.x > gameObject.transform.position.x) {
                                        mirarA = 1;
                                    } else if (mira.transform.position.x <= gameObject.transform.position.x) {
                                        mirarA = -1;
                                    }
                                    if (referenciaDelGolpe < 0.7) {
                                        referenciaDelGolpe += Time.deltaTime;
                                    }
                                }
                            }
                        }
                    }

                    if (impulsor == false && referenciaDelGolpe > 0) {
                        GolpeImpulsor ();
                        referenciaDelGolpe -= Time.deltaTime;
                        speedPunch = true;
                        EnLaHabilidad = true;
                        if (llamado == false) {
                            entraEnElReseteoDelGolpe = true;
                            tiempoTotalDelGolpe = referenciaDelGolpe;
                            if (referenciaDelGolpe <= 0.2) {
                                referenciaDelGolpe = 0.2f;
                            }
                            llamado = true;
                        }
                    } else if (referenciaDelGolpe <= 0 && entraEnElReseteoDelGolpe == true) {
                        resetteo = true;
                        entraEnElReseteoDelGolpe = false;
                    }
                    break;
                case 3:
                    if (ReposoEscudo == false) {
                        if (vidaEscudo >= 10) {
                            escudoActivo = Input.GetButton ("Habilidad 1");
                        }
                    }
                    Escudo ();
                    break;

            }

            switch (tipoHabilidadSecundaria2) {
                case 1:
                    if (Input.GetButtonDown ("Habilidad 2")) {
                        EnEsperaDelBackDash ();
                    }

                    break;
                case 2:
                    impulsor = Input.GetButton ("Habilidad 2");

                    if (tiempoColdDownHabilidad <= Time.time) {
                        if (escudoActivo == false) {
                            if (speedPunch == false) {
                                if (impulsor == true) {
                                    puedeRotar = true;
                                    EnLaHabilidad = true;
                                    canJump = false;
                                    canMoeve = false;
                                    if (llamado == true) {
                                        anim.SetTrigger ("Golpe");
                                        BrazoAnim.SetTrigger("invisible");
                                        llamado = false;
                                    }
                                    if (mira.transform.position.x < gameObject.transform.position.x) {
                                        if (rotado) {
                                            Rotado ();
                                        }

                                    } else if (mira.transform.position.x > gameObject.transform.position.x) {
                                        if (!rotado) {
                                            Rotado ();
                                        }
                                    }
                                    if (mira.transform.position.x > gameObject.transform.position.x) {
                                        mirarA = 1;
                                    } else if (mira.transform.position.x <= gameObject.transform.position.x) {
                                        mirarA = -1;
                                    }
                                    if (referenciaDelGolpe < 0.7) {
                                        referenciaDelGolpe += Time.deltaTime;
                                    }
                                }
                            }
                        }
                    }

                    if (impulsor == false && referenciaDelGolpe > 0) {
                        GolpeImpulsor ();
                        referenciaDelGolpe -= Time.deltaTime;
                        speedPunch = true;
                        EnLaHabilidad = true;
                        if (llamado == false) {
                            entraEnElReseteoDelGolpe = true;
                            tiempoTotalDelGolpe = referenciaDelGolpe;
                            if (referenciaDelGolpe <= 0.2) {
                                referenciaDelGolpe = 0.2f;
                            }
                            llamado = true;
                        }
                    } else if (referenciaDelGolpe <= 0 && entraEnElReseteoDelGolpe == true) {
                        resetteo = true;
                        entraEnElReseteoDelGolpe = false;
                    }
                    break;
                case 3:
                    if (ReposoEscudo == false) {
                        if (vidaEscudo >= 10) {
                            escudoActivo = Input.GetButton ("Habilidad 2");
                        }
                    }
                    Escudo ();
                    break;

            }

        }
    }

    void ReduccionDeTiempo () {
        if (itsdashing == false) {
            if (tiempoDeLaHabilidad != 0) {
                tiempoDeLaHabilidad -= Time.deltaTime;
            }
            if (tiempoDeLaHabilidad <= 0) {
                if (puedeAccederAlArma == true) {
                    BrazoAnim.SetTrigger ("CambioDeArma");
                    tiempoDeEsperaAntesDeDisparar = 1f;
                    puedeAccederAlArma = false;
                }
                EnLaHabilidad = false;
                MovimientoEnProceso = false;
                rb2d.gravityScale = 4;
                canMoeve = true;
            }
        }

    }

    private float tiempoColddownMovimiento;
    private void SeleccionDeHabilidad () {
        ReduccionDeTiempo ();

        if (tiempoColdDownHabilidad > 0) {
            tiempoColdDownHabilidad -= Time.deltaTime;
        }

        if (tiempoColddownMovimiento > 0) {
            tiempoColddownMovimiento -= Time.deltaTime;
        }

        if (TipoUlti == 1 && tiempoColdDownUlti <= 0 && MovimientoEnProceso == false) {
            SobreCarga ();
        }

        if (TipoDeMovimiento == 1 && tiempoColddownMovimiento <= 0 && EnLaHabilidad == false) {
            if (Input.GetButtonDown ("Dash") && canDash == true) {
                AttempToDash ();
            }
        } else if (TipoDeMovimiento == 2 && tiempoColddownMovimiento <= 0) {
            Teleportacion ();
        } else if (TipoDeMovimiento == 3 && tiempoColddownMovimiento <= 0 && EnLaHabilidad == false) {
            SaltoVertical ();
        } else if (TipoDeMovimiento == 4 && tiempoColddownMovimiento <= 0 && EnLaHabilidad == false) {
            Aceleracion ();
        }
    }

    //Sobre Cargaw
    private float tiempoDeLaHabilidad;

    private float tiempoDeLaUlti;
    private bool EnLaUlti;
    private float tiempoColdDownUlti;
    private void TiempoDeUltimates () {

        if (tiempoColdDownUlti > 0) {
            tiempoColdDownUlti -= Time.deltaTime;
        }

        if (tiempoDeLaUlti > 0) {
            tiempoDeLaUlti -= Time.deltaTime;
        } else if (tiempoDeLaUlti <= 0) {
            EnLaUlti = false;
        }
    }
    private void SobreCarga () {

        if (Input.GetButtonDown ("Fire3") && EnLaUlti == false && tiempoDeLaUlti <= 0) {
            rb2d.gravityScale = 0;
            rb2d.velocity = new Vector2 (0, 0);
            canMoeve = false;
            anim.SetTrigger ("EntraLaUlti");
            tiempoDeLaUlti = 15f;
            brazo.GetComponent<brazocontroller> ().invis = true;
            Instantiate (areaDeEfecto, gameObject.transform.position, Quaternion.identity);
            tiempoColdDownUlti = 60;
            EnLaUlti = true;
        }

    }

    //Dasahear a todo lao
    private void AttempToDash () {
        itsdashing = true;
        canDash = false;
        MovimientoEnProceso = true;
        dashtimeLeft = dashTime;
        lastDash = Time.time;

    }
    private void CheckDash () {
        if (itsdashing) {
            if (dashtimeLeft > 0) {
                canMoeve = false;
                canJump = false;
                rb2d.velocity = new Vector2 (dashSpeed * direccion, 0);
                dashtimeLeft -= Time.deltaTime;

                if (Mathf.Abs (transform.position.x - lastImageXpos) > distanceBetweenImage) {
                    Instantiate (trypref, transform.position, gameObject.transform.rotation);

                }
            }

            if (dashtimeLeft <= 0) {
                tiempoColddownMovimiento = 1f;
                MovimientoEnProceso = false;
                canDash = true;
                itsdashing = false;
                canMoeve = true;
                canJump = true;
                puedeAccederAlArma = true;
            }
        }

    }

    //Teleporte
    private float antesDeIr;
    private bool inicio = false;
    Vector2 mouseP;

    private void Teleportacion () {
        if (EnLaHabilidad == false) {
            if (Vector3.Distance (gameObject.transform.position, mira.position) <= 18 && Input.GetButtonDown ("Dash") && inicio == false && noAquiNo == false) {
                mouseP = mira.position;
                Instantiate (estrellaTeleport, mira.position, mira.rotation);

                MovimientoEnProceso = true;
                canMoeve = false;
                tiempoDeLaHabilidad = 1.1f;
                antesDeIr = 0.7f;

                inicio = true;
            }
            if (antesDeIr > 0) {
                rb2d.gravityScale = 0;
                rb2d.velocity = new Vector2 (0, 0);
                antesDeIr -= Time.deltaTime;
                rb2d.velocity = new Vector2 (0, 0);
            }
            if (antesDeIr <= 0 && inicio == true) {
                rb2d.gravityScale = 4;
                gameObject.transform.position = mouseP;
                inicio = false;
                puedeAccederAlArma = true;
                tiempoColddownMovimiento = 3;
            }
        }

    }

    private bool saltoActivado;
    public GameObject balaVer;
    private bool dobleSalto = true;
    void SaltoVertical () {
        if (grounded == true) {
            dobleSalto = false;
            saltoActivado = false;
        }

        if (antesDeIr != 0) {
            antesDeIr -= Time.deltaTime;
        }
        if (dobleSalto == false && grounded == false && jump == false) {
            if (Input.GetButtonDown ("Dash") || Input.GetButtonDown ("Jump")) {
                antesDeIr = 0.28f;
                MovimientoEnProceso = true;
                tiempoDeLaHabilidad = 0.65f;
                dobleSalto = true;
                saltoActivado = true;

            }
        }

        if (grounded == true) {
            MovimientoEnProceso = false;
            saltoActivado = false;
        }

        if (antesDeIr <= 0 && dobleSalto == true && saltoActivado == true) {
            puedeAccederAlArma = true;
            tiempoColddownMovimiento = 2;
            rb2d.AddForce (Vector2.up * 300, ForceMode2D.Impulse);
            Instantiate (balaVer, new Vector2 (gameObject.transform.position.x, gameObject.transform.position.y - 2), Quaternion.identity);
            dobleSalto = false;

        }
    }

    private bool acelerao = false;
    private float nitroTime;
    void Aceleracion () {

        if (Input.GetButtonDown ("Dash") && acelerao == false) {
            acelerao = true;
            nitroTime = Time.time + 10;
            tiempoColddownMovimiento = 2;
            speed = 18;
            puwerJump = 60;
            if (nitroTime <= Time.time) {
                speed = 8;
                puwerJump = 20;
            }
        }
    }

    private void ComprovadoresDeInputs () {
        saltoComprovante = Input.GetButton ("Jump");
    }

    public void ResivirDaño (float daño) {

        if (escudoActivo == false) {
            vida -= daño;
        }

        if (escudoActivo == true) {
            vidaEscudo -= daño;
        }

        while (vidaEscudo < 0) {
            vidaEscudo++;
            vida--;
        }
    }

    public void Impulsos (float X, float Y) {
        rb2d.AddForce (Vector2.right * X, ForceMode2D.Impulse);
        rb2d.AddForce (Vector2.up * Y, ForceMode2D.Impulse);
    }

    private void OnDrawGizmos () {
        Gizmos.DrawLine (gameObject.transform.position, mira.position);
        Gizmos.DrawWireSphere (meleePos.position, meleeRange);
        Gizmos.DrawWireSphere (pisoT.position, areaPiso);
        Gizmos.DrawWireSphere (mira.position, radioDeDenegacion);
    }
}