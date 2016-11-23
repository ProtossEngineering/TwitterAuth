using Twitter;
using UnityEngine;

public class Demo : MonoBehaviour
{
    // You need to save access token and secret for later use.
    // You can keep using them whenever you need to access the user's Twitter account.
    // They will be always valid until the user revokes the access to your application.
    private const string PLAYER_PREFS_TWITTER_USER_ID = "TwitterUserID";

    private const string PLAYER_PREFS_TWITTER_USER_SCREEN_NAME = "TwitterUserScreenName";
    private const string PLAYER_PREFS_TWITTER_USER_TOKEN = "TwitterUserToken";
    private const string PLAYER_PREFS_TWITTER_USER_TOKEN_SECRET = "TwitterUserTokenSecret";
    public string CONSUMER_KEY;
    public string CONSUMER_SECRET;
    public float PIN_ENTER_HEIGHT;
    public float PIN_ENTER_WIDTH;
    public float PIN_ENTER_X;
    public float PIN_ENTER_Y;
    public float PIN_INPUT_HEIGHT;
    public float PIN_INPUT_WIDTH;
    public float PIN_INPUT_X;
    public float PIN_INPUT_Y;
    public float POST_TWEET_HEIGHT;
    public float POST_TWEET_WIDTH;
    public float POST_TWEET_X;
    public float POST_TWEET_Y;
    public float TWEET_INPUT_HEIGHT;
    public float TWEET_INPUT_WIDTH;
    public float TWEET_INPUT_X;
    public float TWEET_INPUT_Y;
    public float USER_LOG_IN_HEIGHT;
    public float USER_LOG_IN_WIDTH;
    public float USER_LOG_IN_X;
    public float USER_LOG_IN_Y;

    private AccessTokenResponse m_AccessTokenResponse;

    private string m_PIN = "Please enter your PIN here.";
    private RequestTokenResponse m_RequestTokenResponse;
    private string m_Tweet = "Please enter your tweet here.";

    // Use this for initialization
    private void Start()
    {
        LoadTwitterUserInfo();
    }

    // Update is called once per frame
    private void Update()
    {
    }

    // GUI
    private void OnGUI()
    {
        // LogIn/Register Button
        var rect = new Rect(Screen.width * USER_LOG_IN_X,
            Screen.height * USER_LOG_IN_Y,
            Screen.width * USER_LOG_IN_WIDTH,
            Screen.height * USER_LOG_IN_HEIGHT);

        if (string.IsNullOrEmpty(CONSUMER_KEY) || string.IsNullOrEmpty(CONSUMER_SECRET))
        {
            string text =
                "You need to register your game or application first.\n Click this button, register and fill CONSUMER_KEY and CONSUMER_SECRET of Demo game object.";
            if (GUI.Button(rect, text))
            {
                Application.OpenURL("https://apps.twitter.com");
            }
        }
        else
        {
            string text = string.Empty;

            if (!string.IsNullOrEmpty(m_AccessTokenResponse.ScreenName))
            {
                text = m_AccessTokenResponse.ScreenName + "\nClick to register with a different Twitter account";
            }
            else
            {
                text = "You need to register your game or application first.";
            }

            if (GUI.Button(rect, text))
            {
                StartCoroutine(API.GetRequestToken(CONSUMER_KEY, CONSUMER_SECRET,
                    OnRequestTokenCallback));
            }
        }

        // PIN Input
        rect.x = Screen.width * PIN_INPUT_X;
        rect.y = Screen.height * PIN_INPUT_Y;
        rect.width = Screen.width * PIN_INPUT_WIDTH;
        rect.height = Screen.height * PIN_INPUT_HEIGHT;

        m_PIN = GUI.TextField(rect, m_PIN);

        // PIN Enter Button
        rect.x = Screen.width * PIN_ENTER_X;
        rect.y = Screen.height * PIN_ENTER_Y;
        rect.width = Screen.width * PIN_ENTER_WIDTH;
        rect.height = Screen.height * PIN_ENTER_HEIGHT;

        if (GUI.Button(rect, "Enter PIN"))
        {
            StartCoroutine(API.GetAccessToken(CONSUMER_KEY, CONSUMER_SECRET, m_RequestTokenResponse.Token, m_PIN,
                OnAccessTokenCallback));
        }

        // Tweet Input
        rect.x = Screen.width * TWEET_INPUT_X;
        rect.y = Screen.height * TWEET_INPUT_Y;
        rect.width = Screen.width * TWEET_INPUT_WIDTH;
        rect.height = Screen.height * TWEET_INPUT_HEIGHT;

        m_Tweet = GUI.TextField(rect, m_Tweet);

        // Post Tweet Button
        rect.x = Screen.width * POST_TWEET_X;
        rect.y = Screen.height * POST_TWEET_Y;
        rect.width = Screen.width * POST_TWEET_WIDTH;
        rect.height = Screen.height * POST_TWEET_HEIGHT;

        if (GUI.Button(rect, "Post Tweet"))
        {
            StartCoroutine(API.PostTweet(m_Tweet, CONSUMER_KEY, CONSUMER_SECRET, m_AccessTokenResponse,
                OnPostTweet));
        }
    }

    private void LoadTwitterUserInfo()
    {
        m_AccessTokenResponse = new AccessTokenResponse();

        m_AccessTokenResponse.UserId = PlayerPrefs.GetString(PLAYER_PREFS_TWITTER_USER_ID);
        m_AccessTokenResponse.ScreenName = PlayerPrefs.GetString(PLAYER_PREFS_TWITTER_USER_SCREEN_NAME);
        m_AccessTokenResponse.Token = PlayerPrefs.GetString(PLAYER_PREFS_TWITTER_USER_TOKEN);
        m_AccessTokenResponse.TokenSecret = PlayerPrefs.GetString(PLAYER_PREFS_TWITTER_USER_TOKEN_SECRET);

        if (!string.IsNullOrEmpty(m_AccessTokenResponse.Token) &&
            !string.IsNullOrEmpty(m_AccessTokenResponse.ScreenName) &&
            !string.IsNullOrEmpty(m_AccessTokenResponse.Token) &&
            !string.IsNullOrEmpty(m_AccessTokenResponse.TokenSecret))
        {
            string log = "LoadTwitterUserInfo - succeeded";
            log += "\n    UserId : " + m_AccessTokenResponse.UserId;
            log += "\n    ScreenName : " + m_AccessTokenResponse.ScreenName;
            log += "\n    Token : " + m_AccessTokenResponse.Token;
            log += "\n    TokenSecret : " + m_AccessTokenResponse.TokenSecret;
            Debug.Log(log);
        }
    }

    private void OnRequestTokenCallback(bool success, RequestTokenResponse response)
    {
        if (success)
        {
            string log = "OnRequestTokenCallback - succeeded";
            log += "\n    Token : " + response.Token;
            log += "\n    TokenSecret : " + response.TokenSecret;
            print(log);

            m_RequestTokenResponse = response;

            API.OpenAuthorizationPage(response.Token);
        }
        else
        {
            print("OnRequestTokenCallback - failed.");
        }
    }

    private void OnAccessTokenCallback(bool success, AccessTokenResponse response)
    {
        if (success)
        {
            string log = "OnAccessTokenCallback - succeeded";
            log += "\n    UserId : " + response.UserId;
            log += "\n    ScreenName : " + response.ScreenName;
            log += "\n    Token : " + response.Token;
            log += "\n    TokenSecret : " + response.TokenSecret;
            print(log);

            m_AccessTokenResponse = response;

            PlayerPrefs.SetString(PLAYER_PREFS_TWITTER_USER_ID, response.UserId);
            PlayerPrefs.SetString(PLAYER_PREFS_TWITTER_USER_SCREEN_NAME, response.ScreenName);
            PlayerPrefs.SetString(PLAYER_PREFS_TWITTER_USER_TOKEN, response.Token);
            PlayerPrefs.SetString(PLAYER_PREFS_TWITTER_USER_TOKEN_SECRET, response.TokenSecret);
        }
        else
        {
            print("OnAccessTokenCallback - failed.");
        }
    }

    private void OnPostTweet(bool success)
    {
        print("OnPostTweet - " + (success ? "succedded." : "failed."));
    }
}