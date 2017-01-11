//
// Copyright 2014-2015 Amazon.com, 
// Inc. or its affiliates. All Rights Reserved.
// 
// Licensed under the AWS Mobile SDK For Unity 
// Sample Application License Agreement (the "License"). 
// You may not use this file except in compliance with the 
// License. A copy of the License is located 
// in the "license" file accompanying this file. This file is 
// distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, express or implied. See the License 
// for the specific language governing permissions and 
// limitations under the License.
//

//Add the Facebook Unity SDK and uncomment this to enable Facebook login
//#define USE_FACEBOOK_LOGIN

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Amazon;
using Amazon.CognitoSync;
using Amazon.Runtime;
using Amazon.CognitoIdentity;
using Amazon.CognitoIdentity.Model;
using Amazon.CognitoSync.SyncManager;
using Amazon.Lambda;
using Amazon.Lambda.Model;
using Amazon.S3;
using Amazon.S3.Model;
using Facebook.Unity;

namespace AWSSDK.Examples
{

    public class CognitoSyncManagerSample : MonoBehaviour
    {
        //public TextAsset testJson;
        public RawImage imageFromS3;
        public InputField playerNameUI, aliasUI;
        public Text statusMessageUI;
        private string playerName, alias;
		private string statusMessage = "";
        public Button InvokeButton = null;
        public InputField FunctionNameText = null;
        public InputField EventText = null;
        public Text ResultText = null;
        
        void Start()
        {
            //Ticket[] objects = JsonHelper.getJsonArray<Ticket>(testJson.text.Replace("\\\"","\""));
            //foreach(Ticket t in objects)
            //{
            //    Debug.Log(t.createdOn);
            //}

            //UnityInitializer.AttachToGameObject(gameObject);

            // Open your datasets
            //Manager.Instance.playerInfo = SyncManager.OpenOrCreateDataset("PlayerInfo");

            // Fetch locally stored data from a previous run
            string aliasVal = Manager.Instance.UserInfo.Get("alias");
            string playerNameVal = Manager.Instance.UserInfo.Get("alias");
            alias = string.IsNullOrEmpty(aliasVal) ? "Enter your alias" : aliasVal;
            playerName = string.IsNullOrEmpty(playerNameVal) ? "Enter your full name" : playerNameVal;
            
			// Define Synchronize callbacks
			// when ds.SynchronizeAsync() is called the localDataset is merged with the remoteDataset 
            // OnDatasetDeleted, OnDatasetMerged, OnDatasetSuccess,  the corresponding callback is fired.
            // The developer has the freedom of handling these events needed for the Dataset
            Manager.Instance.UserInfo.OnSyncSuccess += HandleSyncSuccess; // OnSyncSucess uses events/delegates pattern
            Manager.Instance.UserInfo.OnSyncFailure += HandleSyncFailure; // OnSyncFailure uses events/delegates pattern
            Manager.Instance.UserInfo.OnSyncConflict = HandleSyncConflict;
            Manager.Instance.UserInfo.OnDatasetMerged = HandleDatasetMerged;
            Manager.Instance.UserInfo.OnDatasetDeleted = HandleDatasetDeleted;
            

            InvokeButton.onClick.AddListener(() => { Invoke(); });
            updateUIValue();
        }
        
        public void SetPlayerName(string name)
        {
            playerName = name;
        }

        public void SetAlias(string aliasText)
        {
            alias = aliasText;
        }

        void updateUIValue()
        {
            statusMessageUI.text = statusMessage;
            playerNameUI.text = playerName;
            aliasUI.text = alias;
        }

        public void SaveOffline()
        {
            statusMessage = "Saving offline";

            Manager.Instance.UserInfo.Put("playerName", playerName);
            Manager.Instance.UserInfo.Put("alias", alias);

            string aliasVal = Manager.Instance.UserInfo.Get("alias");
            string playerNameVal = Manager.Instance.UserInfo.Get("alias");
            alias = string.IsNullOrEmpty(aliasVal) ? "Enter your alias" : aliasVal;
            playerName = string.IsNullOrEmpty(playerNameVal) ? "Enter your name" : playerNameVal;

            statusMessage = "Saved offline";
            updateUIValue();
        }

        public void LoadPhoto()
        {
            //Manager.Instance.Save();
            Manager.Instance.UpdateAppInfo();
            //ResultText.text = string.Format("fetching {0} from bucket {1}",
            //Manager.AppInfoFileName, Manager.AppInfoS3BucketName);
            //Manager.Instance.S3Client.GetObjectAsync(Manager.AppInfoS3BucketName, Manager.AppInfoFileName, (responseObj) =>
            //{
            //    if (responseObj.Exception == null)
            //    {
            //        string data = null;
            //        var response = responseObj.Response;
            //        if (response.ResponseStream != null)
            //        {
            //            //using (StreamReader reader = new StreamReader(response.ResponseStream))
            //            //{
            //            //    data = reader.ReadToEnd();
            //            //}
            //            byte[] myBinary = new byte[response.ResponseStream.Length];
            //            response.ResponseStream.Read(myBinary, 0, (int)response.ResponseStream.Length);
            //            Texture2D tex = new Texture2D(2, 2);
            //            tex.LoadImage(myBinary);

            //            imageFromS3.texture = tex;

            //            ResultText.text += "\n";
            //            ResultText.text += data;
            //        }
            //    }
            //    else
            //    {
            //        ResultText.text = responseObj.Exception.ToString();
            //    }
            //});
        }

        public void CognitoUpdate()
        {
            Manager.Instance.UserInfo.Put("updateTime", string.Empty);
        }

        public void CognitoSync()
        {
            statusMessage = "Saving to CognitoSync Cloud";
            Manager.Instance.UserInfo.Put("alias", alias);
            Manager.Instance.UserInfo.Put("playerName", playerName);

            Manager.Instance.UserInfo.Put("firstName", "chain");
            Manager.Instance.UserInfo.Put("lastName", "choonoi");
            Manager.Instance.UserInfo.Put("email", "chain777@hotmail.com");
            Manager.Instance.UserInfo.Put("gender", "male");
            Manager.Instance.UserInfo.Put("tel", "0847052202");
            Manager.Instance.UserInfo.Put("birthday", "621129600000");
            Manager.Instance.UserInfo.Put("interest", "#dev#game#glassesgirl");


            Manager.Instance.UserInfo.SynchronizeAsync();
            
            updateUIValue();
        }

        public void DeleteLocalfile()
        {
            //statusMessage = "Deleting all local data";
            //SyncManager.WipeData(false);
            //playerName = "Enter your name";
            //alias = "Enter your alias";
            //statusMessage = "Deleting all local data complete. ";

            //updateUIValue();
        }

        public void ConnectFacebook()
        {
#if USE_FACEBOOK_LOGIN
            statusMessage = "Connecting to Facebook";
            if (!FB.IsInitialized)
            {
                FB.Init(delegate ()
                {
                    Debug.Log("starting thread");
                    // shows to connect the current identityid or create a new identityid with facebook authentication
                    Login();
                });
            }
            else
            {
                Login();
            }
#else
                 statusMessage = "Not Facebook.";
#endif
            updateUIValue();
        }


        #region Public Authentication Providers
#if USE_FACEBOOK_LOGIN
        void Login()
        {
            FB.LogInWithReadPermissions(new List<string>() { "public_profile", "email", "user_friends" }, FacebookLoginCallback);
        }

        private void FacebookLoginCallback(IResult result)
        {
            Debug.Log("FB.Login completed");

            if (result.Error != null || !FB.IsLoggedIn)
            {
                Debug.LogError(result.Error);
                statusMessage = result.Error;
            }
            else
            {
                Debug.Log(result);
                Manager.Instance.FBLogin();
                //CognitoSync();
                LoadPhoto();
            }
        }

#endif
        #endregion

        #region Sync events
        private bool HandleDatasetDeleted(Dataset dataset)
        {
            statusMessage = dataset.Metadata.DatasetName + "Dataset has been deleted.";
            Debug.Log(dataset.Metadata.DatasetName + " Dataset has been deleted");

            // Clean up if necessary 
            updateUIValue();

            // returning true informs the corresponding dataset can be purged in the local storage and return false retains the local dataset
            return true;
        }

        public bool HandleDatasetMerged(Dataset dataset, List<string> datasetNames)
        {
            statusMessage = "Merging datasets between different identities.";
            Debug.Log(dataset + " Dataset needs merge");

            updateUIValue();
            // returning true allows the Synchronize to resume and false cancels it
            return true;
        }

        private bool HandleSyncConflict(Amazon.CognitoSync.SyncManager.Dataset dataset, List<SyncConflict> conflicts)
        {
            statusMessage = "Handling Sync Conflicts.";
            Debug.Log("OnSyncConflict");
            List<Amazon.CognitoSync.SyncManager.Record> resolvedRecords = new List<Amazon.CognitoSync.SyncManager.Record>();

            foreach (SyncConflict conflictRecord in conflicts)
            {
                // This example resolves all the conflicts using ResolveWithRemoteRecord 
                // SyncManager provides the following default conflict resolution methods:
                //      ResolveWithRemoteRecord - overwrites the local with remote records
                //      ResolveWithLocalRecord - overwrites the remote with local records
                //      ResolveWithValue - for developer logic  
                resolvedRecords.Add(conflictRecord.ResolveWithRemoteRecord());
            }

            // resolves the conflicts in local storage
            dataset.Resolve(resolvedRecords);

            //updateUIValue();

            // on return true the synchronize operation continues where it left,
            //      returning false cancels the synchronize operation
            return true;
        }

        private void HandleSyncSuccess(object sender, SyncSuccessEventArgs e)
        {

            var dataset = sender as Dataset;

			if (dataset.Metadata != null) {
            	Debug.Log("Successfully synced for dataset: " + dataset.Metadata);
			} else {
				Debug.Log("Successfully synced for dataset");
			}

            if (dataset == Manager.Instance.UserInfo)
            {
                string aliasVal = Manager.Instance.UserInfo.Get("alias");
                string playerNameVal = Manager.Instance.UserInfo.Get("alias");

                alias = string.IsNullOrEmpty(aliasVal) ? "Enter your alias" : aliasVal;
                playerName = string.IsNullOrEmpty(playerNameVal) ? "Enter your name" : playerNameVal;
            }
            statusMessage = "Syncing to CognitoSync Cloud succeeded";

            updateUIValue();
            Debug.Log("HandleSyncSuccess : " + playerName);
        }

        private void HandleSyncFailure(object sender, SyncFailureEventArgs e)
        {
            var dataset = sender as Dataset;
            Debug.Log("Sync failed for dataset : " + dataset.Metadata.DatasetName);
            Debug.LogException(e.Exception);

            statusMessage = "Syncing to CognitoSync Cloud failed";
            updateUIValue();
        }
        #endregion


        #region private members
        
        

        #endregion

        #region Invoke
        /// <summary>
        /// Example method to demostrate Invoke. Invokes the Lambda function with the specified
        /// function name (e.g. helloWorld) with the parameters specified in the Event JSON.
        /// Because no InvokationType is specified, the default 'RequestResponse' is used, meaning
        /// that we expect the AWS Lambda function to return a value.
        /// </summary>
        public void Invoke()
        {
            ResultText.text = "Invoking '" + FunctionNameText.text + " function in Lambda... \n";
            Manager.Instance.LambdaClient.InvokeAsync(new Amazon.Lambda.Model.InvokeRequest()
            {
                FunctionName = FunctionNameText.text,
                Payload = EventText.text
            },
            (responseObject) =>
            {
                ResultText.text += "\n";
                if (responseObject.Exception == null)
                {
                    ResultText.text += Encoding.ASCII.GetString(responseObject.Response.Payload.ToArray()) + "\n";
                }
                else
                {
                    ResultText.text += responseObject.Exception + "\n";
                }
            }
            );
        }

        #endregion

        #region List Functions
        /// <summary>
        /// Example method to demostrate ListFunctions
        /// </summary>
        public void ListFunctions()
        {
            ResultText.text = "Listing all of your Lambda functions... \n";
            Manager.Instance.LambdaClient.ListFunctionsAsync(new Amazon.Lambda.Model.ListFunctionsRequest(),
            (responseObject) =>
            {
                ResultText.text += "\n";
                if (responseObject.Exception == null)
                {
                    ResultText.text += "Functions: \n";
                    foreach (FunctionConfiguration function in responseObject.Response.Functions)
                    {
                        ResultText.text += "    " + function.FunctionName + "\n";
                    }
                }
                else
                {
                    ResultText.text += responseObject.Exception + "\n";
                }
            }
            );
        }

        #endregion
    }
}
