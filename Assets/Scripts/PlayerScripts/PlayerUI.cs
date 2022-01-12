using TMPro;
using UnityEngine;
using WeaponScripts;

namespace PlayerScripts
{
    public class PlayerUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI rounds;
        [SerializeField] private TextMeshProUGUI magSize;

        private void Awake() {
            Gun.BroadcastShot += UpdateMag;
        }

        private void UpdateMag(int r, int ms) {
            rounds.text = r + "";
            magSize.text = "/" + ms;
        }

    }
}
