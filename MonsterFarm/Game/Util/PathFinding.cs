using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace MonsterFarm.Game.Util
{
    public static class PathFinding
    {
		public static Point[] bfs(Point source, Point goal, bool[,] map){
            Queue<Point> q = new Queue<Point>();
            List<Point> v = new List<Point>();
            q.Enqueue(source);
            while(q.Count > 0){
                Point c = q.Dequeue();
                if(!v.Contains(c) && map[c.X, c.Y]){
                    v.Add(c);
                    if (c.X > 0) {
                        Point l = c + new Point(-1, 0);
                        if (!v.Contains(l) && map[l.X, l.Y]){
                            v.Add(l);
                        }
                    }
                    if (c.X < map.GetLength(0) - 1) {
                        Point r = c + new Point(1, 0);
                        if (!v.Contains(r) && map[r.X, r.Y]){
                            v.Add(r);
                        }
                    }
                    if (c.Y > 0) {
                        Point t = c + new Point(0, -1);
                        if (!v.Contains(t) && map[t.X, t.Y]){
                            v.Add(t);
                        }
                    }
                    if (c.Y < map.GetLength(1) - 1) {
                        Point b = c + new Point(0, 1);
                        if (!v.Contains(b) && map[b.X, b.Y]){
                            v.Add(b);
                        }
                    }
                }
            }

            return new List<Point>().ToArray();
		}
    }
}
