using UnityEngine;
using System.Collections;

public class Stone : MonoBehaviour {

    public Rigidbody rb;
    public float force;
    Vector3 pushingForce;
    Vector3 initialVelocity;
    public bool curling;
    public bool collided;
    public float lastHit;
    public int handle;
    public string color;
    public Camera endCam;
    public Camera topCam;
    public Canvas canvas;
    public bool freeGuard;
    public Vector3 lastPosition;
    public GameController gc;

	// Use this for initialization
	void Start () {
	    rb = GetComponent<Rigidbody>();
        rb.velocity = (transform.forward * force);
        curling = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boundary"))
        {
            //HIDE SELF
            rb.velocity = Vector3.zero;
            gameObject.transform.position = new Vector3(5, 0.3f, 15 + gc.stonesThrown);
        } else if (other.CompareTag("CameraProc"))
        {
            //CHANGE CAMERA
            endCam.enabled = false;
            topCam.enabled = true;
            canvas.worldCamera = topCam;
        } else if (other.CompareTag("Stone")) {
            Stone otherStone = other.gameObject.GetComponent<Stone>();
            //Set angle of self and other
            curling = false;
            if (Mathf.Abs(lastHit - Time.time) < 0.001)
                return;

            //If the stone is moving, it is the shooter. Keeping the logic here means it only gets executed once.
            if (rb.velocity.magnitude > otherStone.rb.velocity.magnitude)
            {
                GetComponent<AudioSource>().volume = rb.velocity.magnitude / 6.0f;
                Vector3 otherVector = other.gameObject.transform.position - gameObject.transform.position;
                float tangDistance = Vector3.Cross(rb.velocity.normalized, otherVector).magnitude;
                Vector3 shooterVector = Vector3.Cross(otherVector, Vector3.up);
                if (shooterVector.z < 0)
                {
                    shooterVector = shooterVector * -1;
                }
                tangDistance = Mathf.Abs(tangDistance);
                collided = true;
                otherStone.collided = true;
                otherStone.rb.velocity = otherVector.normalized * rb.velocity.magnitude * (1 - tangDistance / 0.31f);
                rb.velocity = shooterVector.normalized * rb.velocity.magnitude * (tangDistance / 0.31f);

                otherStone.lastHit = Time.time;
                lastHit = Time.time;
                GetComponent<AudioSource>().Play();
            }
        }
    }
    // Update is called once per frame
    void FixedUpdate() {
        if (curling)
        {
            //Move camera to stone
            endCam.gameObject.transform.position = new Vector3(0, 3, rb.position.z - 5f);
            float newSpeed, newCurl;
            if (rb.velocity.z > 5)
            {
                newCurl = Mathf.Clamp(rb.velocity.x + (handle * Time.deltaTime * (20 - rb.velocity.z) / 1500), -2, 2);
            }
            else
            {
                newCurl = Mathf.Clamp(rb.velocity.x + (handle * Time.deltaTime * (20 - rb.velocity.z) / 750), -2, 2);
                
            }
            newSpeed = Mathf.Clamp(rb.velocity.z - Time.deltaTime * (20 - rb.velocity.z) / 40, 0, 50);
            if (rb.velocity.z < 0.1)
            {
                newCurl = 0;
                curling = false;
            }
            else
            {
                rb.rotation = Quaternion.Euler(new Vector3(0, rb.rotation.eulerAngles.y +  handle * 2, 0));
            }
            rb.velocity = new Vector3(newCurl, 0, newSpeed);
        } else
        {
            float newSpeed;
            newSpeed = Mathf.Clamp(rb.velocity.magnitude - Time.deltaTime * (20 - rb.velocity.magnitude)/40, 0, 50);
            rb.velocity = rb.velocity.normalized * newSpeed;
            
            if (newSpeed < 0.01)
            {
                if (gc.stonesThrown < 4 )
                {
                    if (freeGuard)
                    {
                        if(transform.position.x == 5)
                        {
                            Debug.Log(string.Format("Stones thrown = {0}",gc.stonesThrown));
                            //Remove offending stone
                            gc.stones[gc.stonesThrown-1].transform.position = new Vector3(5, 0.3f, 15 + gc.stonesThrown);
                            gc.stones[gc.stonesThrown - 1].gameObject.SetActive(false);

                            //Move stone back into play
                            transform.position = lastPosition;
                        }
                    }
                    //If the stone is in front of the tee line and outside the house it is in the freeguard zone
                    if (transform.position.z < 17.37 && (transform.position - new Vector3(0, 0.3f, 17.37f)).magnitude > 1.9 ) {
                        freeGuard = true;
                        lastPosition = transform.position;
                    } else
                    {
                        freeGuard = false;
                    }
                }
            }
        }
    }
}
