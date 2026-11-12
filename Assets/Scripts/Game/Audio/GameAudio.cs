using System.Collections.Generic;
using Scrapwalker.Core.Combat;
using Scrapwalker.Core.Combat.Waves;
using Scrapwalker.Core.Root;
using Scrapwalker.Core.Sessions;
using Scrapwalker.Core.Travel;
using Scrapwalker.Game.App;
using Scrapwalker.Game.Audio.Players;
using Scrapwalker.Game.Definitions;
using Scrapwalker.Game.Audio.Sounds;
using Scrapwalker.Game.Look;
using Scrapwalker.Game.Wording;
using UnityEngine;
using UnityEngine.UIElements;

namespace Scrapwalker.Game.Audio
{
    public sealed class GameAudio
    {
        private const string TabClass = "tab";

        private readonly SoundTable _table;
        private readonly GameData _data;
        private readonly Dictionary<SEChannelKey, SEOutput> _outputs = new Dictionary<SEChannelKey, SEOutput>();
        private readonly SoundPlayer _se;
        private readonly SoundPlayer _screen;
        private readonly MusicPlayer _music;
        private readonly Dictionary<WorkSound, RepeatingPlayer> _work = new Dictionary<WorkSound, RepeatingPlayer>();
        private readonly FiringPlayer _firing;
        private readonly UISoundPicker _ui;
        private readonly TalkVoice _voice;
        private readonly BoolEdge _reaching = new BoolEdge();
        private readonly BoolEdge _carrying = new BoolEdge();
        private readonly BoolEdge _touchedDown = new BoolEdge();
        private readonly BoolEdge _launchFired = new BoolEdge();
        private readonly BoolEdge _waking = new BoolEdge();
        private AudioClip _sceneMusic;

        public GameAudio(Transform owner, SoundTable table, GameData data)
        {
            _table = table;
            _data = data;
            Transform parent = new GameObject("Audio").transform;
            parent.SetParent(owner, false);
            foreach (KeyValuePair<SEChannelKey, SEChannel> channel in table.Channels)
            {
                _outputs.Add(channel.Key, new SEOutput(table.SEGroup, channel.Value.ShiftedGroup, channel.Value.Pitch, channel.Value.Rate));
            }

            SEOutput normal = _outputs[SEChannelKey.Normal];
            _se = new SoundPlayer(parent, normal);
            _screen = new SoundPlayer(parent, normal);
            _music = new MusicPlayer(table.CrossfadeSeconds, table.DuckSeconds, parent, table.BGMGroup);
            foreach (KeyValuePair<WorkSound, WorkLoop> work in table.Work)
            {
                _work.Add(work.Key, new RepeatingPlayer(work.Value.Sound, parent, _outputs[work.Value.Channel], _se));
            }

            _firing = new FiringPlayer(parent, normal, _se);
            _ui = new UISoundPicker(table.UI, _screen);
            _voice = new TalkVoice(table.Scene[SceneSound.AIVoice], table.VoiceMinSeconds, _screen);
            Volumes = new VolumeSettings(table);
        }

        public VolumeSettings Volumes { get; }

        public void Request(UISound sound)
        {
            _ui.Request(sound);
        }

        public void OnWorldReplaced(World world)
        {
            _se.StopAll();
            foreach (RepeatingPlayer work in _work.Values)
            {
                work.Stop();
            }

            _firing.Stop();
            _sceneMusic = SceneMusicOf(world);
        }

        public void Listen(GameEventListeners listeners)
        {
            listeners.On<Landed>(_ => _sceneMusic = null);
            listeners.On<WaveStarted>(started => _sceneMusic = _data.Asset(started.Ruin).Music);
            listeners.On<WaveCleared>(cleared => _music.PlayJingle(JingleOf(cleared)));
            listeners.On<Departed>(_ =>
            {
                _sceneMusic = _table.Music[MusicTrack.Travel];
                _se.Play(_table.Scene[SceneSound.Departure]);
            });
            listeners.On<Defeated>(_ => _music.PlayJingle(_table.Music[MusicTrack.GameOver]));
            listeners.On<ShotFired>(shot => OnShot(_data.Asset(shot.Weapon).Look.Weapon.Sounds));
            listeners.On<ShotLanded>(landed => _se.Play(_data.Asset(landed.Weapon).Look.Weapon.Sounds.Land));
            listeners.On<AttackBegun>(begun => _se.PlayVoice(begun.EnemyID, _data.Asset(begun.Style).Sounds.Begin));
            listeners.On<CellAttacked>(attacked => OnAttacked(attacked, _data.Asset(attacked.Style).Sounds.Hit));
            listeners.On<EnemyKilled>(killed =>
            {
                _se.StopVoice(killed.EnemyID);
                _se.Play(_data.Asset(killed.Enemy.Type).Killed);
            });
        }

        public void Tick(in AudioFrame frame)
        {
            float deltaTime = frame.UIDelta;
            World world = frame.World;
            bool isTitle = frame.IsTitle;
            _music.Play(isTitle ? _table.Music[MusicTrack.Title] : _sceneMusic);
            _music.Tick(deltaTime);
            if (isTitle)
            {
                SetTimeScale(1f);
                TickWork(false, frame, deltaTime);
                _ui.Flush();
                return;
            }

            float timeScale = deltaTime > 0f ? frame.GameDelta / deltaTime : 0f;
            SetTimeScale(timeScale);
            if (timeScale > 0f || world.IsDefeated)
            {
                TickWork(!world.IsDefeated, frame, deltaTime);
            }

            TickMoments(frame.Scene, frame.WakeProgress);
            TickVoice(frame.Speaker, deltaTime);
            _se.StopVoicesOf(id => world.Battle.FindEnemy(id) == null);
            _ui.Flush();
        }

        private void OnShot(WeaponSounds sounds)
        {
            if (sounds.FiresAsLoop)
            {
                _firing.Fired(sounds);
                return;
            }

            _se.Play(sounds.Fire);
        }

        private void OnAttacked(CellAttacked attacked, SoundCue hit)
        {
            if (!hit.IsEmpty)
            {
                _se.StopVoice(attacked.EnemyID);
                _se.Play(hit);
            }

            if (attacked.BarrierTaken > 0)
            {
                _se.Play(_table.Scene[SceneSound.BarrierHit]);
            }

            if (attacked.Reached > 0)
            {
                _se.Play(_table.Scene[SceneSound.HullHit]);
            }
        }

        private AudioClip JingleOf(WaveCleared cleared)
        {
            if (cleared.LeavesRuin)
            {
                return _table.Music[MusicTrack.Completed];
            }

            return cleared.IsTakeoffWave ? null : _table.Music[MusicTrack.WaveClear];
        }

        private AudioClip SceneMusicOf(World world)
        {
            if (!world.Journey.IsAtRuin)
            {
                return _table.Music[MusicTrack.Travel];
            }

            return world.Wave.Number > 0 ? _data.Asset(world.Journey.CurrentRuin).Music : null;
        }

        private void SetTimeScale(float scale)
        {
            foreach (SEOutput output in _outputs.Values)
            {
                output.Follow(scale);
            }

            _se.SetTimeScale(scale);
            foreach (RepeatingPlayer work in _work.Values)
            {
                work.SetTimeScale(scale);
            }

            _firing.SetTimeScale(scale);
        }

        private void TickWork(bool isRunning, in AudioFrame frame, float deltaTime)
        {
            foreach (KeyValuePair<WorkSound, RepeatingPlayer> work in _work)
            {
                work.Value.Tick(isRunning && frame.Scene.IsRunning(work.Key), deltaTime);
            }

            _firing.Tick(isRunning ? frame.GameDelta : 0f, deltaTime);
        }

        private void TickMoments(in SceneCues scene, float wake)
        {
            KeyedList<SceneSound, SoundCue> sounds = _table.Scene;
            if (_reaching.Rose(scene.IsArmReaching))
            {
                _se.Play(sounds[SceneSound.ArmMove]);
            }

            if (_carrying.Rose(scene.IsArmCarrying))
            {
                _se.Play(sounds[SceneSound.ArmGrab]);
                _se.Play(sounds[SceneSound.ArmMove]);
            }

            if (_touchedDown.Rose(scene.HasTouchedDown))
            {
                _se.Play(sounds[SceneSound.TouchDown]);
            }

            if (_launchFired.Rose(scene.IsLaunchFired))
            {
                _se.Play(sounds[SceneSound.Takeoff]);
            }

            if (_waking.Rose(wake > 0f && wake < 1f))
            {
                _screen.Play(sounds[SceneSound.AIBoot]);
            }
        }

        private void TickVoice(ITalkSpeaker speaker, float deltaTime)
        {
            if (speaker.ShowsTalk)
            {
                _voice.Tick(speaker.Line.Wording(_data), speaker.ShownChars, deltaTime);
            }
            else
            {
                _voice.Tick(null, 0, deltaTime);
            }
        }

        public void OnButtonClicked(Button button)
        {
            _ui.Request(button.ClassListContains(TabClass) ? UISound.Tab : UISound.Click);
        }
    }
}
