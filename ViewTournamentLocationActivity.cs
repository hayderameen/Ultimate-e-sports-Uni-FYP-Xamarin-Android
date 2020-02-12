using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Firebase
{
    [Activity(Label = "ViewTournamentLocationActivity")]
    public class ViewTournamentLocationActivity : Activity, IOnMapReadyCallback
    {
        private GoogleMap _map;
        private MapFragment _mapFragment;
        LatLng tournamentLocation;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            string coords = Intent.GetStringExtra("coords") ?? "Not available";
            string[] coordsTemp = coords.Split(',');
            tournamentLocation = new LatLng(double.Parse(coordsTemp[0]), double.Parse(coordsTemp[1]));

            SetContentView(Resource.Layout.ViewTournamentLocation);

            InitMapFragment();
        }

        private void InitMapFragment()
        {
            _mapFragment = FragmentManager.FindFragmentByTag("map") as MapFragment;
            if (_mapFragment == null)
            {
                GoogleMapOptions mapOptions = new GoogleMapOptions()
                    .InvokeMapType(GoogleMap.MapTypeNormal)
                    .InvokeZoomControlsEnabled(true)
                    .InvokeCompassEnabled(true);

                FragmentTransaction fragTx = FragmentManager.BeginTransaction();
                _mapFragment = MapFragment.NewInstance(mapOptions);
                fragTx.Add(Resource.Id.map, _mapFragment, "map");
                fragTx.Commit();
            }
            _mapFragment.GetMapAsync(this);
        }

        public void OnMapReady(GoogleMap map)
        {
            _map = map;

            if (_map != null)
            {
                MarkerOptions markerOpt1 = new MarkerOptions();
                markerOpt1.SetPosition(tournamentLocation);
                markerOpt1.SetTitle("Tournament Location");
                markerOpt1.SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueCyan));
                _map.AddMarker(markerOpt1);

                // We create an instance of CameraUpdate, and move the map to it.
                CameraUpdate cameraUpdate = CameraUpdateFactory.NewLatLngZoom(tournamentLocation, 15);
                _map.AnimateCamera(cameraUpdate);
            }

        }
    }
}