using System;
using UnityEngine;

public class SpearBehaviour : MonoBehaviour
{
    [SerializeField] private float _movementSpeed = 5f;
    [SerializeField] private Collider2D _collider;
    [SerializeField] private bool _hitOnlyWhenLaunched = true;

    private bool _isLaunched = false;
    private Vector3 _move = Vector3.zero;

    private void Update()
    {
        if (_isLaunched)
        {
            transform.position += _move * _movementSpeed * Time.deltaTime;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_hitOnlyWhenLaunched && _isLaunched == false)
            return;

        if (collision.CompareTag("Enemy"))
        {
            if (collision.TryGetComponent(out EnemyBehaviour behaviour))
                behaviour.Die();
            else
                Debug.LogError("Enemy has enemy tag but no enemy behaviour script, cannot kill!");
        }
        else if (collision.CompareTag("Obstacle"))
        {
            _isLaunched = false;
            _collider.isTrigger = false;

            this.enabled = false;
        }
    }

    public void LaunchSpear(bool facingRight)
    {
        this.transform.SetParent(null);

        if (facingRight)
            _move = Vector3.right;
        else
            _move = Vector3.left;

        _isLaunched = true;
    }
}
