using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Controller2D))]
public class Player : MonoBehaviour{

    #region Variables
    [Header("Movement")]
	[SerializeField] Vector3 velocity;
	[Space]

	[Header("CurretSpeed")]
	[SerializeField] float acceleration;
	[SerializeField] float friction;
	[SerializeField] float maxSpeed;

	[Header("Walk")]
	[SerializeField] float walkAcceleration;
	[SerializeField] float walkFriction;
	[SerializeField] float walkMaxSpeed;

	[Header("Run")]
	[SerializeField] float runAcceleration;
	[SerializeField] float runFriction;
	[SerializeField] float runMaxSpeed;
	[SerializeField] float deceleration;

	[Header("Air")]
	[SerializeField] float airAcceleration;
	[SerializeField] float airFriction;
	[SerializeField] float airMaxSpeedAir;

	[Header("Jump")]
	[SerializeField] float maxJumpHeight = 4;
	[SerializeField] float minJumpHeight = 1;
	[SerializeField] float timeToJumpApex = .4f;
	[SerializeField] bool isJumping;
	[SerializeField] float doubleJump = 5;

	[Header("Trampoline Bounce")]
	[SerializeField] float lowBounceHeight;
	float lowBounceVelocity;
	[SerializeField] float highBounceHeight;
	float highBounceVelocity;

	[Header("Enemy Bounce")]
	[SerializeField] float lowEnemyHeight;
	float lowEnemyVelocity;
	[SerializeField] float highEnemyHeight;
	float highEnemyVelocity;

	[Header("Fall")]
	[SerializeField] float fallMaxSpeed;

	[Header("Coyote Time")]
	[SerializeField] float coyoteTime = 0.2f;
	[SerializeField] float coyoteTimeCounter;

	[Header("Buffer Jump")]
	[SerializeField] float jumpBufferTime = 0.2f;
	[SerializeField] float jumpBufferCounter;

	[Header("CornerCorrection")]
	[SerializeField] float correction;
	[SerializeField] Vector3 cornerR;
	[SerializeField] Vector3 cornerMR;
	[SerializeField] Vector3 cornerML;
	[SerializeField] Vector3 cornerL;
	[SerializeField] float correctionDistance;

	[Header("Boleans")]
	[SerializeField] bool isInvencible;
	[SerializeField] bool isMoving;

	[Header("Other")]
	[SerializeField] Projectile projectile;
	[SerializeField] PowerUp mushroom;

	ParticleSystem bigCloud;

	float direction;

	float gravity;
	float maxJumpVelocity;
	float minJumpVelocity;

	Controller2D controller;
	SpriteRenderer sprite;
	Material material;
	Animator anim;

	Vector2 directionalInput;
	#endregion

	public Controller2D Controller { get => controller; set => controller = value; }
    public Vector3 Velocity { get => velocity; set => velocity = value; }
    public float Direction { get => direction; set => direction = value; }

    void Awake()
    {
		controller = GetComponent<Controller2D>();
		anim = GetComponent<Animator>();
		sprite = GetComponent<SpriteRenderer>();
		material = sprite.material;
		isMoving = true;
	}


    void Start() {
		bigCloud = transform.Find("BigCloud").GetComponent<ParticleSystem>();

		anim.runtimeAnimatorController = GameManager.instance.powerUp.aniMario;

		if (GameManager.instance.powerUp.projectile != null)
			projectile = GameManager.instance.powerUp.projectile.GetComponent<Projectile>();

		if (GameManager.instance.powerUp.frogJump == 0)
		{
			gravity = -((2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2));
		}
		else
		{
			gravity = -((2 * GameManager.instance.powerUp.frogJump) / Mathf.Pow(timeToJumpApex, 2));
		}

		maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
		minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);

		lowBounceVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * lowBounceHeight);
		highBounceVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * highBounceHeight);

		lowEnemyVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * lowEnemyHeight);
		highEnemyVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * highEnemyHeight);


	}

	void Update()
	{
		directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

		if (isMoving)
		{
			if (!PauseMenu.instance.IsPaused)
			{
				CoyoteTime();
				Jump();
				WalkRun();
				Flip();
				PowerUpAttack();
				PowerUpDoubleJump();
				Animation();
			}

			if (Input.GetButtonDown("Pause"))
				PauseMenu.instance.TogglePause();
		}

		ApplyMovement();
	}

    void PowerUpAttack()
    {
		if (GameManager.instance.powerUp.hasAttack && GameObject.FindGameObjectsWithTag("PlayerProjectile").Length != 2)
        {
			if (Input.GetButtonDown("Atk"))
			{
				anim.SetTrigger("Atk");
			}
        }
	}

	void PowerUpDoubleJump()
    {
		if (GameManager.instance.powerUp.hasDoubleJump)
		{
			if (Input.GetButtonDown("Jump") && !controller.collisions.below && velocity.y < 0)
			{
				anim.SetTrigger("DoubleJump");
				velocity.y = doubleJump;
			}
		}
	}

	void Animation()
    {
		anim.SetFloat("HInput", directionalInput.x);
		anim.SetFloat("VInput", directionalInput.y);
		anim.SetFloat("HSpeed", velocity.x);
		anim.SetFloat("VSpeed", Mathf.Abs(velocity.y));
		anim.SetBool("IsGrounded", controller.collisions.below);
	}

	void Flip()
    {
		Vector2 scale = transform.localScale;
		if (directionalInput.x > 0)
			scale.x = 1;
		if (directionalInput.x < 0)
			scale.x = -1;
		transform.localScale = scale;
		direction = scale.x;
    }

	void ApplyMovement()
    {
		velocity.y += gravity * Time.deltaTime;

		controller.Move(velocity * Time.deltaTime, directionalInput);

		if (controller.collisions.above || controller.collisions.below)
			if (controller.collisions.slidingDownMaxSlope)
				velocity.y += controller.collisions.slopeNormal.y * -gravity * Time.deltaTime;
			else
				velocity.y = 0;

		if (velocity.y < -fallMaxSpeed)
			velocity.y = -fallMaxSpeed;
	}

	void CoyoteTime()
    {
		if (controller.collisions.below)
			coyoteTimeCounter = coyoteTime;
		else if (coyoteTimeCounter > 0)
			coyoteTimeCounter -= Time.deltaTime;
    }

	void Jump()
	{
		//Buffer
		if (Input.GetButtonDown("Jump"))
			jumpBufferCounter = jumpBufferTime;
		else if (jumpBufferCounter > 0)
			jumpBufferCounter -= Time.deltaTime;

		//Salto
		if (coyoteTimeCounter > 0 && velocity.y <= 0 && jumpBufferCounter > 0)
		{
			if (controller.collisions.slidingDownMaxSlope)
			{
				if (directionalInput.x != -Mathf.Sign(controller.collisions.slopeNormal.x))
				{ // not jumping against max slope
					velocity.y = maxJumpVelocity * controller.collisions.slopeNormal.y;
					velocity.x = maxJumpVelocity * controller.collisions.slopeNormal.x;
				}
			}
            else
            {
				isJumping = true;
                velocity.y = maxJumpVelocity;
			}

			jumpBufferCounter = 0;
			
			AudioManager.instance.Play("JumpSuper");
		}

		//Soltar para salto más corto
		if (Input.GetButtonUp("Jump"))
		{
			if (velocity.y > minJumpVelocity && isJumping)
			{
				velocity.y = minJumpVelocity;
			}

			coyoteTimeCounter = 0;
		}

		if (velocity.y <= 0)
			isJumping = false;
	}

	void WalkRun()
	{
		if (Input.GetButton("Atk"))
		{
			acceleration = runAcceleration;
			friction = runFriction;
			maxSpeed = runMaxSpeed;
			Debug.Log("PLAYER: Correr");
		}
		else
		{
			if (Mathf.Abs(velocity.x) <= walkMaxSpeed)
			{
				acceleration = walkAcceleration;
				friction = walkFriction;
				maxSpeed = walkMaxSpeed;
				Debug.Log("PLAYER: Andar");
			}
			else
			{
				velocity.x -= velocity.x * deceleration * Time.deltaTime;
				Debug.Log("PLAYER: Frenado al correr");
			}
		}
		//Velocidad 0 al colisionar a los lados o cuando es muy reducida
		if (((controller.collisions.right && velocity.x > 0) || (controller.collisions.left && velocity.x < 0)) || Mathf.Abs(velocity.x) < 0.01f)
			velocity.x = 0;
		//Aceleración hasta una velocidad máxima cuando pulso a los lados y no me agacho y frenado si no pulso niguna dirección
		if (directionalInput.x != 0 && (directionalInput.y != -1 || (directionalInput.y == -1 && !controller.collisions.below)))
			velocity.x = Mathf.Clamp(velocity.x + directionalInput.x * acceleration * Time.deltaTime, -maxSpeed, maxSpeed);
		else
			velocity.x -= velocity.x * friction * Time.deltaTime;

		if (controller.collisions.below && (directionalInput.x > 0 && velocity.x < 0 || directionalInput.x < 0 && velocity.x > 0))
			PlaySound("Skid");
	}

	public void BounceEnemy()
	{
		if (Input.GetButton("Jump"))
        {
			velocity.y = highEnemyVelocity;
		}
		else
        {
			velocity.y = lowEnemyVelocity;
		}
		AudioManager.instance.Play("Stomp");
	}

	public void BounceTrampoline()
	{
		if (jumpBufferCounter > 0)
		{
			velocity.y = highBounceVelocity;
		}
		else
		{
			velocity.y = lowBounceVelocity;
		}
	}

	void CornerCorrection()
	{
		RaycastHit2D cornerLeft = Physics2D.Raycast(transform.position + cornerL, Vector2.up, correctionDistance, controller.collisionMask);
		RaycastHit2D cornerMidL = Physics2D.Raycast(transform.position + cornerML, Vector2.up, correctionDistance, controller.collisionMask);
		RaycastHit2D cornerMidR = Physics2D.Raycast(transform.position + cornerMR, Vector2.up, correctionDistance, controller.collisionMask);
		RaycastHit2D cornerRight = Physics2D.Raycast(transform.position + cornerR, Vector2.up, correctionDistance, controller.collisionMask);

		Debug.DrawRay(transform.position + cornerL, Vector2.up * correctionDistance);
		Debug.DrawRay(transform.position + cornerML, Vector2.up * correctionDistance);
		Debug.DrawRay(transform.position + cornerMR, Vector2.up * correctionDistance);
		Debug.DrawRay(transform.position + cornerR, Vector2.up * correctionDistance);

		if (velocity.y > 0 && !controller.collisions.below)
		{
			if (!cornerLeft && !cornerMidL && !cornerMidR && cornerRight)
			{
				Vector2 pos = transform.position;
				pos.x -= correction;
				transform.position = pos;

				Debug.Log("Toca esquina derecha");
			}

			if (cornerLeft && !cornerMidL && !cornerMidR && !cornerRight)
			{
				Vector2 pos = transform.position;
				pos.x += correction;
				transform.position = pos;

				Debug.Log("Toca esquina izquierda");
			}
		}
	}

	public void SetPowerUpWithTransformation(PowerUp pu)
	{
		SetPowerUp(pu);
		StartCoroutine(TransformPowerUp());
	}

	public void SetPowerUp(PowerUp pu)
    {
		GameManager.instance.powerUp = pu;
		anim.runtimeAnimatorController = pu.aniMario;
		
		if (pu.projectile != null)
			projectile = pu.projectile.GetComponent<Projectile>();

		if (pu.frogJump == 0)
        {
			gravity = -((2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2));
		}
		else
        {
			gravity = -((2 * pu.frogJump) / Mathf.Pow(timeToJumpApex, 2));
			maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
			minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
		}
		maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;

	}

	IEnumerator TransformPowerUp()
	{
		Time.timeScale = 0;
		sprite.enabled = false;

		bigCloud.Play();
		yield return new WaitForSecondsRealtime(0.5f);

		Time.timeScale = 1;
		sprite.enabled = true;

	}

	public void ThrowProjectile()
	{
		Vector2 pos = transform.position;
		pos.y += 0.5f;
		Projectile clone = Instantiate(projectile.gameObject, pos, Quaternion.identity, transform.parent).GetComponent<Projectile>();
		clone.Move(direction);
		AudioManager.instance.Play("Fireball");

	}

    void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.CompareTag("Block"))
		{
			Block block = collision.transform.parent.Find("Block").GetComponent<Block>();
			block.Activate();
		}

		if (collision.CompareTag("Flagpole"))
		{
			Vector3 pos = transform.position;
			pos.x = collision.transform.position.x - 0.5f;
			transform.position = pos;

			Vector2 scale = transform.localScale;
			scale.x = 1;
			transform.localScale = scale;

			isInvencible = true;
			isMoving = false;
			gravity = 0;
			velocity = new Vector2(0, -10);

			if (transform.Find("HitboxJump") != null)
				transform.Find("HitboxJump").gameObject.SetActive(false);

			anim.SetTrigger("Climb");

			AudioManager.instance.Play("Flagpole");
			AudioManager.instance.StopMusic();
			AudioManager.instance.Stop("Star");

			collision.transform.Find("Flag").GetComponent<Flag>().MoveDown();
		}
	}

    void OnTriggerStay2D(Collider2D collision)
    {
		if (collision.CompareTag("Ground"))
			if (anim.GetCurrentAnimatorStateInfo(0).IsName("Player_Climb"))
				anim.speed = 0;
	}

    public void StartHurt(float damage)
    {
		if (!isInvencible)
			StartCoroutine(Hurt(damage));
    }

	IEnumerator Hurt(float damage)
	{
		isInvencible = true;
		isMoving = false;
		GameManager.instance.Damage(damage);

		if (GameManager.instance.lives <= 0)
			StartCoroutine(GameOver());
		else
		{
			AudioManager.instance.Play("Pipe");
			if (GameManager.instance.powerUp != mushroom)
			{
				yield return StartCoroutine(TransformPowerUp());
				SetPowerUp(mushroom);
			}
			else
			{
				Time.timeScale = 0;
				yield return StartCoroutine(Blink(5));
				Time.timeScale = 1;
			}
			isMoving = true;
			yield return StartCoroutine(Blink(10));
			isInvencible = false;
		}
    }

	public void InstaKill()
	{
		isInvencible = true;
		isMoving = false;
		GameManager.instance.Damage(999);
		AudioManager.instance.Stop("Star");
		StartCoroutine(GameOver());
	}

	IEnumerator GameOver()
	{
		AudioManager.instance.StopMusic();
		AudioManager.instance.Play("MarioDie");

		Debug.Log("GameOver");

		Time.timeScale = 1;

		anim.SetBool("Die", true);

		velocity = Vector2.zero;
		gravity = 0;

		controller.ActiveCollisions(false);
		GetComponent<BoxCollider2D>().enabled = false;
		controller.enabled = false;

		if (transform.Find("HitboxJump") != null)
			transform.Find("HitboxJump").gameObject.SetActive(false);

		yield return new WaitForSecondsRealtime(1f);

		gravity = -((2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2));
		velocity = Vector2.up * 20;

		yield return new WaitForSecondsRealtime(3);

		SceneLoader.instance.LoadScene("GameOver");
	}

	IEnumerator Blink(float time)
    {
        for (int i = 0; i < time; i++)
        {
			sprite.enabled = false;
			yield return new WaitForSecondsRealtime(0.05f);
			sprite.enabled = true;
			yield return new WaitForSecondsRealtime(0.05f);
		}
	}

	public void StartStarEffect()
    {
		StartCoroutine(Star());
    }

	IEnumerator Star()
    {
		isInvencible = true;

		transform.Find("HitboxStar").gameObject.SetActive(true);
		transform.Find("LittleStars").GetComponent<ParticleSystem>().Play();

		material.EnableKeyword("GLOW_ON");
		material.SetFloat("_Glow", 30);

		AudioManager.instance.Play("Star");
		AudioManager.instance.PauseMusic();

		yield return new WaitForSeconds(10);

		isInvencible = false;

		transform.Find("HitboxStar").gameObject.SetActive(false);
		transform.Find("LittleStars").GetComponent<ParticleSystem>().Stop();

		material.DisableKeyword("GLOW_ON");
		material.SetFloat("_Glow", 0);

		AudioManager.instance.Stop("Star");
		AudioManager.instance.UnPauseMusic();
	}

	public void PlaySound(string sound)
    {
		AudioManager.instance.PlayOneShot(sound);
    }
}


/*
 
	Si mantengo el boton Correr
		maxSpeed = maxSpeedRun
		friction = frictionRun;
		acceleration = aceelerationRun;
		correr = true

	Si no
		friction = frictionWalk;
		acceleration = aceelerationWalk;
	
		Si speed <= maxSpeedWalk
			maxSpeed = maxSpeedWalk
			correr = false
		

 */

