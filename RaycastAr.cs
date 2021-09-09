
   /// <summary>
   /// /////// We are listening for touch in AR mode where there are GPS located 3d objects with colliders. 
   //  /////// User must click to submit proof of quest completion which happens through an in app webview
   /// </summary>

// Update function looks for touch
void Update()
    {
             
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            
            touchPosition = touch.position;

            if(touch.phase == TouchPhase.Began)
            {
                Ray ray = arCamera.ScreenPointToRay(touch.position);
                RaycastHit hit;
                if(Physics.Raycast(ray, out hit))
                {
  					if(hit.collider.tag == "chest" ||hit.collider.tag == "gem" || hit.collider.tag == "key" || hit.collider.tag == "scroll"){

                        //gets webview component
                        GetWebView getwebview = hit.collider.gameObject.GetComponent<GetWebView>();
                        
                        //opens web view canvas with the
                        getwebview.canvasOpen();
                			
		                		
		            
		            }                      

                }

            }
        }

    }


    //in getwebview the object holds lat and long info and userinfo in order to submit proof of quest submission.
    void canvasOpen()
    {
        //opens browser with proper data allowing you to submit proof of completion
        _browseropener.openLocationWindow(baseUrl + "/proof-of-completion/?titleid=" + lat+"&longitude="+lng+"&latitude="+lat+"&guild=" + guildId + "&authorid=" + userID + "&fromAR=true");
    }
