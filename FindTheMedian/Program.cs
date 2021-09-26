using System;
using System.Collections.Generic;
using System.Threading;

namespace FindTheMedian
{
    class Program
    {
        //Количество потоков, которые будут отправлять запросы
        private static readonly int _threadsNumber = 10;

        //Рэндж команд
        private static readonly int _startPoint = 1;
        private static readonly int _endPoint = 2018;

        static void Main(string[] args)
        {
            Console.WriteLine("Start");

            /*
             * Списки объектов класса клиент и потоков. 
             * 
             * Каждый поток будет запускать метод из "своего" клиента
             */
            var clientObjectsList = new List<Client>();
            var threadList = new List<Thread>();

            //Список получаемых списков значений
            var listOfNumberLists = new List<List<long>>();

            //Шаг между отрезками номеров команд
            int step = GetStep();

            //Остаток, не разделенный между потоками
            int remainder = _endPoint - _threadsNumber * step;

            int currentStartPoint = _startPoint;

            //Инициализация списков клиентов и тредов
            for (int i = 0; i < _threadsNumber; i++)
            {
                var client = new Client(currentStartPoint, currentStartPoint + step - 1);
                clientObjectsList.Add(client);
                threadList.Add(new Thread(new ThreadStart(client.RecieveData)));
                currentStartPoint += step;
            }

            Console.WriteLine("Getting numbers...");

            foreach (var thread in threadList)
                thread.Start();

            //Обработка нераспределенного остатка
            if (remainder != 0)
            {
                var client = new Client(currentStartPoint, currentStartPoint + remainder - 1);
                client.RecieveData();
                listOfNumberLists.Add(client.GetList());
            }

            //Ожидание выполнения потоков
            foreach (var thread in threadList)
                thread.Join();

            Console.WriteLine("Main Sort..." + Environment.NewLine);

            //Инициализация списка полученных чисел
            foreach (var client in clientObjectsList)
                listOfNumberLists.Add(client.GetList());

            //Окончательная сортировка алгоритмом
            var sortedListOfNumbers = MainSort(listOfNumberLists);

            foreach (var a in sortedListOfNumbers)
            {
                Console.WriteLine(a);
            }

            Console.WriteLine($"The median is {FindTheMedian(sortedListOfNumbers)}");
        }

        private static long FindTheMedian(List<long> list)
        {
            if (list.Count % 2 != 0)
                return list[(list.Count - 1) / 2];
            else
                return (list[(list.Count - 1) / 2] + list[(list.Count - 1) / 2 + 1]) / 2;
        }

        private static int GetStep()
        {
            return (_endPoint - _startPoint + 1) / _threadsNumber;
        }

        /*
         * Входящие списки попарно сортируются алгоритмом сортировки слиянием.
         * 
         * Я подумал, что это будет быстрее, чем алгоритм быстрой сортировки,
         * т.к. эти списки поступают уже отсортированными.
         * 
         * Постепенно они соеденяются в один список и он возвращается
         */
        private static List<long> MainSort(List<List<long>> listOfNumberLists)
        {
            if (listOfNumberLists.Count == 1)
                return listOfNumberLists[0];

            int listCount = listOfNumberLists.Count;

            while (listCount != 1)
            {
                var subList = new List<List<long>>();
                var threadList = new List<Thread>();
                var delegatorsList = new List<ToSorterDelegator>();

                for (int i = 0; i < listCount / 2 * 2; i += 2)
                {
                    var delegator = new ToSorterDelegator(listOfNumberLists[i], listOfNumberLists[i + 1]);
                    delegatorsList.Add(delegator);
                    threadList.Add(new Thread(new ThreadStart(delegator.Delegate)));
                }

                foreach (var thread in threadList)
                    thread.Start();

                foreach (var thread in threadList)
                    thread.Join();

                foreach (var delegator in delegatorsList)
                    subList.Add(delegator.GetResult());

                if (listCount % 2 != 0)
                    subList.Add(listOfNumberLists[listCount - 1]);

                listOfNumberLists = new List<List<long>>();
                listOfNumberLists.AddRange(subList);
                listCount = listOfNumberLists.Count;
            }

            return listOfNumberLists[0];
        }
    }
}
