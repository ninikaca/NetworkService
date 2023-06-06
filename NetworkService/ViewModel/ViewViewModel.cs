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
    public class ViewViewModel : BindableBase
    {
        public static ObservableCollection<Entity> Entities { get; set; }

        private Entities chosenEntity;

        private int chosenId;

        Measurement measurement1, measurement2, measurement3;
    }
}
