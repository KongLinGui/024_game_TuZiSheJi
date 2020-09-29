using UnityEngine;
using System.Collections;
using UnitySampleAssets.CrossPlatformInput;

public class Player : MonoBehaviour,ITakeDamage
{
	public BoxCollider2D HeadCollider ;
	public ParticleSystem Jetpack;
	public ParticleSystem TouchTheGroundEffect;
	private GameObject _sceneCamera;

	public bool IsDead {get; private set;}
	public int MaxHealth = 100;
	public int Health {get; private set; }
	public GameObject HurtEffect;
	
	public AudioClip PlayerJumpSfx;
	public AudioClip PlayerHitSfx;
	public AudioClip PlayerShootSfx;
	
	public Transform ProjectileFireLocation;
	public Transform ShellsFireLocation;
	public Transform FlamesFireLocation;
	public Weapon InitialWeapon;
		
	private CorgiController2D _controller;
	private Animator _animator;
	private float _normalizedHorizontalSpeed;
	private float _jumpButtonPressTime = 0;
	private bool _jumpButtonPressed=false;
	private bool _jumpButtonReleased=false;
	private bool _isFacingRight=true;
	private Weapon _weapon;
	// INPUT AXIS
	private float _horizontalMove;
	private float _verticalMove;
	private float _originalGravity;
	private float _fireTimer;
	
	private float _dashDirection = 1f;
	private float _boostForce = 1f;
	
	private Vector2 _InitialProjectileFireLocationPosition;
	private Vector2 _initialGunFlamesPosition;
	
	void Awake()
	{
		_sceneCamera = GameObject.FindGameObjectWithTag("MainCamera");
		_controller = GetComponent<CorgiController2D>();
		Health=MaxHealth;
	}

	public void Start()
	{
		// we get the animator
		_animator = GetComponent<Animator>();
		// if the width of the player is positive, then it is facing right.
		_isFacingRight = transform.localScale.x > 0;
		
		_originalGravity = _controller.Parameters.Gravity;
		
		// we initialize all the controller's states with their default values.
		_controller.State.CanMoveFreely = true;
		_controller.State.CanDash = true;
		_controller.State.CanFire = true;
		_controller.State.CanMelee = true;
		_controller.State.Dashing = false;
		_controller.State.Running = false;
		_controller.State.Crouching = false;
		_controller.State.CrouchingPreviously=false;
		_controller.State.TouchingGroundPreviously=true;
		_controller.State.LookingUp = false;
		_controller.State.WallClinging = false;
		_controller.State.Jetpacking = false;
		_controller.State.Diving = false;
		_controller.State.LadderClimbing=false;
		_controller.State.LadderColliding=false;
		_controller.State.LadderTopColliding=false;
		_controller.State.LadderClimbingSpeed=0f;
		_controller.State.Firing = false;
		_controller.State.FiringStop = false;
		_controller.State.FiringDirection = 3;
		_controller.State.MeleeAttacking=false;
		
		// we save the projectile fire initial location.
		// this is used for 8 direction and 360° firing
		_InitialProjectileFireLocationPosition = transform.position-ProjectileFireLocation.transform.position;
		_initialGunFlamesPosition = FlamesFireLocation.transform.localPosition;
		
		ChangeWeapon(InitialWeapon);
		
		Jetpack.enableEmission=false;
	}
	
	void Update()
	{				
		UpdateAnimator ();
		if (!IsDead)
		{
			GravityActive(true);				
			HorizontalMovement();
			VerticalMovement();
			ClimbLadder();
			WallClinging ();
			
			// If the player is dashing, we cancel the gravity
			if (_controller.State.Dashing) 
			{
				GravityActive(false);
				_controller.SetVerticalForce(0);
			}	
			
			if (!_controller.State.Firing)
			{
				_controller.State.FiringStop=false;
			}
			
			if (_controller.State.CanJump)
			{
				if (_controller.State.IsGrounded) 
				{				
					_controller.State.CanDoubleJump=true;
				}	
				
				// If the user releases the jump button and the player is jumping up and enough time since the initial jump has passed, then stop jumping
				if ( (_jumpButtonPressTime!=0) 
				&& (Time.time - _jumpButtonPressTime >= _controller.Parameters.JumpMinimumAirTime) 
				&& (_controller.Velocity.y > Mathf.Sqrt(-_controller.Parameters.Gravity)) 
				&& (_jumpButtonReleased)
				&& (!_jumpButtonPressed||_controller.State.Jetpacking)   )
				{
					_jumpButtonReleased=false;
					_controller.AddForce(new Vector2(0,12 * _controller.Parameters.Gravity * Time.deltaTime ));			
				}
			}
					
		}
		else
		{			
			_controller.SetHorizontalForce(0);
		}
	}
	
	void LateUpdate()
	{
		// if the player is grounded, we reset the doubleJump flag so he can doubleJump again
		if (_controller.State.IsGrounded) 
		{
			_controller.State.CanDoubleJump=true;
		}
	}
	
	private void UpdateAnimator()
	{
		_controller.State.CanFire=true;
		// we set each of the animators parameters to their corresponding State values
		_animator.SetBool("Grounded",_controller.State.IsGrounded);
		_animator.SetFloat("Speed",Mathf.Abs(_controller.Velocity.x));
		_animator.SetFloat("vSpeed",_controller.Velocity.y);
		_animator.SetBool("Running",_controller.State.Running);
		_animator.SetBool("Dashing",_controller.State.Dashing);
		_animator.SetBool("Crouching",_controller.State.Crouching);
		_animator.SetBool("LookingUp",_controller.State.LookingUp);
		_animator.SetBool("WallClinging",_controller.State.WallClinging);
		_animator.SetBool("Jetpacking",_controller.State.Jetpacking);
		_animator.SetBool("Diving",_controller.State.Diving);
		_animator.SetBool("LadderClimbing",_controller.State.LadderClimbing);
		_animator.SetFloat("LadderClimbingSpeed",_controller.State.LadderClimbingSpeed);
		_animator.SetBool("FiringStop",_controller.State.FiringStop);
		_animator.SetBool("Firing",_controller.State.Firing);
		_animator.SetInteger("FiringDirection",_controller.State.FiringDirection);
		_animator.SetBool("MeleeAttacking",_controller.State.MeleeAttacking);
	}
	
	public void SetHorizontalMove(float value)
	{
		_horizontalMove=value;
	}
	
	public void SetVerticalMove(float value)
	{
		_verticalMove=value;
	}

	private void HorizontalMovement()
	{	
		if (!_controller.State.CanMoveFreely)
			return;				
				
		// If the value of the horizontal axis is positive, the player must face right.
		if (_horizontalMove>0.1)
		{
			_normalizedHorizontalSpeed = _horizontalMove;
			if (!_isFacingRight)
				Flip();
		}
		
		// If it's negative, then we're facing left
		else if (_horizontalMove<-0.1)
		{
			_normalizedHorizontalSpeed = _horizontalMove;
			if (_isFacingRight)
				Flip();
		}
		else
		{
			_normalizedHorizontalSpeed=0;
		}
		
		var movementFactor = _controller.State.IsGrounded ? _controller.Parameters.SpeedAccelerationOnGround : _controller.Parameters.SpeedAccelerationInAir;
		_controller.SetHorizontalForce(Mathf.Lerp(_controller.Velocity.x, _normalizedHorizontalSpeed * _controller.Parameters.MovementSpeed, Time.deltaTime * movementFactor));
		
	}

	private void VerticalMovement()
	{
		// Manages the ground touching effect
		if (_controller.State.IsGrounded)
		{
			if (_controller.State.TouchingGroundPreviously!=_controller.State.IsGrounded)
			{
				Instantiate(TouchTheGroundEffect,new Vector2(transform.position.x,transform.position.y-transform.localScale.y/2),transform.rotation);	
				
			}			
		}
		_controller.State.TouchingGroundPreviously=_controller.State.IsGrounded;
			
		// if the player is not in a position where it can move freely, we do nothing.
		if (!_controller.State.CanMoveFreely)
			return;
			
		// Crouch Detection
		if ( (_verticalMove<-0.1) && (_controller.State.IsGrounded) )
		{
			_controller.State.Crouching = true;
			_controller.Parameters.MovementSpeed = _controller.Parameters.CrouchSpeed;
			_controller.State.Running=false;
			
		}
		else
		{		
			bool headCheck = Physics2D.OverlapCircle(HeadCollider.transform.position,HeadCollider.size.x/2,_controller.PlatformMask);
			
			// if the player is not crouched anymore, we set 
			if (!headCheck)
			{
				if (!_controller.State.Running)
				_controller.Parameters.MovementSpeed = _controller.Parameters.WalkSpeed;
				_controller.State.Crouching = false;
				_controller.State.CanJump=true;
				_controller.State.CanDash = true;
			}
			else
			{
				_controller.State.CanJump=false;
				_controller.State.CanDash = false;
			}
		}
		
		if (_controller.State.CrouchingPreviously!=_controller.State.Crouching)
		{
			Invoke ("RecalculateRays",Time.deltaTime*10);		
		}
		
		_controller.State.CrouchingPreviously=_controller.State.Crouching;
		
		// Looking up
		if ( (_verticalMove>0) && (_controller.State.IsGrounded) )
		{
			_controller.State.LookingUp = true;		
		}
		else
		{
			_controller.State.LookingUp = false;
			}
		
	}
	// Use this method to recalculate the rays, especially useful when the size of the player has changed.
	public void RecalculateRays()
	{
		_controller.recalculateDistanceBetweenRays();
	}
		
	// RUN -----------------------------------------------------------------------------------------------------------------------------------
		
	public void RunStart()
	{		
		// if the player is not in a position where it can move freely, we do nothing.
		if (!_controller.State.CanMoveFreely)
			return;
	
		// if the player presses the run button and if we're on the ground and not crouching and we can move freely, 
		// then we change the movement speed in the controller's parameters.
		if (_controller.State.IsGrounded && !_controller.State.Crouching)
		{
			_controller.Parameters.MovementSpeed = _controller.Parameters.RunSpeed;
			_controller.State.Running=true;
		}
	}
	
	public void RunStop()
	{
		// if the run button is released, we revert back to the walking speed.
		_controller.Parameters.MovementSpeed = _controller.Parameters.WalkSpeed;
		_controller.State.Running=false;
	}
	
	// JUMP -----------------------------------------------------------------------------------------------------------------------------------
	
	public void JumpStart()
	{
		if (!_controller.State.CanJump)
			return;
		float wallJumpDirection;
		
		// when the user presses the jump button, if the player is grounded or if it can double jump or if it's wallclinging, we make the player jump
		if( 
			(_controller.State.IsGrounded || _controller.State.LadderClimbing || _controller.State.CanDoubleJump || _controller.State.WallClinging) 
			&& !_controller.State.Jetpacking 
		)
		{
			// if the player is standing on a one way platform (layermask n°11) and is also pressing the down button,
			// we make it fall down below the platform
			if (_verticalMove<0 && _controller.State.IsGrounded)
			{
				if (_controller.StandingOn.layer==11)
				{
					_controller.transform.position=new Vector2(transform.position.x,transform.position.y-0.1f);
					return;
				}
			}
			
			_controller.State.LadderClimbing=false;
			_controller.State.CanMoveFreely=true;
			GravityActive(true);
			
			_jumpButtonPressTime=Time.time;
			_jumpButtonPressed=true;
			_jumpButtonReleased=false;
			
			_controller.SetVerticalForce(Mathf.Sqrt( 2f * _controller.Parameters.JumpHeight * -_controller.Parameters.Gravity ));
			_animator.Play( Animator.StringToHash( "jumpAndFall" ) );
			//AudioSource.PlayClipAtPoint(PlayerJumpSfx,transform.position);
			
			// if the player can double jump and is no more grounded and is not dashing, then it can't double jump anymore
			if (_controller.State.CanDoubleJump && !_controller.State.IsGrounded && !_controller.State.Dashing && !_controller.State.WallClinging && !_controller.State.Jetpacking)
			{
				_controller.State.CanDoubleJump=false;
			}			
			
			// wall jump
			if (_controller.State.WallClinging)
			{
				
				// If the player is colliding to the right with something (probably the wall)
				if (_controller.State.IsCollidingRight)
				{
					wallJumpDirection=-1f;
				}
				else
				{					
					wallJumpDirection=1f;
				}
				StartCoroutine( Boost(_controller.Parameters.WallJumpDuration,wallJumpDirection*_controller.Parameters.WallJumpForce,0,"wallJump") );
				_controller.State.WallClinging=false;
			}
		}
	
	}
	
	public void JumpStop()
	{
		_jumpButtonPressed=false;
		_jumpButtonReleased=true;
	}
		
	public void JetpackStart()
	{
		// if the player is not in a position where it can move freely, we do nothing.
		if (!_controller.State.CanMoveFreely)
			return;
		
		_controller.SetVerticalForce(_controller.Parameters.JetpackForce);
		_controller.State.Jetpacking=true;
		_controller.State.CanMelee=false;
		Jetpack.enableEmission=true;
	}

	public void JetpackStop()
	{
		_controller.State.Jetpacking=false;
		_controller.State.CanMelee=true;
		Jetpack.enableEmission=false;
	}
		
	
	
	void ClimbLadder()
	{
		if (_controller.State.LadderColliding)
		{
			if (_verticalMove>0.1 && !_controller.State.LadderClimbing && !_controller.State.LadderTopColliding  && !_controller.State.Jetpacking)
			{			
				_controller.State.LadderClimbing=true;
				_controller.State.CanMoveFreely=false;
				ShootStop();
				_controller.State.LadderClimbingSpeed=0;
				
				_controller.SetHorizontalForce(0);
				_controller.SetVerticalForce(0);
				GravityActive(false);
			}			
			
			if (_controller.State.LadderClimbing)
			{
				_controller.State.CanFire=false;
				GravityActive(false);
				_controller.SetVerticalForce(_verticalMove * _controller.Parameters.LadderSpeed);
				_controller.State.LadderClimbingSpeed=Mathf.Abs(_verticalMove);				
			}
			
			if (_controller.State.LadderClimbing && _controller.State.IsGrounded)
			{
				_controller.State.LadderColliding=false;
				_controller.State.LadderClimbing=false;
				_controller.State.CanMoveFreely=true;
				_controller.State.LadderClimbingSpeed=0;	
				GravityActive(true);			
			}			
		}
		
		// If the player is colliding with the top of the ladder and is pressing down and is not on the ladder yet and is standing on the ground, we make it go down.
		if (_controller.State.LadderTopColliding && _verticalMove<-0.1 && !_controller.State.LadderClimbing && _controller.State.IsGrounded)
		{
			transform.position=new Vector2(transform.position.x,transform.position.y-0.1f);
			_controller.State.LadderClimbing=true;
			_controller.State.CanMoveFreely=false;
			_controller.State.LadderClimbingSpeed=0;
			
			_controller.SetHorizontalForce(0);
			_controller.SetVerticalForce(0);
			GravityActive(false);
		}
		
	}
	
	public void ShootOnce()
	{
		// if the player can't fire, we do nothing		
		if (!_controller.State.CanFire)
		{			
			// we just reset the firing direction (this happens when the player gets on a ladder for example.
			_controller.State.FiringDirection=3;
			return;		
		}
		
		// if the player is not in a position where it can move freely, we do nothing.
		if (!_controller.State.CanMoveFreely)
			return;
			
		// we fire a projectile and reset the fire timer
		FireProjectile();	
		_fireTimer = 0;	
	}
	
	public void ShootStart()
	{
		// if the player can't fire, we do nothing		
		if (!_controller.State.CanFire)
		{			
			// we just reset the firing direction (this happens when the player gets on a ladder for example.
			_controller.State.FiringDirection=3;
			return;		
		}
	
		// if the player is not in a position where it can move freely, we do nothing.
		if (!_controller.State.CanMoveFreely)
			return;
			
		
		// firing state reset								
		_controller.State.FiringStop = false;			
		_controller.State.Firing = true;
			
		_weapon.gunFlames.enableEmission=true;
		_weapon.gunShells.enableEmission=true;	
		_fireTimer += Time.deltaTime;
		if(_fireTimer > _weapon.FireRate)
		{
			FireProjectile();
			_fireTimer = 0; // reset timer for fire rate
		}			
		//AudioSource.PlayClipAtPoint(PlayerShootSfx,transform.position);
	}
	
	public void ShootStop()
	{
		// if the player can't fire, we do nothing		
		if (!_controller.State.CanFire)
		{			
			// we just reset the firing direction (this happens when the player gets on a ladder for example.
			_controller.State.FiringDirection=3;
			return;		
		}
		_controller.State.FiringStop = true;		
		_controller.State.Firing = false;
		// reset the firing direction
		_controller.State.FiringDirection=3;
		_weapon.gunFlames.enableEmission=false;
		_weapon.gunShells.enableEmission=false;	
	}
	
	public void ChangeWeapon(Weapon newWeapon)
	{
		// weapon instanciation
		_weapon=(Weapon)Instantiate(newWeapon,transform.position,transform.rotation);	
		_weapon.transform.parent = transform;
		// we turn off the gun's emitters.
		_weapon.gunFlames.enableEmission=false;
		_weapon.gunShells.enableEmission=false;	
	}
	
	void FireProjectile () 
	{
		// we get the direction the player is inputing.		
		float HorizontalShoot = _horizontalMove;
		float VerticalShoot = _verticalMove;
		
		// if we don't want 8 direction shooting at all, we set these two floats to zero.
		if (!_controller.Parameters.EightDirectionShooting)
		{
			HorizontalShoot=0;
			VerticalShoot=0;
		}
		
		// if we want a strict 8 direction shot, we round the direction values.		
		if (_controller.Parameters.StrictEightDirectionShooting)
		{
			HorizontalShoot = Mathf.Round(HorizontalShoot);
			VerticalShoot = Mathf.Round(VerticalShoot);
		}
					
		// we calculate the angle based on the buttons the player is pressing to determine the direction of the shoot.									
		float angle = Mathf.Atan2(HorizontalShoot, VerticalShoot) * Mathf.Rad2Deg;
		Vector2 direction = Vector2.up;
		
		// if the user is not pressing any direction button, we set the shoot direction based on the direction it's facing.
		if (HorizontalShoot>-0.1f && HorizontalShoot<0.1f && VerticalShoot>-0.1f && VerticalShoot<0.1f )
		{
			direction=_isFacingRight?Vector2.right : -Vector2.right;
		}
		
		// We set the animation depending on where the player is shooting
		
		// if shooting up
		if ( Mathf.Abs(HorizontalShoot)<0.1f && VerticalShoot>0.1f )
			_controller.State.FiringDirection=1;
		// if shooting diagonal up
		if ( Mathf.Abs(HorizontalShoot)>0.1f && VerticalShoot>0.1f )
			_controller.State.FiringDirection=2;
		// if shooting diagonal down
		if ( Mathf.Abs(HorizontalShoot)>0.1f && VerticalShoot<-0.1f )
			_controller.State.FiringDirection=4;
		// if shooting down
		if ( Mathf.Abs(HorizontalShoot)<0.1f && VerticalShoot<-0.1f )
			_controller.State.FiringDirection=5;
		if (Mathf.Abs(VerticalShoot)<0.1f)
			_controller.State.FiringDirection=3;
		
		// we move the ProjectileFireLocation according to the angle
		float horizontalDirection=_isFacingRight?1f:-1f;
		float horizontalModifier=1;
		if (Mathf.Abs(HorizontalShoot)<0.1f)
		{
			horizontalModifier=0;
		}
		if (Mathf.Abs(VerticalShoot)<0.1f)
		{
			horizontalModifier=0.5f;
		}
		ProjectileFireLocation.transform.position=new Vector2(transform.position.x+horizontalModifier*horizontalDirection*Mathf.Abs(_InitialProjectileFireLocationPosition.x),transform.position.y-Mathf.Abs(_InitialProjectileFireLocationPosition.y)+VerticalShoot);
		
		_weapon.gunFlames.transform.position=new Vector2(transform.position.x+horizontalModifier*horizontalDirection*Mathf.Abs(_initialGunFlamesPosition.x),transform.position.y-Mathf.Abs(_initialGunFlamesPosition.y)+VerticalShoot);
		// we change the direction the gun flames are emitted
		//gunFlames.transform.rotation=Quaternion.Euler(0,0,90);
		
		// we apply the angle rotation to the direction.
		direction = Quaternion.Euler(0,0,-angle) * direction;
		
		// we instantiate the projectile at the projectileFireLocation's position.
				
		var projectile = (Projectile)Instantiate(_weapon.Projectile,ProjectileFireLocation.position,ProjectileFireLocation.rotation);
		projectile.Initialize(gameObject,direction,_controller.Velocity);
		
	}
	
	// Melee / Lightsaber attack
	public void Melee()
	{	
		// if the player is not in a position where it can move freely, we do nothing.
		if (!_controller.State.CanMoveFreely)
			return;
	
		// if the user can melee (for example, not jetpacking)
		if (_controller.State.CanMelee)
		{	
			// we set the meleeAttacking state to true, which will trigger the melee animation, enabling the player's MeleeArea circle collider
			_controller.State.MeleeAttacking=true;
			// we start the coroutine that will end the melee state in 0.3 seconds (tweak that depending on your animation)
			StartCoroutine(MeleeEnd());			
		}
	}
	
	IEnumerator MeleeEnd()
	{
		// after 0.3 seconds, we end the melee state
		yield return new WaitForSeconds(0.3f);
		// reset state
		_controller.State.MeleeAttacking=false;
	}

	// Handles dash and dive
	public void Dash()
	{
		
		// if the player is not in a position where it can move freely, we do nothing.
		if (!_controller.State.CanMoveFreely)
			return;
		
		// If the user presses the dash button and is not aiming down
		if (_verticalMove>-0.8) 
		{	
			if (_controller.State.CanDash)
			{
				_controller.State.Dashing=true;

				
				if (_isFacingRight) { _dashDirection=1f; } else { _dashDirection = -1f; }
				_boostForce=_dashDirection*_controller.Parameters.DashForce;
				_controller.State.CanDash = false;
				StartCoroutine( Boost(_controller.Parameters.DashDuration,_boostForce,0,"dash") );
			}			
		}
		
		if (_verticalMove<-0.8) 
		{
			StartCoroutine(Dive());
		}		
		
	}

	private void WallClinging()
	{
		if (!_controller.State.IsCollidingLeft && !_controller.State.IsCollidingRight)
		{
			_controller.State.WallClinging=false;
		}
	
		// if the player is not in a position where it can move freely, we do nothing.
		if (!_controller.State.CanMoveFreely)
			return;
	
		// if the player is in the air and touching a wall and moving in the opposite direction, then we slow its fall
		if((!_controller.State.IsGrounded) && ( ( (_controller.State.IsCollidingRight) && (_horizontalMove>-0.1f) )	|| 	( (_controller.State.IsCollidingLeft) && (_horizontalMove<0.1f) )	))
		{
			if (_controller.Velocity.y<0)
			{
				_controller.State.WallClinging=true;
				_controller.AddForce(new Vector2(0,-(_controller.Parameters.Gravity * Time.deltaTime)*0.9f));
			}
		}
		else
		{
			_controller.State.WallClinging=false;
		}
	}

	IEnumerator Boost(float boostDuration, float boostForceX, float boostForceY, string name) //Coroutine with a single input of a float called boostDur, which we can feed a number when calling
	{
		float time = 0f; //create float to store the time this coroutine is operating
		
		while(boostDuration > time) //we call this loop every frame while our custom boostDuration is a higher value than the "time" variable in this coroutine
		{
			if (boostForceX!=0)
			{
				_controller.AddForce(new Vector2(boostForceX,0));
			}
			if (boostForceY!=0)
			{
				_controller.AddForce(new Vector2(0,boostForceY));
			}
			time+=Time.deltaTime;
			yield return 0; //go to next frame
		}
		if (name=="dash")
		{
			_controller.State.Dashing=false;
			GravityActive(true);
			//MoveModifierHorizontal=0f;
			yield return new WaitForSeconds(_controller.Parameters.DashCooldown); //Cooldown time for being able to boost again, if you'd like.
			_controller.State.CanDash = true; //set back to true so that we can boost again.
		}	
		if (name=="wallJump")
		{
			//MoveModifierHorizontal=0f;
		}		
	}

	IEnumerator Dive()
	{	
		// Shake parameters : intensity, duration (in seconds) and decay
		Vector3 ShakeParameters = new Vector3(1.5f,0.5f,1f);
		_controller.State.Diving=true;
		while (!_controller.State.IsGrounded)
		{
			_controller.SetVerticalForce(_controller.Parameters.Gravity*2);
			yield return 0; //go to next frame
		}
		
		// Shake the scene 		
		_sceneCamera.SendMessage("Shake",ShakeParameters);
		
		_controller.State.Diving=false;
	}
	
	private void GravityActive(bool state)
	{
		if (state==true)
		{
			if (_controller.Parameters.Gravity==0)
			{
				_controller.Parameters.Gravity = _originalGravity;
			}
		}
		else
		{
			_controller.Parameters.Gravity = 0;
		}
	}
	
	
	public void Pause()
	{	
		// if time is not already stopped		
		if (Time.timeScale>0.0f)
		{
			GameManager.Instance.SetTimeScale(0.0f);
			GameManager.Instance.Paused=true;
			GUIManager.Instance.SetPause();
		}
		else
		{
			GameManager.Instance.ResetTimeScale();	
			GameManager.Instance.Paused=false;
			GUIManager.Instance.UnPause();	
		}		
	}
	
	
	
	IEnumerator Flicker()
	{
		// this function handles the flicker of the player's sprite when it's hit.
		
		Color whateverColor = new Color32(255, 20, 20, 255); //edit r,g,b and the alpha values to what you want
		for(var n = 0; n < 10; n++)
		{
			GetComponent<Renderer>().material.color = Color.white;
			yield return new WaitForSeconds (0.05f);
			GetComponent<Renderer>().material.color = whateverColor;
			yield return new WaitForSeconds (0.05f);
		}
		GetComponent<Renderer>().material.color = Color.white;
		// makes the player colliding again with layer 12 (Projectiles) and 13 (Enemies)
		Physics2D.IgnoreLayerCollision(9,12,false);
		Physics2D.IgnoreLayerCollision(9,13,false);
	}
	
	public void Kill()
	{
		_controller.HandleCollisions=false;
		GetComponent<Collider2D>().enabled=false;
		IsDead=true;
		_controller.SetForce(new Vector2(0,10));
		Health=0;
	}
	
	public void FinishLevel()
	{
		enabled=false;
		_controller.enabled=false;
		GetComponent<Collider2D>().enabled=false;		
	}
	
	public void RespawnAt(Transform spawnPoint)
	{
		if(!_isFacingRight)
		{
			Flip ();
		}
		IsDead=false;
		GetComponent<Collider2D>().enabled=true;
		_controller.HandleCollisions=true;
		transform.position=spawnPoint.position;
		Health=MaxHealth;
	}
	
	public void TakeDamage(int damage,GameObject instigator)
	{
		//AudioSource.PlayClipAtPoint(PlayerHitSfx,transform.position);
		
		// When the player takes damage, we create an auto destroy hurt particle system
		Instantiate(HurtEffect,transform.position,transform.rotation);
		// we prevent the player from colliding with layer 12 (Projectiles) and 13 (Enemies)
		Physics2D.IgnoreLayerCollision(9,12,true);
		Physics2D.IgnoreLayerCollision(9,13,true);
		// We make the player sprite flicker
		StartCoroutine(Flicker());		
		
		Health -= damage;
		if (Health<=0)
		{
			LevelManager.Instance.KillPlayer();
		}
	}
	
	public void GiveHealth(int health,GameObject instigator)
	{
		// this function adds health to the player's Health and prevents it to go above MaxHealth.
		Health = Mathf.Min (Health + health,MaxHealth);
	}
	
	private void Flip()
	{
		// Flips the player horizontally
		transform.localScale = new Vector3(-transform.localScale.x,transform.localScale.y,transform.localScale.z);
		_isFacingRight = transform.localScale.x > 0;
		
		// we flip the emitters individually because they won't flip otherwise.
		Jetpack.transform.eulerAngles = new Vector3(Jetpack.transform.eulerAngles.x,Jetpack.transform.eulerAngles.y+180,Jetpack.transform.eulerAngles.z);
		ShellsFireLocation.transform.eulerAngles = new Vector3(ShellsFireLocation.transform.eulerAngles.x,ShellsFireLocation.transform.eulerAngles.y+180,ShellsFireLocation.transform.eulerAngles.z);
		FlamesFireLocation.transform.eulerAngles = new Vector3(FlamesFireLocation.transform.eulerAngles.x,FlamesFireLocation.transform.eulerAngles.y+180,FlamesFireLocation.transform.eulerAngles.z);		
	}
	
}
