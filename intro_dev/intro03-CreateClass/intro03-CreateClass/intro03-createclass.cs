using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace intro03_CreateClass
{
    class Program
    {
        static void Main(string[] args)
        {
            /*Create and call the person01 Human object here*/
            Human person01 = new Human("Genius", Human.Gender.Male, 22);
            person01.greeting();
            Kid child01 = new Kid("Babe", Human.Gender.Male, 3, 100);
            child01.greeting();
            child01.getWeight();
            
        }
    }


    public class Human
    {
        /*use static to define the class static value that is not affected by the current object.*/
        public static int population = 10000000;
        private int _age;
        public int age
        {
            get { return _age; }
            set
            {
                if (value > 30)
                {
                    _age = 21;
                }
                else
                {
                    _age = value;
                }
            }
        }
        public Gender gender { get; set; }
        public string name { get; set; }

        /*Construct the class attributes, initialize the class with the input values.*/
        public Human(string Name, Gender Gender, int Age)
        {
            name = Name;
            gender = Gender;
            age = Age;
        }
        public void greeting()
        {
            string greets = $"hey wasssssupppp, my name is {name}, and I'm almost {age} years old. It is very nice to meet you here!";
            Console.WriteLine(greets);
        }
        public enum Gender
        {
            Male,
            Female
        }
    }
    public class Kid : Human
    {
        /*Define input attribute data types beforehand*/
        public float weight { set; get; }
        /*Inherit the parent class and initialize both inherited attributes and new attributes*/
        public Kid(string Name, Gender Gender, int Age, float Weight) : base(Name, Gender, Age)
        {
            name = Name;
            gender = Gender;
            age = Age;
            weight = Weight;
        }
        
        public void getWeight()
        {
            Console.WriteLine($"The baby is weighted {weight} LBS!");
        }

    }


}


