using System;
using System.Collections.Generic;

namespace level_11_abstractFactory
{
    /*
        Let us imagine that this is an ecommerce app, there are 2 payment gateways, BankOne and BankTwo. 
        They each have varying ways of charging credit cards based on the purchase
        We present the user with three options:
            1. Use BankOne
            2. Use BankTwo
            3. Best option for user
    */
    //First we define a product model to represent what the user is trying to purchase
    public class Product
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
    }

    //Now we create an interface IPaymentGateway that acts as a contract defining the behavior that each of the Bank classes MUST conform to, this is their relational bond and is considered decoupled
    interface IPaymentGateway
    {
        void MakePayment(Product product);
        //recall that the poor way to do this would have been to have a class B that would have this make payment functionality inside of it, the BankOne and Two would instantiate an instance of B and call the method
    }

    public class BankOne : IPaymentGateway
    {
        public void MakePayment(Product product)
        {
            Console.Write("BankOne to pay for {0}, price {1}", product.Name, product.Price);
        }
    }
    

    public class BankTwo : IPaymentGateway
    {
        public void MakePayment(Product product)
        {
            Console.Write("BankTwo to pay for {0}, price {1}", product.Name, product.Price);
        }
    }

    //Now we create a factory class to handle creating these methods
    //The enum is for the factory class to map to the correct concrete class
    public enum PaymentMethod
    {
        BankOne,
        BankTwo,
        BestForUser
    }
    

    //What this class is doing is returning a concreate class based on computation of the arguments
    public class PaymentGatewayFactory
    {
        public virtual IPaymentGateway CreatePaymentGateway( PaymentMethod method, Product product )
        {
            if ( method == PaymentMethod.BankOne )
            {
                return new BankOne();
            }
            if ( method == PaymentMethod.BankTwo )
            {
                return new BankTwo();
            }
            if ( method == PaymentMethod.BestForUser )
            {
                if ( product.Price < 50 ) {
                    return new BankOne();
                } else {
                    return new BankTwo();
                }
            }
            throw new Exception("Gatewate not recognized");
        }
    }


}
