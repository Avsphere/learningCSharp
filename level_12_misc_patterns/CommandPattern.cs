using System;
using System.Collections.Generic;


    /*
    
    The benefit that I am seeing with the Command Pattern is that we have built this middle layer between our object and execution of methods upon it
    Thus we can easily add things like an undo or redo process by adding to our ICommand interface

    This allows us to not modify our object (in this case the product) and instead abstract execution logic and meta execution to a Command Class

    To be a bit more specific we have a command class that literally performs the execution, think of this like a collection of non executed function

    And we have an Invoker who stores the commands or perhaps alters the invoke pattern


    Following this js translation

    The command class is a bag of unexecuted functions
    The invoker invokes the functions as it sees fit and stores information about their invocation
    
    */

namespace level_12_command
{
    public class Product
    {
        public string Name { get; set; }
        public int Price { get; set; }

        public void IncrementPrice()
        {
            Price++;
        }
    
        public void DecrementPrice()
        {
            Price--;
        }

        public override string ToString() => $"Name : {Name}, Price : {Price}";
    }

    //Product is our receiver class, now the client can perform actions on the price, but the command pattern says that
    //WE SHOULD EXTRACT ALL REQUEST DETAILS into a command class

    public interface ICommand
    {
        void ExecuteAction();
    }

    //in the tutorial the enum all possible actions that the command is to encapsulate
    public enum PriceAction
    {
        Increase,
        Decrease
    }


    public class ProductCommand : ICommand
    {
        private readonly Product _product;
        private readonly PriceAction _priceAction;

        public ProductCommand(Product product, PriceAction priceAction)
        {
            _product = product;
            _priceAction = priceAction;
        }

        //Now this is what has abstracted away the performing actions, aka we are giving product command everything it needs to perform the actions
        public void ExecuteAction()
        {
            if ( _priceAction == PriceAction.Increase )
            {
                _product.IncrementPrice();
            } 
            else
            {
                _product.DecrementPrice();
            }
        }

    }


    public class ModifyPrice
    {
        private readonly List<ICommand> _commands;
        private ICommand _command;


        public ModifyPrice()
        {
            _commands = new List<ICommand>();
        }

        public void SetCommand(ICommand command) => _command = command;

        public void Invoke()
        {
            _commands.Add(_command);
            _command.ExecuteAction();
        }

    }


    public static class CommandDemo
    {

        private static void Execute(Product product, ModifyPrice modifyPrice, ICommand productCommand)
        {
            modifyPrice.SetCommand(productCommand);
            modifyPrice.Invoke();
        }
        public static void Demo()
        {
            ModifyPrice modifyPrice = new ModifyPrice();
            Product product = new Product { Name = "Phone", Price = 100 };

            Execute(product, modifyPrice, new ProductCommand(product, PriceAction.Increase) );
            Execute(product, modifyPrice, new ProductCommand(product, PriceAction.Decrease) );
            Execute(product, modifyPrice, new ProductCommand(product, PriceAction.Decrease) );
            Execute(product, modifyPrice, new ProductCommand(product, PriceAction.Increase) );


        }
    }

}