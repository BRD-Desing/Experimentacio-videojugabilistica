using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class brazocontroller : MonoBehaviour {

    public bool GamePad;
    public GameObject contenedorMira;

    public float municion;
    float municionMaxima;
    public Text textoEnPantallaMuniciones;

    public float distanciaEntreElArma;
    public Vector3 MousePosition, ObjetoPosition;
    public float angulo;
    public float tiempo = 1f;
    private float rotarX;
    public Transform mira;

    public bool comprovador = false;
    public bool trans;
    public bool invis = false;
    private bool meleeMode;

    public bool tocandoSuperficie;
    public float radio;

    public bool meleeMo = false;

    public int tipoDeBala;
    public int tipoHabilidadDisparo;

    public float tiempoDeRecarga;
    private bool puedeDisparar;
    private bool cambiarDeArma;
    private float esperateTantito;
    private bool DisparoArrba;

    public float tiempoEsperaOutsled;
    private float tiempoEsperaZero;
    private float tiempoEntreBalas32;

    private float tiempoEntreBalas;

    //laser shotting
    public float timeBeforeFire;
    public float distanciaDelLaser;
    private float particleSpeed;
    public float dañoPorSegundo;
    public float dañoPorSegundoAtras;
    public ParticleSystem inicioDelLaser, laser, backLaser, finDelLaser, finDelBackLaser;

    public int armaPrimaria, armaSecundaria;
    public float balasPrimaria, balasSecundaria;

    public GameObject disparo;
    public GameObject esferaMeta;
    public GameObject balaDeMU4;
    public GameObject balaRevolver;
    public GameObject AK7bullet;
    public GameObject ZERObullet;
    public GameObject bala32;
    public GameObject BashBullet;
    public GameObject configVBullet;
    public GameObject PrizmaBullet;
    public GameObject Prizma;
    public GameObject V14CkBullet;
    public GameObject MinaP;
    public GameObject ActivadorP;
    public GameObject ACBullet;

    //disparos especiales
    public GameObject plasmaGame;
    public GameObject pesonaje;
    public LayerMask queEsTierra;
    public LayerMask enemigo;
    public LayerMask superficies;
    public Transform generador;
    public Transform MU4Point;
    public Transform Point32;

    public SpriteRenderer render;
    public Animator anim;
    public AudioSource au;

    private float tiempoEspera;
    private float delayT;
    private float coldownFuego;

    public bool habilidadDeMovimiento;

    bool unaVez;
    bool rotarBrazo;
    private float pocisionDeLaY;

    //Balas por segundo
    public int BFS;
    private bool clic;
    public bool clk;
    //meta = metamorfosis
    private bool meta;

    //Disparo del rifle
    public float tiempoEntreBalasDeEsnipers;
    private float referenciaSniper;

    public GameObject sniperBullet;

    public bool disparando;
    public int casilla;

    public int cargaDelPrizma;
    private bool yaDisparó;
    private float cadenciaDeFuego;

    private void Start () {
        render = GetComponent<SpriteRenderer> ();
        anim = GetComponent<Animator> ();
        unaVez = true;
        rotarBrazo = true;
        pocisionDeLaY = generador.position.y;
        DisparoArrba = true;
        llamarElTrigger = true;

        anim.SetTrigger ("CambioDeArma");
        pesonaje.GetComponent<Animator> ().SetTrigger ("MeleeMode");
        inicioDelLaser.Pause ();
        laser.Pause ();
        backLaser.Pause ();

        tipoDeBala = 1;
        municion = 30;

        cargaDelPrizma = 0;
        yaDisparó = false;
        municionMaxima = municion;

    }

    public bool llamarElTrigger;
    private bool dispararConDelay;
    public int armaActual;
    // Update is called once per frame
    void FixedUpdate () {

        contenedorMira.transform.position = gameObject.transform.position;

        if (mira.position.x > gameObject.transform.position.x) {
            rotarX = 0;
            angulo = Mathf.Atan2 ((MousePosition.y - ObjetoPosition.y), (MousePosition.x - ObjetoPosition.x)) * Mathf.Rad2Deg;
        } else if (mira.position.x < gameObject.transform.position.x) {
            rotarX = 180;
            angulo = (Mathf.Atan2 ((MousePosition.y - ObjetoPosition.y), (MousePosition.x - ObjetoPosition.x)) * Mathf.Rad2Deg) * -1;
        }

        var laserMain = laser.main;
        var backLaserMain = backLaser.main;
        //Personaje
        anim.SetBool ("invis", invis);
        anim.SetBool ("MeleeMod", meleeMo);
        anim.SetInteger ("tipoDisparo", tipoDeBala);
        anim.SetBool ("disparando", disparando);
        anim.SetBool ("plasma", plasma);
        anim.SetInteger ("sobrecarga", cargaDelPrizma);

        pesonaje.GetComponent<Animator> ().SetBool ("meleeMode", meleeMo);

        textoEnPantallaMuniciones.text = municion.ToString ();

        if (municion <= (municionMaxima / 100) * 10) {
            textoEnPantallaMuniciones.color = Color.red;
        } else {
            textoEnPantallaMuniciones.color = Color.white;
        }

        tipoHabilidadDisparo = pesonaje.GetComponent<playercontroller> ().TipoUlti;
        habilidadDeMovimiento = pesonaje.GetComponent<playercontroller> ().MovimientoEnProceso;
        enLaHabilidad = pesonaje.GetComponent<playercontroller> ().EnLaHabilidad;

        if (habilidadDeMovimiento == false) {
            invis = enLaHabilidad;
        } else if (habilidadDeMovimiento == true) {
            invis = habilidadDeMovimiento;
        } else if (meleeMo == true) {
            CancelInvoke ("DisparoRapido");
            CancelInvoke ("EsferaMetamorfosis");
            invis = meleeMo;
        }

        if (habilidadDeMovimiento == false && enLaHabilidad == true && meleeMo == false) {
            puedeDisparar = true;
        } else if (habilidadDeMovimiento == true || enLaHabilidad == true || meleeMo == true) {
            inicioDelLaser.Clear ();
            inicioDelLaser.Pause ();
            laser.Clear ();
            laser.Pause ();
            backLaser.Clear ();
            backLaser.Pause ();
            tiempoDeRecarga = pesonaje.GetComponent<playercontroller> ().tiempoDeEsperaAntesDeDisparar;
            puedeDisparar = false;
        }

        if (armaActual != tipoDeBala && meleeMo == false) {
            anim.SetTrigger ("CambioDeArma");
            armaActual = tipoDeBala;
        }

        tocandoSuperficie = Physics2D.OverlapCircle (generador.transform.position, radio, queEsTierra);

        if (Input.GetButtonDown ("CambioDeArma") && tipoDeBala != armaPrimaria && Time.time > esperateTantito) {
            tipoDeBala = armaPrimaria;
            balasSecundaria = municion;
            llamarElTrigger = true;
            esperateTantito = Time.time + 0.2f;
            CancelInvoke ("DisparoRapido");
            CancelInvoke ("EsferaMetamorfosis");

            switch (tipoDeBala) {
                case 1:
                    municion = 20;
                    tiempoDeRecarga = Time.time + 0.2f;
                    break;
                case 2:
                    municion = 90;
                    BFS = 0;
                    tiempoDeRecarga = Time.time + 0.75f;
                    break;
                case 3:
                    municion = 15;
                    tiempoDeRecarga = Time.time + 1;
                    break;
                case 4:
                    municion = 300;
                    tiempoDeRecarga = Time.time + 1.5f;
                    break;
                case 5:
                    municion = 120;
                    tiempoDeRecarga = Time.time + 0.6f;
                    break;
                case 6:
                    municion = 30;
                    tiempoDeRecarga = Time.time + 0.9f;
                    break;
                case 7:
                    municion = 140;
                    tiempoDeRecarga = Time.time + 0.3f;
                    break;
                case 8:
                    municion = 35;
                    tiempoDeRecarga = Time.time + 1.2f;
                    break;
                case 9:
                    municion = 150;
                    tiempoDeRecarga = Time.time + 0.75f;
                    break;
                case 10:
                    municion = 50;
                    tiempoDeRecarga = Time.time + 1.45f;
                    break;
                case 11:
                    municion = 50;
                    tiempoDeRecarga = Time.time + 1;
                    break;
                case 12:
                    municion = 30;
                    tiempoDeRecarga = Time.time + 0.85f;
                    break;
                case 13:
                    municion = 36;
                    tiempoDeRecarga = Time.time + 1.3f;
                    break;
                case 14:
                    municion = 46;
                    tiempoDeRecarga = Time.time + 1f;
                    BFS = 1;
                    break;
                case 16:
                    municion = 140;
                    cadenciaDeFuego = 0.25f;
                    tiempoDeRecarga = Time.time + 2f;
                    break;
            }

            municionMaxima = municion;

        } else if (Input.GetButtonDown ("CambioDeArma") && tipoDeBala != armaSecundaria && Time.time > esperateTantito) {
            tipoDeBala = armaSecundaria;
            balasPrimaria = municion;
            llamarElTrigger = true;
            esperateTantito = Time.time + 0.2f;
            CancelInvoke ("DisparoRapido");
            CancelInvoke ("EsferaMetamorfosis");

            switch (tipoDeBala) {
                case 1:
                    municion = 20;
                    tiempoDeRecarga = Time.time + 0.2f;
                    break;
                case 2:
                    municion = 90;
                    BFS = 0;
                    tiempoDeRecarga = Time.time + 0.75f;
                    break;
                case 3:
                    municion = 15;
                    tiempoDeRecarga = Time.time + 1;
                    break;
                case 4:
                    municion = 300;
                    tiempoDeRecarga = Time.time + 1.5f;
                    break;
                case 5:
                    municion = 120;
                    tiempoDeRecarga = Time.time + 0.6f;
                    break;
                case 6:
                    municion = 30;
                    tiempoDeRecarga = Time.time + 0.9f;
                    break;
                case 7:
                    municion = 140;
                    tiempoDeRecarga = Time.time + 0.3f;
                    break;
                case 8:
                    municion = 35;
                    tiempoDeRecarga = Time.time + 1.2f;
                    break;
                case 9:
                    municion = 150;
                    tiempoDeRecarga = Time.time + 0.75f;
                    break;
                case 10:
                    municion = 50;
                    tiempoDeRecarga = Time.time + 1f;
                    break;
                case 11:
                    municion = 50;
                    tiempoDeRecarga = Time.time + 1;
                    break;
                case 12:
                    municion = 30;
                    tiempoDeRecarga = Time.time + 0.85f;
                    break;
                case 13:
                    municion = 36;
                    tiempoDeRecarga = Time.time + 1.3f;
                    break;
                case 14:
                    municion = 46;
                    tiempoDeRecarga = Time.time + 1f;
                    BFS = 1;
                    break;
                case 16:
                    municion = 140;
                    cadenciaDeFuego = 0.25f;
                    tiempoDeRecarga = Time.time + 2f;
                    break;
            }

            municionMaxima = municion;
        }

        if (municion <= 0) {
            if (meleeMo == false) {
                meleeMo = true;
                pesonaje.GetComponent<Animator> ().SetTrigger ("MeleeMode");
                laser.Clear ();
                laser.Pause ();
                backLaser.Clear ();
                backLaser.Pause ();
                inicioDelLaser.Clear ();
                inicioDelLaser.Pause ();
                finDelLaser.Pause ();
                finDelLaser.Clear ();
                finDelBackLaser.Pause ();
                finDelBackLaser.Clear ();
            }
        } else {
            if (meleeMo == true && municion > 0) {
                meleeMo = false;
                pesonaje.GetComponent<Animator> ().SetTrigger ("MeleeOff");
            }
        }

        if (meleeMo == false && invis == false) {
            //Mira moviendose
            if (GamePad == true) {
                ObjetoPosition = transform.position;
                MousePosition = GameObject.FindGameObjectWithTag ("mira").transform.position;
            } else if (GamePad == false) {
                ObjetoPosition = Camera.main.WorldToScreenPoint (transform.position);
                MousePosition = Input.mousePosition;
            }

            transform.rotation = Quaternion.Euler (new Vector3 (rotarX, 0, angulo));

            if (llamarElTrigger == true) {
                anim.SetTrigger ("CambioDeArma");
                llamarElTrigger = false;
            }

            if (tiempoDeRecarga > Time.time) {
                puedeDisparar = false;
                inicioDelLaser.Clear ();
                inicioDelLaser.Pause ();
                laser.Clear ();
                laser.Pause ();
                backLaser.Clear ();
                backLaser.Pause ();
            } else if (tiempoDeRecarga <= Time.time) {
                puedeDisparar = true;
            }

            if (puedeDisparar == true) {
                if (tipoDeBala == 2) {
                    clic = clk;
                }
                if (tipoDeBala == 4) {
                    clic = disparando;
                }

                disparando = Input.GetButton ("Fire1");

                //reduccion de tiempos
                if (tiempoEspera > 0) {
                    tiempoEspera -= Time.deltaTime;
                }
                if (tocandoSuperficie == false) {

                    //entrada de disparo
                    if (tipoDeBala == 1 && enLaHabilidad == false) {
                        Disparo1 ();
                    }

                    //   MMMMM
                    //  MM   MM
                    //      MM
                    //    MMM
                    //  MMMMMMMM
                    if (tipoDeBala == 2) {
                        if (delayT > 0) {
                            delayT -= Time.deltaTime;
                            tiempoEspera = 0.0001f;
                        } else if (delayT <= 0) {
                            BFS = 0;
                        }
                        if (coldownFuego > 0) {
                            coldownFuego -= Time.deltaTime;
                        }
                        if (tocandoSuperficie == true || delayT <= 0) {
                            CancelInvoke ("EsferaMetamorfosis");
                        }
                        if (BFS == 1 && delayT <= 0) {
                            coldownFuego = 0.5f;
                            clk = false;
                        }
                        if (invis == false && enLaHabilidad == false && municion >= 3) {
                            meta = true;
                            if (disparando && tocandoSuperficie == false) {
                                if (invis == false && delayT <= 0 && coldownFuego <= 0 && clic == false) {
                                    municion -= 3;
                                    BFS = 1;
                                    delayT = 0.3f;
                                    clk = true;
                                }
                                if (BFS == 1 && delayT > 0 && tiempoEspera <= 0) {
                                    InvokeRepeating ("EsferaMetamorfosis", 0.0001f, 0.13f);
                                }
                            }

                        }
                    }
                    //  MMMMMM
                    // MM    MM
                    //     MMM
                    // MM    MM
                    //  MMMMMM

                    if (tipoDeBala == 3 && invis == false) {
                        if (referenciaSniper <= Time.time && disparando == true) {
                            municion -= 1;
                            referenciaSniper = Time.time + tiempoEntreBalasDeEsnipers;
                            Instantiate (sniperBullet, generador.position, generador.rotation);
                        }
                    }

                    //  MMMMMM
                    //  M
                    //  MMMMMM
                    //      MMM
                    //  MMMMMM
                    if (tipoDeBala == 5) {
                        clic = disparando;
                        if (disparando == true && tiempoEntreBalas <= Time.time && municion >= 1) {
                            Instantiate (balaDeMU4, MU4Point.position, MU4Point.rotation);
                            municion -= 1;
                            tiempoEntreBalas = Time.time + 0.15f;
                        }
                    }

                    //   MMMMMM
                    //   MM
                    //   MMMMMM
                    //   MM  MM
                    //   MMMMMM

                    if (tipoDeBala == 6) {
                        clic = false;
                        if (disparando == true && Time.time > tiempoEsperaOutsled && municion >= 1) {
                            clic = true;
                            municion -= 1;
                            Instantiate (balaRevolver, MU4Point.position, MU4Point.rotation);
                            tiempoEsperaOutsled = Time.time + 0.7f;
                        }
                    }

                    //  MMMMMM
                    //     MM
                    //    MM
                    //   MM
                    //   MM

                    if (tipoDeBala == 7) {
                        clic = disparando;
                        if (disparando == true && Time.time >= tiempoEntreBalas && municion >= 1) {
                            Instantiate (AK7bullet, generador.position, generador.rotation);
                            tiempoEntreBalas = Time.time + 0.05f;
                        }
                    }

                    //  MMMMMM
                    //  MM  MM
                    //  MMMMMM
                    //  MM  MM
                    //  MMMMMM

                    if (tipoDeBala == 8) {
                        clic = false;
                        if (disparando == true && Time.time > tiempoEsperaZero && municion >= 1) {
                            clic = true;
                            municion -= 1;
                            Instantiate (ZERObullet, generador.position, generador.rotation);
                            tiempoEsperaZero = Time.time + 0.7f;
                        }
                    }

                    //  MMMMMM
                    //  MM  MM
                    //  MMMMMM
                    //      MM
                    //      MM

                    if (tipoDeBala == 9) {
                        clic = disparando;
                        if (disparando == true) {
                            if (DisparoArrba == true && tiempoEntreBalas32 <= Time.time && municion >= 1) {
                                municion -= 1;
                                Instantiate (bala32, MU4Point.position, MU4Point.rotation);
                                DisparoArrba = false;
                                tiempoEntreBalas32 = Time.time + 0.06f;
                            }
                            if (DisparoArrba == false && tiempoEntreBalas32 <= Time.time) {
                                municion -= 1;
                                Instantiate (bala32, Point32.position, Point32.rotation);
                                DisparoArrba = true;
                                tiempoEntreBalas32 = Time.time + 0.06f;
                            }
                        }
                    }

                    //    MM   MM 
                    //  MMMM MMMM
                    //    MM   MM
                    //    MM   MM
                    //    MM   MM
                    if (tipoDeBala == 11) {
                        if (delayT >= Time.time && tiempoEntreBalas <= Time.time && tiempoEsperaOutsled >= Time.time) {
                            tiempoEntreBalas = Time.time + 0.18f;
                            municion -= 1;
                            Instantiate (configVBullet, Point32.position, Point32.rotation);
                        }

                        if (tiempoEntreBalas <= Time.time && delayT <= Time.time && disparando == true && tiempoEsperaOutsled <= Time.time) {
                            clic = true;
                            delayT = Time.time + 0.18f;
                            tiempoEsperaOutsled = Time.time + 1;
                        } else if (tiempoEsperaOutsled > Time.time && delayT <= Time.time) {
                            clic = false;
                        }
                    }

                    //   MM  MMMMM
                    // MMMM      MM
                    //   MM   MMMM
                    //   MM      MM
                    //   MM  MMMMM

                    if (tipoDeBala == 13) {
                        if (disparando == true && tiempoEsperaOutsled <= Time.time && municion > 0) {
                            anim.SetTrigger ("dispara");
                            Instantiate (V14CkBullet, generador.position, generador.rotation);
                            tiempoEsperaOutsled = Time.time + 0.5f;
                            municion -= 1;
                        }
                    }

                    //   MM      MMM                  
                    // MMMM    MM MM
                    //   MM   MM  MM
                    //   MM  MMMMMMMMM
                    //   MM       MM

                    if (tipoDeBala == 14) {
                        if (disparando == true && tiempoEntreBalas32 <= Time.time && BFS == 1 && municion > 0) {
                            anim.SetTrigger ("dispara");
                            tiempoEntreBalas32 = Time.time + 0.45f;
                            municion -= 1;
                            Instantiate (MinaP, generador.position, generador.rotation);
                            BFS = 2;
                        } else if (disparando == true && tiempoEntreBalas32 <= Time.time && BFS == 2 && municion > 0) {
                            anim.SetTrigger ("dispara");
                            tiempoEntreBalas32 = Time.time + 0.45f;
                            municion -= 1;
                            Instantiate (ActivadorP, generador.position, generador.rotation);
                            BFS = 1;
                        }

                        if (disparando == false) {
                            anim.ResetTrigger ("dispara");
                        }
                    }

                    //   MM  MMMMM            
                    // MMMM  MM
                    //   MM  MMMMMM
                    //   MM  MM   M
                    //   MM  MMMMMM

                    if (tipoDeBala == 16) {
                        if (disparando == true && tiempoEntreBalas32 <= Time.time && municion > 0) {
                            tiempoEntreBalas32 = Time.time + cadenciaDeFuego;
                            municion -= 1;
                            anim.SetTrigger ("dispara");
                            Instantiate (ACBullet, new Vector2 (MU4Point.position.x, MU4Point.position.y - 0.1f), MU4Point.rotation);
                            if (cadenciaDeFuego > 0.05f) {
                                cadenciaDeFuego -= 0.01f;
                            }
                        }
                        if (disparando == false) {
                            anim.ResetTrigger ("dispara");
                            cadenciaDeFuego = 0.25f;
                        }
                    }
                }

                //      MMM
                //    MM MM
                //   MM  MM
                //  MMMMMMMMM
                //       MM

                if (tipoDeBala == 4) {
                    if (disparando == false || municion < 0) {
                        laserMain.startLifetime = 0;
                        backLaserMain.startLifetime = 0;
                        finDelLaser.Pause ();
                        finDelLaser.Clear ();
                        finDelBackLaser.Pause ();
                        finDelBackLaser.Clear ();
                        if (timeBeforeFire < 0.5) {
                            timeBeforeFire += Time.deltaTime;
                        } else {
                            inicioDelLaser.Clear ();
                            inicioDelLaser.Pause ();
                        }

                    }

                    if (disparando == true && municion > 0) {
                        inicioDelLaser.Play ();

                        if (timeBeforeFire > 0) {
                            timeBeforeFire -= Time.deltaTime;
                        }
                        if (timeBeforeFire <= 0) {

                            municion -= 0.25f;

                            RaycastHit2D detectaAlgoFrente = Physics2D.Raycast (laser.transform.position, laser.transform.right, distanciaDelLaser, superficies);
                            RaycastHit2D detectaAlgoAtras = Physics2D.Raycast (generador.position, generador.right * -1, (distanciaDelLaser / 2) + 2, superficies);

                            RaycastHit2D enemyFrente = Physics2D.Raycast (laser.transform.position, laser.transform.right, distanciaDelLaser, enemigo);
                            RaycastHit2D enemyAtras = Physics2D.Raycast (generador.position, generador.right * -1, (distanciaDelLaser / 2) + 2, enemigo);

                            if (enemyFrente) {
                                EnemyController vidaEnemiga = enemyFrente.transform.GetComponent<EnemyController> ();
                                if (vidaEnemiga != null) {
                                    if (dañoPorSegundo < 3.2) {
                                        dañoPorSegundo += 0.01f;
                                    }
                                    vidaEnemiga.Damage (dañoPorSegundo);
                                    if (vidaEnemiga.vida <= 0) {
                                        dañoPorSegundo = 0;
                                    }
                                }
                            }
                            if (enemyAtras) {
                                EnemyController vidaEnemigaAtras = enemyAtras.transform.GetComponent<EnemyController> ();
                                if (vidaEnemigaAtras != null) {
                                    if (dañoPorSegundo < 4.5) {
                                        dañoPorSegundoAtras += 0.01f;
                                    }
                                    vidaEnemigaAtras.Damage (dañoPorSegundoAtras);
                                    if (vidaEnemigaAtras.vida <= 0) {
                                        dañoPorSegundoAtras = 0;
                                    }
                                }
                            }

                            if (detectaAlgoFrente) {
                                finDelLaser.Play ();
                                finDelLaser.transform.position = detectaAlgoFrente.point;
                                if (Vector2.Distance (laser.transform.position, detectaAlgoFrente.point) < 10) {
                                    laserMain.startLifetime = (Vector2.Distance (laser.transform.position, detectaAlgoFrente.point) / laser.startSpeed) + 0.005f;
                                }
                            } else {
                                laserMain.startLifetime = 0.12f;
                                finDelLaser.Pause ();
                                finDelLaser.Clear ();
                            }

                            if (detectaAlgoAtras) {
                                finDelBackLaser.Play ();
                                finDelBackLaser.transform.position = detectaAlgoAtras.point;

                                distanciaEntreElArma = (Vector2.Distance (backLaser.transform.position, detectaAlgoAtras.point) / backLaser.startSpeed) * -1;
                                if (Vector2.Distance (backLaser.transform.position, detectaAlgoAtras.point) < 5.92) {
                                    backLaserMain.startLifetime = (Vector2.Distance (backLaser.transform.position, detectaAlgoAtras.point) / backLaser.startSpeed) * -1;
                                }
                            } else {
                                backLaserMain.startLifetime = 0.065f;
                                finDelBackLaser.Pause ();
                                finDelBackLaser.Clear ();
                            }

                            laser.Play ();
                            backLaser.Play ();
                        }
                    }
                }

                //  MMM  MMMMMM
                //   MM  MM  MM
                //   MM  MM  MM
                //   MM  MM  MM
                //  MMMM MMMMMM

                if (tipoDeBala == 10) {
                    clic = disparando;
                    if (disparando == true && dispararConDelay == true && delayT <= Time.time && municion > 0) {
                        delayT = Time.time + 0.385f;
                        anim.SetTrigger("dispara");
                        dispararConDelay = false;
                    }
                    if (disparando == true && tiempoEntreBalas32 <= Time.time && delayT <= Time.time) {
                        Instantiate (BashBullet, MU4Point.position, MU4Point.rotation);
                        anim.SetTrigger("dispara");
                        tiempoEntreBalas32 = Time.time + 0.385f;
                        municion -= 1;
                    }

                    if (disparando == false) {
                        anim.ResetTrigger("dispara");
                        dispararConDelay = true;
                    }
                }

                //   MM   MMMMM
                // MMMM  MM   MM
                //   MM      MM
                //   MM    MMM
                //   MM  MMMMMMMM

                if (tipoDeBala == 12) {
                    if (delayT <= Time.time && tiempoEsperaOutsled <= Time.time) {
                        if (cargaDelPrizma <= 2 && disparando == true && yaDisparó == true) {
                            delayT = Time.time + 0.25f;
                            municion -= 1;
                            anim.SetTrigger ("dispara");
                            yaDisparó = false;
                        } else if (cargaDelPrizma == 3 && disparando == true && yaDisparó == true) {
                            delayT = Time.time + 0.3f;
                            municion -= 1;
                            anim.SetTrigger ("dispara");
                            yaDisparó = false;
                        }
                    }

                    if (cargaDelPrizma == 4) {
                        cargaDelPrizma = 0;
                    }

                    if (delayT <= Time.time && yaDisparó == false && tiempoEsperaOutsled <= Time.time) {
                        yaDisparó = true;
                        if (cargaDelPrizma <= 2) {
                            cargaDelPrizma++;
                            Instantiate (PrizmaBullet, generador.position, generador.rotation);
                            tiempoEsperaOutsled = Time.time + 0.3f;
                            int color = Random.Range (1, 5);
                            switch (color) {
                                case 1:
                                    PrizmaBullet.GetComponent<SpriteRenderer> ().color = Color.red;
                                    break;
                                case 2:
                                    PrizmaBullet.GetComponent<SpriteRenderer> ().color = Color.blue;
                                    break;
                                case 3:
                                    PrizmaBullet.GetComponent<SpriteRenderer> ().color = Color.green;
                                    break;
                                case 4:
                                    PrizmaBullet.GetComponent<SpriteRenderer> ().color = Color.yellow;
                                    break;
                                case 5:
                                    PrizmaBullet.GetComponent<SpriteRenderer> ().color = Color.white;
                                    break;
                            }
                        } else {
                            Instantiate (Prizma, generador.position, generador.rotation);
                            cargaDelPrizma++;
                            tiempoEsperaOutsled = Time.time + 1f;
                        }
                    }
                }

            }
            Plasma ();
        }
    }

    //Variables de disparos normales [importante]

    //variable del disparo 1

    void Disparo1 () {
        if (enLaHabilidad == false && invis == false) {
            if (Input.GetButtonDown ("Fire1") && invis == false && tiempoEntreBalas <= Time.time) {
                municion -= 1;
                Instantiate (disparo, generador.position, generador.rotation);
                tiempoEntreBalas = Time.time + 0.1f;
            }
        }
    }

    //Disparo 2
    private void EsferaMetamorfosis () {
        Instantiate (esferaMeta, generador.position, generador.rotation);
    }
    //Disparo del plasma
    private void DisparoPlasma () {
        Instantiate (plasmaGame, generador.position, generador.rotation);
    }

    private float tiempoEnLaHabilidad;
    private bool enLaHabilidad;
    private bool plasma;

    void Plasma () {
        if (tiempoEntreBalas > 0) {
            tiempoEntreBalas -= Time.deltaTime;
        }
        if (tipoHabilidadDisparo == 2) {
            if (tiempoEnLaHabilidad > 0) {

                if (Input.GetButtonDown ("Fire1") && tiempoEntreBalas <= 0) {
                    DisparoPlasma ();

                    tiempoEntreBalas = 0.5f;
                }
                anim.SetTrigger ("CambioDeArma");

                tiempoEnLaHabilidad -= Time.deltaTime;
                enLaHabilidad = true;
            }

            if (tiempoEnLaHabilidad <= 0) {
                enLaHabilidad = false;
                plasma = false;
                clic = false;
            }

            if (Input.GetButtonDown ("Fire3") && tiempoEnLaHabilidad <= 0 && clic == false) {
                clic = true;
                anim.SetTrigger ("Cambio");
                plasma = true;
                tiempoEnLaHabilidad = 10;
            }
        }
    }
    private void OnDrawGizmos () {
        Gizmos.DrawWireSphere (generador.transform.position, radio);
    }
}