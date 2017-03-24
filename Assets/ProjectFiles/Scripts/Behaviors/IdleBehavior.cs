using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleBehavior : StateMachineBehaviour {

    
	override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash){
        animator.SetInteger("IdleID", Random.Range(1, 3));
	}

	// OnStateMachineExit is called when exiting a statemachine via its Exit Node
	//override public void OnStateMachineExit(Animator animator, int stateMachinePathHash) {
	//
	//}
}
