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

    public class Coordinate<T> : IEnumerable<T>
    {
        T[,] _data;
        int _count;
        int _xshift;
        int _yshift;
        int _xsize;
        int _ysize;
        bool _resizable;

        public Coordinate(int? x = null, int? y = null, bool resizable = true)
        {
            _count = 0;
            _xsize = (x * 2) ?? 1;
            _ysize = (y * 2) ?? 1;
            _xshift = x ?? 0;
            _yshift = y ?? 0;
            _resizable = resizable;
            _data = new T[_xsize, _ysize];
            _populateNodeList();
        }
        public T Get(){
            Random rnd = new Random();
            List<T> all = new List<T>();
            foreach(T i in this){
                all.Add(i);
            }
            return all[rnd.Next(all.Count)];
        }
        public T Get(int x, int y)
        {
            int sx = x + _xshift;
            int sy = y + _yshift;
            return sx >= _data.GetLength(0) || sx < 0 || sy >= _data.GetLength(1) || sy < 0 ? default(T) : _data[sx, sy];
        }
        public T Get(float x, float y)
        {
            int sx = (int)x + _xshift;
            int sy = (int)y + _yshift;
            return sx >= _data.GetLength(0) || sx < 0 || sy >= _data.GetLength(1) || sy < 0 ? default(T) : _data[sx, sy];
        }
        public void Add(int x, int y, Object entry)
        {
            if(_resizable) _resize(x, y);
            int sx = x + _xshift;
            int sy = y + _yshift;
            if (sx >= _data.GetLength(0) || sx < 0 || sy >= _data.GetLength(1) || sy < 0) return;
            _count++;
            Debug.WriteLine("Adding: " + x + ", " + y);
            _data[sx, sy] = (T)entry;
            _populateNodeList();
        }
        public void Add(float xf, float yf, Object entry)
        {
            int x = (int)xf;
            int y = (int)yf;
            if (_resizable) _resize(x, y);
            int sx = x + _xshift;
            int sy = y + _yshift;
            if (sx >= _data.GetLength(0) || sx < 0 || sy >= _data.GetLength(1) || sy < 0) return;
            _count++;
            Debug.WriteLine("Adding: " + x + ", " + y);
            _data[sx, sy] = (T)entry;
            _populateNodeList();
        }
        public void Remove(int x, int y)
        {
            int sx = x + _xshift;
            int sy = y + _yshift;
            if (sx >= _data.GetLength(0) || sx < 0 || sy >= _data.GetLength(1) || sy < 0)
            {
                _count--;
                _data[sx, y + sy] = default(T);
                _populateNodeList();
            }
        }
        public int Count(){
            return _count;
        }
        public List<CoordinateNode<T>> Nodes { get; private set; }
        void _populateNodeList()
        {
            List<CoordinateNode<T>> nodeList = new List<CoordinateNode<T>>();
            for (int x = 0; x < _data.GetLength(0); x++)
            {
                for (int y = 0; y < _data.GetLength(1); y++)
                {
                    nodeList.Add(new CoordinateNode<T>(x - _xshift, y - _yshift, _data[x, y]));
                }
            }
            Nodes = nodeList;
        }
        T[,] _copy()
        {
            T[,] copy = new T[_xsize, _ysize];
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
                T[,] _dataCopy = _copy();
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
                _data = new T[_xsize, _ysize];
                //Find shifted location of existing data in new coordinate array
                //Converting to double and finding ceiling is done for the single case of 1/2=0
                int _xs = xltb ? (int)Math.Ceiling((double)_xshift / (double)2) : 0;
                int _ys = yltb ? (int)Math.Ceiling((double)_yshift / (double)2) : 0;
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

        public IEnumerator<T> GetEnumerator()
        {
            for (int x = 0; x < _data.GetLength(0); x++)
            {
                for (int y = 0; y < _data.GetLength(1); y++)
                {
                    if (!EqualityComparer<T>.Default.Equals(_data[x, y], default(T))) {
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
}
