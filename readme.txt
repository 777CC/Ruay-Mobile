Install
-Unity3D
-JDK 1.8
-Android Studio
-Set andriod sdk path in unity3d
-Plugin facebooksdk
-OpenSSL64
-Set OpenSSL Path(environment variables in windows)
//-Set Java Path(C:\Program Files\Android\Android Studio\jre\bin)
-Set android minimun API Level same as facebook used.[in AndroidManifest.xml ->  <uses-sdk android:minSdkVersion="15" />]
-Add detail to developer facebook web.

Plugin
-AWSSDK Coginto,Lambda,SNS
-EnhancedScroller v2
-FacebookSDK
-LeanTween
-Log Viewer
-Google Mobile Ad

AWS Setting
-Set imagefolder's S3 in config.json in manualresize node
-create lamdba'zip and upload to s3
-Create stack in cloudformation

Aware
- Unity android must set Stripping Level to Disbled, otherwise it's can't instance AWSPrefab.
- home.json must be [Unicode (UTF-8 without signature)-Codepage 65001] format.
ManualResize simple
{
  "imageName": "test.jpg",
  "w": "200",
  "h": "200"
}

CognitoSynctrigger simple
{
  "datasetName": "datasetName",
  "eventType": "SyncTrigger",
  "region": "us-east-1",
  "identityId": "identityId",
  "datasetRecords": {
    "SampleKey2": {
      "newValue": "newValue2",
      "oldValue": "oldValue2",
      "op": "replace"
    },
    "SampleKey1": {
      "newValue": "newValue1",
      "oldValue": "oldValue1",
      "op": "replace"
    },
    "updateTime": {
      "newValue": "newValue1",
      "oldValue": "oldValue1",
      "op": "replace"
    }
  },
  "identityPoolId": "777",
  "version": 2
}

config
{
  "ImageSizes": {
    "SS": [
      "100x100",
      "777x777"
    ]
  },
  "ValueName": {
    "S": "ImageSizes"
  }
}