using NetworkService.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkService.ViewModel
{
    public class MainWindowViewModel : BindableBase
    {
        #region NAVIGACIONE KOMANDE
        public MyICommand<string> NavCommand { get; private set; } // za menjannje menija
        #endregion

        #region VIEW MODEL INSTANCE
        public HomeViewModel homeViewModel;
        public EntitiesViewModel entitiesViewModel;
        public GraphsViewModel graphsViewModel;
        public static ViewViewModel viewViewModel;
        #endregion

        #region TRENUTNO PRIKAZAN VIEW MODEL
        private BindableBase currentViewModel;
        #endregion

        #region LISTA I BROJ MREZNIH ENTITETA
        // ovo je lista gde ces cuvati sve te svoje entitete, kada u model napravis tu klasu koja modeluje
        // ovde uncomment i dalje idi. daniel
       // public static ObservableCollection<TvojaKlasa> TvojaKlasa { get; set; }
        #endregion

        private int count = 15; // Inicijalna vrednost broja objekata u sistemu
                                // ######### ZAMENITI stvarnim brojem elemenata
                                //           zavisno od broja entiteta u listi

        public MainWindowViewModel()
        {
            createListener(); //Povezivanje sa serverskom aplikacijom

            // komanda za navigaciju
            NavCommand = new MyICommand<string>(OnNav);

            // sada ovde je potrebno i kreirate objekte views
            homeViewModel = new HomeViewModel();
            entitiesViewModel = new EntitiesViewModel();
            graphsViewModel = new GraphsViewModel();
            viewViewModel = new ViewViewModel();

            // trebace ti kasnije kada iz EntitiesViewModel saljes nove entitete kako bi dodala u ovu listu gore TvojaKlasa
            // Messenger.Default.Register<Entitet>(this, DodajUListu);
            // Messenger.Default.Register<int>(this, UkloniIzListe);
        }

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
                            Byte[] data = System.Text.Encoding.ASCII.GetBytes(count.ToString());
                            stream.Write(data, 0, data.Length);
                        }
                        else
                        {
                            //U suprotnom, server je poslao promenu stanja nekog objekta u sistemu
                            Console.WriteLine(incomming); //Na primer: "Entitet_1:272"

                            //################ IMPLEMENTACIJA ####################
                            // Obraditi poruku kako bi se dobile informacije o izmeni
                            // Azuriranje potrebnih stvari u aplikaciji

                            // ovde doradi

                        }
                    }, null);
                }
            });

            listeningThread.IsBackground = true;
            listeningThread.Start();
        }

        // ovo ti sluzi da menjas views
        public BindableBase CurrentViewModel
        {
            get { return currentViewModel; }
            set
            {
                SetProperty(ref currentViewModel, value);
            }
        }

        // pazi ovde na case, tako ces i u meniju kao command parameter isto ovako slati home ili entities, mora biti isto! daniel
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
    }
}
