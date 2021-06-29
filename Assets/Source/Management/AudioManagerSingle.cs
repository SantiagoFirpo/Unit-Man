using UnityEngine;

namespace UnitMan
{
    public sealed class AudioManagerSingle : MonoBehaviour
    {
        public static AudioManagerSingle Instance {get; private set; }
        
        [SerializeField]
        private AudioSource track0;
        [SerializeField]
        private AudioSource track1;
        [SerializeField]
        private AudioSource track2;

        [SerializeField] private AudioClip munch;
        [SerializeField] private AudioClip eatGhost;
        [SerializeField] private AudioClip death;
        [SerializeField] private AudioClip intermission;
        [SerializeField] private AudioClip siren;
        [SerializeField] private AudioClip powerPellet;
        [SerializeField] private AudioClip retreating;


        public bool IsTrackPlaying(int trackNumber) {
            AudioSource source = trackNumber switch {
                0 => track0,
                1 => track1,
                2 => track2,
                _ => track0
            };
            return source.isPlaying;
        }

        public enum AudioEffectType
        {
            Munch, EatGhost, Death, Intermission, Siren,
            PowerPellet,
            Retreating
        }

        private void Awake() {
            if (Instance != null) Destroy(gameObject);

            Instance = this;

        }

        public void PlayClip(AudioEffectType effectType, int trackNumber, bool loop) {
            AudioSource source = trackNumber switch {
                0 => track0,
                1 => track1,
                2 => track2,
                _ => track0
            };
            source.clip = effectType switch {
                AudioEffectType.Munch => munch,
                AudioEffectType.EatGhost => eatGhost,
                AudioEffectType.Death => death,
                AudioEffectType.Intermission => intermission,
                AudioEffectType.Siren => siren,
                AudioEffectType.PowerPellet => powerPellet,
                AudioEffectType.Retreating => retreating,
                _ => source.clip
            };
            source.Play();
            source.loop = loop;
        }
    }
}