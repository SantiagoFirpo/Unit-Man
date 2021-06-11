using UnityEngine;
using UnityEngine.Serialization;

namespace UnitMan
{
    class AudioManager : MonoSingleton
    {
        public static AudioManager Instance {get; private set; }
        
        [SerializeField]
        private AudioSource _track0;
        [SerializeField]
        private AudioSource _track1;
        [SerializeField]
        private AudioSource _track2;

        [SerializeField] private AudioClip _munch;
        [SerializeField] private AudioClip eatGhost;
        [SerializeField] private AudioClip death;
        [SerializeField] private AudioClip intermission;
        [SerializeField] private AudioClip siren;
        [SerializeField] private AudioClip powerPellet;


        public bool IsTrackPlaying(int trackNumber) {
            AudioSource source = trackNumber switch {
                0 => _track0,
                1 => _track1,
                2 => _track2,
                _ => _track0
            };
            return source.isPlaying;
        }

        public enum AudioEffectType
        {
            Munch, EatGhost, Death, Intermission, Siren,
            PowerPellet
        }

        protected virtual void Awake() {
            if (Instance != null) {
                Destroy(gameObject);
            }
            else {
                Instance = this;
            }
            
        }

        public void PlayClip(AudioEffectType effectType, int trackNumber, bool loop) {
            AudioSource source = trackNumber switch {
                0 => _track0,
                1 => _track1,
                2 => _track2,
                _ => _track0
            };
            source.clip = effectType switch {
                AudioEffectType.Munch => _munch,
                AudioEffectType.EatGhost => eatGhost,
                AudioEffectType.Death => death,
                AudioEffectType.Intermission => intermission,
                AudioEffectType.Siren => siren,
                AudioEffectType.PowerPellet => powerPellet,
                _ => source.clip
            };
            source.Play();
            source.loop = loop;
        }
    }
}