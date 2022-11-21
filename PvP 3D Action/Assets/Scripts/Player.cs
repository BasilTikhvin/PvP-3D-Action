using Mirror;
using TMPro;
using UnityEngine;

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

        private LevelManager _levelManager;
        public Vector3 StartPosition { get; set; }

        private Rigidbody _rigidbody;

        private Player _enemy;

        public float InputeMouseX { get; set; }
        public float InputX { get; set; }
        public float InputZ { get; set; }

        private bool _isDashing;
        private float _dashTimer;

        private bool _isHit;
        public bool IsHit => _isHit;
        private float _hitTimer;

        public int DashHitsAmount { get; set; }

        private string _playerNameText;
        public string PlayerNameText => _playerNameText;

        private void Start()
        {
            _rigidbody = transform.GetComponent<Rigidbody>();
            StartPosition = transform.position;

            _levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
            _levelManager.AddPlayer(this);
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

            Move();

            Turn();

            Dash();

            OnHit();
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

        private void OnHit()
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

        public void OnHitted()
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

                    DashHitsAmount++;
                }
            }
        }
    } 
}