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
            public string room;

            public MessageData(string username, string message, string room)
            {
                this.username = username;
                this.message = message;
                this.room = room;
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

        
        public InputField inputCreateRoom;
        public InputField inputJoinRoom;
        public InputField inputUserID;   //new 2/24/2021
        public InputField inputPassword;
        public InputField inputUseridregis;
        public InputField inputNameregis;
        public InputField inputPasswordregis;
        public InputField inputRepasswordregis;

        public GameObject rootConnection;
        public GameObject rootMessenger;
        public GameObject rootMenu;
        public GameObject rootCreateRoom;
        public GameObject rootJoinRoom;
        public GameObject failPanel;
        public GameObject rootLogin;  //new  2/24/2021
        public GameObject rootRegister;
        public GameObject backToMenu;

       
        public Text roomName;
        public InputField inputCreatNameRoomText;
        public InputField inputJoinNameRoomText;
        public InputField inputText;
        public Text sendText;
        public Text receiveText;
        public Text nameUsername; //new 2/25/2021


        private WebSocket ws;

        private string tempMessageString;
        private string createNameRoom;
        private string joinNameRoom;
        private string loginID;
        private string registerID;
        private string sendMessage;

        public void Start()
        {
            rootConnection.SetActive(true);
            rootMessenger.SetActive(false);
            rootMenu.SetActive(false);
            rootCreateRoom.SetActive(false);
            rootJoinRoom.SetActive(false);
            rootLogin.SetActive(false);
            rootRegister.SetActive(false);
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
            rootLogin.SetActive(true);

            //CreateRoom("TestRoom01");
        }

        public void Login()
        {
            

            loginID = inputUserID.text + "#" + inputPassword.text; //new 2/25/2021
            Login(loginID);
        }

        public void Register()
        {
            rootRegister.SetActive(true);
            rootLogin.SetActive(false);

            
        }

        public void RegisterSuccess()
        {
            if (inputPasswordregis.text == inputRepasswordregis.text)
            {
                registerID = inputUseridregis.text + "#" + inputPasswordregis.text + "#" + inputNameregis.text; //new 2/25/2021
                Register(registerID);
            }
            else
            {
                PanelFail();
            }
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

        public void BackToMenu()
        {
            rootMenu.SetActive(true);
            rootCreateRoom.SetActive(false);
            rootJoinRoom.SetActive(false);
        }

        public void GetLeaveRoom()
        {
            LeaveRoom();
            
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

        public void Login(string data)
        {
            if (ws.ReadyState == WebSocketState.Open)
            {
                SocketEvent socketEvent = new SocketEvent("Login", data);

                string toJsonStr = JsonUtility.ToJson(socketEvent);

                ws.Send(toJsonStr);

            }
        }

        public void Register(string data)
        {
            if (ws.ReadyState == WebSocketState.Open)
            {
                SocketEvent socketEvent = new SocketEvent("Register", data);

                string toJsonStr = JsonUtility.ToJson(socketEvent);

                ws.Send(toJsonStr);

            }
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








        public void SendMessage()  //new 2/26/2021
        {
            if (inputText.text == "" || ws.ReadyState != WebSocketState.Open)
                return;

            ws.OnMessage += OnMessage;
            sendMessage = "#"+nameUsername.text + "#" + inputText.text + "#" + roomName.text+"#";
            Sendmessage(sendMessage);
            //MessageData newMessageData = new MessageData();
            //newMessageData.username = nameUsername.text;
            //newMessageData.message = inputText.text;
            //newMessageData.room = roomName.text;

            //string toJsonStr = JsonUtility.ToJson(newMessageData);

            //ws.Send(toJsonStr);
            inputText.text = "";
        }

        public void Sendmessage(string message) //Editttttttttttttt
        {
            if (ws.ReadyState == WebSocketState.Open)
            {
                SocketEvent socketEvent = new SocketEvent("Sendmessage", message);

                string toJsonStr = JsonUtility.ToJson(socketEvent);

                ws.Send(toJsonStr);



            }
        }



        private void UpdateNotifyMessage()
        {
            if (string.IsNullOrEmpty(tempMessageString) == false)
            {
                

                SocketEvent receiveMessageEvent = JsonUtility.FromJson<SocketEvent>(tempMessageString);
                if (receiveMessageEvent.eventName == "CreateRoom")
                {
                    if(receiveMessageEvent.data == "createroomsuccess")
                    {
                        
                        roomName.text = inputCreatNameRoomText.text;
                        rootMessenger.SetActive(true);
                        rootCreateRoom.SetActive(false);
                        print("createroomsuccess");
                    }
                    else
                    {
                        PanelFail();
                    }
                }
                else if(receiveMessageEvent.eventName == "JoinRoom")
                {
            
                   if(receiveMessageEvent.data == "joinroomsuccess")
                    {
                        roomName.text = inputJoinNameRoomText.text;
                        rootMessenger.SetActive(true);
                        rootJoinRoom.SetActive(false);
                        print("joinroomsuccess");
                    }
                    else
                    {
                        PanelFail();

                    }
                }
                else if (receiveMessageEvent.eventName == "LeaveRoom")
                {
                    if (receiveMessageEvent.data == "leaveroomsuccess")
                    {
                        rootMenu.SetActive(true);
                        rootMessenger.SetActive(false);
                        print("leaveroomsuccess");
                        sendText.text = "";
                        receiveText.text = "";
                    }
                    else
                    {
                        PanelFail();
                    }
                }
                else if (receiveMessageEvent.eventName == "Login")
                {
                    if(receiveMessageEvent.data == "fail")
                    {
                        PanelFail();
                    }
                    else
                    {
                        rootMenu.SetActive(true);
                        rootLogin.SetActive(false);

                        nameUsername.text = receiveMessageEvent.data;
                        print("loginsuccess");
                    }


                }
                else if (receiveMessageEvent.eventName == "Register")
                {
                    
                    if (receiveMessageEvent.data == "registersuccess")
                    {
                        rootLogin.SetActive(true);
                        rootRegister.SetActive(false);
                        print("registersuccess");
                    }
                    else
                    {
                        PanelFail();
                    }
                }
                else if (receiveMessageEvent.eventName == "Sendmessage") //new 2/26/2021
                {
                    
                    print("[1]"+receiveMessageEvent.data);
                    var splitStr = receiveMessageEvent.data.Split('#');
                    var data = splitStr[0];
                    string username = splitStr[1];
                    string message = splitStr[2];
                    string room = splitStr[3];
                    ws.OnMessage += OnMessage;

                    print("Data:" + data);
                    print("Username:" + username);
                    print("Message:" + message);
                    print("Room:" + room);
                    
                    if (room == roomName.text)
                    {


                        if (username == nameUsername.text)
                        {
                            sendText.text += message + ": " + "\n";
                            receiveText.text += "\n";
                        }
                        else
                        {
                            receiveText.text += username + ": " + message + "\n";
                            sendText.text += "\n";
                        }

                    }
                }


                tempMessageString = "";
            }
        }


        private void OnMessage(object sender, MessageEventArgs messageEventArgs)
        {
            
            tempMessageString = messageEventArgs.Data;
        }
    }
}