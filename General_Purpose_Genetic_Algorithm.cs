//======================================
//Title: Genetic Algorithm
//Author: Miguel Jiménez Benajes
//Date: 03/04/2017
//======================================

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public class CGenAlg : MonoBehaviour {

	public GameObject bacteria;
	public GameObject alimento;
	private GameObject[] bacterias;
	public int popSize=30;
	public int cromoLength=32;
	public double popFitness;
	double bestFitness;
	double averageFitness;
	double worstFitness;
	int fittestGenome; //Mantiene el genoma del mas fuerte
	public double mutationRate=0.1; //Probabilidad de mutacion genetica
	public double crossOverRate=0.7; //Probabilidad de cruzar cromosomas
	public int generationCounter; //Contador de generaciones
		
	// Use this for initialization
	void Start () {
		popFitness = 0.0;
		bestFitness = 0.0;
		averageFitness = 0.0;
		worstFitness = 99999999.0;
		fittestGenome = 0;
		generationCounter = 0;
		bacterias = new GameObject[popSize];
		for (int i = 0; i < popSize; i++) {
			Vector2 spawnPoint = new Vector2 (Random.Range(0.0f,29.9f), Random.Range(0.0f,29.9f));
			bacterias[i] = (GameObject)Instantiate(bacteria,spawnPoint,Quaternion.Euler(0,0,Random.Range(0.0f,365.0f)));
		}

	}
	List<Player.Genome> obtenerGenomas(){
		List<Player.Genome> lista = new List<Player.Genome> ();
		for (int i = 0; i < popSize; i++) {
			lista.Add (bacterias [i].GetComponent<Player> ().getGenome ());
		}
		return lista;
	}

	void aplicarGenomas(List<Player.Genome> l){
		Player.Genome[] lista = l.ToArray ();
		for (int i = 0; i < popSize; i++) {
			bacterias [i].GetComponent<Player> ().aplicarGenomaANN (lista [i]);
		}
	}
	void FixedUpdate(){
		if (Application.isEditor == false) {
			Debug.Log("--------Datos Finales-------");

		}
		if (Time.fixedTime % 50 == 0) {
			cambiarDeGeneracion ();
		}
	}
	void cambiarDeGeneracion(){
		epoch (obtenerGenomas());
		Debug.Log("--------Datos de la generación anterior-------");
		Debug.Log ("Media de puntuaciones: " + getAverageFitness());
		Debug.Log ("Generación número: " + (++generationCounter));
		redistribuirPoblacion();
		Debug.Log("--------Nueva generacion creada-------");
	}

	void redistribuirPoblacion(){
		for (int i = 0; i < popSize; i++) {
			Vector2 spawnPoint = new Vector2 (Random.Range(0.0f,29.9f), Random.Range(0.0f,29.9f));
			bacterias [i].transform.position = spawnPoint;
		}
	}

	Player.Genome getChromoRoulette(Player.Genome[] pop){
		double slice = Random.Range(0.0f,(float)popFitness);
		Player.Genome elEscogido=null;
		double fitnessMasGrande=0;
		for (int i = 0; i < popSize; i++) {
			fitnessMasGrande += pop[i].dFitness;
			if (fitnessMasGrande >= slice) {
				elEscogido = pop [i];
				break;
			}
		}
		return elEscogido;
	}

	void calculateBestWorstAvTot(){
		popFitness = 0.0;

		double highestSoFar = 0;
		double lowestSoFar = 9999999;

		Player.Genome[] pop = obtenerGenomas().ToArray ();
		for(int i=0; i<popSize;i++){
			if (pop [i].dFitness > highestSoFar) {
				highestSoFar = pop [i].dFitness;
				fittestGenome = i;
				bestFitness = highestSoFar;
			}

			if (pop [i].dFitness < lowestSoFar) {
				lowestSoFar = pop [i].dFitness;
				worstFitness = lowestSoFar;
			}
			popFitness += pop [i].dFitness;
		}
		averageFitness = popFitness / popSize;
	}

	void reset(){
		popFitness = 0;
		bestFitness = 0;
		worstFitness = 99999999;
		averageFitness = 0;
	}

	public List<Player.Genome> epoch(List<Player.Genome> oldPopulation){
		List<Player.Genome> vecPopulaton = oldPopulation;
		reset ();
		//Habría que ordenar la poblacion
		calculateBestWorstAvTot();
		List<Player.Genome> newPopulation = new List<Player.Genome> ();
		//CMovement.Genome[] newPopulation = new CMovement.Genome[popSize];
		//grabNBest (0, 0, newPopulation);
		//Creamos elitismo en la poblacion (Salvamos a las bacterias que han obtenido mejores puntuaciones)
		Player.Genome[] pop = obtenerGenomas().ToArray();
		pop = pop.OrderByDescending (x => x.dFitness).ToArray ();
		newPopulation.Add(new Player.Genome(pop[0].vecWeights,0));
		newPopulation.Add(new Player.Genome(pop[1].vecWeights,0));
		newPopulation.Add(new Player.Genome(pop[2].vecWeights,0));
		newPopulation.Add(new Player.Genome(pop[3].vecWeights,0));

		for (int i = 4; i < popSize-1; i+=2) {
			
			Player.Genome madre = getChromoRoulette (pop);
			Player.Genome padre = getChromoRoulette (pop);
			List<double> hijo1 = new List<double> ();
			List<double> hijo2 = new List<double> ();

			//Cruzamos los cromosomas de los padres a los hijos

			//Crossover (madre.vecWeights, padre.vecWeights, hijo1, hijo2);

			if ((Random.value > crossOverRate) || (padre == madre)) {
				hijo1 = madre.vecWeights;
				hijo2 = padre.vecWeights;
			} else {
				//Determinamos hasta que punto el hijo es igual al padre o madre
				int crossoverPoint = (int)Random.Range (0, cromoLength - 1);
				double[] m = madre.vecWeights.ToArray ();
				double[] p = padre.vecWeights.ToArray ();
				hijo1 = new List<double> ();
				hijo2 = new List<double> ();

				//Creamos los hijos
				for (int j = 0; j < crossoverPoint; j++) {
					hijo1.Add (m [j]);
					hijo2.Add (p [j]);
				}

				for (int j = crossoverPoint; j < m.Length; j++) {
					hijo1.Add (p [j]);
					hijo2.Add (m [j]);
				}
			}


			//Añadimos mutacion en los genes

			double[] mutacion1 = hijo1.ToArray ();
			double[] mutacion2 = hijo1.ToArray ();
			for (int j = 0; j < cromoLength; j++) {
				if (Random.value < mutationRate) {
					mutacion1 [j] += Random.Range (-1.0f, 1.0f) * 0.3;
				}
				if (Random.value < mutationRate) {
					mutacion2 [j] += Random.Range (-1.0f, 1.0f) * 0.3;
				}
			}
			hijo1 = new List<double> (mutacion1);
			hijo2 = new List<double> (mutacion2);

			newPopulation.Add(new Player.Genome(hijo1,0));
			newPopulation.Add(new Player.Genome(hijo2,0));
		}
		vecPopulaton = newPopulation;
		aplicarGenomas (vecPopulaton);
		return vecPopulaton;
	}

	//Getters
	public double getAverageFitness(){
		return popFitness / popSize;
	}

	public double getBestFitness(){
		return bestFitness;
	}

}
