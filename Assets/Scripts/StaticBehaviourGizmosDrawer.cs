using System;
using UnityEngine;

public class StaticBehaviourGizmosDrawer : MonoBehaviour
{
    private void OnDrawGizmos() {
        AntBehaviour[] behaviours = Resources.LoadAll<AntBehaviour>("Behaviours");
        foreach (AntBehaviour behaviour in behaviours) {
            behaviour.DrawStaticGizmos();
        }
    }
}