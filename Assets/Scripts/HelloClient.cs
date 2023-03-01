using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HelloClient : MonoBehaviour
{
    private HelloRequester _helloRequester;
    public TMP_InputField unitymsg;

    private void Start()
    {
        _helloRequester = new HelloRequester();
        _helloRequester.Start();
		unitymsg.onEndEdit.AddListener(delegate{SendMsg(unitymsg);});
    }

    private void SendMsg(TMP_InputField input){
        _helloRequester.telegrammessage = input.text;
        // if(!_helloRequester.isRunning())
        // {
        //     Debug.Log("its running");
        // }
        _helloRequester.sendToPython();
        _helloRequester.receivePython();
        while(_helloRequester.clientresponse == null)
        {
            Debug.Log("waiting for python");
        }
        Debug.Log(_helloRequester.isRunning());
    }

    private void OnDestroy()
    {
        _helloRequester.Stop();
    }
}