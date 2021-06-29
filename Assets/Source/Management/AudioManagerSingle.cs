using System;
using UnitMan.Source.Management;
using UnitMan.Source.Utilities;
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

        [SerializeField] private AudioCollection _audioCollection;


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
            Retreating, IntroMusic
        }

        private void Awake() {
            if (Instance != null) Destroy(gameObject);

            Instance = this;
            SessionManagerSingle.OnReset += Reset;

        }

        private void Reset()
        {
            track0.Stop();
            track1.Stop();
            track2.Stop();
        }

        private void OnDisable()
        {
            SessionManagerSingle.OnReset -= Reset;
        }

        public void PlayClip(AudioEffectType effectType, int trackNumber, bool loop) {
            AudioSource source = trackNumber switch {
                0 => track0,
                1 => track1,
                2 => track2,
                _ => track0
            };
            source.clip = effectType switch {
                AudioEffectType.Munch => _audioCollection.munch,
                AudioEffectType.EatGhost => _audioCollection.eatGhost,
                AudioEffectType.Death => _audioCollection.death,
                AudioEffectType.Intermission => _audioCollection.intermission,
                AudioEffectType.Siren => _audioCollection.siren,
                AudioEffectType.PowerPellet => _audioCollection.powerPellet,
                AudioEffectType.Retreating => _audioCollection.retreating,
                AudioEffectType.IntroMusic => _audioCollection.introMusic,
                _ => source.clip
            };
            source.Play();
            source.loop = loop;
        }
    }
}