using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace www
{
    class Program
    {

        //委托定义
        public delegate void Eat(String foodName);

        public void EatRice(String name)
        {
            Console.WriteLine("I am eating rice, " + name);
        }

        public void EatFruit(String name)
        {
            Console.WriteLine("I am eating fruit, " + name);
        }

        public event Eat eat;

        //所要实现的操作写在同一个方法中，方便调用
        public void Test()
        {
            /*------------实例化委托---------------
             * 注意用new关键字，像实例化一个类一样，然后传入一个参数，但这个参数是一个方法名。
             */
            Eat eatRice = new Eat(EatRice);
            Eat eatFuit = new Eat(EatFruit);

            //通过“+=”来将n个委托实例加到某个事件上，一旦这个事件发生，所有的这些委托实例都会得到通知。
            eat += eatRice;
            eat += eatFuit;

            //触发一个事件，注意我们是直接用一个事件名，然后跟一个参数，
            //这又跟“委托”中定义的那个规矩一致（即，要有一个string类型的参数）。
            eat("Eric");


            /*实现方法二（这种方法省略了“委托”的实例化过程）
             * 我们回过头来再看一下“事件”的定义：
                public event SaySomething come;
                这里已经指出了“委托”的名字，所以，我们可以直接将方法加到事件上，而省略“委托”的实例化过程，
                因此上面的test()方法可以简单写为：
             */
            eat += EatRice;
            eat += EatFruit;
            eat("Mark");

        }


        static void Main(string[] args)
        {
            Console.WriteLine("进程");

            //调用 Test() 方法
            Program program = new Program();
            program.Test();


            Console.ReadKey();
        }
    }
}
