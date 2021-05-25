using UnityEngine;
using UnitMan.Source;

namespace UnitMan.Source
{ 
    public class PlayerTransitionProvider : ITransitionProvider {
        private readonly PlayerController _playerController;
        public PlayerTransitionProvider(PlayerController playerController) {
            this._playerController = playerController;
        }

        public int GetTransition(int currentState) {
            const int nullTransition = -1;
            switch (currentState) {
                case (int) PlayerController.PlayerState.Idle: {
                    if (_playerController.motion != Vector2.zero) {
                        return (int) PlayerController.PlayerState.Move;
                    }
                    
                    return nullTransition;

                }
                case (int) PlayerController.PlayerState.Move: {
                    if (_playerController.motion == Vector2.zero) {
                        return (int) PlayerController.PlayerState.Idle;
                    }

                    return nullTransition;
                }
                default: {
                    return nullTransition;
                }
            };
        }
    }
}