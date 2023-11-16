using UnityEngine;

public class EnemyBehaviour : CharacterController2D
{
    [SerializeField] private bool _startMovingRight = true;

    private bool _isMovingRight;
    private float _move;

    private void Start()
    {
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
        //_move = 0;
        //this.enabled = false;

        Destroy(this.gameObject);
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
        if (collision.collider.CompareTag("Player"))
        {
            if (LevelManager.Instance != null)
                LevelManager.Instance.ReloadLevel();
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
