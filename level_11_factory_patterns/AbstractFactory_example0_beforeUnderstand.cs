using System;
using System.Collections.Generic;

namespace level_11_factory_patterns
{
        public interface IAirConditioner
        {
            void Operate();
        }

        //These Cooling and warming classes are just implementing our interface
        public class Cooling : IAirConditioner
        {
            private readonly double _temperature;
            
            public Cooling(double temperature)
            {
                _temperature = temperature;
            }

            public void Operate()
            {
                Console.WriteLine("Cooling to {0} beep boop", _temperature);
            }

        }

        public class Warming : IAirConditioner
        {
            private readonly double _temperature;

            public Warming( double temperature ) 
            {
                _temperature = temperature;
            }

            public void Operate()
            {
                Console.WriteLine("Warming to {0} beep boop", _temperature);
            }

        }

        //Now we create a factor to create these objects
        public abstract class AirConditionerFactory
        {
            public abstract IAirConditioner Create(double temperature); //this class creating abstraction method...
        }

        public class CoolingFactory : AirConditionerFactory
        {
            public override IAirConditioner Create(double temperature) => new Cooling(temperature);
        }
        public class WarmingFactory : AirConditionerFactory
        {
            public override IAirConditioner Create(double temperature) => new Warming(temperature);
        }

        public enum Actions
        {
            Cooling,
            Warming
        }
        
        //now I define the actual AirConitioner Class


        public class AirConditioner
        {
            private readonly Dictionary<Actions, AirConditionerFactory> _factories;

            private AirConditioner()
            {
                _factories = new Dictionary<Actions, AirConditionerFactory>
                {
                    { Actions.Cooling, new CoolingFactory() },
                    { Actions.Warming, new WarmingFactory() }
                };

            }

            public static AirConditioner InitializeFactories() => new AirConditioner();

            public IAirConditioner ExecuteCreation(Actions action, double temperature) => _factories[action].Create(temperature);
        }

        public static class Example0
        {
            public static void Demo()
            {
                AirConditioner
                    .InitializeFactories()
                    .ExecuteCreation(Actions.Cooling, 22.5)
                    .Operate();
            }
        }

}
