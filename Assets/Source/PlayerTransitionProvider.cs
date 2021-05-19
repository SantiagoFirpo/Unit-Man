using UnityEngine;
using UnitMan.Source;

namespace UnitMan.Source
{ 
    public class PlayerTransitionProvider : ITransitionProvider {
        private readonly Player _player;
        public PlayerTransitionProvider(Player player) {
            this._player = player;
        }

        public int GetTransition(int currentState) {
            const int nullTransition = -1;
            switch (currentState) {
                case (int) Player.PlayerState.Idle: {
                    if (_player.motion != Vector2.zero) {
                        return (int) Player.PlayerState.Move;
                    }
                    
                    return nullTransition;

                }
                case (int) Player.PlayerState.Move: {
                    if (_player.motion == Vector2.zero) {
                        return (int) Player.PlayerState.Idle;
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