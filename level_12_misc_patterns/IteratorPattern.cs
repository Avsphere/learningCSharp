using System;
using System.Collections.Generic;


/*
    Perhaps useful for manually extending LINQ like operations, which are already using iterators
    
    First I define an object, then what it means to have a collection of these objects

    I say that to be a collection you must implement the box collection interface which contains a create iterator method 
        (IEnumberable has only GetEnumberator which is the IEnumerator interface, the IEnumerator interface has MoveNext() current and reset)
    
    The IBoxIterator is essentially IEnumerator and CreateIterator is essentially IEnumberAble's GetEnumerator

    We then implement what it means to traverse this collection along with other accessing methods

 */

namespace level_12_iterator
{
    public class Box
    {
        private int _value;
        public Box(int value) => _value = value;

        //just playing
        public int Value
        {
            get { return _value; }
        }
    }


    public interface IBoxCollection
    {
        IBoxIterator CreateIterator();
    }

    public interface IBoxIterator
    {
        Box First();
        Box Next();
        bool IsDone { get; }
        Box Current { get; }
    }

    class BoxCollection : IBoxCollection
    {
        private List<Box> _items = new List<Box>();

        public BoxIterator CreateIterator() => new BoxIterator(this);

        public int Count
        {
            get { return _items.Count; }
        }

        public object this[int index]
        {
            get { return _items[index]; }
            set { _items.Add(value); }
        }

    }

    class BoxIterator : IBoxIterator
    {
        private BoxCollection _boxes;
        private int _current = 0;
        private int _step = 1;

        public BoxIterator(BoxCollection boxes) => _boxes = boxes;

        public bool IsDone
        {
            get { return _current >= _boxes.Count; }
        }

        Box First() 
        {
            _current = 0;
            return _boxes[_current] as Box;
        }

        Box Next()
        {
            _current += _step;
            return IsDone ? null : _boxes[_current] as Box;
        }

        public Box Current 
        {
            get { return _boxes[_current] as Box; }
        }


    }






}