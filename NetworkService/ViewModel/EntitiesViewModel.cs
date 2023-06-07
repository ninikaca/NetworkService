using MVVMLight.Messaging;
using NetworkService.Helpers;
using NetworkService.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows;

namespace NetworkService.ViewModel
{
    public class EntitiesViewModel : BindableBase
    {
        //filtriranje
        public MyICommand CommandFiltet { get; set; }

        //dodavanje
        public MyICommand CommandAdd { get; set; }

        //brisanje
        public MyICommand CommandDelete { get; set; }

        //sva potrebna polja
        private enum CLASSES { ONE, TWO, THREE, FOUR, FIVE };
        private int chosenClassesAddresses;
        private static readonly ObservableCollection<string> addresses = new ObservableCollection<string> { "Address scope 1", "Address scope 2", "Address scope 3", "Address scope 4", "Address scope 5" };

        private int currentID;
        private bool lessIsChecked;
        private bool moreIsChecked;
        private bool equalsIsChecked;

        private int chosenIdFromFilter_Add;
        private int chosenIdFromFilter_History;
        private bool deleteIsEnabled;

        private Visibility succesfull, error, information;
        private string mess;

        private Entity chosenEntity;
        private Filter chosenFilter = new Filter();

        private static ObservableCollection<Entity> listOfEntities { get; set; }
        public static ObservableCollection<Filter> historyOfFilter { get; set; }

        public EntitiesViewModel()
        {
            currentID = 0;
            chosenClassesAddresses = 0;
            lessIsChecked = true;
            moreIsChecked = false;
            equalsIsChecked = false;

            chosenIdFromFilter_Add = 0;
            chosenIdFromFilter_History = 0;
            DeleteIsEnabled = new MyICommand(CheckDelete);

            ChosenEntity = false;

            listOfEntities = MainWindowViewModel.Entities;
            HistoryOfFilter = new ObservableCollection<Filter>();

            CommandFiltet = new MyICommand(CheckFilter);
            CommandAdd = new MyICommand(CheckAdd);
            CommandDelete = new MyICommand(CheckDelete);

            Succesfull = Error = Information = Visibility.Hidden;
        }

        public static ObservableCollection<string> Address
        {
            get
            {
                return addresses;
            }
        }

        public int CurrentID
        {
            get
            {
                return currentID;
            }

            set
            {
                if (currentID != value)
                {
                    currentID = Math.Abs(value);
                    OnPropertyChanged("CurrentID");
                }
            }
        }
        public Entity ChosenEntity
        {
            get
            {
                return chosenEntity;
            }

            set
            {
                if (chosenEntity != value)
                {
                    chosenEntity = value;
                    OnPropertyChanged("ChosenEntity");
                    OnPropertyChanged("DeleteIsEnabled");
                }
            }
        }

        public int ChosenClassesAddresses
        {
            get
            {
                return chosenClassesAddresses;
            }

            set
            {
                if (chosenClassesAddresses != value)
                {
                    chosenClassesAddresses = value;
                    OnPropertyChanged("ChosenClassesAddresses");
                }
            }
        }

        public int ChosenIdFromFilter_Add
        {
            get
            {
                return chosenIdFromFilter_Add;
            }

            set
            {
                if (chosenIdFromFilter_Add != value)
                {
                    chosenIdFromFilter_Add = value;
                    OnPropertyChanged("ChosenIdFromFilter_Add");
                }
            }
        }

        public int ChosenIdFromFilter_History
        {
            get
            {
                return chosenIdFromFilter_History;
            }

            set
            {
                if (chosenIdFromFilter_History != value)
                {
                    chosenIdFromFilter_History = value;
                    OnPropertyChanged("ChosenIdFromFilter_History");
                }
            }
        }

        public bool DeleteIsEnabled
        {
            get
            {
                return ChosenEntity != null;
            }

            set
            {
                if (deleteIsEnabled != value)
                {
                    deleteIsEnabled = value;
                    OnPropertyChanged("DeleteIsEnabled");
                    OnPropertyChanged("Background");
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

                    if (lessIsChecked)
                    {
                        MoreIsChecked = false;
                        EqualsIsChecked = false;

                        OnPropertyChanged("MoreIsChecked");
                        OnPropertyChanged("EqualsIsChecked");
                    }

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

                    if (moreIsChecked)
                    {
                        LessIsChecked = false;
                        EqualsIsChecked = false;

                        OnPropertyChanged("LessIsChecked");
                        OnPropertyChanged("EqualsIsChecked");
                    }

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

                    if (equalsIsChecked)
                    {
                        MoreIsChecked = false;
                        LessIsChecked = false;

                        OnPropertyChanged("LessIsChecked");
                        OnPropertyChanged("MoreIsChecked");
                    }

                    OnPropertyChanged("EqualsIsChecked");
                }
            }
        }

        public string Mess
        {
            get
            {
                return mess;
            }

            set
            {
                if (mess != value)
                {
                    mess = value;
                    OnPropertyChanged("Mess");
                }
            }
        }

        public Visibility Succesfull
        {
            get
            {
                return succesfull;
            }

            set
            {
                if (succesfull != value)
                {
                    succesfull = value;

                    if (succesfull == Visibility.Visible)
                    {
                        Error = Information = Visibility.Hidden;
                        OnPropertyChanged("Error");
                        OnPropertyChanged("Information");
                    }

                    OnPropertyChanged("Succesfull");
                }
            }
        }

        public Visibility Error
        {
            get
            {
                return error;
            }

            set
            {
                if (error != value)
                {
                    error = value;

                    if (error == Visibility.Visible)
                    {
                        Succesfull = Information = Visibility.Hidden;
                        OnPropertyChanged("Succesfull");
                        OnPropertyChanged("Information");
                    }

                    OnPropertyChanged("Error");
                }
            }
        }

        public Visibility Information
        {
            get
            {
                return information;
            }

            set
            {
                if (information != value)
                {
                    information = value;

                    if (information == Visibility.Visible)
                    {
                        Error = succesfull = Visibility.Hidden;
                        OnPropertyChanged("Error");
                        OnPropertyChanged("Succesfull");
                    }

                    OnPropertyChanged("Information");
                }
            }
        }

        //brisanje entiteta
        public void CheckDelete()
        {
            int id = ChosenEntity.Id;
            string name = ChosenEntity.Name;
            string address = ChosenEntity.IpAddress;

            Messenger.Default.Send(listOfEntities.IndexOf(ChosenEntity));
            ChosenEntity = null;
            Messenger.Default.Send(listOfEntities);

            // poruka korisniku
            Error = Visibility.Visible;
            Mess = "⛔ Entity -> |" + id + " | " + name + " | " + address + "| was deleted!";
        }

        //dodavanje entiteta


        public Filter ChosenFilter
        {
            get
            {
                return chosenFilter;
            }

            set
            {
                if (chosenFilter != value)
                {
                    chosenFilter = value;

                    // primeniti filter na polja
                    ChosenClassesAddresses = chosenFilter.IndexInAddresses;
                    LessIsChecked = chosenFilter.LessIsChecked;
                    MoreIsChecked = chosenFilter.MoreIsChecked;
                    EqualsIsChecked = chosenFilter.EqualsIsChecked;
                    CurrentID = chosenFilter.ChosenId;

                    OnPropertyChanged("ChosenFilter");
                    OnPropertyChanged("ChosenClassesAddresses");
                    OnPropertyChanged("MoreIsChecked");
                    OnPropertyChanged("LessIsChecked");
                    OnPropertyChanged("EqualsIsChecked");
                    OnPropertyChanged("ChosenId");

                    // primeni filter
                    CheckFilter();
                }
            }
        }
    }
}
