using UnityEngine;

namespace PvP3DAction
{
    public class PlayerController : MonoBehaviour
    {
        private Player _player;

        private void Start()
        {
            _player = transform.root.GetComponent<Player>();
        }

        private void Update()
        {
            _player.InputX = Input.GetAxis("Horizontal");
            _player.InputZ = Input.GetAxis("Vertical");
            _player.InputeMouseX = Input.GetAxis("Mouse X");
            _player.InputeMouseY = Input.GetAxis("Mouse Y");

            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}