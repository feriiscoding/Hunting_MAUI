namespace Hunting.Persistence
{
    public class Player
    {
        private List<int[]> hunters;
        private List<int[]> prey;
        private List<int[]> empty;

        public List<int[]> Hunters { get { return hunters; } set { hunters = value; } }
        public List<int[]> Prey { get { return prey; } set { prey = value; } }
        public List<int[]> Empty { get { return empty; } set { empty = value; } }
        public Player(int size)
        {
            hunters = new List<int[]>();
            prey = new List<int[]>();
            empty = new List<int[]>();
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    int[] array = new int[2];
                    array[0] = i;
                    array[1] = j;
                    if (i == 0 && j == 0)
                    {
                        Hunters.Add(array);
                    }
                    else if (i == 0 && j == size - 1)
                    {
                        Hunters.Add(array);
                    }
                    else if (i == size - 1 && j == 0)
                    {
                        Hunters.Add(array);
                    }
                    else if (i == size - 1 && j == size - 1)
                    {
                        Hunters.Add(array);
                    }
                    else if (i == size / 2 && j == size / 2)
                    {
                        Prey.Add(array);
                    }
                    else
                    {
                        Empty.Add(array);
                    }
                }
            }
        }
    }
}
