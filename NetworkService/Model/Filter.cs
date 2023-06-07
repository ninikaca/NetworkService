using NetworkService.Helpers;
using NetworkService.ViewModel;

namespace NetworkService.Model
{
    public class Filter : BindableBase
    {
        //polja klase
        private int chosenId;
        private int indexInAddresses;
        private bool lessIsChecked;
        private bool moreIsChecked;
        private bool equalsIsChecked;

        //prazan konstruktor
        public Filter()
        {
           //prazan
        }

        public int ChosenId
        {
            get
            {
                return chosenId;
            }

            set
            {
                if (chosenId != value)
                {
                    chosenId = value;
                    OnPropertyChanged("ChosenId");
                }
            }
        }

        public int IndexInAddresses
        {
            get
            {
                return indexInAddresses;
            }

            set
            {
                if (indexInAddresses != value)
                {
                    indexInAddresses = value;
                    OnPropertyChanged("IndexInAddresses");
                }
            }
        }

        public bool LessIsChecked
        {
            get
            {
                return lessIsChecked;
            }

            set
            {
                if (lessIsChecked != value)
                {
                    lessIsChecked = value;
                    OnPropertyChanged("LessIsChecked");
                }
            }
        }

        public bool MoreIsChecked
        {
            get
            {
                return moreIsChecked;
            }

            set
            {
                if (moreIsChecked != value)
                {
                    moreIsChecked = value;
                    OnPropertyChanged("MoreIsChecked");
                }
            }
        }

        public bool EqualsIsChecked
        {
            get
            {
                return equalsIsChecked;
            }

            set
            {
                if (equalsIsChecked != value)
                {
                    equalsIsChecked = value;
                    OnPropertyChanged("EqualsIsChecked");
                }
            }
        }

        public override bool Equals(object obj)
        {
            return obj is Filter filter &&
                   IndexInAddresses == filter.IndexInAddresses &&
                   MoreIsChecked == filter.MoreIsChecked &&
                   LessIsChecked == filter.LessIsChecked &&
                   EqualsIsChecked == filter.EqualsIsChecked &&
                   ChosenId == filter.ChosenId;
        }

        public override string ToString()
        {
            string format = EntitiesViewModel.Address[IndexInAddresses] + " | ";

            if (LessIsChecked) format += "ID < " + ChosenId;
            if (MoreIsChecked) format += "ID > " + ChosenId;
            if (EqualsIsChecked) format += "ID = " + ChosenId;

            return format;
        }
    }
}
