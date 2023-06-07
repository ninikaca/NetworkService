using NetworkService.Helpers;
using NetworkService.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.ViewModel
{
    public class EntitiesViewModel : BindableBase
    {
        //filtriranje
        public MyICommand FiltrirajKomanda { get; set; }

        //dodavanje
        public MyICommand DodajKomanda { get; set; }

        //brisanje
        public MyICommand ObrisiKomanda { get; set; }

        //sva potrebna polja
        private enum CLASSES { ONE, TWO, THREE, FOUR, FIVE };
        private int chosenClassesAddresses;
        private static readonly ObservableCollection<string> Addresses = new ObservableCollection<string> { "Address scope 1", "Address scope 2", "Address scope 3", "Address scope 4", "Address scope 5" };

        private int ChosenID;
        private bool LessIsChecked;
        private bool MoreIsChecked;
        private bool EqualsIsChecked;

        private int odabraniIndeksIstorijeFiltera;
        private int odabraniIndeksDodavanjeEntiteta;
        private bool deleteIsEnabled;

        private Visibility succesfull, error, information;
        private string mess;

        private Entity chosenEntity;
        private Filter chosenFilter = new Filter();

        private static ObservableCollection<Entity> listOfEntities { get; set; }
        public static ObservableCollection<Filter> historyOfFilter { get; set; }
    }
}
