using System;
using System.Collections.Generic;
using System.Threading;

namespace FindTheMedian
{
    public static class Sorter
    {
        /*
         * Реализация нерекурсивного алгоритма быстрой сортировки.
         * 
         * Вместо стека вызова тут стек как структура данных
         */
        public static List<long> QuickSort(List<long> list)
        {
            var call = new Stack<List<long>>();
            var sortedList = new List<long>();
            call.Push(list);

            if (list.Count == 0)
                throw new ArgumentException("Empty list");
           
            do
            {
                var listToSort = call.Pop();

                if (listToSort.Count == 1)
                {
                    sortedList.Add(listToSort[0]);
                    continue;
                }

                var pivot = listToSort[0];
                var less = new List<long>();
                var greater = new List<long>();

                for(int i = 1; i < listToSort.Count; i++)
                {
                    if (listToSort[i] > pivot)
                        greater.Add(listToSort[i]);

                    else
                        less.Add(listToSort[i]);
                }

                if (greater.Count != 0)
                    call.Push(greater);

                call.Push(new List<long>() { pivot });

                if (less.Count != 0)
                    call.Push(less);

            } while (call.Count != 0);
            
            return sortedList;
        }

        //Реализация сортировки слиянием
        public static List<long> MergeSort(List<long> firstList, List<long> secondList)     
        {
            if (firstList.Count == 0 && secondList.Count == 0)
                throw new ArgumentException("Empty lists");

            if (firstList.Count == 0)
                return secondList;
            else if (secondList.Count == 0)
                return firstList;

            var sortedList = new List<long>();
            var firstPointer = 0;
            var secondPointer = 0;

            while (firstPointer != firstList.Count && secondPointer != secondList.Count)
            {
                if (firstList[firstPointer] < secondList[secondPointer])
                {
                    sortedList.Add(firstList[firstPointer]);
                    firstPointer++;
                }
                else
                {
                    sortedList.Add(secondList[secondPointer]);
                    secondPointer++;
                }
            }

            if (firstPointer == firstList.Count)
            {
                for (int i = secondPointer; i < secondList.Count; i++)                
                    sortedList.Add(secondList[i]);              
            }
            else
            {
                for (int i = firstPointer; i < firstList.Count; i++)
                    sortedList.Add(firstList[i]);
            }

            return sortedList;  
        }
    }
}
