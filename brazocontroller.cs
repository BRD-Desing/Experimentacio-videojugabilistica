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

    private bool tocandoSuperficie;
    public float radio;

    public bool meleeMo = false;

    public int tipoDeBala;

    public GameObject disparo;
    public GameObject esferaMeta;

    public GameObject pesonaje;

    public LayerMask queEsTierra;

    public Transform generador;
    public SpriteRenderer render;
    public Animator anim;
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
        anim.SetBool("invis", invis);
        anim.SetBool("MeleeMod", meleeMo);

        if (meleeMode == true)
        {
            render.sprite = null;
        }

        tocandoSuperficie = Physics2D.OverlapCircle(generador.transform.position, radio, queEsTierra);

        if (meleeMo == false)
        {
            //Mira moviendose
            MousePosition = Input.mousePosition;
            ObjetoPosition = Camera.main.WorldToScreenPoint(transform.position);

            angulo = Mathf.Atan2((MousePosition.y - ObjetoPosition.y), (MousePosition.x - ObjetoPosition.x)) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angulo));


            if (Input.GetButtonDown("Fire1") && invis == false && tipoDeBala == 1)
            {
                InvokeRepeating("Fire", 0.0001f, 0.2f);
            }
            else if (Input.GetButtonUp("Fire1") && tipoDeBala == 1 || invis == true)
            {
                CancelInvoke("Fire");
            }

            if (tocandoSuperficie == false)
            {
                if (Input.GetButtonDown("Fire1") && tipoDeBala == 2 && invis == false)
                {

                    InvokeRepeating("EsferaMetamorfosis", 0.0001f, 0.2f);
                }
                else if (Input.GetButtonUp("Fire1") && tipoDeBala == 2 || invis == true)
                {
                    CancelInvoke("EsferaMetamorfosis");
                }
            }
            if (tocandoSuperficie == true || Input.GetButtonUp("Fire1"))
            {
                CancelInvoke("EsferaMetamorfosis");
            }
           
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

    //variable del disparo 1
    void Fire()
    {
        Instantiate(disparo, generador.position, generador.rotation);
    }

    //Disparo 2
    private void EsferaMetamorfosis()
    {
        Instantiate(esferaMeta, generador.position, generador.rotation);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(generador.transform.position, radio);
    }
}
