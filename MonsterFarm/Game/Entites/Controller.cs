using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonsterFarm.Game.States;
using static MonsterFarm.Game.Util.KeyboardHandler;

namespace MonsterFarm.Game.Entites
{
    public class Controller
    {
        public Controller()
        {

        }
        public Pawn Pawn { get; set; }

        public void Update(GameTime gameTime){
            if (Pawn == null) throw new Exception("Must set Pawn");

            if (!Pawn.Walking)
            {
                KeyboardState state = Keyboard.GetState();
                Keys[] down = state.GetPressedKeys();
                if(down.Length > 0){
                    Point update = new Point(0, 0);
                    if (down.Contains(Keys.W)){
                        update.Y -= 1;
                        Pawn.Direction = "up";
                    }
                    if (down.Contains(Keys.S)){
                        update.Y += 1;
                        Pawn.Direction = "down";
                    }
                    if (down.Contains(Keys.A)){
                        update.X -= 1;
                        Pawn.Direction = "left";
                    }
                    if (down.Contains(Keys.D)){
                        update.X += 1;
                        Pawn.Direction = "right";
                    }
                    if(update.X != 0 || update.Y != 0){
                        Point destination = Pawn.Position + update;
                        if (!Pawn.CanWalkTo(destination) || (update.X != 0 && update.Y != 0)){
                            if (Pawn.CanWalkTo(destination.X, Pawn.Position.Y)){
                                destination.Y = Pawn.Position.Y;
                            } else if (Pawn.CanWalkTo(Pawn.Position.X, destination.Y)){
                                destination.X = Pawn.Position.X;
                                if (update.Y < 0) Pawn.Direction = "up";
                                if (update.Y > 0) Pawn.Direction = "down";
                            }
                        }
                        Pawn.AddPath(destination);
                        Pawn.Update(gameTime);//smoothing
                    }
                } else {
                    if (Pawn.Animation != null) Pawn.Animation.Reset();
                }
            }
        }
    }
}
