using Stream = Tweetinvi.Stream;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.IO;

public class cTwitterHelper
{

    private cConfigurationHelper configHelper = null;
    public cTwitterHelper(cConfigurationHelper vConfigHelper)
    {
        configHelper = vConfigHelper;
    }

    public long sendTweet(String vText)
    {
        Tweetinvi.Auth.SetUserCredentials(configHelper.consumerKey(), configHelper.consumerSecret(), configHelper.accessToken(), configHelper.accessTokenSecret());

        var publishedTweet = Tweetinvi.Tweet.PublishTweet(vText);
        var tweetId = publishedTweet.Id;
        System.Diagnostics.Debug.WriteLine("Tweet published with ID: " + tweetId.ToString());

        return tweetId;
    }
    //Open listening connecting to twitter account
    public void listenAndProcess(long vQuestionTweetID, cQuestion vQuestion)
    {
        BlockingCollection<cTweet> correctAnswers = new BlockingCollection<cTweet>(1000);
        cTriggerPyramid triggerPyramid = new cTriggerPyramid();
        // An action to consume the ConcurrentQueue.
        Action processQueue = () =>
        {
            while (DateTime.Now <= vQuestion.endDateTime)
            {
                cTweet tempAnswer = correctAnswers.Take();
                System.Diagnostics.Debug.WriteLine("Evaluating TweetID: " + tempAnswer.getID().ToString());
                //evaluate to determine if it is an answer to a question
                if (isTweetReply(tempAnswer, (ulong)vQuestionTweetID))
                {
                    long answerTweetID = tempAnswer.getID();
                    //validate answer, send return tweet
                    String correctAnswerString = null;
                    switch (vQuestion.correctAnswer)
                    {
                        case 1:
                            correctAnswerString = vQuestion.answer1;
                            break;
                        case 2:
                            correctAnswerString = vQuestion.answer2;
                            break;
                        case 3:
                            correctAnswerString = vQuestion.answer3;
                            break;
                        case 4:
                            correctAnswerString = vQuestion.answer4;
                            break;
                        default:
                            correctAnswerString = "ERROR";
                            break;
                    }
                    if (tempAnswer.getText().ToUpper().Contains(correctAnswerString.ToUpper()))
                    {
                        String correctResponse = null;
                        correctResponse = "@" + tempAnswer.getUserAccount() + " That's Correct! " +
                                                                vQuestion.followUpMessage;
                        Tweetinvi.Tweet.PublishTweetInReplyTo(correctResponse, answerTweetID);
                        System.Diagnostics.Debug.WriteLine("Response Sent: " + correctResponse);
                        //trigger pyramid
                        System.Diagnostics.Debug.WriteLine("Triggering Pyramid");
                        cTriggerPyramid triggerPyramidOBJ = new cTriggerPyramid();
                        //for local execution
                        //triggerPyramidOBJ.runPython_cmd("pi@192.168.0.168", "python activatetrigger.py " + vQuestion.pyramidScene);
                        //for server execution    
                        //triggerPyramidOBJ.runPython_cmd("-i /var/pi/pi_privatekey pi@localhost -p 19999 nohup", "python activatetrigger.py " + vQuestion.pyramidScene);
                        //pulling from config file
                        triggerPyramidOBJ.runPython_cmd(configHelper.pyramidTrigger(), configHelper.pyramidScript() + " " + vQuestion.pyramidScene);
                    }
                    else
                    {
                        String incorrectResponse = null;
                        incorrectResponse = "@" + tempAnswer.getUserAccount() + " Oops, that's not it. Try again!";
                        Tweetinvi.Tweet.PublishTweetInReplyTo(incorrectResponse , answerTweetID);
                        System.Diagnostics.Debug.WriteLine("Response Sent: " + incorrectResponse);
                    }
                }
            }
        };

        // Start 1 concurrent consuming actions.
        Task.Run(processQueue);

        Tweetinvi.Auth.SetUserCredentials(configHelper.consumerKey(), configHelper.consumerSecret(), configHelper.accessToken(), configHelper.accessTokenSecret());
        var stream = Stream.CreateUserStream();
        stream.TweetCreatedByAnyoneButMe += (sender, args) =>
        {
            //When tweet received, add to blocking queue
            correctAnswers.TryAdd(new cTweet(args.Tweet.Id, args.Tweet.Text, args.Tweet.CreatedAt, args.Tweet.CreatedBy,
                                    args.Tweet.InReplyToStatusId));
            System.Diagnostics.Debug.WriteLine("Incoming tweet. " + args.Tweet);
        };

        try
        {
            stream.StartStream();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

    }

    private bool isTweetReply(cTweet vTweet, ulong vQuestionTweetID)
    {
        bool reply = false;
        if (vTweet.getReplyToStatusID() != null)
        {
            ulong replyToStatusID = (ulong)vTweet.getReplyToStatusID();
            if (replyToStatusID == (ulong)vQuestionTweetID)
            {
                reply = true;
                System.Diagnostics.Debug.WriteLine("Question Reply Found. This tweet is a reply to: " + vTweet.getReplyToStatusID().ToString());
            }
        }
        return reply;
    }

}