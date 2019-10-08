using System;

//The decorator pattern allows us to wrap a class in another to change its behavior at run time
// a static extension method could not do this at run time


namespace level_12_decorator
{
    public abstract class Item
    {
        public string ItemType { get; set; }

        public abstract void Display();
    }

    public class Book : Item
    {
        public string Title { get; set ; }

        public Book(string title="noTitle")
        {
            base.ItemType = "book";
            Title = title;
        }
        public override void Display()
        {
            Console.WriteLine("book : ItemType {0}, title : {1}", ItemType, Title);
        }
    }

    public abstract class Decorator : Item
    {
        protected Item _item;
        public Decorator(Item i)
        {
            _item = i;
        }

        public override void Display() => _item.Display();
    }

    public class Flammable : Decorator
    {
        protected int _health = 100; 
        public Flammable(Item i) : base(i){}

        public void CatchFire() 
        {
            _health -= 10;
            Console.WriteLine("THIS ITEM IS A BLAZE");
        }
    }


    public static class DecoratorDemo
    {
        public static void Demo()
        {
            Book b = new Book();
            b.Display();

            Flammable flammableBook = new Flammable(b);

            flammableBook.CatchFire();
        }
    }
}