using NetworkService.Helpers;
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
        private static readonly ObservableCollection<string> Addresses = new ObservableCollection<string> { "Address scope 1", "Address scope 2", "Address scope 3"};
    }
}
