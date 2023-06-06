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
            Entities.Add(newEnt);

            //da zaobidjemo beskonacni poziv
            Messenger.Default.Send(new Forwarder() { Entity = newEnt });
        }

        private void RemoveFromList(int index)
        {
            int ind = Entities[index].PositionOnCanvas;
            Entities.RemoveAt(index);

            if (ind != -1)
            {
                Messenger.Default.Send(ind);
            }
        }

        private void GetList(ObservableCollection<Entity> e)
        {
            e = Entities;
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

        void Delimit_File(string str)
        {
            HomeViewModel.DELIMITER_CONST.Add(str);
            int old_id = graphsViewModel.ChosenId;
            graphsViewModel.ChosenId = 1;
            graphsViewModel.ChosenId = old_id;
        }

        private void CloseWindow(Window MainWindow)
        {
            MainWindow.Close();
        }

    }
}
