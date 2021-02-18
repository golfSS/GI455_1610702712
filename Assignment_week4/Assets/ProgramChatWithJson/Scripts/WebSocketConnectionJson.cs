using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using System;
using UnityEngine.UI;

namespace ChatWebSocketWithJson
{
    public class WebSocketConnectionJson : MonoBehaviour
    {
        struct MessageData
        {
            public string username;
            public string message;

            public MessageData(string username, string message)
            {
                this.username = username;
                this.message = message;
            }
        }

        struct SocketEvent
        {
            public string eventName;
            public string data;

            public SocketEvent(string eventName, string data)
            {
                this.eventName = eventName;
                this.data = data;
            }
        }

        public InputField inputUsername;
        public InputField inputCreateRoom;
        public InputField inputJoinRoom;
        public GameObject rootConnection;
        public GameObject rootMessenger;
        public GameObject rootMenu;
        public GameObject rootCreateRoom;
        public GameObject rootJoinRoom;
        public GameObject failPanel;


        public Text RoomName;
        public InputField inputCreatNameRoomText;
        public InputField inputJoinNameRoomText;
        public InputField inputText;
        public Text sendText;
        public Text receiveText;


        private WebSocket ws;

        private string tempMessageString;
        private string createNameRoom;
        private string joinNameRoom;

        public void Start()
        {
            rootConnection.SetActive(true);
            rootMessenger.SetActive(false);
            rootMenu.SetActive(false);
            rootCreateRoom.SetActive(false);
            rootJoinRoom.SetActive(false);
            failPanel.SetActive(false);
        }

        private void Update()
        {
            UpdateNotifyMessage();
        }

        public void Connect()
        {
            string url = $"ws://127.0.0.1:5501/";

            ws = new WebSocket(url);

            ws.OnMessage += OnMessage;

            ws.Connect();

            rootConnection.SetActive(false);
            rootMenu.SetActive(true);

            //CreateRoom("TestRoom01");
        }

        public void CreateRoom()
        {
            rootCreateRoom.SetActive(true);
            rootMenu.SetActive(false);
        }

        public void JoinRoom()
        {
            rootJoinRoom.SetActive(true);
            rootMenu.SetActive(false);
        }

        public void GetLeaveRoom()
        {
            LeaveRoom();
            //rootMenu.SetActive(true);
            //rootMessenger.SetActive(false);
        }

        public void CreateNameRoom()
        {
            createNameRoom = inputCreatNameRoomText.text;
            CreateRoom(createNameRoom);
           
        }

        public void JoinNameRoom()
        {
            joinNameRoom = inputJoinNameRoomText.text;
            JoinRoom(joinNameRoom);
        }

        private void PanelFail()
        {
            failPanel.SetActive(true);
        }

        public void ClosePanelFail()
        {
            failPanel.SetActive(false);
        }


        public void CreateRoom(string roomName)
        {
            if(ws.ReadyState == WebSocketState.Open)
            {
                SocketEvent socketEvent = new SocketEvent("CreateRoom", roomName);

                string toJsonStr = JsonUtility.ToJson(socketEvent);

                ws.Send(toJsonStr);
                
            }
        }

        public void JoinRoom(string roomName)
        {
            if (ws.ReadyState == WebSocketState.Open)
            {
                SocketEvent socketEvent = new SocketEvent("JoinRoom", roomName);

                string toJsonStr = JsonUtility.ToJson(socketEvent);

                ws.Send(toJsonStr);

            }
        }

        public void LeaveRoom()
        {
            SocketEvent socketEvent = new SocketEvent("LeaveRoom", "");

            string toJsonStr = JsonUtility.ToJson(socketEvent);

            ws.Send(toJsonStr);
        }

        public void Disconnect()
        {
            if (ws != null)
                ws.Close();
        }








        public void SendMessage()
        {
            //if (inputText.text == "" || ws.ReadyState != WebSocketState.Open)
            //    return;

            //MessageData newMessageData = new MessageData();
            //newMessageData.username = inputUsername.text;
            //newMessageData.message = inputText.text;

            //string toJsonStr = JsonUtility.ToJson(newMessageData);

            //ws.Send(toJsonStr);
            //inputText.text = "";
        }

        

        private void UpdateNotifyMessage()
        {
            if (string.IsNullOrEmpty(tempMessageString) == false)

            {
                //MessageData receiveMessageData = JsonUtility.FromJson<MessageData>(tempMessageString);
                //if (receiveMessageData.username == inputUsername.text)
                //{
                //    sendText.text += receiveMessageData.username + ": " + receiveMessageData.message + "\n";
                //}
                //else
                //{
                //    receiveText.text += receiveMessageData.username + ": " + receiveMessageData.message + "\n";
                //}
                
                print(tempMessageString);

                SocketEvent receiveMessageEvent = JsonUtility.FromJson<SocketEvent>(tempMessageString);
                if (receiveMessageEvent.eventName == "CreateRoom")
                {
                    if(receiveMessageEvent.data == "createroomsuccess")
                    {
                        
                        RoomName.text = inputCreatNameRoomText.text;
                        rootMessenger.SetActive(true);
                        rootCreateRoom.SetActive(false);
                    }
                    else
                    {
                        PanelFail();
                    }
                }
                if(receiveMessageEvent.eventName == "JoinRoom")
                {
            
                   if(receiveMessageEvent.data == "joinroomsuccess")
                    {
                        RoomName.text = inputJoinNameRoomText.text;
                        rootMessenger.SetActive(true);
                        rootJoinRoom.SetActive(false);
                    }
                    else
                    {
                        PanelFail();

                    }
                }
                if (receiveMessageEvent.eventName == "LeaveRoom")
                {
                    if (receiveMessageEvent.data == "leaveroomsuccess")
                    {
                        rootMenu.SetActive(true);
                        rootMessenger.SetActive(false);
                    }
                    else
                    {
                        PanelFail();
                    }
                }



                tempMessageString = "";
            }
        }


        private void OnMessage(object sender, MessageEventArgs messageEventArgs)
        {
            //Debug.Log(messageEventArgs.Data);
            tempMessageString = messageEventArgs.Data;
        }
    }
}