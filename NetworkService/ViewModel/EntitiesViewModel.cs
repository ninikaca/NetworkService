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

        private int chosenId_Add;
        private int chosenIdFromFilter_History;
        private bool deleteIsEnabled;

        private Visibility succesfull, error, information;
        private string mess;

        private Entity chosenEntity;
        private Filter chosenFilter = new Filter();

        private static ObservableCollection<Entity> listOfEntities { get; set; }
        public static ObservableCollection<Filter> HistoryOfFilter { get; set; }

        public EntitiesViewModel()
        {
            currentID = 0;
            chosenClassesAddresses = 0;
            lessIsChecked = true;
            moreIsChecked = false;
            equalsIsChecked = false;

            chosenId_Add = 0;
            chosenIdFromFilter_History = 0;
            ChosenEntity = null;

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

        public ObservableCollection<Entity> ListOfEntities
        {
            get
            {
                return listOfEntities;
            }

            set
            {
                if (listOfEntities != null)
                {
                    listOfEntities = value;
                    OnPropertyChanged("ListOfEntities");
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

        public int ChosenId_Add
        {
            get
            {
                return chosenId_Add;
            }

            set
            {
                if (chosenId_Add != value)
                {
                    chosenId_Add = value;
                    OnPropertyChanged("ChosenId_Add");
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
        public void CheckAdd()
        {
            ChosenEntity = null;
            ListOfEntities = MainWindowViewModel.Entities;

            //Na osnovu odabrane adresne klase kreirati random entitet

            // novi id je trenutni najveci id + 1
            int max_id = ListOfEntities.Count != 0 ? ListOfEntities.Max(x => x.Id) + 1 : 1;

            int chosenAddressScope = chosenId_Add;
            const int ip_min = 0, ip_max = 255;
            int scope1, scope2, scope3, scope4;
            string address, classes;

            // generisanje na osnovu odabrane adresne klase
            switch (chosenAddressScope)
            {
                case 0: scope1 = new Random().Next(1, 127); classes = "ONE"; break;
                case 1: scope1 = new Random().Next(128, 191); classes = "TWO"; break;
                case 2: scope1 = new Random().Next(192, 223); classes = "THREE"; break;
                case 3: scope1 = new Random().Next(224, 239); classes = "FOUR"; break;
                case 4: scope1 = new Random().Next(240, 255); classes = "FIVE"; break;
                default: scope1 = 0; classes = "ONE"; break;
            }

            scope2 = new Random().Next(ip_min, ip_max);
            Thread.Sleep(50);

            scope3 = new Random().Next(ip_min, ip_max);
            Thread.Sleep(50);

            scope4 = new Random().Next(ip_min, ip_max);

            address = scope1 + "." + scope2 + "." + scope3 + "." + scope4;
            string name = "Entity " + (max_id < 10 ? ("0" + max_id).ToString() : max_id.ToString());

            Messenger.Default.Send(
                new Entity()
                {
                    Id = max_id,
                    Name = name,
                    IpAddress = address,
                    Picture = "/Assets/uredjaj.png",
                    Occupied = new Random().Next(0, 100),
                    Scope = classes,
                    PositionOnCanvas = -1,
                    //povezan sa entitetom = -1
                });

            // informativna poruka
            Succesfull = Visibility.Visible;
            Mess = "⛔ Entity -> |" + max_id + " | " + name + " | " + address + "| was added!";
        }

        //filtriranje
        public void CheckFilter()
        {
            // Sacuvaj u istoriju filtera
            Filter previous = new Filter
            {
                IndexInAddresses = chosenClassesAddresses,
                LessIsChecked = LessIsChecked,
                MoreIsChecked = MoreIsChecked,
                EqualsIsChecked = EqualsIsChecked,
                ChosenId = CurrentID
            };

            // Ako filter vec postoji u listi filtera - ne dodaje ste
            bool wasUsedBefore = false;
            foreach (Filter tmp in HistoryOfFilter)
            {
                if (tmp.Equals(previous))
                {
                    wasUsedBefore = true;
                    break;
                }
            }

            if (!wasUsedBefore)
            {
                HistoryOfFilter.Add(previous);
            }

            // Primena filtera
            listOfEntities = FilterEntities();

            // poruka korisniku
            Information = Visibility.Visible;
            Mess = "Filter by |" + previous + "| is done generating, elements are listed!";
        }

        ObservableCollection<Entity> FilterEntities()
        {
            ObservableCollection<Entity> filteredElntities = new ObservableCollection<Entity>();
            ObservableCollection<Entity> allEntities = MainWindowViewModel.Entities;

            // Nije odabran nijedan entitet
            ChosenEntity = null;

            if (allEntities != null && allEntities.Count > 0)
            {
                foreach (Entity ent in allEntities)
                {
                    int klasa = GetADdressScope(ent.IpAddress);

                    // Pripada odabranoj klasi
                    if (chosenClassesAddresses == klasa)
                    {
                        // Proveri vece, manje, jednako
                        if (SortByEqual(ent))
                        {
                            filteredElntities.Add(ent);
                        }
                    }
                }
            }

            return filteredElntities;
        }

        int GetADdressScope(string address)
        {
            int inScope = 0;
            int scope = int.Parse(address.Split('.')[0]);

            if (scope >= 1 && scope <= 127) inScope = 0;
            if (scope >= 128 && scope <= 191) inScope = 1;
            if (scope >= 192 && scope <= 223) inScope = 2;
            if (scope >= 224 && scope <= 239) inScope = 3;
            if (scope >= 240 && scope <= 255) inScope = 4;

            return inScope;
        }

        bool SortByEqual(Entity current)
        {
            if (LessIsChecked && (current.Id < CurrentID))
            {
                return true;
            }
            else if (MoreIsChecked && (current.Id > CurrentID))
            {
                return true;
            }
            else if (EqualsIsChecked && (current.Id == CurrentID))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

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
