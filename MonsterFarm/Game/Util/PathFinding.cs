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
            public bfsNode(Point point, List<Point> path){
                Point = point;
                Path = path;
            }
            public Point Point { get; set; }
            public List<Point> Path { get; set; }
        };

        public static List<Point> BFS(Point source, Point goal, bool[,] map){

            if (!map[source.X, source.Y] || !map[goal.X, goal.Y]) return new List<Point>();
            bool[,] visited = new bool[map.GetLength(0),map.GetLength(1)];
            visited[source.X, source.Y] = true;
            Queue<bfsNode> Q = new Queue<bfsNode>();
            Q.Enqueue(new bfsNode(source, new List<Point>()));

            while(Q.Count > 0){
                bfsNode current = Q.Dequeue();
                if (current.Point == goal) return current.Path;
                Point point;
                List<Point> path;
                point = new Point(current.Point.X, current.Point.Y - 1);
                if (current.Point.Y > 0 && map[point.X, point.Y] && !visited[point.X, point.Y])
                {//top
                    path = new List<Point>();
                    path.AddRange(current.Path);
                    path.Add(point);
                    Q.Enqueue(new bfsNode(point, path));
                    visited[point.X, point.Y] = true;
                }
                point = new Point(current.Point.X, current.Point.Y + 1);
                if (current.Point.Y < map.GetLength(1) - 1 && map[point.X, point.Y] && !visited[point.X, point.Y])
                {//bottom
                    path = new List<Point>();
                    path.AddRange(current.Path);
                    path.Add(point);
                    Q.Enqueue(new bfsNode(point, path));
                    visited[point.X, point.Y] = true;
                }
                point = new Point(current.Point.X - 1, current.Point.Y);
                if (current.Point.X > 0 && map[point.X, point.Y] && !visited[point.X, point.Y])
                {//left
                    path = new List<Point>();
                    path.AddRange(current.Path);
                    path.Add(point);
                    Q.Enqueue(new bfsNode(point, path));
                    visited[point.X, point.Y] = true;
                }
                point = new Point(current.Point.X + 1, current.Point.Y);
                if (current.Point.X < map.GetLength(0) - 1 && map[point.X, point.Y] && !visited[point.X, point.Y])
                {//right
                    path = new List<Point>();
                    path.AddRange(current.Path);
                    path.Add(point);
                    Q.Enqueue(new bfsNode(point, path));
                    visited[point.X, point.Y] = true;
                }
            }

            return new List<Point>();
        }
    }
}
