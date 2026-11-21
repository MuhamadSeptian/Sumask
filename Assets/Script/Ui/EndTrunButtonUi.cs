using UnityEngine;

public class EndTrunButtonUi : MonoBehaviour
{
    public void OnClick()
    {
        // Karena GameAction sudah bukan MonoBehaviour, 'new' sekarang aman digunakan
        EnemyTurnGA enemyTurnGA = new EnemyTurnGA();

        // Panggil fungsi Perform yang benar
        ActionSystem.Instance.AddReaction(new EnemyTurnGA());
    }
}