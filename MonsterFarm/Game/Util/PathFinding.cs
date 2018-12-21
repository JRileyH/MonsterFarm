using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace MonsterFarm.Game.Util
{
    public static class PathFinding
    {
        struct bfsNode
        {
            public bfsNode(Vector2 point, List<Vector2> path){
                Point = point;
                Path = path;
            }
            public Vector2 Point { get; set; }
            public List<Vector2> Path { get; set; }
        };

        public static List<Vector2> BFS(Vector2 source, Vector2 goal, bool[,] map){
            if (!map[(int)source.X, (int)source.Y] || !map[(int)goal.X, (int)goal.Y]) return new List<Vector2>();
            bool[,] visited = new bool[map.GetLength(0),map.GetLength(1)];
            visited[(int)source.X, (int)source.Y] = true;
            Queue<bfsNode> Q = new Queue<bfsNode>();
            Q.Enqueue(new bfsNode(source, new List<Vector2>()));

            while(Q.Count > 0){
                bfsNode current = Q.Dequeue();
                if (current.Point == goal) return current.Path;
                Vector2 point;
                List<Vector2> path;
                point = new Vector2(current.Point.X, current.Point.Y - 1);
                if (current.Point.Y > 0 && map[(int)point.X, (int)point.Y] && !visited[(int)point.X, (int)point.Y])
                {//top
                    path = new List<Vector2>();
                    path.AddRange(current.Path);
                    path.Add(point);
                    Q.Enqueue(new bfsNode(point, path));
                    visited[(int)point.X, (int)point.Y] = true;
                }
                point = new Vector2(current.Point.X, current.Point.Y + 1);
                if (current.Point.Y < map.GetLength(1) - 1 && map[(int)point.X, (int)point.Y] && !visited[(int)point.X, (int)point.Y])
                {//bottom
                    path = new List<Vector2>();
                    path.AddRange(current.Path);
                    path.Add(point);
                    Q.Enqueue(new bfsNode(point, path));
                    visited[(int)point.X, (int)point.Y] = true;
                }
                point = new Vector2(current.Point.X - 1, current.Point.Y);
                if (current.Point.X > 0 && map[(int)point.X, (int)point.Y] && !visited[(int)point.X, (int)point.Y])
                {//left
                    path = new List<Vector2>();
                    path.AddRange(current.Path);
                    path.Add(point);
                    Q.Enqueue(new bfsNode(point, path));
                    visited[(int)point.X, (int)point.Y] = true;
                }
                point = new Vector2(current.Point.X + 1, current.Point.Y);
                if (current.Point.X < map.GetLength(0) - 1 && map[(int)point.X, (int)point.Y] && !visited[(int)point.X, (int)point.Y])
                {//right
                    path = new List<Vector2>();
                    path.AddRange(current.Path);
                    path.Add(point);
                    Q.Enqueue(new bfsNode(point, path));
                    visited[(int)point.X, (int)point.Y] = true;
                }
            }
            visited = null;

            return new List<Vector2>();
        }
    }
}
