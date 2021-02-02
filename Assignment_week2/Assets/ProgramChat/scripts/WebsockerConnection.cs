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
        public int previous, current;
        
        public List<string> messageServerList = new List<string>();
        public GameObject ChatWindow;


        // Start is called before the first frame update
        void Start()
        {
           
          

        }

        // Update is called once per frame
        void Update()
        {
            if (current > previous)
            {
                if (messageServerList.Count > 0)
                {
                    MeText.text = messageServerList[messageServerList.Count - 1];
                }
            }
            else if (current < previous)
            {
                if (messageServerList.Count > 0)
                {

                    YouText.text = messageServerList[messageServerList.Count - 1];

                }
            }

        }

        public void ClientConnect()
        {
            if (IdText.text == "127.0.0.1" && PortText.text == "5501")
            {
                websocket = new WebSocket("ws://" + IdText.text + ":" + PortText.text);
                websocket.Connect();
                ChatWindow.SetActive(true);
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
            if(websocket.ReadyState == WebSocketState.Open)
            {
                websocket.Send(MessageText.text);
                MeText.text = MessageText.text;
                websocket.OnMessage += OnMessage;
                

            }
        }

        

        //public void OnYouMessage(object sender, MessageEventArgs messageEventArgs)
        //{
        //    Debug.Log(messageEventArgs.Data);
        //    messageServerList.Add(messageEventArgs.Data);
            
        //    current = previous;
        //    previous +=1;

           
        //}

        public void OnMessage(object sender, MessageEventArgs messageEventArgs)
        {

            //messageServerList.Add(messageEventArgs.Data);
            
             if (messageEventArgs.Data != MeText.text)
            {

                messageServerList.Add(messageEventArgs.Data);
                current = previous;
                previous += 1;
              
            }
            else /*if (messageEventArgs.Data != YouText.text)*/
            {

                messageServerList.Add(messageEventArgs.Data);

                previous = current;
                current += 1;
               
            }


        }


    }

}

