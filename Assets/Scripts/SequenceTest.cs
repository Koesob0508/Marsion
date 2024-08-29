using System;
using System.Collections;
using UnityEngine;
using Marsion.Tool;

public class SequenceTest : MonoBehaviour
{
    MyTween.MainSequence ClientSequence;
    // Start is called before the first frame update
    void Start()
    {
        MyTween.Sequence attackSequence = new MyTween.Sequence();

        MyTween.Task task1 = new MyTween.Task();
        task1.Action = () =>
        {
            Debug.Log("Attack1");
            task1.OnComplete?.Invoke();
        };

        MyTween.Task task2 = new MyTween.Task();
        task2.Action = () =>
        {
            Debug.Log("Attack2");
            StartCoroutine(DelayComplete(task2.OnComplete));
        };

        MyTween.Task task3 = new MyTween.Task();
        task3.Action = () =>
        {
            Debug.Log("Attack3");
            task3.OnComplete?.Invoke();
        };

        MyTween.Sequence DeadSequence = new MyTween.Sequence();

        attackSequence.Append(task1);
        attackSequence.Append(task2);
        attackSequence.Append(task3);

        MyTween.Task deadTask = new MyTween.Task();
        deadTask.Action = () =>
        {
            Debug.Log("Dead Sequence");
            deadTask.OnComplete?.Invoke();
        };

        DeadSequence.Append(deadTask);
        
        ClientSequence = new MyTween.MainSequence();
        ClientSequence.Append(attackSequence);
        ClientSequence.Append(DeadSequence);

        ClientSequence.Play();
    }

    IEnumerator DelayComplete(Action action)
    {
        yield return new WaitForSeconds(10f);

        action?.Invoke();
    }
}
