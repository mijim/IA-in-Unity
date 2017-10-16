//======================================
//Title: Classification Problem Generator
//Author: Miguel Jiménez Benajes
//Date: 18/03/2017
//======================================

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ClassificationGenerator : MonoBehaviour {
	public int numeroDePuntos;
	public int numIteraciones;
	public int radioP1X;
	public int radioP1Y;
	public int radioP2X;
	public int radioP2Y;
	public float ruido;
	public GameObject p1;
	public GameObject p2;
	private List<Object> puntos;
	private CNeuralNet nn;
	// Use this for initialization
	void Start () {
		puntos = new List<Object> ();
		nn = GetComponent<CNeuralNet> ();

		float angle;
		float x;
		float y;

		//Generamos los puntos aleatoriamente
		for (int i = 0; i < numeroDePuntos; i++) {
			//Creamos un punto del tipo 1
			angle = Random.value * 360; //Seleccionamos un radio aleatorio
			x = Mathf.Sin (angle) * Random.Range(radioP1X, radioP2X); 
			y = Mathf.Cos (angle) * Random.Range(radioP1Y, radioP2Y);
			puntos.Add (Instantiate (p1,new Vector2 (x, y), Quaternion.identity));

			angle = Random.value * 360; //Seleccionamos un radio aleatorio
			x = Mathf.Sin (angle) * Random.Range(0.01f, radioP1X+ruido); //Se pone +1 para que haya mezcla de puntos 
			y = Mathf.Cos (angle) * Random.Range(0.01f, radioP1Y+ruido); 
			puntos.Add (Instantiate (p2,new Vector2 (x, y), Quaternion.identity));
		}

		//Mezclamos la lista de puntos
		for (int i = 0; i < puntos.Count; i++) {
			Object aux = puntos [i];
			int randomIndex = Random.Range (i, puntos.Count);
			puntos [i] = puntos [randomIndex];
			puntos [randomIndex] = aux;
		}


	}

	void Update(){
		if (Input.GetKeyDown ("e")) {
			Debug.Log ("Ejecutando algoritmo");
			int[] vSalidasEsperadas = new int[puntos.Count];
			for (int i = 0; i < puntos.Count; i++) {
				if (((GameObject)puntos [i]).tag == "azul")
					vSalidasEsperadas [i] = 0;
				else
					vSalidasEsperadas [i] = 1;
			}
			for(int a=0;a<numIteraciones;a++){
				for (int i = 0; i < 300; i++) {
					List<double> inputs = new List<double> ();
					List<double> nuevosPesos = new List<double> ();
					inputs.Add (((GameObject)puntos [i]).transform.position.x);
					inputs.Add (((GameObject)puntos [i]).transform.position.y);
					List<double> salidasE = new List<double> ();
					salidasE.Add (vSalidasEsperadas [i]);
					nn.train (inputs, salidasE);
				}
			}
			int contador = 0;
			for (int i = 300; i < 400; i++) {
				List<double> inputs = new List<double> (2);
				inputs.Add (((GameObject)puntos [i]).transform.position.x);
				inputs.Add (((GameObject)puntos [i]).transform.position.y);
				List<double> outputs = nn.UpdateNeuronal (inputs);
				int res;
				if (outputs [0] > 0.5) {
					res = 1;				
				} else {
					res = 0;
				}
				if (res == vSalidasEsperadas [i]) {
					contador++;
				}
			}
			Debug.Log ("Numero de aciertos: " + contador);
			Debug.Log ("Numero de fallos: " + (100 - contador));
		}
	}

