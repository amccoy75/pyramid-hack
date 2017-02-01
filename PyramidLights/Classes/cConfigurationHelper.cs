using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

public class cConfigurationHelper
{
    private Dictionary<string, string> data = null;
   public cConfigurationHelper()
    {
        data = new Dictionary<string, string>();
        foreach (var row in File.ReadAllLines(@"pyramidLights.config"))
            data.Add(row.Split('=')[0], string.Join("=", row.Split('=').Skip(1).ToArray()));
    }

    public string apiKey()
    {
        return data["apiKey"];
    }
    public string spreadsheetID()
    {
        return data["spreadsheetID"];
    }
    public string consumerSecret()
    {
        return data["consumerSecret"];
    }
    public string consumerKey()
    {
        return data["consumerKey"];
    }
    public string accessTokenSecret()
    {
        return data["accessTokenSecret"];
    }
    public string accessToken()
    {
        return data["accessToken"];
    }
    public string pyramidTrigger()
    {
        return data["pyramidTrigger"];
    }
    public string pyramidScript()
    {
        return data["pyramidScript"];
    }
    public string eMail()
    {
        return data["eMail"];
    }
};

