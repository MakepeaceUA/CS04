namespace ConsoleApp22
{
    internal class Program
    {
        static Random random = new Random();
        static int people = 0;
        const int capacity = 30;
        static ManualResetEvent arrived = new ManualResetEvent(false);
        static object lock_obj = new object();

        static void Main()
        {
            Thread generator = new Thread(GeneratePassengers);
            Thread service = new Thread(RunBus);

            generator.Start();
            service.Start();

            generator.Join();
            service.Join();
        }

        static void GeneratePassengers()
        {
            for (int i = 0; i < 10; i++)
            {
                Monitor.Enter(lock_obj);
                try
                {
                    int NewPass = random.Next(5, 21);
                    people += NewPass;
                    Console.WriteLine($"Пришло:{NewPass}. Всего:{people}");
                }
                finally
                {
                    Monitor.Exit(lock_obj);
                }
                Console.WriteLine();
                Thread.Sleep(random.Next(1000, 3000));
                arrived.Set();
            }
        }

        static void RunBus()
        {
            for (int i = 0; i < 10; i++)
            {
                arrived.WaitOne();
                Monitor.Enter(lock_obj);
                try
                {
                    int BoardPass = Math.Min(capacity, people);
                    people -= BoardPass;
                    Console.WriteLine($"Автобус прибыл.\nПосадка:{BoardPass}. Осталось:{people}");
                }
                finally
                {
                    Monitor.Exit(lock_obj);
                }
                Console.WriteLine();
                arrived.Reset();
                Thread.Sleep(random.Next(2000, 5000));
            }
        }
    }
}