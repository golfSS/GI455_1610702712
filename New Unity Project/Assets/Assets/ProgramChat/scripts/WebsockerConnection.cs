using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using UnityEngine.UI;

namespace ProgramChat
{
    public class WebsockerConnection : MonoBehaviour
    {
        private WebSocket websocket;
        private int firstmsg = 1;

        
        public Text MessageText, PortText, IdText, YouText, MeText;
        public bool Checkmessage;
        
        public List<string> messageServerList = new List<string>();
        public GameObject ChatWindow;


        // Start is called before the first frame update
        void Start()
        {
           
          

        }

        // Update is called once per frame
        void Update()
        {
            


        }

        public void ClientConnect()
        {
            if (IdText.text == "127.0.0.1" && PortText.text == "5501")
            {
                websocket = new WebSocket("ws://" + IdText.text + ":" + PortText.text);
                websocket.Connect();
                ChatWindow.SetActive(true);
                //websocket.OnMessage += OnMessage;
            }
        }

        private void OnDestroy()
        {
            if(websocket != null)
            {
                websocket.Close();
            }
        }

        

        public void SendMessage()
        {
            websocket.OnMessage += OnMessage;
            if (websocket.ReadyState == WebSocketState.Open)
            {
                if (Checkmessage == true)
                {
                    if (messageServerList.Count > 0)
                    {
                        MeText.text += messageServerList[messageServerList.Count - 1] + "\n";
                        YouText.text += "\n";

                    }
                }
                else if (Checkmessage == false)
                {
                    if (messageServerList.Count > 0)
                    {

                        YouText.text = messageServerList[messageServerList.Count - 1];
                        MeText.text += "\n";

                    }
                }
                websocket.Send(MessageText.text);

                websocket.OnMessage += OnMessage;


            }
        }

        

        

        public void OnMessage(object sender, MessageEventArgs messageEventArgs)
        {
             if (messageEventArgs.Data == MessageText.text)
            {
                
                messageServerList.Add(messageEventArgs.Data);
                Checkmessage = true;
                

            }
            else 
            {
                
                messageServerList.Add(messageEventArgs.Data);
                Checkmessage = false;
                
            }


        }


    }

}

