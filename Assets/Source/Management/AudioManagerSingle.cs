using System;
using UnitMan.Source.Management;
using UnitMan.Source.Utilities;
using UnityEngine;

namespace UnitMan
{
    public sealed class AudioManagerSingle : MonoBehaviour
    {
        public static AudioManagerSingle Instance {get; private set; }

        private AudioSource[] _tracks;

        [SerializeField] private AudioCollection _audioCollection;


        public bool IsTrackPlaying(int trackNumber) {
            return _tracks[trackNumber].isPlaying;
        }

        public enum AudioEffectType
        {
            Munch, EatGhost, Death, Intermission, Siren,
            Fleeing,
            Retreating, IntroMusic
        }

        private void Awake() {
            if (Instance != null) Destroy(gameObject);

            Instance = this;
            SessionManagerSingle.OnReset += Reset;

            _tracks = GetComponentsInChildren<AudioSource>();

        }

        private void Reset()
        {
            foreach (AudioSource track in _tracks)
            {
                track.Stop();
            }
        }

        private void OnDisable()
        {
            SessionManagerSingle.OnReset -= Reset;
        }

        public void PlayClip(AudioEffectType effectType, int trackNumber, bool loop)
        {
            AudioSource source = _tracks[trackNumber];
        
            source.clip = effectType switch {
                AudioEffectType.Munch => _audioCollection.munch,
                AudioEffectType.EatGhost => _audioCollection.eatGhost,
                AudioEffectType.Death => _audioCollection.death,
                AudioEffectType.Intermission => _audioCollection.intermission,
                AudioEffectType.Siren => _audioCollection.siren,
                AudioEffectType.Fleeing => _audioCollection.powerPellet,
                AudioEffectType.Retreating => _audioCollection.retreating,
                AudioEffectType.IntroMusic => _audioCollection.introMusic,
                _ => source.clip
            };
            source.Play();
            source.loop = loop;
        }
    }
}