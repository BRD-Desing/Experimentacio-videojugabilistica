using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class brazocontroller : MonoBehaviour {

    private Vector3 MousePosition, ObjetoPosition;
    private float angulo;
    public float tiempo = 1f;
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

    public GameObject disparo;
    public GameObject esferaMeta;

    //disparos especiales
    public GameObject plasmaGame;

    public GameObject pesonaje;


    public LayerMask queEsTierra;

    public Transform generador;
    public SpriteRenderer render;
    public Animator anim;
    public AudioSource au;

    public float tiempoEspera;
    public float delayT;
    private float coldownFuego;

    public bool habilidadDeMovimiento;

    //Balas por segundo
    private int BFS;
    private bool clic;
    public bool clk;
    //meta = metamorfosis
    private bool meta;

    //Disparo del rifle
    public float tiempoEntreBalasDeEsnipers;
    private float referenciaSniper;

    public GameObject sniperBullet;

    private bool disparando;

    private void Start()
    {
        render = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        au = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        clic = clk;

        //Personaje
        anim.SetBool("invis", invis);
        anim.SetBool("MeleeMod", meleeMo);
        anim.SetInteger("tipoDisparo", tipoDeBala);
        anim.SetBool("disparando", clic);
        anim.SetBool("plasma", plasma);

        tipoHabilidadDisparo = pesonaje.GetComponent<playercontroller>().TipoHabilidad;

        habilidadDeMovimiento = pesonaje.GetComponent<playercontroller>().MovimientoEnProceso;

        enLaHabilidad = pesonaje.GetComponent<playercontroller>().EnLaHabilidad;

        if (habilidadDeMovimiento == false)
        {
            invis = enLaHabilidad;
        }
        if (habilidadDeMovimiento == true)
        {
            invis = habilidadDeMovimiento;
        }

        meleeMo = pesonaje.GetComponent<playercontroller>().meleeMode;






        if (meleeMode == true)
        {
            render.sprite = null;
        }

        tocandoSuperficie = Physics2D.OverlapCircle(generador.transform.position, radio, queEsTierra);

        if (meleeMo == false && invis == false)
        {
            //Mira moviendose
            MousePosition = Input.mousePosition;
            ObjetoPosition = Camera.main.WorldToScreenPoint(transform.position);

            angulo = Mathf.Atan2((MousePosition.y - ObjetoPosition.y), (MousePosition.x - ObjetoPosition.x)) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angulo));

            if (tipoDeBala == 1 && enLaHabilidad == false)
            {
                Disparo1();
            }

            //reduccion de tiempos
            if (tiempoEspera > 0)
            {
                tiempoEspera -= Time.deltaTime;
            }

            if (tocandoSuperficie == false)
            {
                //entrada de disparo
                disparando = Input.GetButton("Fire1");


                //   MMMMM
                //  MM   MM
                //      MM
                //    MMM
                //  MMMMMMMM


                    if (delayT > 0)
                    {
                        delayT -= Time.deltaTime;
                        tiempoEspera = 0.0001f;
                    }
                    else if (delayT <= 0)
                    {
                        BFS = 0;
                    }
                    if (coldownFuego > 0)
                    {
                        coldownFuego -= Time.deltaTime;
                    }

                    if (tocandoSuperficie == true || delayT <= 0)
                    {
                        CancelInvoke("EsferaMetamorfosis");

                    }
                    if (BFS == 1 && delayT <= 0)
                    {
                        coldownFuego = 0.5f;
                        clk = false;
                    }

                if (tipoDeBala == 2 && invis == false)
                {
                    meta = true;


                    if (disparando && tocandoSuperficie == false)
                    {
                        if (invis == false && delayT <= 0 && coldownFuego <= 0 && clic == false)
                        {
                            BFS = 1;
                            delayT = 0.3f;
                            clk = true;
                        }
                        if (BFS == 1 && delayT > 0 && tiempoEspera <= 0)
                        {
                            InvokeRepeating("EsferaMetamorfosis", 0.0001f, 0.13f);
                        }
                    }


                    //  MMMMMM
                    // MM    MM
                    //     MMM
                    // MM    MM
                    //  MMMMMM

                    if (tipoDeBala == 3 && enLaHabilidad == false)
                    {
                        if (referenciaSniper != tiempoEntreBalasDeEsnipers)
                        {
                            referenciaSniper += Time.deltaTime;
                        }



                        if (disparando == true && referenciaSniper == tiempoEntreBalasDeEsnipers)
                        {
                            referenciaSniper = 0;

                            Instantiate(sniperBullet, generador.position, generador.rotation);
                        }
                    }
                }
                 

                Plasma();

            }



            if (mira.position.x < gameObject.transform.position.x)
            {
                render.flipY = true;
            }
            else if (mira.position.x > gameObject.transform.position.x)
            {
                render.flipY = false;
            }
        }
    }


        //Variables de disparos normales [importante]

        //variable del disparo 1

        void Disparo1()
        {
            if (enLaHabilidad == false && invis == false)
            {
                meta = false;
                if (Input.GetButtonDown("Fire1") && invis == false)
                {
                    InvokeRepeating("Fire", 0.0001f, 0.15f);
                }
                else if (Input.GetButtonUp("Fire1") || invis == true)
                {
                    CancelInvoke("Fire");
                }
            }
        }
        void Fire()
        {
            Instantiate(disparo, generador.position, generador.rotation);
        }




        //Disparo 2

        private void EsferaMetamorfosis()
        {
            Instantiate(esferaMeta, generador.position, generador.rotation);
        }

        //Disparo del plasma
        private void DisparoPlasma()
        {
            Instantiate(plasmaGame, generador.position, generador.rotation);
        }




    private float tiempoEnLaHabilidad;
    private bool enLaHabilidad;
    private float tiempoEntreBalas;
    private bool plasma;

    private void Plasma()
    {
        if (tiempoEntreBalas > 0)
        {
            tiempoEntreBalas -= Time.deltaTime;
        }
        if (tipoHabilidadDisparo == 2)
        {
            if (tiempoEnLaHabilidad > 0)
            {
                
                
                if (Input.GetButtonDown("Fire1") && tiempoEntreBalas <= 0)
                {
                    DisparoPlasma();
                    
                    tiempoEntreBalas = 0.5f;
                }

               
                tiempoEnLaHabilidad -= Time.deltaTime;
                enLaHabilidad = true;
            }

            if (tiempoEnLaHabilidad <= 0)
            {
                enLaHabilidad = false;
                plasma = false;
                clic = false;
            }

            if (Input.GetButtonDown("Fire2") && tiempoEnLaHabilidad <= 0 && clic == false)
            {
                clic = true;
                anim.SetTrigger("Cambio");
                plasma = true;
                tiempoEnLaHabilidad = 10;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(generador.transform.position, radio);
    }
}
