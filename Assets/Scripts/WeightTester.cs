using UnityEngine;

public class WeightTester : DummyStone
{
    public float w;
    public float r;
    public int h;
    protected void Awake()
    {
        isCurling = true;
        //Give the stone a weight and line and handle.
        weight = w;
        rotation = r;
        handle = h;
        transform.rotation = Quaternion.Euler(0, rotation, 0);
        velocity = weight * transform.forward;
        Debug.Log(velocity);
    }
    protected override void OnEnable()
    {
        return;
    }
    void OnCollisionEnter(Collision c)
    {
        if (c.gameObject.CompareTag("Stone"))
        {
            WeightTester otherStone = c.gameObject.GetComponent<WeightTester>();
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

                //Draw debug lines
                Debug.DrawLine(transform.position + Vector3.up, transform.position + shooterVector.normalized * shooterSize + Vector3.up, Color.red);
                Debug.DrawLine(otherStone.transform.position + Vector3.up, otherStone.transform.position + vectorBetween.normalized * hitSize + Vector3.up, Color.red);

                shooterSize /= 1.1f;
                hitSize *= 1.1f;

                audioSize = audioSize * hitSize / velocity.magnitude; //Will be between 0 and 1.
                


                //Draw debug lines
                Debug.DrawLine(transform.position + Vector3.up, transform.position + shooterVector.normalized * shooterSize + Vector3.up, Color.green);
                Debug.DrawLine(otherStone.transform.position + Vector3.up, otherStone.transform.position + vectorBetween.normalized * hitSize + Vector3.up, Color.green);
                

                velocity = shooterVector.normalized * shooterSize;

                otherStone.velocity = vectorBetween.normalized * hitSize;

                otherStone.lastHit = Time.time;
                lastHit = Time.time;

            }
        }
    }
    public void Update()
    {
        Camera.main.transform.position = transform.position + new Vector3(0, 6, 0);
    }
}