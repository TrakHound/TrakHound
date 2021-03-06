﻿// Copyright (c) 2017 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using NLog;
using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TrakHound;
using TrakHound.API;
using TrakHound.API.Users;
using TrakHound.Configurations;

namespace TrakHound_Dashboard.Pages.Dashboard.Footprint.Controls
{
    /// <summary>
    /// Interaction logic for ListItem.xaml
    /// </summary>
    public partial class ListItem : UserControl
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public ListItem(DeviceDescription device)
        {
            InitializeComponent();
            root.DataContext = this;
            Device = device;
        }

        public DeviceDescription Device
        {
            get { return (DeviceDescription)GetValue(DeviceProperty); }
            set
            {
                SetValue(DeviceProperty, value);

                if (value != null)
                {
                    var device = value;

                    if (device.Description != null)
                    {
                        // Load Device Image
                        if (!string.IsNullOrEmpty(device.Description.ImageUrl)) LoadDeviceImage(device.Description.ImageUrl);
                    }
                }
            }
        }

        public static readonly DependencyProperty DeviceProperty =
            DependencyProperty.Register("Device", typeof(DeviceDescription), typeof(ListItem), new PropertyMetadata(null));

        public UserConfiguration UserConfiguration { get; set; }


        #region "Device Image"

        public ImageSource DeviceImage
        {
            get { return (ImageSource)GetValue(DeviceImageProperty); }
            set { SetValue(DeviceImageProperty, value); }
        }

        public static readonly DependencyProperty DeviceImageProperty =
            DependencyProperty.Register("DeviceImage", typeof(ImageSource), typeof(ListItem), new PropertyMetadata(null));

        public void LoadDeviceImage(string fileId)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(LoadDeviceImage_Worker), fileId);
        }

        void LoadDeviceImage_Worker(object o)
        {
            BitmapSource result = null;

            if (o != null)
            {
                string fileId = o.ToString();

                System.Drawing.Image img = null;

                string path = Path.Combine(FileLocations.Storage, fileId);
                if (File.Exists(path)) img = System.Drawing.Image.FromFile(path);
                else img = Files.DownloadImage(UserConfiguration, fileId);

                if (img != null)
                {
                    try
                    {
                        var bmp = new System.Drawing.Bitmap(img);
                        if (bmp != null)
                        {
                            IntPtr bmpPt = bmp.GetHbitmap();
                            result = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bmpPt, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                            if (result != null)
                            {
                                if (result.PixelWidth > result.PixelHeight)
                                {
                                    result = TrakHound_UI.Functions.Images.SetImageSize(result, 75);
                                }
                                else
                                {
                                    result = TrakHound_UI.Functions.Images.SetImageSize(result, 0, 75);
                                }

                                result.Freeze();
                            }
                        }
                    }
                    catch (Exception ex) { logger.Error(ex); }
                }
            }

            Dispatcher.BeginInvoke(new Action<BitmapSource>(LoadDeviceImage_GUI), System.Windows.Threading.DispatcherPriority.DataBind, new object[] { result });
        }

        void LoadDeviceImage_GUI(BitmapSource img)
        {
            DeviceImage = img;
        }

        #endregion

    }
}
