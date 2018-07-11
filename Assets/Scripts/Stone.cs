using System;
using UnityEngine;

public class Stone : MonoBehaviour, IComparable
{
    public float lastHit;

    public int playerIndex;

    //Flags to signal what state the stone is in
    private bool isFreeGuard;
    private bool isCurling;
    private bool passedHog;
    private bool released;
    public Vector3 lastPosition;
    public bool isBeingSwept;

    public Vector3 velocity;

    //Settings that set the stone's path
    public float weight;
    public int handle;

    //Current modifiers for the stone's path
    private float curl;
    private float drag;

    public Canvas canvas;
    public GameController gameController;

    private AudioSource[] sources;

    // Use this for initialization
    void Start()
    {
        gameController = FindObjectOfType<GameController>();
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
        }
        else if (other.CompareTag("CameraProc"))
        {
            //Change player position;
            if (gameController.gameState.players[playerIndex])
            {
                gameController.gameState.players[playerIndex].transform.position = gameController.gameState.topPosition.transform.position;
                gameController.gameState.players[playerIndex].transform.rotation = gameController.gameState.topPosition.transform.rotation;
            }
            passedHog = true;
        }
        else if (other.CompareTag("FirstHog"))
        {
            released = true;
        }
    }
    void OnCollisionEnter(Collision c)
    {
        if (c.gameObject.CompareTag("Stone"))
        {
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

                //Find theta, the angle between the velocity and shooterVector
                float theta = Vector3.Angle(shooterVector, velocity);
                if (theta > 90)
                {
                    shooterVector *= -1;
                    theta = Vector3.Angle(shooterVector, velocity);
                }

                //The shooter velocity should be found by velocity(initial) * cos (theta) due to conservation of momentum.
                float shooterSize = velocity.magnitude * Mathf.Cos(Mathf.Deg2Rad * theta);
                float hitSize = velocity.magnitude * Mathf.Sin(Mathf.Deg2Rad * theta);

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
    void FixedUpdate()
    {
        if (isCurling)
        {
            if (!passedHog)
            {
                //Move camera to stone
                if (gameController.gameState.players[playerIndex])
                {
                    gameController.gameState.players[playerIndex].transform.position = new Vector3(0, 1.7f, transform.position.z - 2);
                    gameController.gameState.players[playerIndex].transform.rotation = gameController.gameState.stonePosition.transform.rotation;
                }
            }
            //Make the stone curl more at lower speeds
            if (velocity.magnitude > 1)
            {
                curl = handle * Time.deltaTime * (10 - velocity.magnitude) / 2000;
            }
            else
            {
                curl = handle * Time.deltaTime * (10 - velocity.magnitude) / 1000;
            }

            if (released)
            {
                //Subtract the drag from the current velocity.
                drag = 20 * 0.0168f * Time.deltaTime / 4;
            }
            else
            {
                drag = 0;
                curl = 0;
            }
            if (isBeingSwept)
            {
                drag *= 0.95f;
                curl *= 0.80f;
            }

            //Add the curl on to the current velocity.
            velocity = new Vector3(curl + velocity.x, 0, velocity.z);

            if (velocity.magnitude < drag)
            {
                isCurling = false;
            }
            else
            {
                velocity -= drag * velocity.normalized;
            }
            //Spin the stone
            transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y + handle, 0));
        }
        else
        {
            float newSpeed;
            drag = 20 * 0.0168f * Time.deltaTime / 4;
            newSpeed = velocity.magnitude - drag;
            velocity = velocity.normalized * newSpeed;

            //When the stone stops
            if (velocity.magnitude < drag)
            {
                velocity = Vector3.zero;
                //Determine if the free guard zone is still applicable
                if (gameController.gameState.stonesThrown < 4)
                {
                    if (isFreeGuard)
                    {
                        //Check if the stone was removed from play, and if so replace it
                        if (transform.position.x == 5)
                        {
                            Debug.Log(string.Format("Stones thrown = {0}", gameController.gameState.stonesThrown));
                            //Remove offending stone
                            gameController.gameState.lastThrownStone.transform.position = new Vector3(5, -1, 18);
                            gameController.gameState.lastThrownStone.gameObject.SetActive(false);

                            //Move stone back into play
                            transform.position = lastPosition;
                        }
                    }
                    if (IsGuard)
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
    public bool IsGuard
    {
        //If the stone is in front of the tee line and outside the house it is in the freeguard zone
        get
        {
            if (!IsInHouse)
                return (transform.position.z < 17.37);
            return false;
        }
    }
    public bool IsBehindTee
    {
        get
        {
            return (transform.position.z > 19);
        }
    }
    public bool IsCentreGuard
    {
        get
        {
            if (IsGuard)
            {
                return (Math.Abs(transform.position.x) < 0.5);
            }
            return false;
        }
    }
    public bool IsInHouse
    {
        get
        {
            return (Distance < 1.98);
        }
    }
    public bool IsGuarded
    {
        get
        {
            foreach (Stone stone in gameController.gameState.stones)
            {
                if (stone != null)
                {
                    if ((Mathf.Abs(transform.position.x - stone.transform.position.x) < 0.2) &&
                        (stone.IsGuard))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
    public Stone GetGuard()
    {
        if (!IsGuarded) return null;
        foreach (Stone stone in gameController.gameState.stones)
        {
            if (stone!=null)
            {
                if ((Mathf.Abs(transform.position.x - stone.transform.position.x) < 0.2) &&
                        (stone.IsGuard))
                {
                    return stone;
                }
            }
        }
        return null;
    }

    public int CompareTo(object obj)
    {
        Stone toCompare = (Stone)obj;
        if (toCompare.Distance < Distance)
            return 1;
        else if (toCompare.Distance == Distance)
            return 0;
        else
            return -1;
    }

    public float Distance
    {
        get
        {
            return (transform.position - new Vector3(0, 0.3f, 17.37f)).magnitude;
        }
    }
}
