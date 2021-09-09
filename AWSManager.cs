using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Amazon;
using Amazon.CognitoIdentity;
using Amazon.S3;
using Amazon.S3.Model;
using UnityEngine.UI;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


public class AWSManager : MonoBehaviour
{

   /// <summary>
   /// ///////We are connecting to AWS and uploading videos and photos that were chosen using Native Gallery for IOS and Android
   /// </summary>

    //set with the identity pool which can upload files to our bucket
    public string IdentityPoolId = "removedforsecurity";

    //set to closest region
    public string Region = RegionEndpoint.USEast2.SystemName;

    private static AWSManager _instance;

    public Button nextSaveBtn;
    public GameObject universalPreloader;

    public Text statusText;
    public string vidURL;

    public string imgURL;

    public bool isVideoUpload;

    public bool isAvatarImage;

    public bool isQuestImage;

    public static AWSManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.Log("Instance is Null");
            }
            return _instance;
        }
    }

    private CognitoAWSCredentials _credentials;

    private CognitoAWSCredentials Credentials
    {
        get
        {
            if (_credentials == null)
            {
                _credentials = new CognitoAWSCredentials(IdentityPoolId, RegionEndpoint.USEast2);
            }
            return _credentials;
        }
    }

    public string identityId;


    //S3 Setup
    public string S3Region = RegionEndpoint.USEast2.SystemName;
    private RegionEndpoint _S3Region
    {
        get
        {
            return RegionEndpoint.GetBySystemName(S3Region);
        }
    }

    private AmazonS3Client _S3Client;

    public AmazonS3Client S3Client
    {
        get
        {
            if (_S3Client == null)
            {
                _S3Client = new AmazonS3Client(new CognitoAWSCredentials(IdentityPoolId, RegionEndpoint.USEast2), _S3Region);
            }
            return _S3Client;
        }
    }



     public addQuest _addquest;


    void Start()
    {
       
        Debug.Log("start!");
        _instance = this;

        UnityInitializer.AttachToGameObject(this.gameObject);
        try
        {
           // AWS connection is working!
            AWSConfigs.HttpClient = AWSConfigs.HttpClientOption.UnityWebRequest;

        }
        catch (Exception e)
        {
            // AWS connection is not working!);
        }

        Credentials.GetIdentityIdAsync(delegate (AmazonCognitoIdentityResult<string> result)
        {
            if (result.Exception != null)
            {
                Debug.Log(result.Exception);
            }
            string identityId = result.Response;
            Debug.Log(identityId);
        });

    }




    public Button avatarSelect;


    public void Upload(byte[] data, string filename, string type)
    {
        //Deactivating Avatar selection navigation
        if(type == "isAvatarImage"){
            avatarSelect.interactable = false;
        }
        nextSaveBtn.interactable = false;       
        universalPreloader.SetActive(true);
        MemoryStream ms = new MemoryStream(data);
        Debug.Log( "starting upload" );
        PostObjectRequest request = new PostObjectRequest()
        {
            Bucket = "removedforsecurity",
            Key = filename,
            InputStream = ms,
            CannedACL = S3CannedACL.Private,
            Region = _S3Region
        };

        S3Client.PostObjectAsync(request, (responseObj) =>
        {
            Debug.Log( "PostObjectAsync started");
            if (responseObj.Exception == null)
            {
                Debug.Log("Uploaded!! Whew!!");

                if(type =="video"){
                    vidURL = baseAWSUrl+filename;
                    Debug.Log("succesfully uploaded "+vidURL+" to AWS. Now setting vidurl in addquest script");
                    _addquest = GameObject.Find("addContentPanel").GetComponent<addQuest>();
                    _addquest.setVideoUrl(vidURL);
                }
                if(type == "isQuestImage"){
                    imgURL = baseAWSUrl+filename;
                    
                    _addquest = GameObject.Find("addContentPanel").GetComponent<addQuest>();
                    _addquest.setImageUrl(imgURL);   
                }


               universalPreloader.SetActive(false);
               nextSaveBtn.interactable = true;
               statusText.text = "Uploaded Quest Cover Image succesfully to the cloud! Click the forward arrow to submit.";

            } else
            {
                //response with errors
                Debug.Log("Error" + responseObj.Exception);
                statusText.text = "Error" + responseObj.Exception;

                //turn off preloader and let form nav buttons work again so user can skip media upload
                universalPreloader.SetActive(false);
                nextSaveBtn.interactable = true;
                avatarSelect.interactable = true;
            }
        });
    }

    
}
