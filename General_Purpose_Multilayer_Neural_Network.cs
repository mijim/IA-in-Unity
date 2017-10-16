//======================================
//Title: Multilayer-Neural Network
//Author: Miguel Jiménez Benajes
//Date: 01/03/2017
//======================================

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CNeuralNet : MonoBehaviour {

	public int numInputs;
	public int numOutputs;
	public int numHiddenLayers;
	public int numNeuronsPerHiddenLayer;
	public double bias=-1.0;
	public double learnRate=0.2;
	private List<NeuronLayer> nn;

	//Clase encargada de manegar las capas
	private class NeuronLayer
	{
		private List<List<double>> neurons;

		//Creamos la capa
		public NeuronLayer(int numNeurons, int numWeights){
			neurons = new List<List<double>>(numNeurons);
			for(int a=0; a<numNeurons ;a++){
				List<double> wV = new List<double>(numWeights+1);
				for(int b=0; b<numWeights +1;b++){     //+1 porque incluimos el bias
					wV.Add(Random.Range(-1.0f,1.0f));
				}
				neurons.Add(wV);
			}
		}

		public List<List<double>> getLayer(){
			return neurons;
		}

		public void setLayer(List<List<double>> l){
			neurons = l;
		}
	}

	// Use this for initialization
	void Start () {
		crearRed ();
	}

	void crearRed(){
		nn = new List<NeuronLayer> (numHiddenLayers+1);

		//Creamos la primera capa conectada a las entradas
		nn.Add (new NeuronLayer (numNeuronsPerHiddenLayer, numInputs));

		//Creamos las capas intermedias entre la entrada y salida
		for (int i = 0; i < numHiddenLayers - 1; i++) {
			nn.Add (new NeuronLayer (numNeuronsPerHiddenLayer, numNeuronsPerHiddenLayer));
		}

		//Creamos la capa de salida
		nn.Add(new NeuronLayer(numOutputs, numNeuronsPerHiddenLayer));

	}

	public List<double> GetWeights(){
		List<double> ret = new List<double> (0);
		for (int a = 0; a < nn.Capacity; a++) {
			List<List<double>> nl = nn [a].getLayer ();
			for (int b = 0; b < nl.Capacity; b++){
				ret.AddRange(nl [b]);
			}
		}
		return ret;
	}

	public void PutWeights(List<double> l){
		int n = 0;
		for (int a = 0; a < nn.Capacity; a++) {
			List<List<double>> nl = nn [a].getLayer ();
			for (int b = 0; b < nl.Capacity; b++){
				List<double> neuron = nl[b];
				for (int c = 0; c < neuron.Capacity; c++) {
					neuron [c] = l [n];
					n++;
				}
			}
		}
	}

	public int GetNumberOfWeights(){
		return (numInputs+1) * numHiddenLayers * numNeuronsPerHiddenLayer;
	}

	//Calculo de las salidas
	private List<List<double>> calculate(List<double> inputs){
		List<List<double>> outputsL = new List<List<double>>(1);
		List<double> outputs = null;
		outputs = new List<double> (1);
		for (int i = 0; i < nn.Capacity; i++) {
			
			List<List<double>> layer = nn [i].getLayer();

			if (i > 0) {
				inputs = outputs;
				outputs = new List<double> (1);
			}
				
			for (int a = 0; a < layer.Capacity; a++) {
				List<double> neuron = layer [a];
				double output = 0;
				for (int b = 0; b < neuron.Capacity-1; b++) {
					output += inputs[b] * neuron [b];
				}
				output += bias * neuron [neuron.Capacity - 1];
				outputs.Add (Sigmoid(output));
			}
			outputsL.Add (outputs);
		}
		return outputsL;
	}

	//Método para el test
	public List<double> UpdateNeuronal(List<double> inputs){
		List<List<double>> l = calculate (inputs);
		return l [1];
	}
		
	//TODO modificar para más de una capa oculta

	//Entrenamiento para una iteración
	public List<List<double>> train(List<double> inputs, List<double> desiredOutputs){
		
		List<List<double>> outputs = calculate (inputs);
		List<List<double>> errors = new List<List<double>> (nn.Capacity);

		//Calculamos el error de salida
		List<double> error_capa2 = new List<double> (numOutputs);
		for (int i = 0; i < numOutputs; i++) {
			error_capa2.Add((desiredOutputs[i] - outputs [1] [i]) * derivadaSigmoid(outputs[1][i]));
		}

		//Calculamos el error de las capas ocultas
		List<List<double>> layer = nn [0].getLayer ();
		List<double> error_capa1 = new List<double> (layer.Capacity);
		for (int i = 0; i < layer.Capacity; i++) {
			double error = 0;
			for (int a = 0; a < numOutputs; a++) {
				error += error_capa2 [a] * outputs [0] [i];
			}
			error_capa1.Add(error);
			error_capa1 [i] *= derivadaSigmoid(outputs[0][i]);
		}

		//Obtenemos la neurona de la última capa y actualizamos sus pesos
		List<double> wOut = nn [nn.Capacity - 1].getLayer ()[0];
		for (int a = 0; a < numOutputs; a++) {
			for (int i = 0; i < wOut.Capacity-1; i++) {
				wOut [i] += learnRate * error_capa2[a] * outputs[0][i];
			}
			wOut [wOut.Capacity - 1] += learnRate * error_capa2[a];
		}
			
		layer = nn [0].getLayer ();
		for(int i=0; i<layer.Capacity;i++){
			for (int a = 0; a < layer[i].Capacity-1; a++) {
				layer [i] [a] += learnRate * error_capa1 [i] * inputs [a];
			}
			layer [i] [layer[i].Capacity-1] += learnRate * error_capa1 [i];
		}

		return outputs;
	}

	//Función de activación
	double derivadaSigmoid(double inp){
		return inp * (1 - inp);
	}

	//Derivada parcial de la función de activación
	double Sigmoid(double netinput){
		return (double)(1 / (1 + Mathf.Exp((float)(-netinput / 1.0))));
	}
}
