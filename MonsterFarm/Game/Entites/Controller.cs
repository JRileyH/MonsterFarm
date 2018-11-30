using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonsterFarm.Game.States;
using static MonsterFarm.Game.Util.KeyboardHandler;

namespace MonsterFarm.Game.Entites
{
    public class Controller
    {
        List<string> _directionInput;
        Dictionary<string, Point> _ = new Dictionary<string, Point>()
        {
            { "up", new Point(0,-1) },
            { "down", new Point(0,1) },
            { "left", new Point(-1,0) },
            { "right", new Point(1,0) }
        };
        public Controller()
        {
            _directionInput = new List<string>();
            WorldState.keyboardHandler.Subscribe(KeyTrigger.Press, Keys.W, () => { _directionInput.Insert(0, "up"); });
            WorldState.keyboardHandler.Subscribe(KeyTrigger.Press, Keys.S, () => { _directionInput.Insert(0, "down"); });
            WorldState.keyboardHandler.Subscribe(KeyTrigger.Press, Keys.A, () => { _directionInput.Insert(0, "left"); });
            WorldState.keyboardHandler.Subscribe(KeyTrigger.Press, Keys.D, () => { _directionInput.Insert(0, "right"); });
            WorldState.keyboardHandler.Subscribe(KeyTrigger.Release, Keys.W, () => { _directionInput.Remove("up"); });
            WorldState.keyboardHandler.Subscribe(KeyTrigger.Release, Keys.S, () => { _directionInput.Remove("down"); });
            WorldState.keyboardHandler.Subscribe(KeyTrigger.Release, Keys.A, () => { _directionInput.Remove("left"); });
            WorldState.keyboardHandler.Subscribe(KeyTrigger.Release, Keys.D, () => { _directionInput.Remove("right"); });
        }
        public Pawn Pawn { get; set; }
        public void Update(GameTime gameTime){
            if (Pawn == null) throw new Exception("Must set Pawn");
            if (!Pawn.Walking){
                if(_directionInput.Count > 0){
                    Pawn.AddPath(Pawn.Position + _[_directionInput[0]]);
                    Pawn.Update(gameTime);//smoothing
                } else {
                    if (Pawn.Animation != null) Pawn.Animation.Reset();
                }
            }
        }
    }
}
