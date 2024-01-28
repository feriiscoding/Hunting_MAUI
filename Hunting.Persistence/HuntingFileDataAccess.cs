using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hunting.Persistence
{
    public class HuntingFileDataAccess : IHuntingDataAccess
    {
        private String? _directory = String.Empty;
        private int _turnCount;
        private string? _who;
        public int TurnCount { get { return _turnCount; } set { _turnCount = value; } }
        public string? Who { get { return _who; } set { _who = value; } }
        public HuntingFileDataAccess(String? saveDirectory = null)
        {
            _directory = saveDirectory;
        }
        public async Task<Player> LoadAsync(String path)
        {
            if (!String.IsNullOrEmpty(_directory))
                path = Path.Combine(_directory, path);

            try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    String[] fields = (await reader.ReadLineAsync())!.Split(" ");
                    int size = int.Parse(fields[0]);
                    Player player = new Player(size);
                    player.Hunters = new List<int[]>();
                    player.Prey = new List<int[]>();
                    player.Empty = new List<int[]>();
                    fields = (await reader.ReadLineAsync())!.Split(" ");
                    for (int i = 0; i < fields.Length - 1; i++)
                    {
                        int[] arr = new int[2];
                        String[] coordinates = fields[i].Split(",");
                        arr[0] = int.Parse(coordinates[0]);
                        arr[1] = int.Parse(coordinates[1]);
                        player.Hunters.Add(arr);
                    }
                    fields = (await reader.ReadLineAsync())!.Split(" ");


                    for (int i = 0; i < fields.Length - 1; i++)
                    {
                        int[] arr = new int[2];
                        String[] coordinates = fields[i].Split(",");
                        arr[0] = int.Parse(coordinates[0]);
                        arr[1] = int.Parse(coordinates[1]);
                        player.Prey.Add(arr);
                    }
                    fields = (await reader.ReadLineAsync())!.Split(" ");


                    for (int i = 0; i < fields.Length - 1; i++)
                    {
                        int[] arr = new int[2];
                        String[] coordinates = fields[i].Split(",");
                        arr[0] = int.Parse(coordinates[0]);
                        arr[1] = int.Parse(coordinates[1]);
                        player.Empty.Add(arr);
                    }

                    string[] datas = (await reader.ReadLineAsync())!.Split(",");
                    _turnCount = int.Parse(datas[0]);
                    _who = datas[1];
                    return player;
                }
            }
            catch
            {
                throw new DataException("Error occurred during reading.");
            }
        }
        public async Task SaveAsync(String path, Player fields, int turnCount, string who, int tableSize)
        {
            if (!String.IsNullOrEmpty(_directory))
                path = Path.Combine(_directory, path);

            try
            {
                using (StreamWriter writer = new StreamWriter(path))
                {
                    await writer.WriteLineAsync($"{tableSize}");
                    for (int i = 0; i < fields.Hunters.Count; i++)
                    {
                        await writer.WriteAsync(fields.Hunters[i][0] + "," + fields.Hunters[i][1] + " ");
                    }
                    await writer.WriteLineAsync();
                    for (int i = 0; i < fields.Prey.Count; i++)
                    {
                        await writer.WriteAsync(fields.Prey[i][0] + "," + fields.Prey[i][1] + " ");
                    }
                    await writer.WriteLineAsync();
                    for (int i = 0; i < fields.Empty.Count; i++)
                    {
                        await writer.WriteAsync(fields.Empty[i][0] + "," + fields.Empty[i][1] + " ");
                    }
                    await writer.WriteLineAsync();
                    await writer.WriteAsync(turnCount + "," + who);
                }
            }
            catch
            {
                throw new DataException("Error occurred during writing.");
            }
        }
    }
}
