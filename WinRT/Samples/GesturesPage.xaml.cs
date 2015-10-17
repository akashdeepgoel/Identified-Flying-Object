﻿using Samples.Common;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WindowsPreview.Kinect;
using LightBuzz.Vitruvius;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace Samples
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class GesturesPage : Page
    {
        NavigationHelper _navigationHelper;
        public NavigationHelper NavigationHelper { get { return _navigationHelper; } }

        KinectSensor _sensor;
        MultiSourceFrameReader _reader;
        GestureController _gestureController;

        public GesturesPage()
        {
            InitializeComponent();
            
            _navigationHelper = new NavigationHelper(this);

            _sensor = KinectSensor.GetDefault();

            if (_sensor != null)
            {
                _sensor.Open();

                _reader = _sensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color | FrameSourceTypes.Depth | FrameSourceTypes.Infrared | FrameSourceTypes.Body);
                _reader.MultiSourceFrameArrived += Reader_MultiSourceFrameArrived;

                _gestureController = new GestureController();
                _gestureController.GestureRecognized += GestureController_GestureRecognized;
            }
        }

        private void PageRoot_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_reader != null)
            {
                _reader.Dispose();
            }

            if (_sensor != null)
            {
                _sensor.Close();
            }
        }

        void Reader_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            var reference = e.FrameReference.AcquireFrame();

            // Color
            using (var frame = reference.ColorFrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    if (viewer.Visualization == Visualization.Color)
                    {
                        viewer.Image = frame.ToBitmap();
                    }
                }
            }

            // Body
            using (var frame = reference.BodyFrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    Body body = frame.Bodies().Closest();

                    if (body != null)
                    {
                        _gestureController.Update(body);
                    }
                }
            }
        }

        void GestureController_GestureRecognized(object sender, GestureEventArgs e)
        {
            tblGestures.Text = e.GestureType.ToString();
        }
    }
}
