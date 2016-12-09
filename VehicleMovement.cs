using UnityEngine;
using System.Collections;

/// <summary>
/// Author: Scott Gartz
/// Parent class vehicles to inherit from. Contains methods for steering behaviors and movement.
/// </summary>
public abstract class VehicleMovement : MonoBehaviour
{
	public float alignmentWeight;
	public float cohesionWeight;
	float mass; 
    public float maxSpeed; //Maximum speed that the 'vehicle' can move at
	public float radius; //Collision radius
	public float seperationWeight;
    public float wanderRadius; //Radius for wandering behavior

	public GameObject[] obstacles; //Stores all obstacles in the scene

	RaycastHit hit;
	public Terrain terrain;

	Vector3 acceleration; //Used in calculating the movement of the object
	Vector3 boundsVector;
	Vector3 direction; //Direction that the object is heading
	Vector3 tempPos;
	Vector3 seperationForce;
	Vector3 wanderLoc;
	public Vector3 velocity; //Velocity that the object is moving at currently

    // Use this for initialization
    protected virtual void Start(float mass, float maxSpeed, float radius, Vector3 direction)
    {
		boundsVector = new Vector3 (50f, 0, 50f);
        this.mass = mass;
        this.maxSpeed = maxSpeed;
		this.radius = radius;
        this.direction = direction;
    }

    // Update is called once per frame
    protected void Update()
    {
        CalcSteeringForces();
        UpdatePosition();
        SetTransform();
    }

	/// <summary>
	/// Abstract. Zombie and Human build off of this.
	/// </summary>
    protected abstract void CalcSteeringForces();

	/// <summary>
	/// Calculates the velocity of the object, sets the direction, moves the Character Controller component accordingly, and zeros out acceleration.
	/// </summary>
    protected void UpdatePosition()
    {
        velocity += acceleration *Time.deltaTime;
		velocity.y = 0;
		Vector3.ClampMagnitude(velocity, maxSpeed);
        direction = velocity.normalized;
        acceleration = Vector3.zero;
		gameObject.GetComponent<CharacterController> ().Move (velocity * Time.deltaTime);
	//	Debug.Log (gameObject.transform.position);
    }

	/// <summary>
	/// Sets the forward of the object
	/// </summary>
    protected void SetTransform()
    {
		tempPos = transform.position;
		if(Physics.Raycast(transform.position, Vector3.down, out hit))
		{
			tempPos.y = hit.point.y + 1;
		}
		transform.position = tempPos;
        transform.forward = direction;
    }
 
	/// <summary>
	/// Run of the mill acceleration calculation based on force and mass
	/// </summary>
	/// <param name="force">Force being applied to the object</param>
    protected void ApplyForce(Vector3 force)
    {
        acceleration += force / mass;
    }

	/// <summary>
	/// Steering behavior that seeks the position that is passed in as a parameter
	/// </summary>
	/// <param name="targetPosition">Target position.</param>
    protected Vector3 Seek(Vector3 targetPosition)
    {
        
		Vector3 seeker = (targetPosition - transform.position);
		seeker.y = 0;
		return (((seeker.normalized) * maxSpeed) - velocity);


    }

	/// <summary>
	/// Steering behavior that flees from the position that is passed in as a parameter
	/// </summary>
	/// <param name="targetPosition">Target position.</param>
    protected Vector3 Flee(Vector3 targetPosition)
    {
        targetPosition.y = transform.position.y;
        return -((((targetPosition -transform.position).normalized) * maxSpeed) - velocity);
    }

	/// <summary>
	/// If the object attempts to move outside of the bounds of the terrain, it creates a strong force towards the center of the map
	/// </summary>
    protected Vector3 Bounds()
    {
		if (transform.position.x <= 5f|| transform.position.x >= 95f || transform.position.z <= 5f || transform.position.z >= 95f)
        {
			return ((((boundsVector - transform.position).normalized) * maxSpeed) - velocity);
        }
        else { return Vector3.zero; }
    }

	/// <summary>
	/// Steering behavior that causes the object to wander around the map
	/// </summary>
    protected Vector3 Wander()
    {
		wanderLoc = Random.insideUnitCircle * 5f;
		wanderLoc.z = wanderLoc.y;
		wanderLoc += gameObject.transform.forward;
		wanderLoc.y = 0;
		return (((wanderLoc * wanderRadius).normalized) * maxSpeed) - velocity;
    }

    /// <summary>
    /// Seeks the position
    /// </summary>
    /// <param name="position">Parameter for the average position of the flockers</param>
	protected Vector3 Cohesion(Vector3 position)
	{
		return Seek (position);
	}
	/// <summary>
	/// Steering force to align all flockers to the average direction
	/// </summary>
	/// <param name="flockDirection">Average direction of the flock</param>
	protected Vector3 Alignment(Vector3 flockDirection)
	{
		return ((flockDirection.normalized * maxSpeed) - velocity);	
	}
	/// <summary>
	/// Keeps flockers slightly seperated from one another if they get too close
	/// </summary>
	/// <param name="flockers">Array of all the members of a flock</param>
	protected Vector3 Seperation(GameObject[] flockers)
	{
		foreach(GameObject flocker in flockers)
		{
			if(transform.position != flocker.transform.position)
			{
				if(Vector3.Distance(gameObject.transform.position, flocker.transform.position) < 5f)
				{
					seperationForce += Flee(flocker.transform.position);
				}
			}
		}
		return (seperationForce.normalized*maxSpeed) - velocity;
	}

	protected Vector3 Avoid(GameObject[] obstacles)
	{
		Vector3 avoidForce = Vector3.zero;
		foreach(var obstacle in obstacles)
		{
			Vector3 dist = obstacle.transform.position - transform.position;
			if(Vector3.Distance(obstacle.transform.position,transform.position) < 5f)
			{
				if(Vector3.Dot(dist, transform.forward) > 0)
				{
					if(Vector3.Dot(dist, transform.right) > 0)
					{
						avoidForce += -transform.right * maxSpeed;				}
					else
					{
						avoidForce += transform.right * maxSpeed;
					}
				}
			}

		}
		return (avoidForce - velocity);
	}


}
