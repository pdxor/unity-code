/*         INFINITY CODE         */
/*   https://infinity-code.com   */

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using InfinityCode.OnlineMapsExamples;

namespace InfinityCode.OnlineMapsDemos
{
   /// <summary>
   /// ///////This is a search widget for the map that uses a string of text to reverse geocode and retrieve lat and long data

   /// </summary>


    [AddComponentMenu("Infinity Code/Online Maps/Demos/Search Panel")]
    public class SearchPanel:MonoBehaviour
    {
        //search field for typing in addresses and Points of interest
        public InputField inputField;
 
        //where we display responses
        public Text resulttext;

        //reverse geocode strings go here when results arrive
        public GameObject suggestedResultPanel;
       
        //our users location
        public double currentLatitude;
        public double currentLongitude;

        //click to start location services
        public Button findLocationBtn;

        //preloader
        public GameObject loaderAni;

        //dynamic. updates user to search status
        public Text findMyLocationBtnTxt;
        
        //send the google api creds
        public string googleMapsApi;

        //profile changes immediately
        public Text liveLocationProfileTxt;

        //sets the connection state
        public string coordAreSet;

        //connects search panel to quest submission form script
        public addQuest _addquest;

        //defines if we are in avatar build mode
        public bool isAvatar;

        //for map view search 
        public bool isMapSearch;
        

        private void Start()
        {

            findLocationBtn.interactable = false;
            findMyLocationBtnTxt.text = "Attempting to find Location Services";
            loaderAni.SetActive(true);
            coordAreSet = "false";
            StartCoroutine(StartLocationService());
        }
        private IEnumerator StartLocationService()
        {
                 #if UNITY_EDITOR
                        // No permission handling needed in Editor
                #elif UNITY_ANDROID
                        if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.CoarseLocation)) {
                            UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.CoarseLocation);
                        }

                        // First, check if user has location service enabled
                        if (!UnityEngine.Input.location.isEnabledByUser) {
                            // TODO Failure
                            //debugText.text = "Android and Location not enabled";
                            yield break;
                        }

                #elif UNITY_IOS
                        if (!UnityEngine.Input.location.isEnabledByUser) {
                            // TODO Failure
                            //debugText.text = "IOS and Location not enabled";
                            yield break;
                        }
                #endif            
                if (!Input.location.isEnabledByUser)
                {
                    // this is fallback code for using in editor when location services arent available.
                    #if UNITY_EDITOR
                        coordAreSet = "true";
                        Debug.Log("unity editor only code");
                        currentLatitude =  42.43208521562845;
                        currentLongitude = -72.58940386157315;
                        findLocationBtn.interactable = true;
                        loaderAni.SetActive(false);
                        findMyLocationBtnTxt.text = "Location set by unity editor";
                        OnFindLocation();
                    #endif                        
                   
                        yield break;
                }
                Input.location.Start();
                while(Input.location.status == LocationServiceStatus.Initializing)
                {
                        yield return new WaitForSeconds(1);
                }
                if (Input.location.status == LocationServiceStatus.Failed)
                {
                        //debugText.text = "Unable to determine device location";
                        yield break;
                }
                findLocationBtn.interactable = true;
                loaderAni.SetActive(false);  
                findMyLocationBtnTxt.text = "Find My Location";   
                
                currentLatitude = Input.location.lastData.latitude;
                currentLongitude = Input.location.lastData.longitude;
                coordAreSet = "true";

        }    
        public void Search()
        {
            if (googleMapsApi == ""|| googleMapsApi == null)
            {
           
                return;
            }

            if (inputField == null) {
                suggestedResultPanel.SetActive(false);
                return;
            }
            
            if (inputField.text.Length < 5) {
                suggestedResultPanel.SetActive(false);
                return;
            }
            suggestedResultPanel.SetActive(true);
            string locationName = inputField.text;

            OnlineMapsGoogleGeocoding request = new OnlineMapsGoogleGeocoding(locationName, googleMapsApi);
            request.OnComplete += OnGeocodingComplete;
            request.Send();
        }

        private void OnGeocodingComplete(string response)
        {
            OnlineMapsGoogleGeocodingResult[] results = OnlineMapsGoogleGeocoding.GetResults(response);
            if (results == null || results.Length == 0)
            {
                Debug.Log(response);
                return;
            }

            OnlineMapsGoogleGeocodingResult r = results[0];

            resulttext.text = r.formatted_address;
            string city = r.address_components[2].short_name;
            string state = r.address_components[5].long_name;
            Debug.Log("location"+ city+", "+state+" and long"+r.geometry_location.x+" and lat "+ r.geometry_location.y);
            if(isMapSearch){
                //maponlyfunctionality
            }else{
                if(isAvatar){
                    PlayerPrefs.SetString("location", city+", "+state);
                }else{
                    _addquest = GameObject.Find("addContentPanel").GetComponent<addQuest>();
                    _addquest.setLatLong(r.geometry_location.y, r.geometry_location.x);
                };
            }  
        }


        private void Update()
        {
            EventSystem eventSystem = EventSystem.current;
            if ((Input.GetKeyUp(KeyCode.KeypadEnter) || Input.GetKeyUp(KeyCode.Return)) && eventSystem.currentSelectedGameObject == inputField.gameObject)
            {
                Search();
            }
        }

        public void suggestionToField()
        {
            suggestedResultPanel.SetActive(false);
             inputField.text = resulttext.text;
        }
        public void setProfileLocationText(){
            if(isAvatar){
              liveLocationProfileTxt.text = inputField.text;  
            }else{
                _addquest = GameObject.Find("addContentPanel").GetComponent<addQuest>();
                _addquest.setLocationString(inputField.text);
            }
 
        }
        public void OnFindLocation()
        {
    
            if(coordAreSet == "false"){

                findLocationBtn.interactable = false;
                findMyLocationBtnTxt.text = "GPS Unavailable. Enter location manually."; 
                return;
            }
            OnlineMapsGoogleGeocoding request = new OnlineMapsGoogleGeocoding(currentLongitude,currentLatitude, googleMapsApi);
            request.Send();
            request.OnComplete += OnRequestComplete;
        }

        private void OnRequestComplete(string response)
        {
            OnlineMapsGoogleGeocodingResult[] results = OnlineMapsGoogleGeocoding.GetResults(response);
            if (results == null || results.Length == 0)
            {
                Debug.Log(response);
                return;
            }

            OnlineMapsGoogleGeocodingResult r = results[0];
            inputField.text = r.formatted_address;
            resulttext.text = r.formatted_address;
        }
 


    }
}