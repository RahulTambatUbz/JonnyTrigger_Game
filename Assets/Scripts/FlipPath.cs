using UnityEngine;

public enum PlayerStates
{
    Flipping,
    Moving,
    Jumping
}

public class FlipPath : MonoBehaviour
{
    public Transform pointB;
    public float flipDuration;
    public PlayerStates currentState;  // New property to define the action type

    public Transform GetPathPoint()
    {
        return pointB;
    }

    public float GetFlipDuration()
    {
        return flipDuration;
    }

    public PlayerStates GetActionType()
    {
        return currentState;
    }
}
