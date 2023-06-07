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
        //binding elementi
        public static BindingList<KlasifikovaniEntiteti> EntitetiCanvas { get; set; }
        public static BindingList<KlasifikovaniEntiteti> EntitetiTreeView { get; set; }
    }
}
