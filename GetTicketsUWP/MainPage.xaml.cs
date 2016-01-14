using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;

using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Gpio;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Cryptography.Certificates;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using Windows.Web.Http.Filters;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace GetTicketsUWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private DispatcherTimer timer;
        static string queryUrl = "https://kyfw.12306.cn/otn/lcxxcx/query?purpose_codes=0X00&queryDate=2016-01-24&from_station=WHN&to_station=NDC";

        private const int LED_PIN = 5;
        private GpioPin pin;
        private GpioPinValue pinValue;

        List<Train> TrainList = new List<Train>();
        public MainPage()
        {
            this.InitializeComponent();


            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += Timer_Tick;
            InitGPIO();
            if (pin != null)
            {
                timer.Start();
            }
        }
        private void InitGPIO()
        {
            var gpio = GpioController.GetDefault();

            // Show an error if there is no GPIO controller
            if (gpio == null)
            {
                pin = null;
                
                return;
            }

            pin = gpio.OpenPin(LED_PIN);
            pinValue = GpioPinValue.Low;
            pin.Write(pinValue);
            pin.SetDriveMode(GpioPinDriveMode.Output);

            
        }
        private async void Timer_Tick(object sender, object e)
        {
            var filter = new HttpBaseProtocolFilter();

            filter.IgnorableServerCertificateErrors.Add(ChainValidationResult.Expired);
            filter.IgnorableServerCertificateErrors.Add(ChainValidationResult.Untrusted);
            filter.IgnorableServerCertificateErrors.Add(ChainValidationResult.InvalidName);


            using (var httpClient = new HttpClient(filter))
            {
                var response =await httpClient.GetAsync(new Uri(queryUrl));
                var result = response.Content.ToString();

                dynamic dynObj = JsonConvert.DeserializeObject(result);
                foreach (var data in dynObj.data.datas)
                {


                    Train traindata = new Train();
                    traindata.train_code = data.station_train_code;
                    traindata.end_station = data.end_station_name;
                    traindata.from_station = data.from_station_name;
                    traindata.start_train_date = data.start_train_date;
                    traindata.yw_num = data.yw_num;
                    traindata.wz_num = data.wz_num;
                    TrainList.Add(traindata);

                }
            }
            foreach(var train in TrainList)
            {
                if(train.yw_num!="无")
                {
                    pin.Write(GpioPinValue.High);
                    Debug.WriteLine(train.train_code + "有票了");
                }
            }
        }
    }

    public class Train
    {
        public string train_code { get; set; }
        public string end_station { get; set; }
        public string from_station { get; set; }
        public string start_train_date { get; set; }
        public string yw_num { get; set; }
        public string wz_num { get; set; }

    }
}
