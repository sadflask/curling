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
            gameObject.SetActive(false);
        } else if (other.CompareTag("CameraProc"))
        {
            //CHANGE CAMERA
            endCam.enabled = false;
            topCam.enabled = true;
            canvas.worldCamera = Camera.current;
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
                otherStone.rb.velocity = otherVector.normalized * rb.velocity.magnitude * (1 - tangDistance / 0.3f);
                rb.velocity = shooterVector.normalized * rb.velocity.magnitude * (tangDistance / 0.3f);

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
                newCurl = Mathf.Clamp(rb.velocity.x + (handle * Time.deltaTime * (20 - rb.velocity.z) / 1250), -2, 2);
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
        }
    }
}
