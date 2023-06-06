using MVVMLight.Messaging;
using NetworkService.Helpers;
using NetworkService.Model;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;

namespace NetworkService.ViewModel
{
    public class MainWindowViewModel : BindableBase
    {
        // za menjannje menija
        public MyICommand<string> NavCommand { get; private set; }
        
        //view model instance
        public HomeViewModel homeViewModel;
        public EntitiesViewModel entitiesViewModel;
        public GraphsViewModel graphsViewModel;
        public static ViewViewModel viewViewModel;

        //trenutni view model
        private BindableBase currentViewModel;

        //lista mreznih entiteta
        public static ObservableCollection<Entity> Entities { get; set; }

        //informacija / poruka
        private string mess;

        public MainWindowViewModel()
        {
            createListener(); //Povezivanje sa serverskom aplikacijom

            // komanda za navigaciju
            NavCommand = new MyICommand<string>(OnNav);
            CloseWindowCommand = new MyICommand<Window>(CloseWindow);

            Entities = new ObservableCollection<Entity>();

            // sada ovde je potrebno i kreirate objekte views
            homeViewModel = new HomeViewModel();
            entitiesViewModel = new EntitiesViewModel();
            graphsViewModel = new GraphsViewModel();
            viewViewModel = new ViewViewModel();

            Messenger.Default.Register<Entity>(this, AddToList);
            Messenger.Default.Register<int>(this, RemoveFromList);
            Messenger.Default.Register<ObservableCollection<Entity>>(this, GetList);

            //testiranje kroz 15 primera
            for (int i = 0; i < 3; i++)
            {
                entitiesViewModel.OnAdd();
            }

            entitiesViewModel.SelectedIndexOfAddedEntity = 0;
            viewViewModel = new ViewViewModel();

            // brisanje starog log-a ako postoji
            if (File.Exists("log.txt"))
            {
                File.Delete("log.txt");
            }

            viewViewModel.UpdateMeasurement();

            entitiesViewModel.Information = Visibility.Visible;
            entitiesViewModel.Mess = "The program is ready. You can start using the application to work with network entities";

            graphsViewModel.Information = Visibility.Visible;
            graphsViewModel.Mess = "The program is ready. You can start using the application to work with network entities";
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

        //tcp konekcija
        private void createListener()
        {
            var tcp = new TcpListener(IPAddress.Any, 27567);
            tcp.Start();

            var listeningThread = new Thread(() =>
            {
                while (true)
                {
                    var tcpClient = tcp.AcceptTcpClient();
                    ThreadPool.QueueUserWorkItem(param =>
                    {
                        //Prijem poruke
                        NetworkStream stream = tcpClient.GetStream();
                        string incomming;
                        byte[] bytes = new byte[1024];
                        int i = stream.Read(bytes, 0, bytes.Length);

                        //Primljena poruka je sacuvana u incomming stringu
                        incomming = System.Text.Encoding.ASCII.GetString(bytes, 0, i);

                        //Ukoliko je primljena poruka pitanje koliko objekata ima u sistemu -> odgovor
                        if (incomming.Equals("Need object count"))
                        {
                            //Response
                            /* Umesto sto se ovde salje count.ToString(), potrebno je poslati 
                             * duzinu liste koja sadrzi sve objekte pod monitoringom, odnosno
                             * njihov ukupan broj (NE BROJATI OD NULE, VEC POSLATI UKUPAN BROJ)
                             * */
                            Byte[] data = System.Text.Encoding.ASCII.GetBytes(Entities.Count.ToString());
                            stream.Write(data, 0, data.Length);
                        }
                        else
                        {
                            //U suprotnom, server je poslao promenu stanja nekog objekta u sistemu
                            Console.WriteLine(incomming); //Na primer: "Entitet_1:272"

                            //################ IMPLEMENTACIJA ####################
                            // Obraditi poruku kako bi se dobile informacije o izmeni
                            // Azuriranje potrebnih stvari u aplikaciji

                            //dolazna poruka
                            int len = incomming.Length;

                            //preskakanje entiteta
                            string substring = incomming.Substring(8, len - 8);


                            string[] split = substring.Split(':');

                            //id od entiteta koji menjamo
                            int id = int.Parse(split[0]);

                            //zauzece koje se menja
                            int occupied = int.Parse(split[1]); 

                            Entity change = Entities.FirstOrDefault(p => p.Id == id + 1);

                            //obrisan objekat i simulator se jos nije restartovao tako da moramo odbaciti
                            if (change != null)
                            {
                                int old_one = change.Occupied;
                                change.Occupied = occupied;

                                //ispis nove poruke
                                entitiesViewModel.Information = Visibility.Visible;
                                entitiesViewModel.Mess = "⛔ Entity -> | " + change.Id + " | " + change.Name + " | " + change.IpAddress + " | has changed value of occupations" + old_one + " -> " + change + "!";
                            }

                            // upis merenja u txt fajl
                            string write_to_txt = ((id + 1) + "-" + occupied).ToString() + "-" + DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                            File.AppendAllText("log.txt", write_to_txt + "\n");
                            Delimit_File(write_to_txt);
                        }
                    }, null);
                }
            });

            listeningThread.IsBackground = true;
            listeningThread.Start();
        }

        // za promenu views
        public BindableBase CurrentViewModel
        {
            get 
            { 
                return currentViewModel; 
            }

            set
            {
                SetProperty(ref currentViewModel, value);
            }
        }

        private void OnNav(string destination)
        {
            switch (destination)
            {
                case "home":
                    CurrentViewModel = homeViewModel;
                    break;
                case "entities":
                    CurrentViewModel = entitiesViewModel;
                    break;
                case "view":
                    CurrentViewModel = viewViewModel;
                    break;
                case "graphs":
                    CurrentViewModel = graphsViewModel;
                    break;
            }
        }

        private void AddToList(Entity newEnt)
        {
            Entity.Add(newEnt);

            //da zaobidjemo beskonacni poziv
            Messenger.Default.Send(new PassForwardDummy() { Entity = newEnt });
        }

        private void RemoveFromList(int index)
        {
            int idx = Entiteti[index].Canvas_pozicija;
            Entiteti.RemoveAt(index);

            if (idx != -1)
            {
                Messenger.Default.Send(idx);
            }
        }

        private void GetList(ObservableCollection<Entitet> e)
        {
            e = Entiteti;
        }

        public void OnAdd()
        {
            OdabraniEntitet = null;
            ListaEntiteta = MainWindowViewModel.Entiteti;

            // CG1 - Na osnovu odabrane adresne klase kreira random entitet

            // novi id je trenutni najveci id + 1
            int max_id = ListaEntiteta.Count != 0 ? ListaEntiteta.Max(x => x.Id) + 1 : 1;

            int odabrana_adresna_klasa = OdabraniIndeksDodavanjeEntiteta;
            const int ip_min = 0, ip_max = 255;
            int ip_prvi_oktet, ip_drugi_oktet, ip_treci_oktet, ip_cetvrti_oktet;
            string ip, klasa;

            // generisanje na osnovu odabrane adresne klase
            switch (odabrana_adresna_klasa)
            {
                case 0: ip_prvi_oktet = new Random().Next(1, 127); klasa = "A"; break;
                case 1: ip_prvi_oktet = new Random().Next(128, 191); klasa = "B"; break;
                case 2: ip_prvi_oktet = new Random().Next(192, 223); klasa = "C"; break;
                case 3: ip_prvi_oktet = new Random().Next(224, 239); klasa = "D"; break;
                case 4: ip_prvi_oktet = new Random().Next(240, 255); klasa = "E"; break;
                default: ip_prvi_oktet = 0; klasa = "A"; break;
            }

            ip_drugi_oktet = new Random().Next(ip_min, ip_max);
            Thread.Sleep(50);
            ip_treci_oktet = new Random().Next(ip_min, ip_max);
            Thread.Sleep(50);
            ip_cetvrti_oktet = new Random().Next(ip_min, ip_max);

            ip = ip_prvi_oktet + "." + ip_drugi_oktet + "." + ip_treci_oktet + "." + ip_cetvrti_oktet;
            string naziv = "Entitet " + (max_id < 10 ? ("0" + max_id).ToString() : max_id.ToString());

            Messenger.Default.Send(
                new Entitet()
                {
                    Id = max_id,
                    Naziv = naziv,
                    IP = ip,
                    Slika = "/Assets/uredjaj.png",
                    Zauzece = new Random().Next(0, 100),
                    Klasa = klasa,
                    Canvas_pozicija = -1,
                    //Povezan_sa_entitet_id = -1
                });

            // informativna poruka
            Uspesno = Visibility.Visible;
            Poruka = "ℹ Novi entitet (" + max_id + ", " + naziv + ", " + ip + ") je uspešno dodat u infrastrukturni sistem!";
        }

        public void OnBrisanjePress()
        {
            int id = OdabraniEntitet.Id;
            string naziv = OdabraniEntitet.Naziv;
            string ip = OdabraniEntitet.IP;

            Messenger.Default.Send(ListaEntiteta.IndexOf(OdabraniEntitet));
            OdabraniEntitet = null;
            Messenger.Default.Send(ListaEntiteta);

            // poruka korisniku
            Greska = Visibility.Visible;
            Poruka = "❎ Entitet (" + id + ", " + naziv + ", " + ip + ") je uspešno izbrisan iz infrastrukturnog sistema!";
        }
    }
}
