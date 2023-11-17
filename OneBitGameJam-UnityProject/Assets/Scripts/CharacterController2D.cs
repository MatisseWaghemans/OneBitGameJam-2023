using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))]
public class CharacterController2D : MonoBehaviour
{
	[Header("General")]
	[SerializeField] private float _movementspeed = 100f;
	[SerializeField] private SpearBehaviour _spearBehaviour = null;
	[SerializeField] private bool _canCrouch = true;
	[SerializeField] private bool _canJump = true;
	[SerializeField] private float m_JumpForce = 400f;                          // Amount of force added when the player jumps.
	[Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;          // Amount of maxSpeed applied to crouching movement. 1 = 100%
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;  // How much to smooth out the movement
	[SerializeField] private bool m_AirControl = false;                         // Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
	[SerializeField] private Transform m_GroundCheck;                           // A position marking where to check if the player is grounded.
	[SerializeField] private Transform m_CeilingCheck;                          // A position marking where to check for ceilings
	[SerializeField] private Collider2D m_CrouchDisableCollider;                // A collider that will be disabled when crouching

	[Header("Audio")]
	[SerializeField] private AudioClip[] _gruntAudioClips = null;
	[SerializeField] private AudioClip _dieAudioClip = null;

	const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
	private bool m_Grounded;            // Whether or not the player is grounded.
	const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
	private Rigidbody2D m_Rigidbody2D;
	private bool m_FacingRight = true;  // For determining which way the player is currently facing.
	private Vector3 m_Velocity = Vector3.zero;
	private AudioSource _audioSource;

    public AudioSource AudioSource { get => _audioSource; }

    [Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	public BoolEvent OnCrouchEvent;
	private bool m_wasCrouching = false;

	private float _horizontalInput;
	private bool _crouch;
	private bool _jump;
	private bool _canMove;

    public bool CanMove { set => _canMove = value; }

    public virtual void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();
		_audioSource = GetComponent<AudioSource>();

		_canMove = true;

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

		if (OnCrouchEvent == null)
			OnCrouchEvent = new BoolEvent();

		_jump = false;
		_crouch = false;
	}

	public virtual void Update()
    {
		if (_canMove == false)
		{
			_horizontalInput = 0;
			return;
		}

		_horizontalInput = Input.GetAxisRaw("Horizontal");

        if (_canCrouch)
        {
			if (Input.GetKeyDown(KeyCode.C))
			{
				_crouch = true;
			}
			else if (Input.GetKeyUp(KeyCode.C))
			{
				_crouch = false;
			}
		}
        
		if (_canJump && Input.GetKeyDown(KeyCode.Space))
        {
			_jump = true;
        }

		if (_spearBehaviour != null && Input.GetKeyDown(KeyCode.E))
		{
			_spearBehaviour.LaunchSpear(m_FacingRight);
			_spearBehaviour = null;

			_audioSource.PlayOneShot(_gruntAudioClips[Random.Range(0, _gruntAudioClips.Length)]);
		}
	}

	public virtual void FixedUpdate()
	{
		CheckGrounded();

		Move(_horizontalInput, _crouch, _jump);
	}

	public void CheckGrounded()
	{
		bool wasGrounded = m_Grounded;
		m_Grounded = false;

		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				m_Grounded = true;
				if (!wasGrounded)
					OnLandEvent.Invoke();
			}
		}
	}

	public void PlayDeadAudioClip()
    {
		_audioSource.PlayOneShot(_dieAudioClip);
    }

	public void Move(float move, bool crouch, bool jump)
	{
		// If crouching, check to see if the character can stand up
		if (!crouch && m_wasCrouching)
		{
			// If the character has a ceiling preventing them from standing up, keep them crouching
			if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
			{
				Debug.Log("ceiling true");
				crouch = true;
			}
		}

		//only control the player if grounded or airControl is turned on
		if (m_Grounded || m_AirControl)
		{

			// If crouching
			if (crouch)
			{
				if (!m_wasCrouching)
				{
					m_wasCrouching = true;
					OnCrouchEvent.Invoke(true);
				}

				// Reduce the speed by the crouchSpeed multiplier
				move *= m_CrouchSpeed;

				// Disable one of the colliders when crouching
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = false;
			}
			else
			{
				// Enable the collider when not crouching
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = true;

				if (m_wasCrouching)
				{
					m_wasCrouching = false;
					OnCrouchEvent.Invoke(false);
				}
			}

			// Move the character by finding the target velocity
			Vector3 targetVelocity = new Vector2(move * _movementspeed * Time.fixedDeltaTime, m_Rigidbody2D.velocity.y);
			// And then smoothing it out and applying it to the character
			m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

			// If the input is moving the player right and the player is facing left...
			if (move > 0 && !m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (move < 0 && m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
		}
		// If the player should jump...
		if (m_Grounded && jump)
		{
			_audioSource.PlayOneShot(_gruntAudioClips[Random.Range(0, _gruntAudioClips.Length)]);
			// Add a vertical force to the player.
			m_Grounded = false;
			m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
			_jump = false;
		}
	}


	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}
