using System.ComponentModel;

namespace NetworkService.Model
{
    class BindingEntity
    {
        public string addresses { get; set; }
        public BindingList<Entity> listOfEntities { get; set; }

        public BindingEntity()
        {
            listOfEntities = new BindingList<Entity>();
        }
    }
}
