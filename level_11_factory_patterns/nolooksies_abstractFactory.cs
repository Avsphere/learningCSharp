/*
    Imagine we have an application that has a light theme and a dark theme

    Each theme has several UI components: buttons, alerts, text boxes, inputs etc

    A single factory may be used to create UI components.
    
        Imagine an IUIComponent c = UIComponentFactory.Create('button');
        c.draw()

            recall that if we did not use a factory then it would have to be something like

            IUIComponent c;

            if ( 'button' ) { c = new Button() }
            if ('input' ) { c = new Input() }

            Thus we choose to decouple this logic
        
    This is great as we can now pass this factory into a method and it can use this to create pieces

    HomePageRenderer( UIComponentFactory f )
    {
        navBar()
        {
            f.create('...')
            f.create('...)
        }

        footer()
        {
            f.create('...')
            f.create('...')
        }

    }


    Going back to our application, dark theme and light theme both need to have all the same UI components

    in this case though light theme would return to us a factory that can create UI components that have been initialized with white as its main color
    while the dark theme would be opposite


    then we would pass this "lightThemeFactory" to HomePageRenderer which then would be able to maintain its functionality without changing

    HomePageRenderer would then ASK the factory to create a button, or to create a list

    Hmm I wonder how we design the components such that they can support X themes without being coupled to them??
        
 */


using System;
using System.Collections.Generic;

namespace level_11_factory_patterns
{

    public static class Extensions
    {
        public static string UIComponentToString(this IUIComponent component )
        {
            return $@"
            Properties {component.Properties}. 
            Color : {component.Color}
            Html : {component.Html}
            ";
        }
    }
    public struct Position
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public override string ToString()
        {
            return $"X : {X}, Y: {Y}, Z : {Z}";
        }

    }
    public class Properties 
    {
        public Position Position { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return $"Position {Position}. Name : {Name}";
        }

    }
    public interface IUIComponent
    {
        Properties Properties { get; set; }
        string Color { get; set; }
        string Html { get; set; }
    }


    public class Button : IUIComponent
    {
        public Properties Properties { get; set; }
        public string Color { get; set; }
        private string _Html;
        public string Html
        {
            get { return _Html; }
            set
            {
                _Html = $"<button>{value}</button>";
            }
        }




    }

    public class Link : IUIComponent
    {
        public Properties Properties { get; set; }
        public string Color { get; set; }
        private string _Html;
        public string Html
        {
            get { return _Html; }
            set
            {
                _Html = $"<a>{value}</a>";
            }
        }

    }

    public abstract class ThemeFactory
    {
        public abstract IUIComponent Create(string name, Properties properties, string html);

    }

    public class LightThemeFactory : ThemeFactory
    {
        public override IUIComponent Create(string name, Properties properties, string html )
        {
            if ( name == "Button" )
            {
                return new Button { Color = "white", Properties = properties, Html = html };
            }
            else if ( name == "Link" )
            {
                return new Link { Color = "white", Properties = properties, Html = html };
            }
            else {
                throw new Exception("Factory cannot create unrecognized component");
            }
        } 
    }

    public class DarkThemeFactory : ThemeFactory
    {
        public override IUIComponent Create(string name, Properties properties, string html )
        {
            if ( name == "Button" )
            {
                return new Button { Color = "black", Properties = properties, Html = html };
            }
            else if ( name == "Link" )
            {
                return new Link { Color = "black", Properties = properties, Html = html };
            }
            else {
                throw new Exception("Factory cannot create unrecognized component");
            }

        } 
    }


    public static class AbstractFactoryDemo
    {

        public static void BuildNavButton(ThemeFactory factory)
        {
            IUIComponent button = factory.Create(
                "Button",
                new Properties { 
                    Position = new Position { X = 2, Y = 2, Z = 1 }, 
                    Name = "testButton" 
                },
                "button html"
            );
            
            IUIComponent link = factory.Create(
                "Link",
                new Properties { 
                    Position = new Position { X = 0, Y = 21, Z = 13 }, 
                    Name = "testLink" 
                },
                "Link html"
            );
        }
        public static void Demo()
        {
            /*
            
            First to use as a factory method I can
                1. Instantiate factory
                2. Use this to create my objects
            */
            ThemeFactory lightThemeFactory = new DarkThemeFactory();
            BuildNavButton(lightThemeFactory);

            

        
        
        }
    }


}