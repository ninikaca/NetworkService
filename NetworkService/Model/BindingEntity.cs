using NetworkService.Helpers;
using System.ComponentModel;

namespace NetworkService.Model
{
    public class BindingEntity : BindableBase
    {
        public string Addresses { get; set; }
        public BindingList<Entity> listOfEntities { get; set; }

        public BindingEntity()
        {
            listOfEntities = new BindingList<Entity>();
        }
    }
}
