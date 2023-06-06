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
        private enum CLASSES { ONE, TWO, THREE };
        private int chosenClassesAddresses;
        private static readonly ObservableCollection<string> Addresses = new ObservableCollection<string> { "Address scope 1", "Address scope 2", "Address scope 3", "Address scope 4", "Address scope 5" };

        private int ChosenID;

        private bool LessIsChecked;
        private bool MoreIsChecked;
        private bool EqualsIsChecked;

        private Entity chosenEntity;

        private static ObservableCollection<Entity> listOfEntities { get; set; }
    }
}
