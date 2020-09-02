using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

    public float enemyLife;
    public float vida;

    public float radioDeAtaque;
    public float dañoAlJugador;
    public float empujonX, empujonY;

    public float referenciaAlTiempoDeAtaque;
    public float delayParaAtacar;
    public float tiemopEntreAtaques;
    private float tiempoEspera;
    public float distanciaParaAtacar = 2;

    public float radioCircunferenciaDeLaPelota;

public float tiempoParalizado; 
    private bool tocandoAlJugador;
    private bool atacando;
    private bool puedeAtacarAlJugador;
    public float tiempoDeAtaque;
    private bool puedeEmpujarAlJugador;

    public float direccion = 1f;
    public float velocidadX;

    public float enemySpeed;
    public float fuerzaDeMovimiento;

    private float fuerzaEnX;
    public float jumpPower;
    public Vector2 pos;

    public bool grounded;
    public float tiempoStuneo;

    private bool puedeRotar;
    public bool tocaMuro;
    public float radioParaTocarMuro;

    public LayerMask layer;
    private Rigidbody2D enemy;
    private Animator anim;
    private SpriteRenderer render;

    public GameObject player;
    private Transform playerPosition;

    public Transform comprovadorPiso;
    public Transform puntoParaTocarMuro;

    public float radioPiso;
    public LayerMask queEsTierra;
    public LayerMask muro;
    public LayerMask queEsJugador;
    private void Awake () {
        player = GameObject.FindGameObjectWithTag ("CY");

    }

    // Use this for initialization
    void Start () {
        enemy = GetComponent<Rigidbody2D> ();
        anim = GetComponent<Animator> ();

        vida = enemyLife;
        playerPosition = player.GetComponent<Transform> ();

        puedeAtacarAlJugador = true;

    }

    private void FixedUpdate () {

        if (tiempoEspera > 0) {
            tiempoEspera -= Time.deltaTime;
        }

        if (gameObject.tag == "enemy") {
            grounded = Physics2D.OverlapCircle (comprovadorPiso.position, radioPiso, queEsTierra);
            tocaMuro = Physics2D.Raycast (puntoParaTocarMuro.position, puntoParaTocarMuro.right, radioParaTocarMuro, muro);

            if (Vector2.Distance (gameObject.transform.position, playerPosition.position) <= distanciaParaAtacar && atacando == false && tiempoEspera <= Time.time && tiempoStuneo <= Time.time) {
                tiempoDeAtaque = referenciaAlTiempoDeAtaque;
                atacando = true;
                puedeAtacarAlJugador = true;
                puedeEmpujarAlJugador = true;
            }

            if (tiempoDeAtaque > 0 && tiempoStuneo <= Time.time) {
                atacando = true;
                tiempoDeAtaque -= Time.deltaTime;
                enemy.velocity = new Vector2 (0, enemy.velocity.y);
                AtacarAlJugador ();
                EmpujarAlJugador (empujonX * direccion, empujonY);
                if (playerPosition.position.x < gameObject.transform.position.x) {
                    if (puedeRotar == false) {
                        Rotar ();
                    }
                } else if (playerPosition.position.x > gameObject.transform.position.x) {
                    if (puedeRotar == true) {
                        Rotar ();
                    }
                }
            } else if (tiempoDeAtaque <= 0) {
                atacando = false;
            }

            pos = playerPosition.position;

            if (atacando == false && tiempoStuneo <= Time.time) {
                //Movimiento del enemigo
                if (playerPosition.position.y >= gameObject.transform.position.y - 2.5 && playerPosition.position.y <= gameObject.transform.position.y + 2.5) {
                    if (Vector2.Distance (gameObject.transform.position, playerPosition.position) > 2.5) {
                        enemy.velocity = new Vector2 (enemySpeed * direccion, enemy.velocity.y);

                        if (playerPosition.position.x < gameObject.transform.position.x) {
                            direccion = -1;
                            if (puedeRotar == false) {
                                Rotar ();
                            }
                        } else if (playerPosition.position.x > gameObject.transform.position.x) {
                            direccion = 1;
                            if (puedeRotar == true) {
                                Rotar ();
                            }
                        }
                    }
                } else {
                    enemy.velocity = new Vector2 (enemySpeed * direccion, enemy.velocity.y);

                    if (tocaMuro) {
                        Rotar ();
                    }

                }

                if (grounded == true && tocaMuro == false && velocidadX < 0.1f) {
                    enemy.AddForce (gameObject.transform.up * jumpPower, ForceMode2D.Impulse);
                    fuerzaEnX = 0;
                }
            }

        }

        //    MMMMMM
        //    MM  MM
        //    MMMMM
        //    MM  MM
        //    MM   MM

        if (gameObject.tag == "rapido") {

            grounded = Physics2D.OverlapCircle (comprovadorPiso.position, radioPiso, queEsTierra);
            tocaMuro = Physics2D.Raycast (puntoParaTocarMuro.position, puntoParaTocarMuro.right, radioParaTocarMuro, muro);

            pos = playerPosition.position;

            enemy.velocity = new Vector2 (fuerzaEnX * direccion, enemy.velocity.y);

            if (Mathf.Abs (enemy.velocity.x) < 18) {
                fuerzaEnX += fuerzaDeMovimiento;
            }

            if (Vector2.Distance (gameObject.transform.position, playerPosition.position) <= 2 && atacando == false && Mathf.Abs (enemy.velocity.x) < 12 && tiempoEspera <= Time.time && tiempoStuneo <= Time.time) {
                tiempoDeAtaque = referenciaAlTiempoDeAtaque;
                atacando = true;
                puedeAtacarAlJugador = true;
                puedeEmpujarAlJugador = true;
            } else if (Vector2.Distance (gameObject.transform.position, playerPosition.position) <= 2 && atacando == false && Mathf.Abs (enemy.velocity.x) >= 12.01f && tiempoEspera <= Time.time) {
                Collider2D jugador = Physics2D.OverlapCircle (new Vector2 (gameObject.transform.position.x, gameObject.transform.position.y - 1.25f), radioCircunferenciaDeLaPelota, queEsJugador);
                if (jugador != null) {
                    tiempoEspera = Time.time + tiemopEntreAtaques;
                    jugador.GetComponent<playercontroller> ().ResivirDaño (25);
                    jugador.GetComponent<playercontroller> ().Impulsos (20 * direccion, 10);
                }
                enemy.velocity = new Vector2 (0, enemy.velocity.y);
            }

            if (tiempoDeAtaque > 0 && tiempoStuneo <= Time.time) {
                atacando = true;
                tiempoDeAtaque -= Time.deltaTime;
                enemy.velocity = new Vector2 (0, enemy.velocity.y);
                AtacarRapidoAlJugador ();
                 if (playerPosition.position.x < gameObject.transform.position.x) {
                        direccion = -1;
                        if (puedeRotar == false) {
                            Rotar ();
                            fuerzaEnX = 0;
                        }
                    } else if (playerPosition.position.x > gameObject.transform.position.x) {
                        direccion = 1;
                        if (puedeRotar == true) {
                            Rotar ();
                            fuerzaEnX = 0;
                        }
                    }
            } else if (tiempoDeAtaque <= 0) {
                atacando = false;
            }

            if (atacando == false && tiempoStuneo <= Time.time) {
                if (playerPosition.position.y >= gameObject.transform.position.y - 2.5 && playerPosition.position.y <= gameObject.transform.position.y + 2.5) {
                    if (playerPosition.position.x < gameObject.transform.position.x) {
                        direccion = -1;
                        if (puedeRotar == false) {
                            Rotar ();
                            fuerzaEnX = 0;
                        }
                    } else if (playerPosition.position.x > gameObject.transform.position.x) {
                        direccion = 1;
                        if (puedeRotar == true) {
                            Rotar ();
                            fuerzaEnX = 0;
                        }
                    }
                } else {

                    if (tocaMuro) {
                        Rotar ();
                        fuerzaEnX = 0;
                    }
                }

                if (grounded == true && tocaMuro == false && velocidadX < 0.1f) {
                    enemy.AddForce (gameObject.transform.up * jumpPower, ForceMode2D.Impulse);
                    fuerzaEnX = 0;
                }
            }

        }
    }

    // Update is called once per frame
    void Update () {
        velocidadX = Mathf.Abs (enemy.velocity.x);
        anim.SetFloat ("speed", Mathf.Abs (enemy.velocity.x));
        anim.SetBool ("atacando", atacando);
    }

    void Rotar () {
        puedeRotar = !puedeRotar;
        direccion = direccion * -1;
        gameObject.transform.Rotate (0, 180, 0);
    }

    public void Damage (float damage) {
        vida -= damage;
        Debug.Log(damage);
        if(damage == 0){
            Debug.Log("Miss");
        } 
        

        if (vida <= 0) {
            Destroy (gameObject);
        }
    }

    public void Empujon (float X, float Y) {
        enemy.AddForce (Vector2.right * X, ForceMode2D.Impulse);
        enemy.AddForce (Vector2.up * Y, ForceMode2D.Impulse);
        tiempoStuneo = Time.time + tiempoParalizado;
    }

    private void OnDrawGizmos () {
        Gizmos.DrawWireSphere (comprovadorPiso.position, radioPiso);
        Gizmos.DrawWireSphere (puntoParaTocarMuro.position, radioParaTocarMuro);
        Gizmos.DrawWireSphere (puntoParaTocarMuro.position, radioDeAtaque);
        Gizmos.DrawWireSphere (new Vector2 (gameObject.transform.position.x, gameObject.transform.position.y - 1.25f), radioCircunferenciaDeLaPelota);
    }

    private void AtacarAlJugador () {
        Collider2D jugador = Physics2D.OverlapCircle (puntoParaTocarMuro.position, radioDeAtaque, queEsJugador);

        if (tiempoDeAtaque < delayParaAtacar && tiempoDeAtaque > 0f) {
            if (jugador != null) {
                if (puedeAtacarAlJugador == true) {
                    puedeAtacarAlJugador = false;
                    tiempoEspera = tiemopEntreAtaques;
                    jugador.GetComponent<playercontroller> ().ResivirDaño (dañoAlJugador);
                }
            }
        }
    }

    private void EmpujarAlJugador (float X, float Y) {

        Collider2D jugador = Physics2D.OverlapCircle (puntoParaTocarMuro.position, radioDeAtaque, queEsJugador);

        if (tiempoDeAtaque < delayParaAtacar && tiempoDeAtaque > 0f) {
            if (jugador != null) {
                if (puedeEmpujarAlJugador == true) {
                    puedeEmpujarAlJugador = false;
                    jugador.GetComponent<playercontroller> ().Impulsos (X * direccion, Y);
                    Debug.Log (X);
                }
            }
        }
    }

    private void AtacarRapidoAlJugador () {
        Collider2D jugador = Physics2D.OverlapCircle (puntoParaTocarMuro.position, radioDeAtaque, queEsJugador);

        if (tiempoDeAtaque < 0.2f && tiempoDeAtaque > 0.1f) {
            if (jugador != null) {
                if (puedeAtacarAlJugador == true) {
                    puedeAtacarAlJugador = false;
                    jugador.GetComponent<playercontroller> ().ResivirDaño (dañoAlJugador);
                    tiempoEspera = Time.time + tiemopEntreAtaques;
                }
            }
        }
    }
}