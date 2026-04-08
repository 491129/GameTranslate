using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AlarmManager : MonoBehaviour
{
    public List<AlarmClock> allClocks;          // 四个闹钟
    public float[] intervals = { 15f, 12f, 9f, 6f, 3f }; // 触发间隔（秒）
    public float initialDelay = 3f;              // 游戏开始后等待3秒再触发第一个闹钟

    private int intervalIndex = 0;
   // private bool isTriggering = false;

    void Start()
    {
        if (allClocks == null || allClocks.Count == 0)
        {
            Debug.LogError("请将闹钟拖入 AlarmManager 的 allClocks 列表");
            return;
        }
        // 延迟 initialDelay 秒后开始触发闹钟
        StartCoroutine(DelayedStart());
    }

    IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(initialDelay);
        // 触发第一个随机闹钟
        TriggerRandomAlarm();
        // 启动后续间隔循环
        StartCoroutine(TriggerLoop());
    }

    IEnumerator TriggerLoop()
    {
       // isTriggering = true;
        while (true)
        {
            // 获取本次需要等待的时间
            float waitTime;
            if (intervalIndex < intervals.Length)
            {
                waitTime = intervals[intervalIndex];
                intervalIndex++;
            }
            else
            {
                // 所有间隔用完后，一直使用最后一个间隔（3秒）
                waitTime = intervals[intervals.Length - 1];
            }

            yield return new WaitForSeconds(waitTime);

            // 选择一个未在响铃的闹钟（已经停止的闹钟可以再次被触发）
            var available = allClocks.Where(c => !c.IsRinging).ToList();
            if (available.Count == 0)
            {
                Debug.Log("所有闹钟都在响铃中，无法触发新闹钟，等待下次间隔");
                continue;
            }

            int randomIndex = Random.Range(0, available.Count);
            AlarmClock selected = available[randomIndex];
            selected.StartRinging();
            Debug.Log($"触发闹钟 ID: {selected.clockID}, 间隔序号: {intervalIndex - 1}, 等待时间: {waitTime}s");
        }
    }


    void TriggerRandomAlarm()
    {
        // 过滤掉 null 元素
        var available = allClocks.Where(c => c != null && !c.IsRinging).ToList();
        if (available.Count == 0)
        {
            Debug.LogWarning("没有可触发的闹钟（可能全部为 null 或都在响铃）");
            return;
        }
        int rand = Random.Range(0, available.Count);
        available[rand].StartRinging();
    }
}