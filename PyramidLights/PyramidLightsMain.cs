using System;
using System.Globalization;
using System.Threading;


    class PyramidLightsMain
    {

    static void Main(string[] args)
        {

            cConfigurationHelper  configHelper = new cConfigurationHelper();
            cTwitterHelper twitterHelper = new cTwitterHelper(configHelper);
            cQuestion question = new cQuestion(configHelper);

            //get question from googledocs
            question.getQuestion();
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
                    long questionTweetID = twitterHelper.sendTweet(question.tweetString + DateTime.Now.ToString());
                    //listen for responses 
                    twitterHelper.listenAndProcess(questionTweetID, question);
                }
            }
    }
  
}

