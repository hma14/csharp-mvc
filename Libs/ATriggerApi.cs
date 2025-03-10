using ATriggerLib;
using Serilog;
using System.Collections.Generic;
using System.Configuration;

namespace Omnae.Libs
{
    public class ATriggerApi
    {
        public static void Initialize()
        {
            // Start ATrigger       
            ATrigger.Initialize(ConfigurationManager.AppSettings["ATrigger_API_Key"], ConfigurationManager.AppSettings["ATrigger_API_Secret"], false, true);
        }

        public static void Create(string typeName, string unit, string interval, int productId, string callback)
        {
            var tags = new Dictionary<string, string> {{"type", typeName}};

            //postData:
            var dataToBePostedByATrigger = new Dictionary<string, string> {{"productId", productId.ToString()}};
            
            var urlToTrigger = ConfigurationManager.AppSettings["OldSystem.URL"] + @"TaskDatas/" + callback;
            Log.Information($"Interval: {interval}, urlToTrigger: {urlToTrigger}, dataToBePostedByATrigger: {dataToBePostedByATrigger["productId"]}");
            ATrigger.Client.doCreate(unit, interval, urlToTrigger, tags, "", 1, 2, dataToBePostedByATrigger);
        }

        public static void Remove(string typeName)
        {
            var tags = new Dictionary<string, string> {{"type", typeName}};

            //Delete
            ATrigger.Client.doDelete(tags);
        }

        public static void Pause(string typeName)
        {
            var tags = new Dictionary<string, string> {{"type", typeName}};

            //Pause
            ATrigger.Client.doPause(tags);
        }
        public static void Resume(string typeName)
        {
            var tags = new Dictionary<string, string> {{"type", typeName}};

            //Pause
            ATrigger.Client.doResume(tags);
        }
    }
}
