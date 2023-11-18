using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EnemyBehaviour : CharacterController2D
{
    [SerializeField] private bool _startMovingRight = true;
    [SerializeField] private float _reloadLevelAfterKillingPlayer = 1f;

    private bool _isMovingRight;
    private float _move;
    private bool _dead;

    private Collider2D _collider;

    private void Start()
    {
        _collider = GetComponent<Collider2D>();

        if (_startMovingRight)
            _isMovingRight = true;
        else
            _isMovingRight = false;

        UpdateMoveDirection();
    }

    public override void Update()
    {

    }

    public override void FixedUpdate()
    {
        CheckGrounded();

        Move(_move, false, false);
    }

    public void Die()
    {
        Animator.SetBool("Death", true);
        Rigidbody2D.isKinematic = true;
        _collider.enabled = false;

        _move = 0;
        _dead = true;
        PlayDeadEnemyAudioClip();
    }

    private void UpdateMoveDirection()
    { 
        if (_isMovingRight)
            _move = 1;
        else
            _move = -1;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_dead)
            return;

        if (collision.collider.CompareTag("Player"))
        {
            if (LevelManager.Instance != null)
            {
                if (collision.collider.TryGetComponent(out CharacterController2D controller))
                    controller.CanMove = false;

                PlayDeadAudioClip();
                LevelManager.Instance.ReloadLevel(_reloadLevelAfterKillingPlayer);
            }
            else
                Debug.LogError("There was no levelmanager in the scene! Add one");
        }
        else if (collision.collider.CompareTag("Obstacle"))
        {
            _isMovingRight = !_isMovingRight;
            UpdateMoveDirection();
        }
    }
}
