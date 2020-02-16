using Godot;
using System;

public class playerMovment : KinematicBody2D
{
	
	[Export]
	NodePath animationsPath;

	[Export]
	float ropeLength = 300.0f;

	[Export]
	float wallSlideSpeed = 100.0f;

	[Export]
	public float maxXVel = 300.0f;
	[Export]
	public float maxYVel = 500.0f;

	[Export]
	float airDashMod = 10f;

	[Export]
	public float gravity = 1000f;

	[Export]
	 int walkSpeed = 300;
	 int strafeSpeed = 100;

	[Export]
	int jumpStrength = 1000;

	[Export]
	int player = -1;

	[Export]
	float wallNoSlideTime = 1.0f;

	AnimatedSprite animations;

	float currentWallNoSlideTime = 0.0f;
	public bool isAnchored = false;
	bool onWall = false;
	bool onFloor = false;
	bool wallJumpReady = true;

	public float airXAccel;

	public Vector2 velocity;
	
	double maxJumpVelocity;
	double minJumpVelocity;
	
	double maxJumpHeight = 5 * Globals.UNIT_SIZE;
	double minJumpHeight = 0.8 * Globals.UNIT_SIZE;
	double jumpDuration =  0.5;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		animations = (AnimatedSprite)GetNode(animationsPath);
		airXAccel = 0;
	}
	
	public override void _Input (InputEvent @event)
	{
		if(@event.IsActionReleased("player2_move_jump") && velocity.y < minJumpVelocity)
		{
			velocity.y = (float)minJumpVelocity;
		}
		
	}

	public override void _PhysicsProcess(float delta)
	{
		airXAccel = 0.0f;

		if (onFloor || onWall)
		{
			isAnchored = true;
		}
		else
		{
			isAnchored = false;
		}

		if (!onWall)
		{
			velocity.y += gravity * delta;
			velocity.y = Math.Min(velocity.y, maxYVel);
		}
		else
		{
			currentWallNoSlideTime += delta;
			if(currentWallNoSlideTime < wallNoSlideTime)
			{

				velocity.y = 0;
			}
			else
			{
				velocity.y = wallSlideSpeed;

			}
			//GD.Print("On the wall!");
		}

		if (onFloor)
		{
			currentWallNoSlideTime = 0;
			wallJumpReady = true;
		}

		//Aight this is pretty bad I admit but w/e game jam!
		if (player == 0)
		{

			if (Input.IsActionPressed("player1_move_left"))
			{
				if (onFloor)
				{
					velocity.x = -walkSpeed;
				}
				else
				{
					airXAccel = walkSpeed * airDashMod * delta;
					if (velocity.x > -maxXVel)
					{
						velocity.x -= airXAccel;
					}
					//velocity.x = Math.Max(-maxXVel, velocity.x - airXAccel);
				}
				
			}
			else if (Input.IsActionPressed("player1_move_right"))
			{
				if (onFloor)
				{
					velocity.x = walkSpeed;
				}
				else
				{
					airXAccel = walkSpeed * airDashMod * delta;
					if (velocity.x < maxXVel)
					{
						velocity.x += airXAccel;
					}
					// velocity.x = Math.Min(maxXVel, velocity.x + airXAccel);
				}
			}
			else
			{
				if (onFloor)
				{
					velocity.x = 0;
				}
			}

			if (Input.IsActionPressed("player1_move_jump"))
			{
				if (onFloor)
				{
					velocity.y = -jumpStrength;
				}
				if (onWall && wallJumpReady)
				{
					velocity.y = -jumpStrength;
					wallJumpReady = false;
				}
			}
		}
		else
		{

			if (Input.IsActionPressed("player2_move_left"))
			{
				if (onFloor)
				{
					velocity.x = -walkSpeed;
				}
				else
				{
					airXAccel = walkSpeed * airDashMod * delta;
					if (velocity.x > -maxXVel)
					{
						velocity.x -= airXAccel;
					}
					// velocity.x = Math.Max(-maxXVel, velocity.x - airXAccel);
				}
			}
			else if (Input.IsActionPressed("player2_move_right"))
			{
				if (onFloor)
				{
					velocity.x = walkSpeed;
				}
				else
				{
					airXAccel = walkSpeed * airDashMod * delta;
					if (velocity.x < maxXVel)
					{
						velocity.x += airXAccel;
					}
					//velocity.x = Math.Min(maxXVel, velocity.x + airXAccel);
				}
			}
			else
			{
				if (onFloor)
				{
					velocity.x = 0;
				}
			}
			if (Input.IsActionPressed("player2_move_jump"))
			{
				if (onFloor)
				{
					velocity.y = -jumpStrength;
				}
				if (onWall && wallJumpReady)
				{
					velocity.y = -jumpStrength;
					wallJumpReady = false;
				}
			}
		}
		
		//animation stuff
		if(onFloor && velocity.x != 0){
			if (velocity.x < 0)
			{
				animations.FlipH = false;
			}
			else if (velocity.x > 0)
			{
				animations.FlipH = true;
			}
			animations.Play("run");
		}else if(!onFloor) {
			animations.Play("swing");
		}
		else{
			animations.Play("idle");
		}

		// rope physics below


	}

	public void move()
	{
		MoveAndSlide(velocity, new Vector2(0, -1));

		onFloor = IsOnFloor();
		onWall = (IsOnWall() && wallJumpReady);
	}

	public bool hitNonWall()
	{

		if (GetSlideCount() > 0 && !onFloor)
		{
			KinematicCollision2D col = GetSlideCollision(0);
			return col.Normal.y > 0;
			// true;
		}
		return false;
	}
	public void applyForce(Vector2 force)
	{
		velocity += force;
	}
}
