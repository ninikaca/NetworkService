using MVVMLight.Messaging;
using NetworkService.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NetworkService.Model
{
    public class Entity : ValidationBase
    {
        //polja klase
        private int id;
        private int name;
        private int address;
        private int occupied;
        private int position_on_canvas;

        private string scope;
        private string picture;

        private bool color;

        //prazan konstruktor
        public Entity()
        {

        }

        public int Id
        {
            get
            {
                return id;
            }

            set
            {
                if(id != value)
                {
                    id = value;
                    OnPropertyChanged("Id");
                }
            }
        }

        public int Name
        {
            get
            {
                return name;
            }

            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        public int IpAddress
        {
            get
            {
                return address;
            }

            set
            {
                if (address != value)
                {
                    address = value;
                    OnPropertyChanged("IpAddress");
                }
            }
        }

        public int Occupied
        {
            get
            {
                return occupied;
            }

            set
            {
                if (occupied != value)
                {
                    occupied = value;
                    OnPropertyChanged("Occupied");
                }

                if (occupied < 45 || occupied > 75)
                {
                    Color = true;
                    Picture = "/Assets/uredjaj_error.png";

                    // ukoliko je vec na canvasu ispisuje se poruka
                    if (PositionOnCanvas != -1)
                    {
                        MessageChange message = new MessageChange()
                        {
                            Visibility_Success = Visibility.Hidden,
                            Visibility_Error = Visibility.Visible,
                            Mess = "🛑 Entity | " + Id + " | " + Name + " | " + IpAddress + " | has critical value of " + Occupied + "%!"
                        };

                        Messenger.Default.Send(message);
                    }
                }
                else
                {
                    Color = false;
                    Picture = "/Assets/uredjaj.png";

                    // ukoliko je vec na canvasu ispisuje se poruka
                    if (PositionOnCanvas != -1)
                    {
                        MessageChange message = new MessageChange()
                        {
                            Visibility_Success = Visibility.Visible,
                            Visibility_Error = Visibility.Hidden,
                            Mess = "✔ Entity | " + Id + " | " + Name + " | " + IpAddress + " | has critical value of " + Occupied + "%!"
                        };

                        Messenger.Default.Send(message);
                    }
                }

                OnPropertyChanged("Color");
                OnPropertyChanged("Picture");
            }
        }

        public string Picture
        {
            get
            {
                return picture;
            }

            set
            {
                if (picture != value)
                {
                    picture = value;
                    OnPropertyChanged("Picture");
                }
            }
        }

        public string Scope
        {
            get
            {
                return scope;
            }

            set
            {
                if (scope != value)
                {
                    scope = value;
                    OnPropertyChanged("Scope");
                }
            }
        }

        public int PositionOnCanvas
        {
            get
            {
                return position_on_canvas;
            }

            set
            {
                if (position_on_canvas != value)
                {
                    position_on_canvas = value;
                    OnPropertyChanged("PositionOnCanvas");
                }
            }
        }

        public bool Color
        {
            get
            {
                return color;
            }

            set
            {
                if (color != value)
                {
                    color = value;
                    OnPropertyChanged("Color");
                }
            }
        }

        //ispis entiteta
        public override string ToString()
        {
            return "Entity " + Id + ": " + Name + " -> " + IpAddress;
        }
    }
}
