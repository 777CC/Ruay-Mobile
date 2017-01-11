using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading;

namespace Battlehub.Dispatcher
{
    public class DispatcherTest : MonoBehaviour
    {
        [SerializeField]
        private Text Output;

        private void Start()
        {
            for (int i = 0; i < 10; ++i)
            {
                Thread t = new Thread(() => {
                    ThreadFunction("Dispatched from Thread " + i, i * 1000);
                });
                t.Start();
            }
        }
        
        private void ThreadFunction(string param,int wait)
        {
            Thread.Sleep(wait);
            Dispatcher.Current.BeginInvoke(() =>
            {
                Output.text += param;
                Output.text += Environment.NewLine;
            });
        }
    }
}
