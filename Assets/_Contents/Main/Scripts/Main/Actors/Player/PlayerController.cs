using Unity.Mathematics;
using UnityEngine;
using Main.Interfaces;

namespace Main.Actor
{
    sealed class PlayerController
    {
        const string Horizontal = "Horizontal";
        const string Vertical = "Vertical";
        const string Drumming = "Drumming";

        IPlayerActor _playerActor = default;


        public PlayerController(IPlayerActor actor) => _playerActor = actor;

        public void CallUpdate()
        {
            // Player Controller
            var horizontalAxis = Input.GetAxis(Horizontal);
            var verticalAxis = Input.GetAxis(Vertical);

            if (Input.GetButtonDown(Drumming))
            {
                _playerActor.StartDrumming();
            }
            else if (Input.GetButtonUp(Drumming))
            {
                _playerActor.StopDrumming();
            }

            if (math.abs(horizontalAxis) > 0f || math.abs(verticalAxis) > 0f)
            {
                _playerActor.Move(horizontalAxis, verticalAxis);
            }
            else
            {
                _playerActor.Idle();
            }
        }
    }
}
