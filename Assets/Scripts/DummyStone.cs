using UnityEngine;

public class DummyStone : MonoBehaviour
{
    public float lastHit;

    //Flags to signal what state the stone is in
    protected bool isCurling;

    public Vector3 velocity;

    //Settings that set the stone's path
    public float weight;
    public float rotation;
    public int handle;

    //Current modifiers for the stone's path
    private float curl;
    private float drag;

    // Use this for initialization
    protected virtual void OnEnable()
    {
        isCurling = true;
        //Give the stone a random weight and line and handle.
        weight = Random.Range(3.91f,4.05f);
        rotation = Random.Range(-1.8f, 1.8f);
        handle = (int) ( -1 * Mathf.Sign(rotation));
        transform.rotation = Quaternion.Euler(0,rotation,0);
        velocity = weight * transform.forward;
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boundary"))
        {
            //HIDE SELF
            gameObject.SetActive(false);
        }
    }
    void OnCollisionEnter(Collision c)
    {
        if (c.gameObject.CompareTag("Stone"))
        {
            DummyStone otherStone = c.gameObject.GetComponent<DummyStone>();
            //Set angle of self and other
            isCurling = false;
            if (Mathf.Abs(lastHit - Time.time) < 0.01)
                return;

            //If the stone is moving faster than the other, it is the shooter. Keeping the logic here means it only gets executed once.
            if (velocity.magnitude > otherStone.velocity.magnitude)
            {
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
            //Make the stone curl more at lower speeds
            if (velocity.magnitude > 1)
            {
                curl = Mathf.Clamp(velocity.x + (handle * Time.deltaTime * (10 - velocity.magnitude) / 2500), -2, 2);
            }
            else
            {
                curl = Mathf.Clamp(velocity.x + (handle * Time.deltaTime * (10 - velocity.magnitude) / 1500), -2, 2);
            }

            //Add the curl on to the current velocity.
            velocity = new Vector3(curl, 0, velocity.z);

            //Subtract the drag from the current velocity.
            drag = 20 * 0.0168f * Time.deltaTime / 11;

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

        {
            float newSpeed;
            drag = 20 * 0.0168f * Time.deltaTime / 11;
            newSpeed = velocity.magnitude - drag;
            velocity = velocity.normalized * newSpeed;

            //When the stone stops
            if (velocity.magnitude < drag)
            {
                velocity = Vector3.zero;
            }
        }
        transform.position = transform.position + velocity * Time.deltaTime;
    }
}
