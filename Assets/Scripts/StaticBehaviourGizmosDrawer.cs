using System;
using UnityEngine;

public class StaticBehaviourGizmosDrawer : MonoBehaviour
{
    private void OnDrawGizmos() {
        PartBehaviour[] behaviours = Resources.LoadAll<PartBehaviour>("PartBehaviours");
        foreach (PartBehaviour behaviour in behaviours) {
            behaviour.DrawStaticGizmos();
        }
    }
}