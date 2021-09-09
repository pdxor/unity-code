            
            
            
            
        //calling the post data via json feed
        StartCoroutine(AllContentLoop(new WWW(baseUrl+"/wp-json/wp/v2/citizen_data?_embed&per_page=100&filter[tag]="+tagFilter)));

        //using SimpleJSON to parse the post data 
        IEnumerator AllContentLoop(WWW www )
        {
        
            yield return www;
            if(www.error == null)
            {

            //start the preloader
            loaderAni.SetActive(true); 
            
            //parse the json
            var N = JSON.Parse(www.text);

            //loop through results and add each to the map.
            for (int i = 0; i < N.Count; i++)
            {
                //our location
                double lat = N[i]["acf"]["latitude"];
                double lng = N[i]["acf"]["longitude"];

                //1-15 depending on guild and object type -> See pg5 in Game Guide
                int guildInt =  N[i]["acf"]["guild"];

                //set the 3d prefab based on guild ID see pg5 in Game Guide
                GameObject prefab = defaultPrefab;

                //our array of 3d prefabs this holds all the 3d prefabs that pop up on the map
                defaultPrefab = objectPrefabs[guildInt];

                //rescaling based on associated 3d object type
                if(guildInt == 12){//scroll
                marker3D.scale = 30;         
                } else if(guildInt == 13) {//chest
                marker3D.scale = 10;
                } else if(guildInt == 14||guildInt == 15) {//keys
                marker3D.scale = 900;
                } else {
                marker3D.scale = 300;//gems
                }

                //add the 3d object on the map now that we have location, Guild ID and Scale.
                OnlineMapsMarker3D marker3D = Create(lng, lat, defaultPrefab);                                                              
            } 

            //set map to 15 zoom once the objects are loaded
            SetZoom(15);
            //endpreloader
            loaderAni.SetActive(false); 

            } 

        }   