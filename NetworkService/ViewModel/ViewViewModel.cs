using System;
using System.IO;
using System.Linq;
using System.Windows;
using MVVMLight.Messaging;
using NetworkService.Model;
using NetworkService.Views;
using System.Windows.Media;
using System.ComponentModel;
using NetworkService.Helpers;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace NetworkService.ViewModel
{
    public class ViewViewModel : BindableBase
    {
        //drag & drop
        public MyICommand<Canvas> CommandDragOver { get; private set; }
        public MyICommand<Canvas> CommandDrop { get; private set; }
        public MyICommand<Canvas> CommandRemoveFromGrid { get; private set; }
        public MyICommand<Grid> CommandAddAllToGrid { get; private set; }
        public MyICommand<TreeView> ChosenTreeView { get; private set; }
        public MyICommand ClickLeftMouse { get; private set; }

        //drag & drop trenutni
        private Entity draggedItem = null;
        private bool dragging = false;
        private int selected;
        private Canvas start = null;

        public static Grid right;

        private Visibility succesfull, error, information;
        private string mess;

        //binding elementi
        public static BindingList<BindingEntity> canvasBindingList { get; set; }
        public static BindingList<BindingEntity> treeViewBindingList { get; set; }

        //drag & drop na samom canvasu medjusobno
        public MyICommand<Canvas> CommandPreviewMouseUp { get; private set; }
        public MyICommand<Canvas> CommandPreviewMouseDown { get; private set; }
        public MyICommand<Canvas> CommandPreviewMouseMove { get; private set; }


        public ViewViewModel()
        {
            CommandDragOver = new MyICommand<Canvas>(DoDragOver);
            CommandDrop = new MyICommand<Canvas>(DoDrop);

            CommandAddAllToGrid = new MyICommand<Grid>(RandomAddAllToGrid);
            CommandRemoveFromGrid = new MyICommand<Canvas>(DoRemoveFromGrid);
            ChosenTreeView = new MyICommand<TreeView>(ChangedChosenTreeView);
            ClickLeftMouse = new MyICommand(ClickLeftMouseUp);

            CommandPreviewMouseUp = new MyICommand<Canvas>(PreviewMouseUp);
            CommandPreviewMouseMove = new MyICommand<Canvas>(PreviewMouseDown);
            CommandPreviewMouseDown = new MyICommand<Canvas>(PreviewMouseMove);

            Succesfull = Error = Visibility.Hidden;
            information = Visibility.Visible;
            Mess = "Wellcome! Program started.";

            InitList();

            // za prijem novih entiteta
            Messenger.Default.Register<Forwarder>(this, AddToTreeView);

            // za uklanjanje entiteta ako se ukloni iz liste svih
            Messenger.Default.Register<Deleter>(this, RemoveFromTreeView);

            Messenger.Default.Register<MessageChange>(this, Notify);

            Messenger.Default.Register<int>(this, RemoveIfOnCanvas);

        }

        public void DoDragOver(Canvas kanvas)
        {
            //do
        }

        private void DoDrop(Canvas kanvas)
        {
            TextBlock write = ((TextBlock)(kanvas).Children[0]);

            if (draggedItem != null)
            {
                if (kanvas.Resources["taken"] == null)
                {
                    BitmapImage img = new BitmapImage();
                    img.BeginInit();
                    string putanja = Directory.GetCurrentDirectory() + "/Assets/uredjaj.png";
                    img.UriSource = new Uri(putanja, UriKind.Absolute);
                    img.EndInit();
                    kanvas.Background = new ImageBrush(img);
                    write.Text = draggedItem.Name;
                    write.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#000000"));
                    draggedItem.PositionOnCanvas = GetCanvasId(kanvas.Name);
                    kanvas.Resources.Add("taken", true);
                    RemoveEnt(draggedItem);
                }
                draggedItem = null;
                dragging = false;
            }

        }

        private int GetCanvasId(string name)
        {
            int id = 1;

            if (name.Equals("c1")) id = 1;
            if (name.Equals("c2")) id = 2;
            if (name.Equals("c3")) id = 3;
            if (name.Equals("c4")) id = 4;
            if (name.Equals("c5")) id = 5;
            if (name.Equals("c6")) id = 6;
            if (name.Equals("c7")) id = 7;
            if (name.Equals("c8")) id = 8;
            if (name.Equals("c9")) id = 9;
            if (name.Equals("c10")) id = 10;
            if (name.Equals("c11")) id = 11;
            if (name.Equals("c12")) id = 12;

            return id;
        }

        private void RemoveEnt(Entity draggedItem)
        {
            if (draggedItem.Scope.Equals("ONE"))
            {
                treeViewBindingList[0].listOfEntities.RemoveAt(selected);
                canvasBindingList[0].listOfEntities.Add(draggedItem);
            }
            else if (draggedItem.Scope.Equals("TWO"))
            {
                treeViewBindingList[1].listOfEntities.RemoveAt(selected);
                canvasBindingList[1].listOfEntities.Add(draggedItem);
            }
            else if (draggedItem.Scope.Equals("THREE"))
            {
                treeViewBindingList[2].listOfEntities.RemoveAt(selected);
                canvasBindingList[2].listOfEntities.Add(draggedItem);
            }
            else if (draggedItem.Scope.Equals("FOUR"))
            {
                treeViewBindingList[3].listOfEntities.RemoveAt(selected);
                canvasBindingList[3].listOfEntities.Add(draggedItem);
            }
            else if (draggedItem.Scope.Equals("FIVE"))
            {
                treeViewBindingList[4].listOfEntities.RemoveAt(selected);
                canvasBindingList[4].listOfEntities.Add(draggedItem);
            }
        }

        private void DoRemoveFromGrid(Canvas parentCanvas)
        {
            if (parentCanvas.Resources["taken"] != null)
            {
                BringBack(parentCanvas);

                parentCanvas.Background = Brushes.White;
                ((TextBlock)parentCanvas.Children[0]).Text = string.Empty;
                parentCanvas.Resources.Remove("taken");

                int id = GetCanvasId(parentCanvas.Name);
            }
        }

        private void BringBack(Canvas parentCanvas)
        {
            string entityName = ((TextBlock)parentCanvas.Children[0]).Text;

            Entity item = null;
            BindingEntity addressClassOfItem = null;
            int coundAddressClass = 0;

            // prolazimo kroz sve adresne klase
            foreach (BindingEntity be in canvasBindingList)
            {
                // i trazimo u listi entiteta odredjene klase onaj entitet koji je na canvasu
                foreach (Entity e in be.listOfEntities)
                {
                    if (e.Name.Equals(entityName))
                    {
                        // pronasli smo entitet, zapamti
                        addressClassOfItem = be;
                        item = e;
                        goto Izlaz;
                    }
                }
                // prelazimo u sledecu adresnu klasu (ONE = 0, TWO = 1, THREE = 2, FOUR = 3, FIVE = 4)
                coundAddressClass += 1;
            }

        Izlaz:
            if (item == null || addressClassOfItem == null || coundAddressClass > 4)
            {
                return;
            }

            // u pronadjenoj klasi, za pronadjeni entitet - ukloniti referencu
            item.PositionOnCanvas = -1;

            addressClassOfItem.listOfEntities.Remove(item);

            // dodajemo u tree view u odredjenu klasu adresa kojoj entitet i pripada
            treeViewBindingList[coundAddressClass].listOfEntities.Add(item);
        }

        private void ChangedChosenTreeView(TreeView tv)
        {
            var window = ViewViewModel.UserControl;

            if (!dragging && tv != null && tv.SelectedItem != null && tv.SelectedItem.GetType() == typeof(Entity))
            {
                dragging = true;
                draggedItem = (Entity)tv.SelectedItem;
                selected = PronadjiElement(draggedItem);
                DragDrop.DoDragDrop(window, draggedItem, DragDropEffects.Move | DragDropEffects.Copy);
            }
        }

        private int PronadjiElement(Entity draggedItem)
        {
            int index = 0;
            if (draggedItem.Scope.Equals("ONE"))
            {
                index = treeViewBindingList[0].listOfEntities.IndexOf(draggedItem);
            }
            else if (draggedItem.Scope.Equals("TWO"))
            {
                index = treeViewBindingList[1].listOfEntities.IndexOf(draggedItem);
            }
            else if (draggedItem.Scope.Equals("THREE"))
            {
                index = treeViewBindingList[2].listOfEntities.IndexOf(draggedItem);
            }
            else if (draggedItem.Scope.Equals("FOUR"))
            {
                index = treeViewBindingList[3].listOfEntities.IndexOf(draggedItem);
            }
            else if (draggedItem.Scope.Equals("FIVE"))
            {
                index = treeViewBindingList[4].listOfEntities.IndexOf(draggedItem);
            }

            return index;
        }

        private void ClickLeftMouseUp()
        {
            dragging = false;
            draggedItem = null;
        }

        private void PreviewMouseUp(Canvas canvas)
        {
            // Samo ako imamo pocetni canvas i odabrani element i canvas na koji prebacujemo nije vec zauzet
            if (draggedItem != null && start != null && canvas.Resources["taken"] == null)
            {
                // prebaci na novi canvas
                TextBlock ispis = ((TextBlock)(canvas).Children[0]);

                if (draggedItem != null)
                {
                    if (canvas.Resources["taken"] == null)
                    {
                        // pomeri linije sa starog na novi canvas
                        // uzmi staru poziciju
                        int stara_canvas_id = draggedItem.PositionOnCanvas;

                        // uzmi novu poziciju
                        int nova_canvas_id = GetCanvasId(canvas.Name);
                    }
                }
                BitmapImage img = new BitmapImage();
                img.BeginInit();
                string putanja = Directory.GetCurrentDirectory() + "/Assets/uredjaj.png";
                img.UriSource = new Uri(putanja, UriKind.Absolute);
                img.EndInit();
                canvas.Background = new ImageBrush(img);
                ispis.Text = draggedItem.Name;
                ispis.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#000000"));
                draggedItem.PositionOnCanvas = GetCanvasId(canvas.Name);
                canvas.Resources.Add("taken", true);
            }
            draggedItem = null;
            dragging = false;

            if (start.Resources["taken"] != null)
            {
                start.Background = Brushes.White;
                ((TextBlock)start.Children[0]).Text = string.Empty;
                start.Resources.Remove("taken");
            }

            draggedItem = null;
            start = null;
        }

        private void PreviewMouseDown(Canvas canvas)
        {
            // prvo pronadjemo koji je to element na canvasu
            string entityName = ((TextBlock)canvas.Children[0]).Text;
            Entity ent = null;

            foreach (BindingEntity be in canvasBindingList)
            {
                foreach (Entity e in be.listOfEntities)
                {
                    if (e.Name.Equals(entityName))
                    {
                        ent = e;
                        goto Izlaz;
                    }
                }
            }

        Izlaz:
            draggedItem = ent;
            start = canvas;
        }

        private void PreviewMouseMove(Canvas canvas)
        {
            if (draggedItem == null)
                return;
        }

        //za poruke
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
        public Visibility Succesfull
        {
            get
            {
                return succesfull;
            }

            set
            {
                if (succesfull != value)
                {
                    succesfull = value;

                    if (succesfull == Visibility.Visible)
                    {
                        Error = Information = Visibility.Hidden;
                        OnPropertyChanged("Error");
                        OnPropertyChanged("Information");
                    }

                    OnPropertyChanged("Succesfull");
                }
            }
        }

        public Visibility Error
        {
            get
            {
                return error;
            }

            set
            {
                if (error != value)
                {
                    error = value;

                    if (error == Visibility.Visible)
                    {
                        Succesfull = Information = Visibility.Hidden;
                        OnPropertyChanged("Succesfull");
                        OnPropertyChanged("Information");
                    }

                    OnPropertyChanged("Error");
                }
            }
        }

        public Visibility Information
        {
            get
            {
                return information;
            }

            set
            {
                if (information != value)
                {
                    information = value;

                    if (information == Visibility.Visible)
                    {
                        Error = Succesfull = Visibility.Hidden;
                        OnPropertyChanged("Error");
                        OnPropertyChanged("Succesfull");
                    }

                    OnPropertyChanged("Information");
                }
            }
        }

        //inicijalizacija listi
        public void InitList()
        {
            canvasBindingList = new BindingList<BindingEntity>()
            {
                new BindingEntity() { Addresses = "Address scope 1" },
                new BindingEntity() { Addresses = "Address scope 2" },
                new BindingEntity() { Addresses = "Address scope 3" },
                new BindingEntity() { Addresses = "Address scope 4" },
                new BindingEntity() { Addresses = "Address scope 5"}
            };

            treeViewBindingList = new BindingList<BindingEntity>()
            {
                new BindingEntity() { Addresses = "Address scope 1" },
                new BindingEntity() { Addresses = "Address scope 2" },
                new BindingEntity() { Addresses = "Address scope 3" },
                new BindingEntity() { Addresses = "Address scope 4" },
                new BindingEntity() { Addresses = "Address scope 5" }
            };
        }

        private void AddToTreeView(Forwarder fw)
        {
            Entity newEnt = fw.Entity;
            int classes = 0;

            if (newEnt.Scope.Equals("ONE")) classes = 0;
            if (newEnt.Scope.Equals("TWO")) classes = 1;
            if (newEnt.Scope.Equals("THREE")) classes = 2;
            if (newEnt.Scope.Equals("FOUR")) classes = 3;
            if (newEnt.Scope.Equals("FIVE")) classes = 4;

            treeViewBindingList[classes].listOfEntities.Add(newEnt);
        }
        
        private void RemoveFromTreeView(Deleter dl)
        {
            Entity toDelete = dl.Entity;
            int classes = 0;

            if (toDelete.Scope.Equals("ONE")) classes = 0;
            if (toDelete.Scope.Equals("TWO")) classes = 1;
            if (toDelete.Scope.Equals("THREE")) classes = 2;
            if (toDelete.Scope.Equals("FOUR")) classes = 3;
            if (toDelete.Scope.Equals("FIVE")) classes = 4;

            // ako se element nalazi na canvasu, ukloniti ga iz liste i sa canvasa
            if (canvasBindingList[classes].listOfEntities.Contains(toDelete))
            {
                canvasBindingList[classes].listOfEntities.Remove(toDelete);

            }

            // ako se nalazi u tree view - ukloniti ga
            if (treeViewBindingList[classes].listOfEntities.Contains(toDelete))
            {
                treeViewBindingList[classes].listOfEntities.Remove(toDelete);
            }
        }

        private void Notify(MessageChange message)
        {
            Error = message.Visibility_Error;
            Succesfull = message.Visibility_Success;
            Mess = message.Mess;
        }

        private void RemoveIfOnCanvas(int idCanvas)
        {
            if (right == null)
                return;

            // indeksiranje
            // dock paneli krecu od indeksa 1
            // indeks 1 u dock panelu je canvas
            List<Canvas> kanvasi = new List<Canvas>();

            for (int i = 1; i < 13; i++)
            {
                DockPanel panel = (DockPanel)(right.Children[i]);
                Canvas canvas = (Canvas)(panel.Children[1]);
                kanvasi.Add(canvas);
            }

            // poziva se oslobodi za dati canvas
            DoRemoveFromGrid(kanvasi[idCanvas]);
        }

        private void RandomAddAllToGrid(Grid rightGridOnCanvas)
        {
            // Rasporedi na preostala slobodna mesta
            for (int i = 1; i <= 12; i++)
            {
                // uzmemo canvas
                Canvas kanvas = ((Canvas)((DockPanel)(rightGridOnCanvas.Children[i])).Children[1]);
                TextBlock current = (TextBlock)((kanvas).Children[0]);
                string entityName = current.Text.Trim();

                if (entityName.Equals(""))
                {
                    // prazan je canvas
                    if (treeViewBindingList[0].listOfEntities.Count > 0)
                    {
                        draggedItem = treeViewBindingList[0].listOfEntities[0];
                        treeViewBindingList[0].listOfEntities.RemoveAt(0);
                        canvasBindingList[0].listOfEntities.Add(draggedItem);
                    }
                    else if (treeViewBindingList[1].listOfEntities.Count > 0)
                    {
                        draggedItem = treeViewBindingList[1].listOfEntities[0];
                        treeViewBindingList[1].listOfEntities.RemoveAt(0);
                        canvasBindingList[1].listOfEntities.Add(draggedItem);
                    }
                    else if (treeViewBindingList[2].listOfEntities.Count > 0)
                    {
                        draggedItem = treeViewBindingList[2].listOfEntities[0];
                        treeViewBindingList[2].listOfEntities.RemoveAt(0);
                        canvasBindingList[2].listOfEntities.Add(draggedItem);
                    }
                    else if (treeViewBindingList[3].listOfEntities.Count > 0)
                    {
                        draggedItem = treeViewBindingList[3].listOfEntities[0];
                        treeViewBindingList[3].listOfEntities.RemoveAt(0);
                        canvasBindingList[3].listOfEntities.Add(draggedItem);
                    }
                    else if (treeViewBindingList[4].listOfEntities.Count > 0)
                    {
                        draggedItem = treeViewBindingList[4].listOfEntities[0];
                        treeViewBindingList[4].listOfEntities.RemoveAt(0);
                        canvasBindingList[4].listOfEntities.Add(draggedItem);
                    }

                    if (draggedItem != null)
                    {
                        draggedItem.PositionOnCanvas = i; // pozicija na canvasu

                        if (kanvas.Resources["taken"] == null)
                        {
                            BitmapImage img = new BitmapImage();
                            img.BeginInit();
                            string putanja = Directory.GetCurrentDirectory() + "/Assets/uredjaj.png";
                            img.UriSource = new Uri(putanja, UriKind.Absolute);
                            img.EndInit();
                            kanvas.Background = new ImageBrush(img);
                            current.Text = draggedItem.Name;
                            current.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#000000"));
                            draggedItem.PositionOnCanvas = GetCanvasId(kanvas.Name);
                            kanvas.Resources.Add("taken", true);
                        }
                        draggedItem = null;
                        dragging = false;
                    }
                }
            }
        }

        // Kako bi se pamtilo stanje na canvasu - potrebno je koristiti konstruktor sa parametrima
        public ViewViewModel(Grid rightGridOnCanvas)
        {
            right = rightGridOnCanvas;

            CommandAddAllToGrid = new MyICommand<Grid>(RandomAddAllToGrid);

            // komande
            CommandDragOver = new MyICommand<Canvas>(DoDragOver);
            CommandDrop = new MyICommand<Canvas>(DoDrop);
            ClickLeftMouse = new MyICommand(ClickLeftMouseUp);
            ChosenTreeView = new MyICommand<TreeView>(ChangedChosenTreeView);
            CommandRemoveFromGrid = new MyICommand<Canvas>(DoRemoveFromGrid);

            // komande za d&d
            CommandPreviewMouseUp = new MyICommand<Canvas>(PreviewMouseUp);
            CommandPreviewMouseDown = new MyICommand<Canvas>(PreviewMouseDown);
            CommandPreviewMouseMove = new MyICommand<Canvas>(PreviewMouseMove);

            Succesfull = Error = Visibility.Hidden;
            Information = Visibility.Visible;
            Mess = "Wellcome! The aplication is ready.";

            // restauracija canvasa
            // indeksiranje
            // dock paneli krecu od indeksa 1
            // indeks 1 u dock panelu je canvas
            List<Canvas> kanvasi = new List<Canvas>();

            for (int i = 1; i < 13; i++)
            {
                DockPanel panel = (DockPanel)(rightGridOnCanvas.Children[i]);
                Canvas canvas = (Canvas)(panel.Children[1]);
                kanvasi.Add(canvas);
            }

            foreach (BindingEntity be in canvasBindingList.ToList())
            {
                foreach (Entity e in be.listOfEntities.ToList())
                {
                    if (e.PositionOnCanvas != -1)
                    {
                        draggedItem = MainWindowViewModel.Entities.FirstOrDefault(p => p.Id == e.Id);
                        Canvas kanvas = kanvasi[e.PositionOnCanvas - 1];

                        TextBlock write = ((TextBlock)(kanvas).Children[0]);

                        if (draggedItem != null)
                        {
                            if (kanvas.Resources["taken"] == null)
                            {
                                BitmapImage img = new BitmapImage();
                                img.BeginInit();
                                string putanja = Directory.GetCurrentDirectory() + "/Assets/uredjaj.png";
                                img.UriSource = new Uri(putanja, UriKind.Absolute);
                                img.EndInit();
                                kanvas.Background = new ImageBrush(img);
                                write.Text = draggedItem.Name;
                                write.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#000000"));
                                draggedItem.PositionOnCanvas = GetCanvasId(kanvas.Name);
                                kanvas.Resources.Add("taken", true);
                            }
                            draggedItem = null;
                            dragging = false;
                        }
                    }
                }
            }

            Messenger.Default.Register<MessageChange>(this, Notify);
            Messenger.Default.Register<int>(this, RemoveIfOnCanvas);

        }
    }
}