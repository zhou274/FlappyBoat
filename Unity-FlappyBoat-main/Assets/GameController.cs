using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using TTSDK.UNBridgeLib.LitJson;
using TTSDK;
using StarkSDKSpace;

public class GameController : MonoBehaviour
{
    public GameObject player;
    private PlayerController playerController;
    private bool isSpawnEnemy = false;
    public GameObject menuCanvas;
    public GameObject tutorialCanvas;
    public GameObject ingameCanvas;
    public GameObject gameOverCanvas;
    public GameObject enemyPrefab;
    public GameObject enemySpawnLocation;
    public float enemySpawnTime;
    private float spawnTimeCount;
    public GameObject backgroundPrefab;
    private float spawnRate = 1f;


    public TextMeshProUGUI HighScore;
    public string clickid;
    private StarkAdManager starkAdManager;


    private void Start() {
        playerController = player.GetComponent<PlayerController>();
        spawnTimeCount = enemySpawnTime;
    }

    private void Update() {
        if(isSpawnEnemy) {
            if(spawnTimeCount < 0) {
                addNewEnemy();
                spawnTimeCount = enemySpawnTime;
            }
            spawnRate = spawnRate + 0.2f * Time.deltaTime;
            spawnTimeCount -= spawnRate*Time.deltaTime;
        }
    }

    public void play() {
        Debug.Log("Play");
        menuCanvas.SetActive(false);
        tutorialCanvas.SetActive(true);
        ingameCanvas.SetActive(true);
        playerController.isControlable = true;
    }

    public void startSpawnEnemy() {
        tutorialCanvas.SetActive(false);
        isSpawnEnemy = true;
    }

    private void addNewEnemy() {
        Vector3 randomLocation = new Vector3(enemySpawnLocation.transform.position.x+Random.Range(0, 2f), enemySpawnLocation.transform.position.y+Random.Range(0, 2f), enemySpawnLocation.transform.position.z);
        Instantiate(enemyPrefab, randomLocation, enemySpawnLocation.transform.rotation);
    }

    public void gameOver() {
        ShowInterstitialAd("3hai1a0e71d53t86r7",
           () => {
               Debug.LogError("--插屏广告完成--");
               var data = new JsonData
               {
                   ["event_type"] = "game_addiction",
                   ["extra"] = "{product_name: '插屏广告完成'}",
               };
               StarkSDK.API.StarkSendToTAQ(data);
           },
           (it, str) => {
               Debug.LogError("Error->" + str);
               StarkSDKSpace.AndroidUIManager.ShowToast("广告加载异常，请稍后再试");
           });


        gameOverCanvas.SetActive(true);
        int score = GameObject.FindObjectOfType<Score>().GetComponent<Score>().getScore();
        PlayerPrefs.SetInt("Score", score);
        if(score>PlayerPrefs.GetInt("HighScore"))
        {
            PlayerPrefs.SetInt("HighScore", score);
        }
        HighScore.text = PlayerPrefs.GetInt("HighScore").ToString();

        GameObject.FindObjectOfType<Score>().GetComponent<Score>().setScore(0);

        GameObject.FindObjectOfType<EndScore>().GetComponent<EndScore>().setScore(score);
        ingameCanvas.SetActive(false);
    }

    public void restart(GameObject player) {
        isSpawnEnemy = false;
        spawnRate = 1f;
        
        // Restart player properties
        PlayerController playerController = player.GetComponent<PlayerController>();
        playerController.restart();

        // Clear all enemies an bullets on screen
        Enemy[] enemies = GameObject.FindObjectsOfType<Enemy>();
        foreach(Enemy c in enemies) {
            c.dead();
        }
        Bullet[] bullets = GameObject.FindObjectsOfType<Bullet>();
        foreach(Bullet b in bullets) {
            b.dead();
        }

        // Change UI
        menuCanvas.SetActive(true);
        gameOverCanvas.SetActive(false);
    }
    public void Continue(GameObject player)
    {
        ShowVideoAd("192if3b93qo6991ed0",
            (bol) => {
                if (bol)
                {
                    Score.instance.setScore(PlayerPrefs.GetInt("Score"));
                    //GameObject.FindObjectOfType<Score>().GetComponent<Score>().setScore(PlayerPrefs.GetInt("Score"));
                    isSpawnEnemy = false;
                    spawnRate = 1f;

                    // Restart player properties
                    PlayerController playerController = player.GetComponent<PlayerController>();
                    playerController.restart();

                    // Clear all enemies an bullets on screen
                    Enemy[] enemies = GameObject.FindObjectsOfType<Enemy>();
                    foreach (Enemy c in enemies)
                    {
                        c.dead();
                    }
                    Bullet[] bullets = GameObject.FindObjectsOfType<Bullet>();
                    foreach (Bullet b in bullets)
                    {
                        b.dead();
                    }

                    // Change UI
                    //menuCanvas.SetActive(true);
                    gameOverCanvas.SetActive(false);
                    tutorialCanvas.SetActive(true);
                    ingameCanvas.SetActive(true);
                    playerController.isControlable = true;
                    clickid = "";
                    getClickid();
                    apiSend("game_addiction", clickid);
                    apiSend("lt_roi", clickid);


                }
                else
                {
                    StarkSDKSpace.AndroidUIManager.ShowToast("观看完整视频才能获取奖励哦！");
                }
            },
            (it, str) => {
                Debug.LogError("Error->" + str);
                //AndroidUIManager.ShowToast("广告加载异常，请重新看广告！");
            });
        
    }




    public void getClickid()
    {
        var launchOpt = StarkSDK.API.GetLaunchOptionsSync();
        if (launchOpt.Query != null)
        {
            foreach (KeyValuePair<string, string> kv in launchOpt.Query)
                if (kv.Value != null)
                {
                    Debug.Log(kv.Key + "<-参数-> " + kv.Value);
                    if (kv.Key.ToString() == "clickid")
                    {
                        clickid = kv.Value.ToString();
                    }
                }
                else
                {
                    Debug.Log(kv.Key + "<-参数-> " + "null ");
                }
        }
    }

    public void apiSend(string eventname, string clickid)
    {
        TTRequest.InnerOptions options = new TTRequest.InnerOptions();
        options.Header["content-type"] = "application/json";
        options.Method = "POST";

        JsonData data1 = new JsonData();

        data1["event_type"] = eventname;
        data1["context"] = new JsonData();
        data1["context"]["ad"] = new JsonData();
        data1["context"]["ad"]["callback"] = clickid;

        Debug.Log("<-data1-> " + data1.ToJson());

        options.Data = data1.ToJson();

        TT.Request("https://analytics.oceanengine.com/api/v2/conversion", options,
           response => { Debug.Log(response); },
           response => { Debug.Log(response); });
    }


    /// <summary>
    /// </summary>
    /// <param name="adId"></param>
    /// <param name="closeCallBack"></param>
    /// <param name="errorCallBack"></param>
    public void ShowVideoAd(string adId, System.Action<bool> closeCallBack, System.Action<int, string> errorCallBack)
    {
        starkAdManager = StarkSDK.API.GetStarkAdManager();
        if (starkAdManager != null)
        {
            starkAdManager.ShowVideoAdWithId(adId, closeCallBack, errorCallBack);
        }
    }
    /// <summary>
    /// 播放插屏广告
    /// </summary>
    /// <param name="adId"></param>
    /// <param name="errorCallBack"></param>
    /// <param name="closeCallBack"></param>
    public void ShowInterstitialAd(string adId, System.Action closeCallBack, System.Action<int, string> errorCallBack)
    {
        starkAdManager = StarkSDK.API.GetStarkAdManager();
        if (starkAdManager != null)
        {
            var mInterstitialAd = starkAdManager.CreateInterstitialAd(adId, errorCallBack, closeCallBack);
            mInterstitialAd.Load();
            mInterstitialAd.Show();
        }
    }
}
