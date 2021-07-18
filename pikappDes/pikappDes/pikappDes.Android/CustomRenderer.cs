using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.Android;
using pikappDes.Droid;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using pikappDes;
using Xamarin.Essentials;

[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomRenderer))]
namespace pikappDes.Droid
{

    class CustomRenderer : MapRenderer, GoogleMap.IInfoWindowAdapter
    {

        public CustomRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Xamarin.Forms.Maps.Map> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                NativeMap.InfoWindowClick -= OnInfoWindowClick;
            }

            if (e.NewElement != null)
            {
                var formsMap = (CustomMap)e.NewElement;
                //customPins = formsMap.CustomPins;
            }
        }

        private void OnInfoWindowClick(object sender, GoogleMap.InfoWindowClickEventArgs e)
        {

            var customPin = GetCustomPin(e.Marker);
            if (customPin == null)
            {
                throw new Exception("Custom pin not found");
            }

            //AlertDialog.Builder dialog = new AlertDialog.Builder(this.Context);
            //AlertDialog alert = dialog.Create();
            //alert.SetTitle(customPin.Phone.ToString());

            Clipboard.SetTextAsync(customPin.phone); //  ========================================= COPY PHONE TO CLIPBOARD
            //alert.Show();


            //if (!string.IsNullOrWhiteSpace(customPin.Url))
            //{
            //    var url = Android.Net.Uri.Parse(customPin.Url);
            //    var intent = new Intent(Intent.ActionView, url);
            //    intent.AddFlags(ActivityFlags.NewTask);
            //    Android.App.Application.Context.StartActivity(intent);
            //}

        }

        protected override void OnMapReady(GoogleMap map)
        {
           
            base.OnMapReady(map);

          

            NativeMap.InfoWindowClick += OnInfoWindowClick;
            NativeMap.SetInfoWindowAdapter(this);
        }

        protected override MarkerOptions CreateMarker(Pin pin)
        {
            var marker = new MarkerOptions();
            marker.SetPosition(new LatLng(pin.Position.Latitude, pin.Position.Longitude));
            marker.SetTitle(pin.Label);
            marker.SetSnippet(pin.Address);
            if(pin.Type == PinType.Generic)
                marker.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.pinS)); // ================ PUT AN ICON HERE
            else
                marker.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.pinC)); // ================ PUT AN ICON HERE



            return marker;
        }

        Android.Views.View GoogleMap.IInfoWindowAdapter.GetInfoContents(Marker marker)
        {
            var inflater = Android.App.Application.Context.GetSystemService(Context.LayoutInflaterService) as Android.Views.LayoutInflater;
            if (inflater != null)
            {
                Android.Views.View view;

                var customPin = GetCustomPin(marker);

                if (customPin == null)
                {
                    throw new Exception("Custom pin not found");
                }

                //====================== we can cutomize info window here

                //if (customPin.Name.Equals("Xamarin"))
                //{
                //    view = inflater.Inflate(Resource.Layout.XamarinMapInfoWindow, null);
                //}
                //else
                //{
                //    view = inflater.Inflate(Resource.Layout.MapInfoWindow, null);
                //}
                //=============================================================

                view = inflater.Inflate(Resource.Layout.CustomInfoWindow, null);

                var infoTitle = view.FindViewById<TextView>(Resource.Id.InfoWindowTitle) ;
                var infoSubtitle = view.FindViewById<TextView>(Resource.Id.InfoWindowSubtitle);

                if (infoTitle != null)
                {
                    //infoTitle.Text = customPin.Phone.ToString();
                    infoTitle.Text = "Ping : ";
                }
                if (infoSubtitle != null)
                {
                    infoSubtitle.Text = customPin.phone;
                }

                return view;


                // return null;
            }
            return null;
        }

        Android.Views.View GoogleMap.IInfoWindowAdapter.GetInfoWindow(Marker marker)
        {
            return null;
        }

        CustomPins GetCustomPin(Marker annotation)
        {
            var position = new Position(annotation.Position.Latitude, annotation.Position.Longitude);
            foreach (var pin in Map.Pins)
            {
                if (pin.Position == position)
                {
                    return (CustomPins)pin;
                }
            }
            return null;
        }


    }
}