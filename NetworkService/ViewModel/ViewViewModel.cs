using NetworkService.Helpers;
using NetworkService.Model;
using System;
using System.Collections.ObjectModel;
using Assets.File;


namespace NetworkService.ViewModel
{
    public class ViewViewModel : BindableBase
    {
        //polja klase
        public static ObservableCollection<Entity> Entities { get; set; }
        private Entity chosenEntity;
        private int chosenId;
        Measurement measurement1, measurement2, measurement3, measurement4, measurement5;

        //konstruktor klase
        public ViewViewModel()
        {
            Entities = MainWindowViewModel.Entities;
            ChosenEntity = Entities[0];
            OnPropertyChanged("ChosenEntity");

            // poslednjih 5 merenja
            Measurement1 = new Measurement() { Measured = ChosenEntity.Occupied };
            Measurement2 = new Measurement() { Measured = 0, OutOfRange = true };
            Measurement3 = new Measurement() { Measured = 0, OutOfRange = true };
            Measurement4 = new Measurement() { Measured = 0, OutOfRange = true };
            Measurement5 = new Measurement() { Measured = 0, OutOfRange = true };
        }

        public Measurement Measurement1
        {
            get
            {
                return measurement1;
            }

            set
            {
                if (measurement1 != value)
                {
                    measurement1 = value;
                    OnPropertyChanged("Measurement1");
                }
            }
        }

        public Measurement Measurement2
        {
            get
            {
                return measurement2;
            }

            set
            {
                if (measurement2 != value)
                {
                    measurement2 = value;
                    OnPropertyChanged("Measurement2");
                }
            }
        }

        public Measurement Measurement3
        {
            get
            {
                return measurement3;
            }

            set
            {
                if (measurement3 != value)
                {
                    measurement3 = value;
                    OnPropertyChanged("Measurement3");
                }
            }
        }

        public Measurement Measurement4
        {
            get
            {
                return measurement4;
            }

            set
            {
                if (measurement4 != value)
                {
                    measurement4 = value;
                    OnPropertyChanged("Measurement4");
                }
            }
        }

        public Measurement Measurement5
        {
            get
            {
                return measurement5;
            }

            set
            {
                if (measurement5 != value)
                {
                    measurement5 = value;
                    OnPropertyChanged("Measurement5");
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
                }

                ChosenId = ChosenEntity.Id;
                OnPropertyChanged("ChosenId");
            }
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

                if (Measurement1 != null)
                {
                    Measurement1.Measured = 0;
                    Measurement2.Measured = 0;
                    Measurement3.Measured = 0;
                    Measurement4.Measured = 0;
                    Measurement5.Measured = 0;

                    UpdateMeasurement();

                    OnPropertyChanged("Measurement1");
                    OnPropertyChanged("Measurement2");
                    OnPropertyChanged("Measurement3");
                    OnPropertyChanged("Measurement4");
                    OnPropertyChanged("Measurement5");
                }
            }
        }

        public void UpdateMeasurement()
        {
            // citanje iz fajla na osnovu trenutnog id-ja dok se ne nadje merenje
            if (!File.Exists("log.txt"))
                return;

            string[] procitano = File.ReadAllLines("log.txt");
            Array.Reverse(procitano); // citamo unazad log datoteku
            int measured = 1;

            // simulira stek
            foreach (string red in procitano)
            {
                if (measured > 5)
                    measured = 0;

                string[] column = red.Split('-');

                if (int.Parse(column[0]) == ChosenId)
                {
                    int merenje_log = int.Parse(column[1]); // izmerena vrednost

                    switch (measured)
                    {
                        case 1: Measurement1.Measured = merenje_log; OnPropertyChanged("Measurement1"); break;
                        case 2: Measurement2.Measured = merenje_log; OnPropertyChanged("Measurement2"); break;
                        case 3: Measurement3.Measured = merenje_log; OnPropertyChanged("Measurement3"); break;
                        case 4: Measurement4.Measured = merenje_log; OnPropertyChanged("Measurement4"); break;
                        case 5: Measurement5.Measured = merenje_log; OnPropertyChanged("Measurement5"); break;
                        default: Measurement1.Measured = merenje_log; OnPropertyChanged("Measurement1"); break;
                    }

                    measured++;
                }
            }
        }
    }
}
