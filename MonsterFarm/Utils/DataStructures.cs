using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace MonsterFarm.Utils.DataStructures
{
    public class CoordinateNode<T>
    {
        public CoordinateNode(int x, int y, T value)
        {
            X = x;
            Y = y;
            Value = value;
        }
        public int X { get; }
        public int Y { get; }
        public T Value { get; }
    }

    public class Nodes<T> : IEnumerable<CoordinateNode<T>>
    {
        private CoordinateNode<T>[,] _data;
        public Nodes(CoordinateNode<T>[,] data){
            _data = data;
        }
        public IEnumerator<CoordinateNode<T>> GetEnumerator()
        {
            for (int y = 0; y < _data.GetLength(1); y++)
            {
                for (int x = 0; x < _data.GetLength(0); x++)
                {
                    if (_data[x, y] != null)
                    {
                        yield return _data[x, y];
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class Coordinate<T> : IEnumerable<T>
    {
        CoordinateNode<T>[,] _data;
        int _count, _xshift, _yshift, _xsize, _ysize;
        public Coordinate()
        {
            _count = 0;
            _xsize = _ysize = 4;
            _xshift = _yshift = 2;
            _data = new CoordinateNode<T>[_xsize, _ysize];
        }
        public CoordinateNode<T> Get(int? maxConnection = null)
        {
            List<CoordinateNode<T>> all = new List<CoordinateNode<T>>();
            foreach (CoordinateNode<T> i in Nodes){
                if (maxConnection == null){
                    all.Add(i);
                } else {
                    int n = 0;
                    if (Get(i.X+1, i.Y) != null) n++;
                    if (Get(i.X-1, i.Y) != null) n++;
                    if (Get(i.X, i.Y+1) != null) n++;
                    if (Get(i.X, i.Y-1) != null) n++;
                    if (n <= maxConnection) all.Add(i);
                }
            }
            return all[new Random().Next(all.Count)];
        }
        public CoordinateNode<T> Get(int x, int y)
        {
            int sx = x + _xshift;
            int sy = y + _yshift;
            return sx >= _data.GetLength(0) || sx < 0 || sy >= _data.GetLength(1) || sy < 0 ? null : _data[sx, sy];
        }
        public CoordinateNode<T> Get(float xf, float yf)
        {
            return Get((int)xf, (int)yf);
        }
        public void Add(int x, int y, Object entry)
        {
            _resize(x, y);
            int sx = x + _xshift;
            int sy = y + _yshift;
            if (sx >= _data.GetLength(0) || sx < 0 || sy >= _data.GetLength(1) || sy < 0) return;
            _count++;
            _data[sx, sy] = new CoordinateNode<T>(x, y, (T)entry);
        }
        public void Add(float xf, float yf, Object entry)
        {
            Add((int)xf, (int)yf, entry);
        }
        public void Remove(int x, int y)
        {
            int sx = x + _xshift;
            int sy = y + _yshift;
            if (sx >= _data.GetLength(0) || sx < 0 || sy >= _data.GetLength(1) || sy < 0)
            {
                _count--;
                _data[sx, sy] = null;
            }
        }
        public void Remove(float xf, float yf)
        {
            Remove((int)xf, (int)yf);
        }
        public int Count()
        {
            return _count;
        }
        CoordinateNode<T>[,] _copy()
        {
            CoordinateNode<T>[,] copy = new CoordinateNode<T>[_xsize, _ysize];
            for (int x = 0; x < _xsize; x++)
            {
                for (int y = 0; y < _ysize; y++)
                {
                    copy[x, y] = _data[x, y];
                }
            }
            return copy;
        }
        void _resize(int x, int y)
        {
            int sx = x + _xshift;
            int sy = y + _yshift;
            //Determin if out of bounds and in what direction is it out of bounds
            bool xgtb = sx >= _data.GetLength(0);
            bool xltb = sx < 0;
            bool ygtb = sy >= _data.GetLength(1);
            bool yltb = sy < 0;

            if (xgtb || xltb || ygtb || yltb)
            {
                //Clone existing data
                CoordinateNode<T>[,] _dataCopy = _copy();
                if (xgtb || xltb)
                { //resize if x was out of bounds
                    _xsize += _xsize;
                    if (xltb)
                    { //only shift values if x out of bounds in a negative direction
                        _xshift = _xsize / 2;
                    }
                }
                if (ygtb || yltb)
                { //resize if y was out of bounds
                    _ysize += _ysize;
                    if (yltb)
                    { //only shift values if y out of bounds in a negative direction
                        _yshift = _ysize / 2;
                    }
                }
                //Create data of new size
                _data = new CoordinateNode<T>[_xsize, _ysize];
                //Find shifted location of existing data in new coordinate array
                int _xs = xltb ? _xshift / 2 : xgtb ? _xsize / 2 : 0;
                int _ys = yltb ? _yshift / 2 : ygtb ? _ysize / 2 : 0;
                //write existing data into new coordinate array
                for (int ix = 0; ix < _dataCopy.GetLength(0); ix++)
                {
                    for (int iy = 0; iy < _dataCopy.GetLength(1); iy++)
                    {
                        _data[ix + _xs, iy + _ys] = _dataCopy[ix, iy];
                    }
                }
                //try again to determine if resize was sufficient to write new input
                _resize(x, y);
            }
        }

        public Nodes<T> Nodes { get { return new Nodes<T>(_data); } } 
        public IEnumerator<T> GetEnumerator()
        {
            for (int y = 0; y < _data.GetLength(1); y++)
            {
                for (int x = 0; x < _data.GetLength(0); x++)
                {
                    if (_data[x, y] != null) {
                        yield return _data[x, y].Value;
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
