using Mirror;
using TMPro;
using UnityEngine;

namespace PvP3DAction
{
    public class Player : NetworkBehaviour
    {
        [SerializeField] private GameObject _playerName;
        [SerializeField] private Animator _playerAnimator;

        [SerializeField] private float _moveSpeed;
        [SerializeField] private float _maxMoveSpeed;
        [SerializeField] private float _turnSpeed;
        [SerializeField] private float _dashForce;
        [SerializeField] private float _dashTime;
        [SerializeField] private float _hitTime;

        [SyncVar]
        private int _dashHitsAmount;
        public int DashHitsAmount => _dashHitsAmount;

        [SyncVar]
        private string _playerNameText;
        public string PlayerNameText => _playerNameText;

        [SyncVar]
        private bool _isDashing;
        private float _dashTimer;

        [SyncVar]
        private bool _isHit;
        private float _hitTimer;

        public Vector3 StartPosition { get; set; }
        public float InputeMouseX { get; set; }
        public float InputeMouseY { get; set; }
        public float InputX { get; set; }
        public float InputZ { get; set; }

        private LevelManager _levelManager;
        private Rigidbody _rigidbody;
        private Player _enemy;
        private float _cameraRotationAngleY;
        private Quaternion _cameraStartRotation;

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

            _cameraStartRotation = Camera.main.transform.rotation;

            _playerNameText = $"Player {Random.Range(100, 999)}";
            _playerName.GetComponentInChildren<TextMeshPro>().text = _playerNameText;

            CmdPlayerName(_playerNameText);
        }

        void Update()
        {
            if (isLocalPlayer == false)
            {
                _playerName.transform.LookAt(Camera.main.transform);

                return;
            }

            _playerName.transform.LookAt(Camera.main.transform);

            PlayerCameraRotation();

            Move();

            Dash();

            AfterHit();
        }

        private void Move()
        {
            _rigidbody.AddForce(_moveSpeed * InputX * Time.fixedDeltaTime * transform.right, ForceMode.Force);
            _rigidbody.AddForce(_moveSpeed * InputZ * Time.fixedDeltaTime * transform.forward, ForceMode.Force);
            _rigidbody.AddForce((_moveSpeed - _maxMoveSpeed) * Time.fixedDeltaTime * -_rigidbody.velocity, ForceMode.Force);

            transform.rotation *= Quaternion.AngleAxis(InputeMouseX * _turnSpeed * Time.fixedDeltaTime, Vector3.up);
        }


        private void PlayerCameraRotation()
        {
            _cameraRotationAngleY += InputeMouseY;

            Quaternion rotationY = Quaternion.AngleAxis(Mathf.Clamp(-_cameraRotationAngleY, -25, 25), Vector3.right);

            Camera.main.transform.localRotation = _cameraStartRotation * rotationY;
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

                CmdIsDashingSync(_isDashing);
            }

            if (Input.GetKeyDown(KeyCode.Mouse0) && _isDashing == false)
            {
                _dashTimer = _dashTime;

                _rigidbody.AddForce(transform.forward * _dashForce, ForceMode.Impulse);

                _isDashing = true;

                CmdIsDashingSync(_isDashing);
            }
        }

        public void DashHitsReset()
        {
            _dashHitsAmount = 0;
        }

        private void AfterHit()
        {
            if (_hitTimer > 0)
            {
                _hitTimer -= Time.deltaTime;
            }
            else
            {
                _isHit = false;

                CmdIsHitSync(_isHit);
            }
        }

        public void OnHit()
        {
            _isHit = true;

            _hitTimer = _hitTime;

            CmdIsHitSync(_isHit);

            _playerAnimator.SetTrigger("hit");
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.transform.TryGetComponent(out _enemy) && _isDashing)
            {
                if (_enemy._isHit == false)
                {
                    _enemy.OnHit();

                    CmdAddDashHit();
                }
            }
        }

        [Command]
        public void CmdAddDashHit()
        {
            _dashHitsAmount++;
        }

        [Command]
        public void CmdPlayerName(string name)
        {
            _playerNameText = name;
        }

        [Command]
        public void CmdIsHitSync(bool isHit)
        {
            _isHit = isHit;
        }

        [Command]
        public void CmdIsDashingSync(bool isDashing)
        {
            _isDashing = isDashing;
        }
    } 
}