using System.Collections.Generic;
using Scrapwalker.Game.Audio.Sounds;
using UnityEngine;

namespace Scrapwalker.Game.Audio.Players
{
    public sealed class FiringPlayer
    {
        private readonly Transform _parent;
        private readonly SEOutput _output;
        private readonly SoundPlayer _accents;
        private readonly Dictionary<WeaponSounds, Loop> _loops = new Dictionary<WeaponSounds, Loop>();

        public FiringPlayer(Transform parent, SEOutput output, SoundPlayer accents)
        {
            _parent = parent;
            _output = output;
            _accents = accents;
        }

        public void Fired(WeaponSounds sounds)
        {
            if (!_loops.TryGetValue(sounds, out Loop loop))
            {
                loop = new Loop(new RepeatingPlayer(sounds.Firing, _parent, _output, _accents));
                _loops.Add(sounds, loop);
            }

            loop.SecondsLeft = sounds.FiringHoldSeconds;
        }

        public void Tick(float gameDeltaTime, float deltaTime)
        {
            foreach (Loop loop in _loops.Values)
            {
                loop.SecondsLeft -= gameDeltaTime;
                loop.Player.Tick(gameDeltaTime > 0f && loop.SecondsLeft > 0f, deltaTime);
            }
        }

        public void SetTimeScale(float scale)
        {
            foreach (Loop loop in _loops.Values)
            {
                loop.Player.SetTimeScale(scale);
            }
        }

        public void Stop()
        {
            foreach (Loop loop in _loops.Values)
            {
                loop.SecondsLeft = 0f;
                loop.Player.Stop();
            }
        }

        private sealed class Loop
        {
            public Loop(RepeatingPlayer player)
            {
                Player = player;
            }

            public RepeatingPlayer Player { get; }
            public float SecondsLeft { get; set; }
        }
    }
}
