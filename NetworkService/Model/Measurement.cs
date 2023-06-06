using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Model
{
    class Measurement
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
                    vanOpsega = true;
                    OnPropertyChanged("OutOfRange");
                }
                else
                {
                    vanOpsega = false;
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
