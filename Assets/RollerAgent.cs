using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollerAgent : Agent
{
    
    Rigidbody rBody;

	// Use this for initialization
	void Start () {
        rBody = GetComponent<Rigidbody>();	
	}

    public Transform Target;
    public override void AgentReset()
    {
        if (this.transform.position.y < -1.0)
        {
            //the agent fell
            this.transform.position = Vector3.zero;
            this.rBody.angularVelocity = Vector3.zero;
            this.rBody.velocity = Vector3.zero;
        }
        else
        {
            //Move the target to a new spot
            Target.position = new Vector3(Random.value * 8 - 4, 0.5f, Random.value * 8 - 4);
        }
    }


    public override List<float> CollectState()
    {
        List<float> state = new List<float>();
        // Calculate relative position
        Vector3 relativePosition = Target.position - this.transform.position;

        // Relative position
        state.Add(relativePosition.x / 5);
        state.Add(relativePosition.z / 5);

        // Distance to edges of platform
        state.Add((this.transform.position.x + 5) / 5);
        state.Add((this.transform.position.x - 5) / 5);
        state.Add((this.transform.position.z + 5) / 5);
        state.Add((this.transform.position.z - 5) / 5);

        // Agent velocity
        state.Add(rBody.velocity.x / 5);
        state.Add(rBody.velocity.z / 5);

        return state;
    }

    public float speed = 10;
    private float previousDistance = float.MaxValue;
    //public override void AgentStep(float[] vectorAction, string textAction) { }

    public override void AgentStep(float[] vectorAction)
    {
        // Rewards
        float distanceToTarget = Vector3.Distance(this.transform.position,
                                                  Target.position);

        // Reached target
        if (distanceToTarget < 1.42f)
        {
            reward = reward + 1;
            //AddReward(1.0f);
            //Done();
            done = true;
        }

        // Getting closer
        if (distanceToTarget < previousDistance)
        {
            reward = reward + 0.1f;
            //AddReward(0.1f);
        }

        // Time penalty
        //AddReward(-0.05f);
        reward = reward + -0.05f;

        // Fell off platform
        if (this.transform.position.y < -1.0)
        {
            //AddReward(-1.0f);
            reward =  reward -1;
            //Done();
            done = true;
        }
        previousDistance = distanceToTarget;



        // Actions, size = 2
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = vectorAction[0];
        controlSignal.z = vectorAction[1];
        rBody.AddForce(controlSignal * speed);
        Debug.Log(reward);
    }

}
