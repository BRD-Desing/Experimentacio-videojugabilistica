using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class brazocontroller : MonoBehaviour {

    private Vector3 MousePosition, ObjetoPosition;
    private float angulo;
    public float tiempo = 1f;

    //variables para el esniper
    public float tiempoPorDefault;
    public float tiempoDerecarga;
    public int arma = 2;
    public GameObject sniperBullet;
    public Transform mira;

    //Variables del laser
    public float tiempoDeDisparo;
    private float tiempoDeEnfriamiento;
    public float tiempoDeEnfriaD;
    public bool end = true;
    public bool fire_2 = false;


    public bool sniper;
    public bool comprovador = false;
    public bool trans;
    public bool invis = false;


    public GameObject disparo;
    public GameObject laser;
    public Transform laserpoint;
    public Transform generador;
    public SpriteRenderer render;
    public Animator anim;
    public Animator laserAnim;
    public AudioSource au;

    

    private void Start()
    {
        render = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        au = GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {

        //Personaje
        anim.SetBool("Fire_2", fire_2);
        anim.SetBool("invis", invis);
        anim.SetBool("SniperTrue", sniper);

        //Mira moviendose
        MousePosition = Input.mousePosition;
        ObjetoPosition = Camera.main.WorldToScreenPoint(transform.position);

        angulo = Mathf.Atan2((MousePosition.y - ObjetoPosition.y), (MousePosition.x - ObjetoPosition.x)) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angulo));

        //Tiempos de espera
        if (tiempoDerecarga >= 0f)
        {
            tiempoDerecarga -= Time.deltaTime;
        }

        if (tiempoDeEnfriamiento > 0)
        {
            tiempoDeEnfriamiento -= Time.deltaTime;
        }

        if (mira.position.x < gameObject.transform.position.x)
        {
            render.flipY = true;
        }
        else if (mira.position.x > gameObject.transform.position.x)
        {
            render.flipY = false;
        }

        if (fire_2 == false)
        {
            //disparo de la bala
            if (Input.GetButtonDown("Fire1"))
            {
                if (sniper == false)
                {
                   InvokeRepeating("Fire", 0.001f, 0.20f);
                }
                if (sniper == true && tiempoDerecarga <= 0)
                {
                    tiempoDerecarga = tiempoPorDefault;
                    Instantiate(sniperBullet,
                       laserpoint.position,
                        laserpoint.rotation);
                }
               
            }
            else if (Input.GetButtonUp("Fire1"))
            {
                //detener
                CancelInvoke("Fire");
            }
        }

        //laser program
        if (Input.GetButtonDown("Fire2") && arma == 1)
        {
            sniper = true;
        }
        else if(Input.GetButtonUp("Fire2") && arma == 1)
        {
            sniper = false;
        }

        if (tiempoDeEnfriamiento <= 0 && Input.GetButtonDown("Fire2") && arma == 2)
        {
            //laser
            fire_2 = true;
            end = false;
            tiempoDeDisparo = tiempoDeDisparo += Time.deltaTime;
            if (tiempoDeDisparo >= tiempoDeEnfriaD)
            {
                tiempoDeEnfriamiento = tiempoDeDisparo;
                fire_2 = false;
                end = true;
            }
        }
        else if(Input.GetButtonUp("Fire2") && arma == 2)
        {
            fire_2 = false;
            end = true;
            tiempoDeEnfriamiento = tiempoDeDisparo;
        }
        determinadordearma();
    }

    //variable del disparo 1
    void Fire()
    {
        Instantiate(disparo, generador.position, generador.rotation);
    }

    //Cual arma desea?
    void determinadordearma()
    {
        if (Input.GetKey(KeyCode.Alpha2) && arma == 1)
        {
            arma = 2;
            Debug.Log("Cambio de arma");
        }
        if (Input.GetKey(KeyCode.Alpha1) && arma == 2)
        {
            arma = 1;
            Debug.Log("Cambio de arma");
        }
    }
}
