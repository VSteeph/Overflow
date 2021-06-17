using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class PhysicConfig : MonoBehaviour, ISerializationCallbackReceiver
{
    public static float GroundedOffset = -0.14f;
    public static LayerMask GroundLayers;

    [Tooltip("Useful for rough ground")]
    public float groundedOffset;
    [Tooltip("What layers the character uses as ground")]
    public LayerMask groundLayers;

    public void OnAfterDeserialize()
    {
        GroundedOffset = groundedOffset;
        GroundLayers = groundLayers;
    }

    public void OnBeforeSerialize()
    {
        groundedOffset = GroundedOffset;
        groundLayers = GroundLayers;

    }
}
