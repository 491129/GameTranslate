using Unity.Burst.CompilerServices;
using UnityEngine;

public class AC2 : MonoBehaviour
{
    public interface IPanelController
    {
        void Show(AlarmClock clock);
        void Hide();
    }
}