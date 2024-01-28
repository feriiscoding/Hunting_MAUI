using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hunting.ViewModel
{
    public class HuntingField : ViewModelBase
    {
        private bool isHunter;
        private bool isPrey;
        private bool isEmpty;

        public bool IsHunter
        {
            get { return isHunter; }
            set
            {
                isHunter = value;
                OnPropertyChanged();
            }
        }

        public bool IsPrey
        {
            get { return isPrey; }
            set
            {
                isPrey = value;
                OnPropertyChanged();
            }
        }


        public bool IsEmpty
        {
            get { return isEmpty; }
            set
            {
                isEmpty = value;
                OnPropertyChanged();
            }
        }

        public int X { get; set; }

        public int Y { get; set; }
        public DelegateCommand? ImageClickCommand { get; set; }
        //public DelegateCommand? StepCommand { get; set; }
    }
}
