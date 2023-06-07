using System.ComponentModel;

namespace NetworkService.Model
{
    public class BindingEntity
    {
        public string Addresses { get; set; }
        public BindingList<Entity> listOfEntities { get; set; }

        public BindingEntity()
        {
            listOfEntities = new BindingList<Entity>();
        }
    }
}
