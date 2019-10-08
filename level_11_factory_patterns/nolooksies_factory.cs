



namespace level_11_factory_patterns
{

    public class Product {
        public string Name { get; set; }
    }


    public interface IBusinessObject
    {
        int ImportantData { get; set; }
        Product product { get; }
    }
    public class ConcreteBusinessObj : IBusinessObject
    {
        public int ImportantData { get; set; }
        public Product product { get; }

        public ConcreteBusinessObj(string name) 
        {
            product = new Product { Name = name };
        }
    
    }

    public class SomeOtherConcreteBusinessObj : IBusinessObject
    {
        public int ImportantData { get; set; }
        public Product product { get; }

        public SomeOtherConcreteBusinessObj() 
        {
            product = new Product { Name = "beepBoop" };
        }
    
    }

    public class ObjectFactory
    {
        public static IBusinessObject Create(string productName="default")
        { 
            if ( productName == "ConcreteBusinessObject" )
            {
                return new ConcreteBusinessObj(productName);
            }
            else
            {
                return new SomeOtherConcreteBusinessObj();
            }
        }
    }



    public static class NoLooksies
    {
        public static int Demo()
        {
            // var a = new ConcreteBusinessObj();
            //but now I want to switch it out for the factory
            
            // var a = ObjectFactory.Create();
            //and i may need to adjust how the object is created

            IBusinessObject a = ObjectFactory.Create("ConcreteBusinessObject");


            return 0;
        }
    }


}