using UnityEngine;
using System.Collections;

/// <summary>
/// Author: Scott Gartz
/// Handles movement of the flocker.
/// </summary>
public class Flocker : VehicleMovement {

	public float maxForce = 200f;

	//Reference to the flockerManager in the scene
	private FlockerManager flockerManager;

	Vector3 ultimate;
	// Use this for initialization
	void Start ()
	{
		base.Start (1f, 15f, 5f, transform.forward);
		flockerManager = GameObject.FindGameObjectWithTag ("Manager").GetComponent<FlockerManager> ();
	}

	/// <summary>
	/// Handles calculation of the ultimate force applied to the flocker, based on 
	/// Cohesion, Seperation, Alignment, Bounds(if necessary), and Seeking
	/// </summary>
	protected override void CalcSteeringForces ()
	{
		ultimate = Vector3.zero;
		ultimate += Cohesion (flockerManager.avgPos) * flockerManager.cohesionWeight;
		ultimate += Seperation (flockerManager.flockers) * flockerManager.seperationWeight;
		ultimate += Alignment (flockerManager.avgDirection) * flockerManager.alignmentWeight;
		ultimate += Bounds () *20f;
		ultimate += Seek(flockerManager.goals[flockerManager.goalNum].transform.position) * flockerManager.seekWeight;
		ultimate += Avoid(flockerManager.obstacles) * 8f;
		Vector3.ClampMagnitude (ultimate, maxForce);
		ApplyForce (ultimate);
	}
}
