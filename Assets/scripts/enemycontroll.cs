using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemycontroll : MonoBehaviour {

    public float healt;
    public float damageRefence;
    private float sniper = 5;

    public Transform lectorIz;
    public Transform lectorDer;

    private Vector2 player;
    private Vector2 esteObjeto;
    public Transform jugadorPos;
    

	// Use this for initialization
	void Start () {
        
		
	}
	
	// Update is called once per frame
	void Update () {
        player = jugadorPos.position;
        if (healt <= 0)
        {
            Destroy(gameObject);
        }
        DetectoresDeAproximidad();

    }

    private void DetectoresDeAproximidad()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "bullet")
        {
            healt -= damageRefence;
        }
        if (col.gameObject.tag == "sniperBullet")
        {
            healt -= sniper;
        }
    }
}
