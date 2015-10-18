using Samples.Common;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WindowsPreview.Kinect;
using LightBuzz.Vitruvius;
using System.Diagnostics;
using Windows.Storage.Pickers;
using Windows.Graphics.Imaging;
using Windows.Graphics.Display;
using Windows.Storage;
using System.Collections.Generic;
using System.ComponentModel;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace Samples
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private List<GestureDetector> gestureDetectorList = null;
        public bool isTakingScreenshot = false;
        NavigationHelper _navigationHelper;
        public NavigationHelper NavigationHelper { get { return _navigationHelper; } }

        KinectSensor _sensor;
        MultiSourceFrameReader _reader;
        GestureController _gestureController;
        bool isRunning = false;
        public MainPage()
        
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
            // Initialize the gesture detection objects for our gestures
            this.gestureDetectorList = new List<GestureDetector>();

            // Create a gesture detector for each body (6 bodies => 6 detectors)
            int maxBodies = this._sensor.BodyFrameSource.BodyCount;
            for (int i = 0; i < maxBodies; ++i)
            {
                GestureResultView result =
                     new GestureResultView(i, false, false, 0.0f);
                GestureDetector detector =
                    new GestureDetector(this._sensor, result);
                result.PropertyChanged += GestureResult_PropertyChanged;
                this.gestureDetectorList.Add(detector);
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
        private void RegisterGesture(BodyFrame bodyFrame)
        {
            bool dataReceived = false;
            Body[] bodies = null;

            if (bodyFrame != null)
            {
                if (bodies == null)
                {
                    // Creates an array of 6 bodies, which is the max number of bodies that Kinect can track simultaneously
                    bodies = new Body[bodyFrame.BodyCount];
                }

                // The first time GetAndRefreshBodyData is called, Kinect will allocate each Body in the array.
                // As long as those body objects are not disposed and not set to null in the array,
                // those body objects will be re-used.
                bodyFrame.GetAndRefreshBodyData(bodies);
                dataReceived = true;
            }

            if (dataReceived)
            {
                // We may have lost/acquired bodies, so update the corresponding gesture detectors
                if (bodies != null)
                {
                    // Loop through all bodies to see if any of the gesture detectors need to be updated
                    for (int i = 0; i < bodyFrame.BodyCount; ++i)
                    {
                        Body body = bodies[i];
                        ulong trackingId = body.TrackingId;

                        // If the current body TrackingId changed, update the corresponding gesture detector with the new value
                        if (trackingId != this.gestureDetectorList[i].TrackingId)
                        {
                            this.gestureDetectorList[i].TrackingId = trackingId;

                            // If the current body is tracked, unpause its detector to get VisualGestureBuilderFrameArrived events
                            // If the current body is not tracked, pause its detector so we don't waste resources trying to get invalid gesture results
                            this.gestureDetectorList[i].IsPaused = trackingId == 0;
                        }
                    }
                }
            }
        }
        void Reader_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            BodyFrame bodyFrame = null;
            MultiSourceFrame multiSourceFrame = e.FrameReference.AcquireFrame();
            if (multiSourceFrame == null)
            {
                return;
            }
            var reference = e.FrameReference.AcquireFrame();
            using (bodyFrame = multiSourceFrame.BodyFrameReference.AcquireFrame())
            {
                RegisterGesture(bodyFrame);
            }
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
        void GestureResult_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            GestureResultView result = sender as GestureResultView;
            if (result.Confidence > 0.8)
            {
                OurFunction();
            }
        }
        async private void OurFunction()
        {
            MyHttpClient request1 = new MyHttpClient();
            if(isRunning==false)
            {
                Debug.WriteLine("Taking Off!");
                request1.send_request("/takeoff");
                isRunning = true;
            }

            else
            {
                Debug.WriteLine("Stop!");
                request1.send_request("/stop");
            }


        }
        void GestureController_GestureRecognized(object sender, GestureEventArgs e)
        {
            MyHttpClient request = new MyHttpClient();
            tblGestures.Text = e.GestureType.ToString();
            switch (e.GestureType)
            {

                case GestureType.JoinedHands:
                    //Debug.WriteLine("JH");
                    request.send_request("/dance");
                    Debug.WriteLine("Dance");
                    break;
                case GestureType.Menu:
                    Debug.WriteLine("Menu");
                    break;
                case GestureType.SwipeDown:
                    Debug.WriteLine("SD");
                    break;
                case GestureType.SwipeLeft:
                    Debug.WriteLine("Swipe Left!");
                    request.send_request("/left");
                    break;
                case GestureType.SwipeRight:
                    Debug.WriteLine("Swipe Right!");
                    request.send_request("/right");
                    //Debug.WriteLine("Going Right!");
                    break;
                case GestureType.SwipeUp:
                    Debug.WriteLine("Swipe Up!");
                    request.send_request("/up");
                    //Debug.WriteLine("Stay High All The Time!");
                    break;
                case GestureType.WaveLeft:
                    Debug.WriteLine("WL");
                    break;
                case GestureType.WaveRight:
                    Debug.WriteLine("Going Down!");
                    request.send_request("/land");
                    //Debug.WriteLine("Au Revoir");
                    isRunning = false;
                    break;
                case GestureType.ZoomIn:
                    Debug.WriteLine("Zoom In");
                    request.send_request("/forward");
                    //Debug.WriteLine("Winter is Coming!");
                    break;
                case GestureType.ZoomOut:
                    Debug.WriteLine("Zoom Out");
                    request.send_request("/back");
                    //Debug.WriteLine("You know nothing Jon Snow.");
                    break;
                default:
                    break;
            }

        }
    }
}
