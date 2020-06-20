using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
//using Serilog;

namespace DemonstrationHarness
{
    public class Harness
    {
        internal bool RunLocal { get; set; }

        internal void RunFoundDefectService()
        {
            //Log.Information("Running RunFoundDefectService");
            //exercise the web service
            try
            {
                string serialNumber = "FY99255G73XMNH09F";
                string cameraId = "CAM03";
                Console.WriteLine("The program is about to request a found defect." +
                    $"\r\nIt will request a list of defects for part {serialNumber}." +
                    "\r\nPress a key to send the request to the web service:");
                Console.ReadKey();
                Console.WriteLine();
                using (HttpClient httpClient = new HttpClient())
                {
                    List<string> stringList = new List<string>();
                    string localString = $"https://localhost:44306/api/founddefects/serialnumber/{serialNumber}/cameraid/{cameraId}";
                    string azureString = $"https://founddefect.azurewebsites.net/api/founddefects/serialnumber/{serialNumber}/cameraid/{cameraId}";
                    string url = RunLocal ? localString : azureString;
                    Console.WriteLine($"Sending an HTTP message to {url}");
                    var response = httpClient.GetAsync(new Uri(url)).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("\r\nThe API call was successful.\r\nThe data returned is:\r\n");
                        var str = response.Content.ReadAsStringAsync().Result;
                        Debug.WriteLine(Clean(str));
                        Console.WriteLine(Clean(str));
                    }
                    else
                    {
                        var message = response.Content.ReadAsStringAsync().Result;
                        Console.WriteLine($"\r\nThe API call failed. The failure message is: {response.ReasonPhrase}.\r\n{message}\r\n");
                        Console.WriteLine("Press any key to continue.");
                        Console.ReadKey();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(string.Concat("\r\n", e.Message, e.StackTrace, "\r\n"));
            }

            Console.WriteLine("Press any key to continue to the next test.\r\n");
            Console.ReadKey();
        }

        internal void RunTaggedDefectService()
        {
            //Log.Information("Running RunTaggedDefectService");
            //exercise the web service
            try
            {
                List<string> rawJsonStringList = new List<string>
                { "{\"Part\": \"0003\", \"ZeissPartId\": \"42879DBE-0037-4C85-93F8-BCAFB29B2E7A\",\"TimeStamp\":\"19000101T120000\", \"SerialNumber\": \"G0L9373K7ZAMNH39D\", \"ZeissLineNumber\": \"1\", \"RawImagePath\": \"//root/folder/subfolder\", \"PieceStartCd\": 3600, \"PieceStopCd\": 0, \"PieceStartMd\": 2400, \"PieceStopMd\": 0, \"PieceCsTransform\": \"[0 1 -140; -1 0 6272; 0 0 1]\", \"date_taken\":\"20200329T123110\", \"TaggedDefects\": [{\"TaggedDefectId\": 0, \"DefectName\": \"Texture NU (Area)\", \"DefectTypeId\": 1, \"ZeissDefectId\": \"D7E3AB07-6AAB-420E-A5EC-22BDEACC502A\", \"ZeissPartId\": \"42879DBE-0037-4C85-93F8-BCAFB29B2E7A\", \"PartId\": \"0003\", \"DefectCameraId\": \"CAM010\", \"TaggerName\": \"Tele\", \"TaggerTimeStamp\": \"20200329T123110\", \"GroundhogStation\": \"test station id\", \"DefectROI\": {\"x\": 420, \"y\": 150, \"width\": 180, \"height\": 120}, \"DefectLevel\": \"NG\"}]}"
                    /*"{\"Part\": \"0003\", \"ZeissPartId\": \"42879DBE-0037-4C85-93F8-BCAFB29B2E7A\", \"SerialNumber\": \"G0L9373K7ZAMNH39D\", \"ZeissLineNumber\": \"1\", \"RawImagePath\": \"//root/folder/subfolder\", \"PieceStartCd\": 3600, \"PieceStopCd\": 0, \"PieceStartMd\": 2400, \"PieceStopMd\": 0, \"PieceCsTransform\": \"[0 1 -140; -1 0 6272; 0 0 1]\", \"TaggedDefects\": [{\"TaggedDefectId\": 0, \"DefectName\": \"Texture NU (Area)\", \"DefectTypeId\": \"1\",  \"ZeissDefectId\": \"D7E3AB07-6AAB-420E-A5EC-22BDEACC502A\", \"ZeissPartId\": \"42879DBE-0037-4C85-93F8-BCAFB29B2E7A\", \"PartId\": \"0003\", \"DefectCameraId\": \"CAM010\", \"TaggerName\": \"default user (12345-Inspector)\", \"TaggerTimeStamp\": \"20200329T123110\", \"GroundhogStation\": \"test station id\", \"DefectROI\": {\"x\": 420, \"y\": 150,  \"width\": 180, \"height\": 120}, \"DefectLevel\": \"NG\"}]}",
                    "{\"Part\": \"0003\", \"ZeissPartId\": \"42879DBE-0037-4C85-93F8-BCAFB29B2E7A\", \"SerialNumber\": \"G0L9373K7ZAMNH39D\", \"ZeissLineNumber\": \"1\", \"RawImagePath\": \"//root/folder/subfolder\", \"PieceStartCd\": 3600, \"PieceStopCd\": 0, \"PieceStartMd\": 2400, \"PieceStopMd\": 0, \"PieceCsTransform\": \"[0 1 -140; -1 0 6272; 0 0 1]\", \"TaggedDefects\": [{\"TaggedDefectId\": 1, \"DefectName\": \"Texture NU (Line)\", \"DefectTypeId\": \"3\",  \"ZeissDefectId\": \"7CE85D8B-730D-4205-AEF5-24C6FBB7D3FD\", \"ZeissPartId\": \"42879DBE-0037-4C85-93F8-BCAFB29B2E7A\", \"PartId\": \"0003\", \"DefectCameraId\": \"CAM010\", \"TaggerName\": \"default user (12345-Inspector)\", \"TaggerTimeStamp\": \"20200421T103840\", \"GroundhogStation\": \"test station id\", \"DefectROI\": {\"x\": 980, \"y\": 320,  \"width\": 300, \"height\": 280}, \"DefectLevel\": \"INSPEC\"}]}",
                    "{\"Part\": \"0003\", \"ZeissPartId\": \"42879DBE-0037-4C85-93F8-BCAFB29B2E7A\", \"SerialNumber\": \"G0L9373K7ZAMNH39D\", \"ZeissLineNumber\": \"1\", \"RawImagePath\": \"//root/folder/subfolder\", \"PieceStartCd\": 3600, \"PieceStopCd\": 0, \"PieceStartMd\": 2400, \"PieceStopMd\": 0, \"PieceCsTransform\": \"[0 1 -140; -1 0 6272; 0 0 1]\", \"TaggedDefects\": [{\"TaggedDefectId\": 0, \"DefectName\": \"Camera Hole Distortion\", \"DefectTypeId\": \"42\", \"ZeissDefectId\": \"D7E3AB07-6AAB-420E-A5EC-22BDEACC502A\", \"ZeissPartId\": \"42879DBE-0037-4C85-93F8-BCAFB29B2E7A\", \"PartId\": \"0003\", \"DefectCameraId\": \"CAM010\", \"TaggerName\": \"default user (12345-Inspector)\", \"TaggerTimeStamp\": \"20200421T103840\", \"GroundhogStation\": \"test station id\", \"DefectROI\": {\"x\": 420, \"y\": 150,  \"width\": 180, \"height\": 120}, \"DefectLevel\": \"NG\"}]}",
                    "{\"Part\": \"0003\", \"ZeissPartId\": \"42879DBE-0037-4C85-93F8-BCAFB29B2E7A\", \"SerialNumber\": \"G0L9373K7ZAMNH39D\", \"ZeissLineNumber\": \"1\", \"RawImagePath\": \"//root/folder/subfolder\", \"PieceStartCd\": 3600, \"PieceStopCd\": 0, \"PieceStartMd\": 2400, \"PieceStopMd\": 0, \"PieceCsTransform\": \"[0 1 -140; -1 0 6272; 0 0 1]\", \"TaggedDefects\": [{\"TaggedDefectId\": 1, \"DefectName\": null, \"DefectTypeId\": null,   \"ZeissDefectId\": \"7CE85D8B-730D-4205-AEF5-24C6FBB7D3FD\", \"ZeissPartId\": \"42879DBE-0037-4C85-93F8-BCAFB29B2E7A\", \"PartId\": \"0003\", \"DefectCameraId\": \"CAM09\",  \"TaggerName\": \"default user (12345-Inspector)\", \"TaggerTimeStamp\": \"20200421T103840\", \"GroundhogStation\": \"test station id\", \"DefectROI\": {\"x\": 410, \"y\": 3370, \"width\": 200, \"height\": 150}, \"DefectLevel\": \"OK\"}]}",
                    "{\"Part\": \"0003\", \"ZeissPartId\": \"42879DBE-0037-4C85-93F8-BCAFB29B2E7A\", \"SerialNumber\": \"G0L9373K7ZAMNH39D\", \"ZeissLineNumber\": \"1\", \"RawImagePath\": \"//root/folder/subfolder\", \"PieceStartCd\": 3600, \"PieceStopCd\": 0, \"PieceStartMd\": 2400, \"PieceStopMd\": 0, \"PieceCsTransform\": \"[0 1 -140; -1 0 6272; 0 0 1]\", \"TaggedDefects\": [{\"TaggedDefectId\": 0, \"DefectName\": \"Graininess\", \"DefectTypeId\": \"4\", \"ZeissDefectId\": \"D7E3AB07-6AAB-420E-A5EC-22BDEACC502A\", \"ZeissPartId\": \"42879DBE-0037-4C85-93F8-BCAFB29B2E7A\", \"PartId\": \"0003\", \"DefectCameraId\": \"CAM09\", \"TaggerName\": \"default user (12345-Inspector)\", \"TaggerTimeStamp\": \"20200421T103840\", \"GroundhogStation\": \"test station id\", \"DefectROI\": {\"x\": 1230.5, \"y\": 90.5, \"width\": 290.5, \"height\": 650.5}, \"DefectLevel\": \"INSPEC\"}]}",
                    "{\"Part\": \"0003\", \"ZeissPartId\": \"42879DBE-0037-4C85-93F8-BCAFB29B2E7A\", \"SerialNumber\": \"G0L9373K7ZAMNH39D\", \"ZeissLineNumber\": \"1\", \"RawImagePath\": \"//root/folder/subfolder\", \"PieceStartCd\": 3600, \"PieceStopCd\": 0, \"PieceStartMd\": 2400, \"PieceStopMd\": 0, \"PieceCsTransform\": \"[0 1 -140; -1 0 6272; 0 0 1]\", \"TaggedDefects\": [{\"TaggedDefectId\": 1, \"DefectName\": \"Spline Chip\", \"DefectTypeId\": \"66\", \"ZeissDefectId\": \"7CE85D8B-730D-4205-AEF5-24C6FBB7D3FD\", \"ZeissPartId\": \"42879DBE-0037-4C85-93F8-BCAFB29B2E7A\", \"PartId\": \"0003\", \"DefectCameraId\": \"CAM04\", \"TaggerName\": \"default user (12345-Inspector)\", \"TaggerTimeStamp\": \"20200421T103840\", \"GroundhogStation\": \"test station id\", \"DefectROI\": {\"x\": 360, \"y\": 3360, \"width\": 160, \"height\": 110}, \"DefectLevel\": \"NG\"}]}",
                    "{\"Part\": \"0003\", \"ZeissPartId\": \"42879DBE-0037-4C85-93F8-BCAFB29B2E7A\", \"SerialNumber\": \"G0L9373K7ZAMNH39D\", \"ZeissLineNumber\": \"1\", \"RawImagePath\": \"//root/folder/subfolder\", \"PieceStartCd\": 3600, \"PieceStopCd\": 0, \"PieceStartMd\": 2400, \"PieceStopMd\": 0, \"PieceCsTransform\": \"[0 1 -140; -1 0 6272; 0 0 1]\", \"TaggedDefects\": [{\"TaggedDefectId\": 0, \"DefectName\": \"Area Discoloration\", \"DefectTypeId\": \"17\", \"ZeissDefectId\": \"D7E3AB07-6AAB-420E-A5EC-22BDEACC502A\", \"ZeissPartId\": \"42879DBE-0037-4C85-93F8-BCAFB29B2E7A\", \"PartId\": \"0003\", \"DefectCameraId\": \"CAM04\", \"TaggerName\": \"default user (12345-Inspector)\", \"TaggerTimeStamp\": \"20200421T103840\", \"GroundhogStation\": \"test station id\", \"DefectROI\": {\"x\": 950, \"y\": 2210, \"width\": 170, \"height\": 140}, \"DefectLevel\": \"NG\"}]}",
                    "{\"Part\": \"0003\", \"ZeissPartId\": \"42879DBE-0037-4C85-93F8-BCAFB29B2E7A\", \"SerialNumber\": \"G0L9373K7ZAMNH39D\", \"ZeissLineNumber\": \"1\", \"RawImagePath\": \"//root/folder/subfolder\", \"PieceStartCd\": 3600, \"PieceStopCd\": 0, \"PieceStartMd\": 2400, \"PieceStopMd\": 0, \"PieceCsTransform\": \"[0 1 -140; -1 0 6272; 0 0 1]\", \"TaggedDefects\": [{\"TaggedDefectId\": 1, \"DefectName\": null, \"DefectTypeId\": null, \"ZeissDefectId\": \"7CE85D8B-730D-4205-AEF5-24C6FBB7D3FD\", \"ZeissPartId\": \"42879DBE-0037-4C85-93F8-BCAFB29B2E7A\", \"PartId\": \"0003\", \"DefectCameraId\": \"CAM03\", \"TaggerName\": \"default user (12345-Inspector)\", \"TaggerTimeStamp\": \"20200421T103840\", \"GroundhogStation\": \"test station id\", \"DefectROI\": {\"x\": 1180, \"y\": 2950, \"width\": 600, \"height\": 600}, \"DefectLevel\": \"OK\"}]}",
                    "{\"Part\": \"0003\", \"ZeissPartId\": \"42879DBE-0037-4C85-93F8-BCAFB29B2E7A\", \"SerialNumber\": \"G0L9373K7ZAMNH39D\", \"ZeissLineNumber\": \"1\", \"RawImagePath\": \"//root/folder/subfolder\", \"PieceStartCd\": 3600, \"PieceStopCd\": 0, \"PieceStartMd\": 2400, \"PieceStopMd\": 0, \"PieceCsTransform\": \"[0 1 -140; -1 0 6272; 0 0 1]\", \"TaggedDefects\": [{\"TaggedDefectId\": 0, \"DefectName\": \"Spline Chip\", \"DefectTypeId\": \"66\", \"ZeissDefectId\": \"D7E3AB07-6AAB-420E-A5EC-22BDEACC502A\", \"ZeissPartId\": \"42879DBE-0037-4C85-93F8-BCAFB29B2E7A\", \"PartId\": \"0003\", \"DefectCameraId\": \"CAM03\", \"TaggerName\": \"default user (12345-Inspector)\", \"TaggerTimeStamp\": \"20200421T103840\", \"GroundhogStation\": \"test station id\", \"DefectROI\": {\"x\": 370, \"y\": 170, \"width\": 170, \"height\": 80}, \"DefectLevel\": \"NG\"}]}"*/
                };

                using (HttpClient httpClient = new HttpClient())                         
                {
                    List<string> stringList = new List<string>();
                    string localString = "https://localhost:44365/api/taggeddefects";
                    string azureString = "https://taggeddefect.azurewebsites.net/api/taggeddefects";
                    foreach (var rawJsonString in rawJsonStringList)
                    { 
                        var payload = JsonConvert.SerializeObject(Clean(rawJsonString));
                        string url = RunLocal ? localString : azureString;
                        Console.WriteLine("The program is about to post a tagged defect." +
                                            "\r\nThe JSON for this post is:\r\n" +
                                            Clean(rawJsonString) +
                                            "\r\nPress a key to send the post to the web service:");
                        Console.ReadKey();
                        Console.WriteLine($"\r\nSending an HTTP message to {url}");
                        HttpContent httpContent = new StringContent(payload, Encoding.UTF8, "application/json");
                        var response = httpClient.PostAsync(new Uri(url), httpContent).Result;
                        httpContent.Dispose();
                        if (response.IsSuccessStatusCode)
                        {
                            Console.WriteLine($"\r\nThe API call was successful.\r\nThe result is: {response.ReasonPhrase}  {response.Headers.ToString()}");
                        }
                        else
                        {
                            Console.WriteLine($"\r\nThe API call failed. The failure message is: {response.ReasonPhrase}\r\n");
                            Console.WriteLine("Press any key to continue.");
                            Console.ReadKey();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(string.Concat("\r\n", e.Message, e.StackTrace, "\r\n"));
                WriteInnerException(e);
            }

            Console.WriteLine("\r\nPress any key to continue to the next test.\r\n");
            Console.ReadKey();
        }

        internal void RunQ4LineOutputService()
        {
            //Log.Information("Running RunQ4LineOutputService");
            //exercise the web service
            try
            {
                string rawJsonString = "{\"PartId\":\"FM77E3J2101234567\",\"FieldOfView\":\"D42_CAM03_A_P3\",\"PieceStartCD\":3600,\"PieceStopCD\":0,\"PieceStartMD\":2400,\"PieceStopMD\":0,\"Transform\":\"[0 1 -140; -1 0 6272; 0 0 1]\",\"DefectType\":\"BG Dent Cam03\",\"SegmentDataTable\":[{\"SegmentLabel\":1,\"Area\":147.0,  \"CenterOfGravity.X\":2851.625,  \"CenterOfGravity.Y\":12.625,  \"BoundingBox.Left\":2840.0,  \"BoundingBox.Right\":2864.0,  \"BoundingBox.Top\":1003.0,  \"BoundingBox.Bottom\":1023.0,  \"BoundingBox.Width\":24.0,  \"BoundingBox.Height\":20.0,  \"BoundingBox.Area\":480.0,  \"Compactness\":4.1921628547742769,  \"Elongatedness\":3.601088125737518,  \"Feret.Minimum\":0.318467597478957,  \"Feret.Maximum\":29.955534889613773,  \"Frayness\":0.8698636928884711,  \"MajorMinorAxesLength.MinorAxisLength\":10.353689193725586,  \"MajorMinorAxesLength.MajorAxisLength\":36.172119140625,  \"Perimeter\":88.0,  \"Roundness\":0.41057736281489504,  \"Solidity\":0.72413793103448276,  \"PixelStatistic_LFImage.MinPixelValue\":-0.71479922533035278,  \"PixelStatistic_LFImage.MaxPixelValue\":1.1754943508222875E-38,  \"PixelStatistic_LFImage.StandardDeviation\":0.069494202970956973,  \"PixelStatistic_LFImage.RootMeanSquareError\":0.5586919147980518,  \"PixelStatistic_Grayscale.MinPixelValue\":142.39999389648438,  \"PixelStatistic_Grayscale.MaxPixelValue\":178.0,  \"PixelStatistic_Grayscale.Mean\":147.56462543513499,  \"CombinedScore\":14.23976399444328,  \"Graymean\":53.71474235900553,\"Diff\":1.4318138957023621,\"PreviousRunResultExists\":false,\"HasMatch\":true,\"Result\":\"IsOk\",\"Color\":\"#FF008000\"}]}";

                using (HttpClient httpClient = new HttpClient())
                {
                    List<string> stringList = new List<string>();
                    string localString = "https://localhost:44325/api/q4lineoutput/post";
                    string azureString = "https://q4lineoutput.azurewebsites.net/api/q4lineoutput/post";
                    var payload = JsonConvert.SerializeObject(Clean(rawJsonString));
                    string url = RunLocal ? localString : azureString;
                    Console.WriteLine("The program is about to post a Q4Line output document." +
                                        "\r\nThe JSON for this post is:\r\n" +
                                        Clean(rawJsonString) +
                                        "\r\nPress a key to send the post to the web service:");
                    Console.ReadKey();
                    Console.WriteLine($"\r\nSending an HTTP message to {url}");
                    HttpContent httpContent = new StringContent(payload, Encoding.UTF8, "application/json");
                    var response = httpClient.PostAsync(new Uri(url), httpContent).Result;
                    httpContent.Dispose();
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"\r\nThe API call was successful.\r\nThe result is: {response.ReasonPhrase}  {response.Headers.ToString()}");
                    }
                    else
                    {
                        Console.WriteLine($"\r\nThe API call failed. The failure message is: {response.ReasonPhrase}\r\n");
                        Console.WriteLine("Press any key to continue.");
                        Console.ReadKey();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(string.Concat("\r\n", e.Message, e.StackTrace, "\r\n"));
                WriteInnerException(e);
            }

            Console.WriteLine("\r\nPress any key to end the program.");
            Console.ReadKey();
        }

        private static string Clean(string str)
        {
            return str.Replace("\\", "", StringComparison.InvariantCulture).Replace("\"{", "{", StringComparison.InvariantCulture) + "\r\n";
        }

        private void WriteInnerException(Exception e)
        {
            if (e.InnerException != null)
            {
                Console.WriteLine(e.InnerException.Message);
                Console.WriteLine(e.InnerException.StackTrace);
                WriteInnerException(e.InnerException);// yes, this is supposed to be recursive.
            }
        }


    }
}

