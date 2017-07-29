using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;


    class PyramidLightsMain
    {


    private const string EventLogSource = "PyramidLights";
    static void Main(string[] args)
    {

        cConfigurationHelper  configHelper = new cConfigurationHelper();

        if (!EventLog.SourceExists(EventLogSource))
            EventLog.CreateEventSource(EventLogSource, "Creating log source for Pyramid Lights");


        try
        {
            while (true)
            {
                DateTime dtStartToday = DateTime.Now;
                DateTime dtResetDay = DateTime.Now;
                cTwitterHelper twitterHelper = new cTwitterHelper(configHelper);
                cQuestion question = new cQuestion(configHelper);

                //get question from googledocs
                System.Diagnostics.Debug.WriteLine("Getting question from spreadsheet");
                question.getQuestion();

                while (true)
                {

                    if (question.startDateTime > DateTime.Now)
                    {
                        TimeSpan offset;
                        offset = question.startDateTime - DateTime.Now;
                        Thread.Sleep(offset);
                        question.getQuestion();
                    }
                    if (question.questionText != null)
                    {
                        //tweet question
                        TextInfo myTI = new CultureInfo("en-US", false).TextInfo;

                        if (!question.tweetTooLong())
                        {
                            //long questionTweetID = twitterHelper.sendTweet(question.tweetString + " " + DateTime.Now.ToString());
                            long questionTweetID = twitterHelper.sendTweet(question.tweetString);

                            //var twitterTask = Task.Run(async () => await cTwitterHelper.ProcessTweet(questionTweetID, question));
                            //listen for responses 
                            twitterHelper.listenAndProcess(questionTweetID, question);

                        }

                    }
                    //dtResetDay = DateTime.Now;

                    //if (dtStartToday.AddDays(1) > dtResetDay && dtStartToday.Day != dtResetDay.Day)
                    //{
                    //    break;
                    //}

                    if (question.endDateTime < DateTime.Now)
                        break;
                }
                System.Diagnostics.Debug.WriteLine("End of while loop in PyramidLightsMain. Question Expired. Getting new question.");

            }
        }
        catch (Exception Ex)
        {
            System.Diagnostics.EventLog.WriteEntry(EventLogSource, Ex.ToString(), System.Diagnostics.EventLogEntryType.Error);
           
            //try
            //{
            //    PyramidLights.Classes.cMail.SendEmail(configHelper, Ex.Message);
            //}
            //catch(Exception ignoreEx)
            //{
            //    System.Diagnostics.EventLog.WriteEntry("PyramidLightsError-Mail", ignoreEx.ToString(), System.Diagnostics.EventLogEntryType.Error);
            //}
        }
    }

}

