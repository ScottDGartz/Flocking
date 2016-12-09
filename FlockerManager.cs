using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

/// <summary>
/// Author: Scott Gartz
/// Handles a reference to each flocker it creates, as well as each goal. Checks for collisions between flockers and
/// the currentGoal, and handles setting the next target.
/// </summary>
public class FlockerManager : MonoBehaviour
{
	//	Weights for the steering forces
	public float alignmentWeight;
	public float cohesionWeight;
	public float seekWeight;
	public float seperationWeight;

	// Cube that gets centered on the avgPos
	public GameObject debug;

	private GameObject flocker;
	// Holds the prefab for instantiating flockers
	public GameObject flockerPrefab;

	private GameObject goal;
	//	Holds prefab for the goals that the flockers seek 
	public GameObject goalPrefab;

	private GameObject obstacle;
	//	Holds obstacle prefab
	public GameObject obstaclePrefab;

	public GameObject[] flockers;
	public GameObject[] goals;
	public GameObject[] obstacles;

	// Amount of flockers to instantiate
	public int flockerAmt;
	public int goalNum = 0;

	//	Material for debug line
	public Material mat1;

	RaycastHit hit;

	public Terrain terrain;

	// Holds the average direction and average position of all the flockers
	public Vector3 avgDirection;
	public Vector3 avgPos;
	Vector3 posTemp;


	// Use this for initialization
	void Start()
	{
		// Instantiates as many flockers as the flockerAmt says to
		for(int i = 0; i < flockerAmt; i++)
		{
			flocker = Instantiate(flockerPrefab, new Vector3(Random.Range(0, 30), 21f, Random.Range(0, 30)), Quaternion.identity) as GameObject;
			flocker.GetComponent<Flocker>().terrain = terrain;
		}

		// Instantiates 10 goals for the flockers to seek
		for(int i = 0; i < 10; i++)
		{
			goal = Instantiate(goalPrefab, new Vector3(Random.Range(0, terrain.terrainData.size.x), 28f, Random.Range(0, terrain.terrainData.size.z)), Quaternion.identity)as GameObject;
			posTemp = goal.transform.position;
			if(Physics.Raycast(goal.transform.position,Vector3.down,out hit))
			{
				posTemp.y = hit.point.y + 7;
				goal.transform.position = posTemp;
			}
		}

		// Instantiates all the obstacles
		for(int i = 0; i <  20; i++)
		{
			obstacle = Instantiate(obstaclePrefab, new Vector3(Random.Range(0, terrain.terrainData.size.x), 21f, Random.Range(0, terrain.terrainData.size.z)),Quaternion.identity) as GameObject;
			posTemp = obstacle.transform.position;
			if(Physics.Raycast(obstacle.transform.position,Vector3.down,out hit))
			{
				posTemp.y = hit.point.y + 1;
				obstacle.transform.position = posTemp;
			}
		}
		flockers = GameObject.FindGameObjectsWithTag("Flocker");
		goals = GameObject.FindGameObjectsWithTag("Finish");
		obstacles = GameObject.FindGameObjectsWithTag("Obstacle");

	}
	
	// Update is called once per frame
	void Update()
	{
		// Resets position and direction back to zero
		avgPos = Vector3.zero;
		avgDirection = Vector3.zero;

		//Sums up the direction and position of the flockers
		foreach(GameObject item in flockers)
		{
			avgDirection += item.transform.forward;
			avgPos += item.transform.position;
		}

		//	Calculates average direction and position, and sets the debug cube's
		//  position and forward equal to them.
		avgDirection = avgDirection / (flockerAmt);
		//	Debug.Log("avgDir: " + avgDirection);

		avgDirection.Normalize();
		// Debug.Log("avgDirN: " + avgDirection);

		avgPos = avgPos / (flockerAmt);
		// Debug.Log("avgPos: " + avgPos);

		debug.transform.position = avgPos;
		debug.transform.forward = avgDirection;
		// Debug.Log("debugPos: " + debug.transform.position);
		// Debug.Log("Distance: " + Vector3.Distance(debug.transform.position, goals[goalNum].transform.position));

		// Checks if the average position of the flockers is within a certain distance of a goal
		// If true, the goal gets destroyed, and the goalNum gets incremented by 1
		// Gives the flockers the next goal to seek.
		if(goalNum != goals.Length)
		{
			if(Vector3.Distance(debug.transform.position, goals[goalNum].transform.position) <= 7.5f)
			{
				Destroy(goals[goalNum]);
				goalNum++;
			}
		}
	}

	// Used for the Reset button to reset the scene in case something breaks or if it ends.
	public void Reset()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}


/* These were used in the exercise as a quick way to change weights to 20 and back to 0 for testing.
	public void SetCohesion()
	{
		if(cohesionWeight == 0)
		{
			cohesionWeight += 20;
		} else
		{
			cohesionWeight = 0;
		}
	}

	public void SetAlignment()
	{
		if(alignmentWeight == 0)
		{
			alignmentWeight += 20;
		} else
		{
			alignmentWeight = 0;
		}
	}

	public void SetSeperation()
	{
		if(seperationWeight == 0)
		{
			seperationWeight += 20;
		} else
		{
			seperationWeight = 0;
		}
	}*/

	// Renders the direction debug line
	void OnRenderObject()
	{
		mat1.SetPass(0);
		GL.Begin(GL.LINES);
		GL.Vertex(avgPos);
		GL.Vertex(avgPos + (avgDirection.normalized));
		GL.End();
	}
}
