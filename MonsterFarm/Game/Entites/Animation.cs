﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonsterFarm.Game.Entites
{
    public class AnimationFrame
    {
        public Rectangle SourceRectangle { get; set; }
        public TimeSpan Duration { get; set; }
        public bool Reverse { get; set; }
    }

    public class Animation
    {
        Dictionary<string, List<AnimationFrame>> _sequences;
        Texture2D _sheet;
        TimeSpan _timeIntoAnimation;
        AnimationFrame _currentFrame;
        bool _running;
        string _sequence;

        public Animation(Texture2D sheet)
        {
            _sheet = sheet;
            _sequences = new Dictionary<string, List<AnimationFrame>>();
            _running = false;
        }

        public bool Running { get { return _running; } }
        public void Start() {
            if (!_running) {
                _running = true;
            }
        }
        public void Stop() {
            if (_running) {
                _running = false;
            }
        }

        public void Reset() {
            _currentFrame = _sequences[Sequence][0];
        }

        public int Width { get { return _currentFrame.SourceRectangle.Width; } }
        public int Height { get { return _currentFrame.SourceRectangle.Height; } }

        public string Sequence { get { return _sequence; } set
            {
                if (_sequences.ContainsKey(value) && _sequences[value].Count > 0)
                {
                    _sequence = value;
                    Reset();
                }
            }
        }

        public TimeSpan Duration(string sequence)
        {
            double totalSeconds = 0;
            foreach (AnimationFrame frame in _sequences[sequence]){
                totalSeconds += frame.Duration.TotalSeconds;
            }
            return TimeSpan.FromSeconds(totalSeconds);
        }
        private void _addSequence(string name){
            _sequences[name] = new List<AnimationFrame>();
        }
        public void AddFrames(string sequence, int startingFrame, int frameCount, int frameWidth, int frameHeight, TimeSpan duration, bool reverse = false)
        {
            for (int i = startingFrame; i < startingFrame + frameCount; i++){
                int x = i * frameWidth %_sheet.Width;
                int y = i / (_sheet.Width / frameWidth) * frameHeight;
                AddFrame(sequence, new Rectangle(x, y, frameWidth, frameHeight), duration, reverse);
            }
        }
        public void AddFrame(string sequence, Rectangle rectangle, TimeSpan duration, bool reverse = false)
        {
            if(!_sequences.ContainsKey(sequence)){
                _addSequence(sequence);
            }
            _sequences[sequence].Add(new AnimationFrame() {
                SourceRectangle = rectangle,
                Duration = duration,
                Reverse = reverse
            });
        }

        public void Update(GameTime gameTime) {
            if (Sequence == null) throw new Exception("No Frames Set");
            if (_running) {
                double secondsIntoAnimation = _timeIntoAnimation.TotalSeconds + gameTime.ElapsedGameTime.TotalSeconds;
                double remainder = secondsIntoAnimation % Duration(_sequence).TotalSeconds;
                _timeIntoAnimation = TimeSpan.FromSeconds(remainder);

                TimeSpan accumulatedTime;
                foreach (AnimationFrame frame in _sequences[_sequence]) {
                    if (accumulatedTime + frame.Duration >= _timeIntoAnimation) {
                        _currentFrame = frame;
                        break;
                    } else {
                        accumulatedTime += frame.Duration;
                    }
                }
            }
        }

        public void Render(Vector2 position, SpriteBatch spriteBatch){
            if (_currentFrame.Reverse){
                #pragma warning disable CS0618 // Type or member is obsolete
                spriteBatch.Draw(
                    texture: _sheet,
                    position: position,
                    sourceRectangle: _currentFrame.SourceRectangle,
                    color: Color.White,
                    effects: SpriteEffects.FlipHorizontally
                );
                #pragma warning restore CS0618 // Type or member is obsolete
            } else {
                spriteBatch.Draw(_sheet, position, _currentFrame.SourceRectangle, Color.White);
            }
        }
    }
}
