//======================================
//Title: Bacteria genoma en movment
//Author: Miguel Jiménez Benajes
//Date: 10/04/2017
//======================================

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CMovement : MonoBehaviour {

	public GameObject axisIzq;
	public GameObject axisDer;
	public double izqG=1;
	public double derG=1;

	public int cromoLength=44;
	private int maxIzq = 20;
	private int maxDer = 20;
	public Vector3 direccionVectorComidaMasCercana;
	Rigidbody2D rigid2d;
	public CNeuralNet nn;
	public Genome gm;

	public class Genome{
		public List<double> vecWeights;
		public double dFitness;

		public Genome(){
			dFitness=0;
		}

		public Genome(List<double> w, double f){
			vecWeights = w;
			dFitness = f;
		}

	}

	// Use this for initialization
	void Start () {
		rigid2d = GetComponent<Rigidbody2D> ();
		List<double> vecWeights = new List<double> (cromoLength);
		for(int j=0; j< cromoLength;j++){
			vecWeights.Add (Random.Range (-1.0f, 1.0f));
		}
		gm = new Genome (vecWeights, 0);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		//Si la bacteria se sale del recuadro vuelve por abajo (efecto espejo)

		if (transform.position.x > 33) {
			Vector2 aux = transform.position;
			aux.x = 0;
			transform.position = aux;
		}
		if (transform.position.y > 31) {
			Vector2 aux = transform.position;
			aux.y = 0;
			transform.position = aux;
		}
		if (transform.position.x < 0) {
			Vector2 aux = transform.position;
			aux.x = 33;
			transform.position = aux;
		}
		if (transform.position.y < 0) {
			Vector2 aux = transform.position;
			aux.y = 31;
			transform.position = aux;
		}

		//Actualizamos el vector de direccion de la comida mas cercana
		direccionVectorComidaMasCercana = (this.transform.position - this.FindClosestFood().transform.position).normalized;
		//Vector2 posicion = (transform.position - Vector3.up).normalized;
		//La bacteria usa la red neuronal para tomar una decision
		List<double> inputs = new List<double> ();
		List<double> outputs;
		inputs.Add (-Mathf.Sin(transform.rotation.z));
		inputs.Add (Mathf.Cos(transform.rotation.z));
		//inputs.Add (posicion.y);
		inputs.Add (direccionVectorComidaMasCercana.x);
		inputs.Add (direccionVectorComidaMasCercana.y);
		outputs = nn.UpdateNeuronal (inputs);
		double[] op = outputs.ToArray ();
		izqG = op [0] * maxIzq;
		derG = op [1] * maxDer;

		//Movimiento de la bacteria
		transform.RotateAround (axisIzq.transform.position, new Vector3 (0, 0, 1),(float) izqG);
		transform.RotateAround (axisDer.transform.position, new Vector3 (0, 0, 1),(float) -derG);
	}

	//Metodo que devuelve la comida mas cercana a la bacteria
	GameObject FindClosestFood(){
		GameObject[] gos = GameObject.FindGameObjectsWithTag ("food");
		GameObject closest = null;
		float distance = Mathf.Infinity;
		foreach (GameObject g in gos) {
			Vector3 diff = g.transform.position - transform.position;
			float curDistance = diff.sqrMagnitude;
			if (curDistance < distance) {
				closest = g;
				distance = curDistance;
			}
		}
		return closest;
	}

	public void aplicarGenomaANN(Genome genoma){
		gm = genoma;
		double[] weights = new double[cromoLength];
		double[] genes = genoma.vecWeights.ToArray ();
		int j = 0;
		int k = 0;
		for (int i = 0; i < weights.Length; i++) {
			if (j == nn.numInputs) {
				weights [i] = 0;
				j = 0;
			} else {
				weights [i] = genes [k];
				k++;
			}
			j++;
		}
		nn.PutWeights (new List<double> (weights));
	}

	public Genome getGenome(){
		return gm;
	}

	void OnTriggerEnter2D(Collider2D other){
		if (other.gameObject.tag == "food") {
			gm.dFitness += 1;
		}
}
