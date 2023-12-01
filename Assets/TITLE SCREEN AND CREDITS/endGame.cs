using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class endGame : MonoBehaviour
{
    // Start is called before the first frame update
    public ScoreCardManager scoreCardManager;
    public float total;

    public float stars; 
    public float starThreshold;
    public int patientThreshold;
    public void endGameButton()
    {
        if(scoreCardManager.stars < starThreshold || scoreCardManager.numberOfCustomers < patientThreshold)
        {
            SceneManager.LoadScene("StanleyEnding");
        }
        else if (scoreCardManager.total < 0)
        {
            SceneManager.LoadScene("PoorEnding");
        }
        else
        {
            SceneManager.LoadScene("RichEnding");
        }

}
}
