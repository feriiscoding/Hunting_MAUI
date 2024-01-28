using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hunting.Persistence
{
    public interface IHuntingDataAccess
    {
        public int TurnCount { get; set; }
        public string? Who { get; set; }
        Task<Player> LoadAsync(String path);

        Task SaveAsync(String path, Player fields, int turns, string who, int tableSize);
    }
}
