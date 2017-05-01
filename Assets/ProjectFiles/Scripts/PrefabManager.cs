using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    public static bool CoinStartFromTop;
    public GameObject PlatformPrefab;

    public static void CheckCoins()
    {
        CoinStartFromTop = !CoinStartFromTop; //Change the Starting Position for the Coin Movement.
    }

    private static PrefabManager m_Instance = null;
    public static PrefabManager Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = (PrefabManager)FindObjectOfType(typeof(PrefabManager));
            }
            return m_Instance;
        }
    }
 

}
