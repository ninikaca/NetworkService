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
        public MyICommand<Canvas> ClickRightMouse { get; private set; }

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

            CommandRemoveFromGrid = new MyICommand<Canvas>(DoRemoveFromGrid);
            ChosenTreeView = new MyICommand<TreeView>(ChangedChosenTreeView);
            ClickLeftMouse = new MyICommand(ClickLeftMouseUp);
            ClickRightMouse = new MyICommand(CombineEntities);

            CommandPreviewMouseUp = new MyICommand<Canvas>(PreviewMouseUp);
            CommandPreviewMouseMove = new MyICommand<Canvas>(PreviewMouseDown);
            CommandPreviewMouseDown = new MyICommand<Canvas>(PreviewMouseMove);

            Succesfull = Errpr = information = Visibility.Hidden;
            information = Visibility.Visible;
            Mess = "Wellcome! Program started.";

            InicijalizacijaListi();

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
    }

}
