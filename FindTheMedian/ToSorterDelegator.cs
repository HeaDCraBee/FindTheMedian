using System.Collections.Generic;

namespace FindTheMedian
{
    public class ToSorterDelegator
    {
        private List<long> _firstList = new List<long>();
        private List<long> _secondList = new List<long>();
        private List<long> _result = new List<long>();

        public ToSorterDelegator(List<long> firstList, List<long> secondList)
        {
            _firstList.AddRange(firstList);
            _secondList.AddRange(secondList);          
        }

        public List<long> GetResult()
        {
            return _result;
        }

        public void Delegate()
        {
            _result = Sorter.MergeSort(_firstList, _secondList);
        }
    }
}
