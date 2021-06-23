using System;
using UnityEngine;

[Serializable]
public struct PlayerConfig
{
	[Header("Player")]
	[Tooltip("Move speed of the character in m/s")]
	public float MoveSpeed;
	[Tooltip("Max speed of the character in m/s")]
	public float MaxSpeed;
	[Tooltip("How fast the character turns to face movement direction")]
	[Range(0.0f, 0.3f)]
	public float RotationSmoothTime;
	[Tooltip("Acceleration and deceleration")]
	public float SpeedChangeRate;

	[Space(10)]
	[Tooltip("The height the player can jump")]
	public float JumpHeight;
	[Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
	public float Gravity;
	[Tooltip("The evolution of the gravity overtime while the player is in the air, default is 1")]
	[Range(0.8f,1.3f)]
	public float GravityMultiplier;

	[Space(10)]
	[Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
	public float JumpTimeout;
	[Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
	public float FallTimeout;

	public float animationBlend;
	public float terminalVelocity;
}
