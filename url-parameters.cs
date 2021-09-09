     /// <summary>
    /// This method is called when the player adds a specific marker to the map.
    /// When it is called, it creates an instance of the 3d object on the map based on the Lat and Long values
    /// </summary>

    /// <param name="guildInt"></param>       
        
        
        void placeMarkerInitForm(int guildInt){

                // get authorid from global userid variable which is stored in playerprefs once authenticated
                int authorID = userID;

                //grabs the web view component so we can open browser with url string at end of function
                BrowserOpener _browseropener = GameObject.Find("BrowserOpener").GetComponent<BrowserOpener>();

                //gets the center of the screen which coincides with current gps location
                int x = (Screen.width / 2);
                int y = (Screen.height / 2);

                //gets surrent lat and long from screen location
                map.control.GetCoords(new Vector2(x,y), out double lng, out double lat);

                //our array of 3d prefabs this holds all the 3d prefabs that pop up on the map
                defaultPrefab = objectPrefabs[guildInt];

                //places object on map right before it launches the form
                OnlineMapsMarker3D marker3D = Create(lng, lat, defaultPrefab); 

                //scales based on guild see pg5 of game guide
                if(guildInt == 12){//scroll
                marker3D.scale = 30;       
                } else if(guildInt == 13) {//chest
                marker3D.scale = 10;
                } else if(guildInt == 14||guildInt == 15) {//key
                marker3D.scale = 800;
                } else {
                marker3D.scale = 300;//gems
                }   

                //opens web form with some parameters to define the data submission
                _browseropener.openLocationWindow( baseUrl + "/submit-data?titleid=" + lat + "&longitude=" + lng + "&latitude=" + lat + "&guild=" + guildInt + "&authorid=" + authorID + "&fromgame=true");
                    
        }    