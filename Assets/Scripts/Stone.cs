using UnityEngine;

public class Stone : MonoBehaviour {
    public float lastHit;

    public int playerIndex;

    //Flags to signal what state the stone is in
    private bool isFreeGuard;
    private bool isCurling;
    private bool passedHog;
    public Vector3 lastPosition;

    public Vector3 velocity;

    //Settings that set the stone's path
    public float weight;
    public int handle;

    //Current modifiers for the stone's path
    private float curl;
    private float drag;

    public Canvas canvas;
    public GameController gc;

    private AudioSource[] sources;

	// Use this for initialization
	void Start () {
        gc = FindObjectOfType<GameController>();
        isCurling = true;
        passedHog = false;
        velocity = weight * transform.forward;
        sources = GetComponents<AudioSource>();
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boundary"))
        {
            //HIDE SELF
            velocity = Vector3.zero;
            gameObject.transform.position = new Vector3(5, -1, 18);
        } else if (other.CompareTag("CameraProc"))
        {
            //Change player position;
            if (gc.gState.players[playerIndex])
            {
                gc.gState.players[playerIndex].transform.position = gc.gState.topPosition.transform.position;
                gc.gState.players[playerIndex].transform.rotation = gc.gState.topPosition.transform.rotation;
            }
            passedHog = true;
        }
    }
    void OnCollisionEnter(Collision c) {
        if (c.gameObject.CompareTag("Stone")) {
            Stone otherStone = c.gameObject.GetComponent<Stone>();
            //Set angle of self and other
            isCurling = false;
            if (Mathf.Abs(lastHit - Time.time) < 0.01)
                return;

            //If the stone is moving faster than the other, it is the shooter. Keeping the logic here means it only gets executed once.
            if (velocity.magnitude > otherStone.velocity.magnitude)
            {
                float audioSize = velocity.magnitude / 4.5f;//Never greater than 1

                //Find the vector between the two stones, this will be the new velocity of the hit stone.
                Vector3 vectorBetween = otherStone.transform.position - gameObject.transform.position;
                //Find the tangential distance between the velocity of the shooter and the vector between the stones.
                float tangDistance = Vector3.Cross(velocity.normalized, vectorBetween).magnitude;
                tangDistance = Mathf.Abs(tangDistance);
                //Get the vector that the shooter will travel along using the cross product. If this is greater than 90 degrees 
                //away from the initial velocity then reverse it.
                Vector3 shooterVector = Vector3.Cross(vectorBetween, Vector3.up);
                if (Vector3.Angle(shooterVector, velocity) > 90)
                {
                    shooterVector *= -1;
                }

                //Set the sizes of the velocities
                float hitSize = velocity.magnitude * (1 - Mathf.Clamp01(tangDistance / vectorBetween.magnitude));
                float shooterSize = velocity.magnitude * (Mathf.Clamp01(tangDistance / vectorBetween.magnitude));

                audioSize = audioSize * hitSize / velocity.magnitude; //Will be between 0 and 1.
                
                sources[0].volume = audioSize;
                sources[0].Play();

                //Set velocities of the rigidbodies through the xCurl and zWeight attributes
                velocity = shooterVector.normalized * shooterSize;

                otherStone.velocity = vectorBetween.normalized * hitSize;

                otherStone.lastHit = Time.time;
                lastHit = Time.time;

            }
        }
    }
   
    // Update is called once per frame
    void FixedUpdate() {
        if (isCurling)
        {
            if (!passedHog)
            {
                //Move camera to stone
                if (gc.gState.players[playerIndex])
                {
                    gc.gState.players[playerIndex].transform.position = new Vector3(0, 2, transform.position.z - 3.5f);
                    gc.gState.players[playerIndex].transform.rotation = gc.gState.stonePosition.transform.rotation;
                }
            }
            //Make the stone curl more at lower speeds
            if (velocity.magnitude > 3)
            {
                curl = Mathf.Clamp(velocity.x + (handle * Time.deltaTime * (20 - velocity.magnitude) / 2500), -2, 2);
            }
            else
            {
                curl = Mathf.Clamp(velocity.x + (handle * Time.deltaTime * (20 - velocity.magnitude) / 1500), -2, 2);
            }
            
            //Add the curl on to the current velocity.
            velocity = new Vector3(curl, 0, velocity.z);

            //Subtract the drag from the current velocity.
            drag = Time.deltaTime * (10 - velocity.magnitude) / 35;

            velocity -= drag * velocity.normalized;

            if (velocity.magnitude < 0.02)
            {
                isCurling = false;
            }
            else
            {
                //Spin the stone
                transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y + handle, 0));
            }
        }
        else 
        {
            float newSpeed;
            drag = Time.deltaTime * (20 - velocity.magnitude) / 80;
            newSpeed = velocity.magnitude - drag;
            velocity = velocity.normalized * newSpeed;

            //When the stone stops
            if (newSpeed < 0.01)
            {
                velocity = Vector3.zero;
                //Determine if the free guard zone is still applicable
                if (gc.gState.stonesThrown < 4)
                {
                    if (isFreeGuard)
                    {
                        //Check if the stone was removed from play, and if so replace it
                        if (transform.position.x == 5)
                        {
                            Debug.Log(string.Format("Stones thrown = {0}", gc.gState.stonesThrown));
                            //Remove offending stone
                            gc.gState.stones[gc.gState.stonesThrown - 1].transform.position = new Vector3(5, -1, 18);
                            gc.gState.stones[gc.gState.stonesThrown - 1].gameObject.SetActive(false);

                            //Move stone back into play
                            transform.position = lastPosition;
                        }
                    }
                    if (IsGuard())
                    {
                        isFreeGuard = true;
                        lastPosition = transform.position;
                    }
                    else
                    {
                        isFreeGuard = false;
                    }
                }
            }
        }
        transform.position = transform.position + velocity * Time.deltaTime;
    }
    //If the stone is in front of the tee line and outside the house it is in the freeguard zone
    public bool IsGuard()
    {
        if (!InHouse())
        {
            if (transform.position.z < 17.37)
            {
                return true;
            }
        }
        return false;
    }
    public bool InHouse()
    {
        float distance = (transform.position - new Vector3(0, 0.3f, 17.37f)).magnitude;
        if (distance < 1.98)
        {
            return true;
        }
        return false;
    }
}
