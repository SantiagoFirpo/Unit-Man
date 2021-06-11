using UnityEngine;
using UnityEngine.Serialization;

namespace UnitMan
{
    class AudioManager : MonoSingleton
    {
        public static AudioManager Instance {get; private set; }
        
        [FormerlySerializedAs("_track1")] [SerializeField]
        private AudioSource _track0;

        [SerializeField] private AudioClip _munch;
        [SerializeField] private AudioClip _eatGhost;
        [SerializeField] private AudioClip _death;
        [SerializeField] private AudioClip _intermission;


        public enum AudioEffectType
        {
            Munch, EatGhost, Death, Intermission
        }

        protected virtual void Awake() {
            if (Instance != null) {
                Destroy(gameObject);
            }
            else {
                Instance = this;
            }
            
        }

        public void PlayClip(AudioEffectType effectType, int trackNumber) {
            _track0.clip = effectType switch {
                AudioEffectType.Munch => _munch,
                AudioEffectType.EatGhost => _eatGhost,
                AudioEffectType.Death => _death,
                AudioEffectType.Intermission => _intermission,
                _ => _track0.clip
            };
            _track0.Play();
        }
    }
}