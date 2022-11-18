using Mirror;
using UnityEngine;

namespace PvP3DAction
{
    public class Bot : NetworkBehaviour
    {
        [SerializeField] private float _moveSpeed;
        [SerializeField] private float _maxMoveSpeed;
        [SerializeField] private float _dashForce;
        [SerializeField] private float _turnSpeed;
        [SerializeField] private float _dashTime;
        [SerializeField] private float _hitTime;

        private Rigidbody _rigidbody;

        private float InputeMouseX;
        private float InputX;
        private float InputZ;

        private bool _isDashing;
        private float _dashTimer;

        private bool _isHit;
        public bool IsHit => _isHit;
        private float _hitTimer;

        private void Start()
        {
            _rigidbody = transform.GetComponent<Rigidbody>();
        }
        public override void OnStartLocalPlayer()
        {
            Camera.main.transform.SetParent(transform);
            Camera.main.transform.localPosition = new Vector3(0, 2, -4);
        }

        void Update()
        {
            //if (!isLocalPlayer) return;

            //Turn();
            //UpdateRigidbody();
            //Dash();
            Hit();
        }

        private void UpdateRigidbody()
        {
            _rigidbody.AddForce(_moveSpeed * InputX * Time.fixedDeltaTime * transform.right, ForceMode.Force);
            _rigidbody.AddForce(_moveSpeed * InputZ * Time.fixedDeltaTime * transform.forward, ForceMode.Force);
            _rigidbody.AddForce((_moveSpeed - _maxMoveSpeed) * Time.fixedDeltaTime * -_rigidbody.velocity, ForceMode.Force);
        }


        private void Turn()
        {
            transform.rotation *= Quaternion.AngleAxis(InputeMouseX * _turnSpeed * Time.fixedDeltaTime, Vector3.up);
        }

        private void Dash()
        {
            if (_dashTimer > 0)
            {
                _dashTimer -= Time.deltaTime;
            }
            else
            {
                _isDashing = false;
            }

            if (Input.GetKeyDown(KeyCode.Mouse0) && _isDashing == false)
            {
                _dashTimer = _dashTime;

                _rigidbody.AddForce(transform.forward * _dashForce, ForceMode.Impulse);

                _isDashing = true;
            }
        }

        private void Hit()
        {
            if (_hitTimer > 0)
            {
                _hitTimer -= Time.deltaTime;
            }
            else
            {
                _isHit = false;

                MeshRenderer mesh = GetComponent<MeshRenderer>();

                mesh.material.color = Color.green;
            }
        }

        public void OnHit()
        {
            _isHit = true;

            _hitTimer = _hitTime;
        }
    }
}