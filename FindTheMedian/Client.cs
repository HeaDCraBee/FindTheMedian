using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace FindTheMedian
{
    public class Client
    {
        private string _adress = "88.212.241.115";
        private int _port = 2012;
        private NetworkStream _netStream;
        private string _regex = @"\d";
        private List<long> _numbersList = new List<long>();
        private TcpClient _tcpClient = new TcpClient();
        private bool _isConnected = false;
        private int _startPoint;
        private int _endPoint;

        public Client(int startPoint, int endPoint)
        {
            _startPoint = startPoint;
            _endPoint = endPoint;
        }

        public List<long> GetList()
        {
            return _numbersList;
        }

        public void RecieveData()
        {
            Connect();
            for (int i = _startPoint; i < _endPoint + 1; i++)
            {
                Console.WriteLine(i);
                var messageToSend = Encoding.UTF8.GetBytes(i.ToString() + Environment.NewLine);
                var received = new StringBuilder();
                var isNumberStart = false;
                var isNumberEnd = false;
                try
                {
                    _netStream = _tcpClient.GetStream();
                    _netStream.Write(messageToSend);

                    while (!isNumberEnd)
                    {
                        var bytes = new byte[1];
                        _netStream.Read(bytes, 0, 1);

                        if (Regex.IsMatch(Encoding.UTF8.GetString(bytes), _regex))
                        {
                            if (!isNumberStart)
                                isNumberStart = true;

                            received.Append(Encoding.UTF8.GetString(bytes));
                        }

                        if (isNumberStart && !Regex.IsMatch(Encoding.UTF8.GetString(bytes), _regex))
                            isNumberEnd = true;
                    }

                    var number = received.ToString();
                    Console.WriteLine(number);
                    _numbersList.Add(long.Parse(number));
                }
                catch (Exception ex)
                {
                    /*
                     * Если хост разрывает соединение, то реконектимся и снова запрашиваем ту же команду, которая не успела обработаться.
                     * 
                     * System.IO.IOException -> Ошибка получения данных из-за пренудительного разрыва соединения.
                     * 
                     * Остальные исключения выбрасываем, т.к. это, скорее всего, вызвано ошибкой в написании кода.
                     */
                    if (ex is SocketException || ex is System.IO.IOException)
                    {
                        Console.WriteLine(ex.Message);
                        //   i--;
                        _tcpClient.Close();
                        _isConnected = false;
                        Connect();
                    }
                    else
                        throw;
                }
            }

            //Диспоузим объект после отработки всех запросов
            _tcpClient.Close();

            /*
             * Сортируем полученный список, пока другие потоки получают данные.
             * 
             * В двнном случае получение данных намного дольше сортировке, поэтому я думаю, что это имеет смысл
             */
            _numbersList = Sorter.QuickSort(_numbersList);
        }

        private void Connect()
        {
            while (!_isConnected)
            {
                try
                {
                    _tcpClient = new TcpClient(_adress, _port);
                    _isConnected = true;
                }
                catch (SocketException ex)
                {
                    Console.WriteLine(ex.Message);
                    _tcpClient.Close();
                }
            }
        }
    }
}
