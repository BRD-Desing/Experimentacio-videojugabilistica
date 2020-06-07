using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class triplicacionDeLaDaga : MonoBehaviour {

    public Transform ubicacion1;
    public Transform ubicacion2;

    public GameObject dagaNormal;

    // Use this for initialization
    void Start () {
        Instantiate(dagaNormal, ubicacion1.position, ubicacion1.rotation);
        Instantiate(dagaNormal, ubicacion2.position, ubicacion2.rotation);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
