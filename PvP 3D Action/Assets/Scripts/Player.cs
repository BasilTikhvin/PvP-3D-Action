using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PvP3DAction
{
    public class Player : NetworkBehaviour
    {
        [SerializeField] private GameObject _playerName;

        [SerializeField] private float _moveSpeed;
        [SerializeField] private float _maxMoveSpeed;
        [SerializeField] private float _turnSpeed;
        [SerializeField] private float _dashForce;
        [SerializeField] private float _dashTime;
        [SerializeField] private float _hitTime;

        private Rigidbody _rigidbody;
        private Bot _enemy;

        private float InputeMouseX;
        private float InputX;
        private float InputZ;

        private bool _isDashing;
        private float _dashTimer;

        private bool _isHit;
        public bool IsHit => _isHit;

        private float _hitTimer;

        private int _dashHitsAmount;
        public int DashHitsAmount => _dashHitsAmount;

        private string _playerNameText;

        private void Start()
        {
            _rigidbody = transform.GetComponent<Rigidbody>();
        }

        public override void OnStartLocalPlayer()
        {
            Camera.main.transform.SetParent(transform);
            Camera.main.transform.localPosition = new Vector3(0, 2, -4);

            _playerNameText = $"Player{Random.Range(100, 999)}";
            transform.GetComponentInChildren<TextMeshPro>().text = _playerNameText;
        }

        void Update()
        {
            if (!isLocalPlayer)
            {
                _playerName.transform.LookAt(Camera.main.transform);

                return;
            }

            InputX = Input.GetAxis("Horizontal");
            InputZ = Input.GetAxis("Vertical");
            InputeMouseX = Input.GetAxis("Mouse X");

            UpdateRigidbody();

            OnScore();
        }

        private void UpdateRigidbody()
        {
            Turn();
            Move();
            Dash();
            Hit();
        }

        private void Move()
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

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.transform.TryGetComponent(out _enemy) && _isDashing)
            {
                if (_enemy.IsHit == false)
                {
                    _enemy.OnHit();

                    MeshRenderer mesh = _enemy.GetComponent<MeshRenderer>();

                    mesh.material.color = Color.red;

                    _dashHitsAmount++;
                }
            }
        }

        private void OnScore()
        {
            if (_dashHitsAmount == 3)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    } 
}