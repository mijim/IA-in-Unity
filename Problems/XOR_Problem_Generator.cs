//======================================
//Title: XOR Problem Generator
//Author: Miguel Jiménez Benajes
//Date: 20/03/2017
//======================================

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class XORGenerator : MonoBehaviour {
	public int numIteraciones;
	public float ruido=0.0f;
	void Update () {
		if (Input.GetKeyDown ("e")) {
			CNeuralNet nn = this.GetComponent<CNeuralNet> ();
			Debug.Log ("Red neuronal creada, empezando el entrenamiento...");
			double[] inp1 = { 0, 0, 1, 1 };
			double[] inp2 = { 0, 1, 0, 1 };
			double[] desiredOutputs = { 0, 1, 1, 0 };

			for (int a = 0; a < numIteraciones; a++) {
				List<double> inp1L = new List<double> (1);
				inp1L.Add (inp1 [a%inp1.Length]);
				inp1L.Add (inp2 [a%inp1.Length]);
				List<double> outL = new List<double> (1);
				outL.Add (desiredOutputs [a%inp1.Length]);
				nn.train (inp1L, outL);
			}

			Debug.Log ("Entrenamiento finalizado.");
			Debug.Log ("Testing NN...");
			int numAciertos = 0;
			for (int a = 0; a < inp2.Length; a++) {
				List<double> inp1L = new List<double> (1);
				inp1L.Add (inp1 [a] + Random.Range(-ruido,ruido)); //Añadimos ruido a la entrada
				inp1L.Add (inp2 [a] + Random.Range(-ruido,ruido)); //Añadimos ruido a la entrada
				double output = nn.UpdateNeuronal (inp1L) [0];
				//Debug.Log (output);
				//Debug.Log (desiredOutputs [a]);
				int outP =0;
				if (output > 0.5) {
					outP = 1;				
				} else {
					outP = 0;
				}
				if (outP == desiredOutputs [a])
					numAciertos++;
			}
			Debug.Log ("Total de aciertos: " + ((float)numAciertos / inp1.Length) * 100 + "%");
		}

	}

}
