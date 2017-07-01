using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;


    class PyramidLightsMain
    {

    static void Main(string[] args)
    {

        cConfigurationHelper  configHelper = new cConfigurationHelper();

        try
        {
            while (true)
            {
                DateTime dtStartToday = DateTime.Now;
                DateTime dtResetDay = DateTime.Now;
                cTwitterHelper twitterHelper = new cTwitterHelper(configHelper);
                cQuestion question = new cQuestion(configHelper);

                //get question from googledocs
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
                            long questionTweetID = twitterHelper.sendTweet(question.tweetString + " " + DateTime.Now.ToString());

                            //var twitterTask = Task.Run(async () => await cTwitterHelper.ProcessTweet(questionTweetID, question));
                            //listen for responses 
                            twitterHelper.listenAndProcess(questionTweetID, question);

                        }

                    }
                    dtResetDay = DateTime.Now;

                    if (dtStartToday.AddDays(1) > dtResetDay && dtStartToday.Day != dtResetDay.Day)
                    {
                        break;
                    }
                }

            }
        }
        catch (Exception Ex)
        {
            System.Diagnostics.EventLog.WriteEntry("PyramidLightsError", Ex.ToString(), System.Diagnostics.EventLogEntryType.Error);

            try
            {
                PyramidLights.Classes.cMail.SendEmail(configHelper, Ex.Message);
            }
            catch(Exception ignoreEx)
            {
                System.Diagnostics.EventLog.WriteEntry("PyramidLightsError-Mail", ignoreEx.ToString(), System.Diagnostics.EventLogEntryType.Error);
            }
        }
    }

}

