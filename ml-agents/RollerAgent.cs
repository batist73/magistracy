﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class RollerAgent : Agent {
	
	Rigidbody rBody;
	public Transform Target;
	
	public float speed = 10;
    private float previousDistance = float.MaxValue;
	
	void Start () {
		rBody = GetComponent<Rigidbody>();
	}
	
	public override void AgentReset() {
        if (this.transform.position.y < -1.0) {  
            // The agent fell
            this.transform.position = Vector3.zero;
            this.rBody.angularVelocity = Vector3.zero;
            this.rBody.velocity = Vector3.zero;
        } else { 
            // Move the target to a new spot
            Target.position = new Vector3(Random.value * 8 - 4, 0.5f, Random.value * 8 - 4);
        }
    }
	
	public override void CollectObservations() {
        // Calculate relative position
        Vector3 relativePosition = Target.position - this.transform.position;
        
        // Relative position
        AddVectorObs(relativePosition.x / 5);
        AddVectorObs(relativePosition.z / 5);
        
        // Distance to edges of platform
        AddVectorObs((this.transform.position.x + 5) / 5);
        AddVectorObs((this.transform.position.x - 5) / 5);
        AddVectorObs((this.transform.position.z + 5) / 5);
        AddVectorObs((this.transform.position.z - 5) / 5);
        
        // Agent velocity
        AddVectorObs(rBody.velocity.x / 5);
        AddVectorObs(rBody.velocity.z / 5);
    }
	
	public override void AgentAction(float[] vectorAction, string textAction) {
        // Rewards
        float distanceToTarget = Vector3.Distance(this.transform.position, Target.position);
        
        // Reached target
        if (distanceToTarget < 1.42f) {
            AddReward(1.0f);
            Done();
        }
        
        // Getting closer
        if (distanceToTarget < previousDistance) {
            AddReward(0.1f);
        }
    
        // Time penalty
        AddReward(-0.05f);
    
        // Fell off platform
        if (this.transform.position.y < -1.0) {
            AddReward(-1.0f);
            Done();
        }
        previousDistance = distanceToTarget;
		
        // Actions, size = 2
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = vectorAction[0];
        controlSignal.z = vectorAction[1];
        rBody.AddForce(controlSignal * speed);
    }
	
}
