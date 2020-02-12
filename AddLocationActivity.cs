using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Maps;
using Android.Locations;
using Android.Gms.Maps.Model;
using Android.Gms.Common;
using Android.Util;
using Android.Gms.Location;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Database;

namespace Firebase
{
    [Activity(Label = "AddLocationActivity")]
    public class AddLocationActivity : Activity, IOnMapReadyCallback   
    {
        private GoogleMap _map;
        private MapFragment _mapFragment;
        private LatLng coords;
        FusedLocationProviderClient fusedLocationProviderClient;
        //TournamentViewModel tournamentObject;

        FirebaseAuth auth;
        DatabaseReference mDatabase;
        string tourneyID;

        async Task GetLastLocationFromDevice()
        {
            // This method assumes that the necessary run-time permission checks have succeeded.
            //getLastLocationButton.SetText(Resource.String.getting_last_location);
            Android.Locations.Location location = await fusedLocationProviderClient.GetLastLocationAsync();

            if (location == null)
            {
                // Seldom happens, but should code that handles this scenario
            }
            else
            {
                // Do something with the location 
                coords = new LatLng(location.Latitude, location.Longitude);
            }
        }

        private void MapOnMarkerDragEnd(object sender, GoogleMap.MarkerDragEndEventArgs e)
        {
            coords.Latitude = e.Marker.Position.Latitude;
            coords.Longitude = e.Marker.Position.Longitude;

            // Setting new location coordinates once user drops marker
            var coordinates = coords.Latitude + "," + coords.Longitude;
            mDatabase.Child("tournaments").Child(tourneyID).Child("location").SetValueAsync(coordinates);


        }

        private void InitMapFragment()
        {
            _mapFragment = FragmentManager.FindFragmentByTag("map") as MapFragment;
            if (_mapFragment == null)
            {
                GoogleMapOptions mapOptions = new GoogleMapOptions()
                    .InvokeMapType(GoogleMap.MapTypeNormal)
                    .InvokeZoomControlsEnabled(true);

                FragmentTransaction fragTx = FragmentManager.BeginTransaction();
                _mapFragment = MapFragment.NewInstance(mapOptions);
                fragTx.Add(Resource.Id.map, _mapFragment, "map");
                fragTx.Commit();
            }

            _mapFragment.GetMapAsync(this);
        }

        public void OnMapReady(GoogleMap map)
        {
            // 33.6518268, 73.1565937 (_currentLocation.Latitude, _currentLocation.Longitude
            //map.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(new LatLng(33.6518268, 73.1565937), 15));
            _map = map;


        }

        protected override void OnCreate(Bundle savedInstanceState)
        {

            // Getting current location
            fusedLocationProviderClient = LocationServices.GetFusedLocationProviderClient(this);

            Task.Run(GetLastLocationFromDevice);

            base.OnCreate(savedInstanceState);

            auth = FirebaseAuth.GetInstance(MainActivity.app);
            mDatabase = FirebaseDatabase.Instance.Reference;

            tourneyID = Intent.GetStringExtra("tourneyID") ?? "None";

            SetContentView(Resource.Layout.AddLocation);

            // Adding location button
            Button currentLocationButton = FindViewById<Button>(Resource.Id.currentLocationButton);
            currentLocationButton.Click += (sender, e) =>
            {
                CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
                builder.Target(coords);
                builder.Zoom(18);
                builder.Bearing(155);
                builder.Tilt(65);
                CameraPosition cameraPosition = builder.Build();
                CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
                if (_map != null)
                {
                    // Puttin a marker in this location for admin to move around
                    MarkerOptions tournamentLocationMarker = new MarkerOptions();
                    tournamentLocationMarker.SetPosition(coords);
                    tournamentLocationMarker.SetTitle("Tournament Location");
                    tournamentLocationMarker.SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueCyan));
                    tournamentLocationMarker.Draggable(true);
                    _map.MarkerDragEnd += MapOnMarkerDragEnd;
                    _map.AddMarker(tournamentLocationMarker);

                    // Moving Camera
                    _map.AnimateCamera(CameraUpdateFactory.NewCameraPosition(cameraPosition));

                    // Setting current location as tournament location in case admin does not want to change
                    var coordinates = coords.Latitude + "," + coords.Longitude;
                    mDatabase.Child("tournaments").Child(tourneyID).Child("location").SetValueAsync(coordinates);

                    // Changing button text
                    currentLocationButton.Text = "Tournament Location";
                }
                else
                {
                    Toast.MakeText(ApplicationContext, "Wait for location to load..", ToastLength.Long);
                }
            };

            // Adding Save location button
            Button saveLocationButton = FindViewById<Button>(Resource.Id.saveLocationButton);
            saveLocationButton.Click += async (sender, e) =>
            {
                if (coords != null)
                {
                    await mDatabase.Child("tournaments").Child(tourneyID).Child("online").SetValueAsync("false");
                    // Going back to Tournament Main page
                    Intent addTournamentActivity = new Intent(Application.Context, typeof(AddTournamentActivity));
                    addTournamentActivity.PutExtra("tourneyID", tourneyID);   
                    StartActivity(addTournamentActivity);
                    Finish();
                }
                else
                {
                    Toast.MakeText(ApplicationContext, "Load Current Location and move Marker First", ToastLength.Long);
                }
            };

            InitMapFragment();
        }
    }
}