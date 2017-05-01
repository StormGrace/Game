using UnityEngine;

public class Item_Movement : MonoBehaviour {
    public static int Speed = 4;
    public static float Distance = 0.5F;

    private bool StartFromTop;
    private Vector2 StartPosition;

    void Start() { StartPosition = transform.position; PrefabManager.CheckCoins(); StartFromTop = PrefabManager.CoinStartFromTop; }

    private void Update() {
        switch (StartFromTop) {
            case true: transform.position = StartPosition + new Vector2(0, Mathf.Sin(Time.time * Speed) * Distance); break;
            case false: transform.position = StartPosition + new Vector2(0, Mathf.Cos(Time.time * Speed) * Distance); break;
        }
	}
}
