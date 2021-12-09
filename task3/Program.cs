using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace task3
{
    public class Edge
    {
        public int Price;
        public int Direction;

        public Edge(int price, int direction)
        {
            this.Price = price;
            this.Direction = direction;
        }
    }
    
    class Program
    {
        static void Main(string[] args)
        {
            var input = new StreamReader("in.txt");

            var firstLine = input.ReadLine().Split();
            var totalRoomsCount = int.Parse(firstLine[0]);
            var totalDoorsCount = int.Parse(firstLine[1]);
            var initialRoom = int.Parse(firstLine[2]);
            var initialMoney = int.Parse(firstLine[3]);
            
            var roomToDoors = ParseRoomToDoors(totalRoomsCount, totalDoorsCount, input);

            var escapePrice = GetEscapePrice(roomToDoors, initialRoom);
            Console.WriteLine(escapePrice);
            WriteResult(initialMoney, escapePrice);
        }

        static void WriteResult(int initialMoney, int escapePrice)
        {
            var output = new StreamWriter("out.txt");
            if (escapePrice > initialMoney)
            {
                output.Write("N");
            }
            else
            {
                output.WriteLine("Y");
                output.Write(escapePrice);
            }
            
            output.Close();
        }
        
        static Dictionary<int, List<Edge>> ParseRoomToDoors(int totalRoomsCount, int totalDoorsCount,StreamReader input)
        {
            var doorToRooms = Enumerable
                .Range(1, totalDoorsCount)
                .ToDictionary(doorId => doorId, _ => new List<int>());
            for (int roomIndex = 1; roomIndex <= totalRoomsCount; roomIndex++)
            {
                var line = input.ReadLine().Split();
                var roomsCount = int.Parse(line[0]);
                
                for (int j = 0; j < roomsCount; j++)
                {
                    var doorId = int.Parse(line[j + 1]);
                    if (doorToRooms.ContainsKey(doorId))
                    {
                        doorToRooms[doorId].Add(roomIndex);
                    }
                    else
                    {
                        doorToRooms.Add(doorId, new List<int> {roomIndex});
                    }
                }
            }

            var prices = input.ReadToEnd()
                .Replace("\n", " ")
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select((strPrice, edgeId) => (edgeId + 1, int.Parse(strPrice)))
                .ToDictionary(kv => kv.Item1, kv => kv.Item2);

            var roomToDoors = Enumerable
                .Range(1, totalRoomsCount)
                .ToDictionary(roomId => roomId, _ => new List<Edge>());
            roomToDoors.Add(-1, new List<Edge>());
            
            for (int doorId = 1; doorId <= totalDoorsCount; doorId++)
            {
                var price = prices[doorId];
                if (doorToRooms[doorId].Count == 1)
                {
                    var room = doorToRooms[doorId][0];
                    roomToDoors[room].Add(new Edge(price, -1));
                    roomToDoors[-1].Add(new Edge(price, room));
                }
                else
                {
                    var firstRoom = doorToRooms[doorId][0];
                    var secondRoom = doorToRooms[doorId][1];
                    roomToDoors[firstRoom].Add(new Edge(price, secondRoom));
                    roomToDoors[secondRoom].Add(new Edge(price, firstRoom));
                }
            }

            return roomToDoors;
        }

        static int GetEscapePrice(Dictionary<int, List<Edge>> roomToDoors, int initialRoom)
        {
            var minPrice = roomToDoors
                .ToDictionary(kv => kv.Key, kv => int.MaxValue);
            minPrice[initialRoom] = 0;
            
            var visited = new HashSet<int>();
            

            var currentRoom = initialRoom;
            while (currentRoom != -1)
            {
                currentRoom = minPrice
                    .Where(kv => !visited.Contains(kv.Key))
                    .Aggregate((curMin, x) => x.Value < curMin.Value ? x : curMin)
                    .Key;
                visited.Add(currentRoom);
                
                foreach (var attachedEdge in roomToDoors[currentRoom])
                {
                    var newPrice = minPrice[currentRoom] + attachedEdge.Price;
                    var currentPrice = minPrice[attachedEdge.Direction];
                    minPrice[attachedEdge.Direction] = Math.Min(newPrice, currentPrice);
                }
            }

            return minPrice[-1];
        }
    }
}