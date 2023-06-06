using NetworkService.Helpers;

namespace NetworkService.Model
{
    public class Measurement : BindableBase
    {
        //polja klase
        private int measured;
        private bool outOfRange;

        public Measurement()
        {
           
        }

        public int Measured
        {
            get
            {
                return measured;
            }

            set
            {
                if (measured != value)
                {
                    measured = value;
                    OnPropertyChanged("Measured");
                }

                if (measured < 45 || measured > 75)
                {
                    outOfRange = true;
                    OnPropertyChanged("OutOfRange");
                }
                else
                {
                    outOfRange = false;
                    OnPropertyChanged("OutOfRange");
                }
            }
        }

        public bool OutOfRange
        {
            get
            {
                return outOfRange;
            }

            set
            {
                if (outOfRange != value)
                {
                    outOfRange = value;
                    OnPropertyChanged("OutOfRange");
                }
            }
        }
    }
}
