using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mapaController : MonoBehaviour {

    public GameObject mapa1;
    public GameObject mapa2;
    public GameObject mapa3;
    public GameObject mapa4;
    public GameObject mapa5;

    private bool puede = true;

    private int mapa;

    // Use this for initialization
    void Start()
    {
        mapa = Random.Range(1, 9);
        if (mapa <= 4 && puede)
        {
            Instantiate(mapa1, gameObject.transform.position, Quaternion.identity);
            puede = false;
        }
        if (mapa >= 5 && puede)
        {
            Instantiate(mapa2, gameObject.transform.position, Quaternion.identity);
            puede = false;
        }
    }
	// Update is called once per frame
	void Update () {
		
	}
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(gameObject.transform.position, 30);
    }
}
