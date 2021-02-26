const sqlite = require('sqlite3').verbose();

let db = new sqlite.Database('./db/chatDB.db', sqlite.OPEN_CREATE | sqlite.OPEN_READWRITE, (err)=>{
    if(err) throw err;
    
    

    console.log('connected to database.');

    var websocket = require('ws');

    var callbackInitServer = ()=>{
    console.log("seawserver is running.");
    }

    var wss = new websocket.Server({port:5501}, callbackInitServer);

    var wsList = [];
    var roomList = [];

   

    wss.on("connection", (ws)=>{
    {
        //LobbyZone
        ws.on("message", (data)=>{

            console.log(data);
            
            var toJson = { 
                roomName:"",
                data:""
            }


           
            var toJson = JSON.parse(data);

            
            
            //CreateRoom
            if(toJson.eventName == "CreateRoom")
            {
                console.log("=======1========");
                console.log("client request CreateRoom [" +toJson.data+ "]");
                
                var isFoundRoom = false;
                for(var i = 0; i< roomList.length; i++)
                {
                    if(roomList[i].roomName == toJson.data)
                    {
                        isFoundRoom = true;
                        break;
                    }
                   
                }
                
                if(isFoundRoom == true)
                {
                    //Callback to client : roomName is exist.
                    console.log("Create room : room is found");

                    var callbackMsg = {
                        eventName:"CreateRoom",
                        data:"createroomfail"
                    }
                    //ws.send(JSON.stringify(callbackMsg));
                    var toJsonStr = JSON.stringify(callbackMsg);
                    ws.send(toJsonStr);
                }
                else if (isFoundRoom == false)
                {
                    //Create Room here.
                    console.log("Create room : room is not found");
                    
                    var newRoom = {
                        roomName: toJson.data,
                        wsList: []
                    }

                    newRoom.wsList.push(ws);

                    roomList.push(newRoom);

                    var callbackMsg = {
                        eventName:"CreateRoom",
                        data:"createroomsuccess"
                    }
                    

                    var toJsonStr = JSON.stringify(callbackMsg);
                    ws.send(toJsonStr);

                    console.log("client create room success");
                }

            }
            //JoinRoom
            else if(toJson.eventName == "JoinRoom")
            {
                console.log("=======2========");
                console.log("client request JoinRoom");
                var joinRoomOK = false;
                for(var i = 0;  i < roomList.length; i++)
                {
                    if(roomList[i].roomName == toJson.data)
                    {
                        joinRoomOK = true;
                        roomList[i].wsList.push(ws);
                        break;
                    }
                }

                if(joinRoomOK == false)
                {
                    console.log("join room : not found");
                    
                    var callbackMsg = {
                        eventName: "JoinRoom",
                        data: "joinroomfail"
                    }

                    var toJsonStr = JSON.stringify(callbackMsg);
                    ws.send(toJsonStr);
                    console.log("join room fail");
                }
                else if(joinRoomOK == true)
                {
                                     
                    console.log("join room : found");
                    
                    var callbackMsg = {
                        eventName: "JoinRoom",
                        data: "joinroomsuccess"
                    }
                     
                   
                    //roomList.wsList.push(ws);

                    var toJsonStr = JSON.stringify(callbackMsg);
                    ws.send(toJsonStr);
                    console.log("Join room success");
                }

            }
            //LeaveRoom
            else if(toJson.eventName == "LeaveRoom")
            {

                var isLeaveSuccess = false;
                for(var i = 0; i < roomList.length; i++)
                {
                    for(var j = 0; j< roomList[i].wsList.length; j++)
                    {
                        if(ws == roomList[i].wsList[j])
                        {
                            roomList[i].wsList.splice(j, 1);
                            if(roomList[i].wsList.length <=0)
                            {
                                roomList.splice(i, 1);
                            }
                            isLeaveSuccess = true;
                            console.log("=======3========");
                            console.log("isLeaveSuccess")
                            break;
                        }
                    }
                }
                if(isLeaveSuccess == true)
                {
                

                    //ws.send("LeaveRoomSuccess");

                
                    var callbackMsg = {
                        eventName:"LeaveRoom",
                        data:"leaveroomsuccess"
                    }
                    var toJsonStr = JSON.stringify(callbackMsg);
                    ws.send(toJsonStr);
                    //=======================================================

                    console.log("leave room success");
                }
                else if (isLeaveSuccess == false)
                {
                    //========== Send callback message to Client ============

                    //ws.send("LeaveRoomFail");

            
                    var callbackMsg = {
                        eventName:"LeaveRoom",
                        data:"leaveroomfail"
                    }
                    var toJsonStr = JSON.stringify(callbackMsg);
                    ws.send(toJsonStr);
                    //=======================================================

                    console.log("leave room fail");
                }   
            }
            //Login
            else if(toJson.eventName == "Login")
            {
                var dataFromClient ={
                    eventName: "Login",
                    data: toJson.data
                }

                var splitStr = dataFromClient.data.split('#');
                var userID = splitStr[0];
                var password = splitStr[1];
                var name = splitStr[2];

                var sqlSelect = "SELECT * FROM UserData WHERE UserID='"+userID+"' AND Password='"+password+"'";//Login

                db.all(sqlSelect, (err, rows)=>{
                    if(err)
                    {
                        var callbackMsg = {
                            eventName: "Login",
                            data: "fail"
                        }

                        var toJsonStr = JSON.stringify(callbackMsg);
                        //console.log("[0]"+toJsonStr);
                        ws.send(toJsonStr);
                    }
                    else
                    {
                        if(rows.length > 0)
                        {
                            console.log("======4======");
                            console.log(rows);

                            var callbackMsg = {
                                eventName: "Login",
                                data:rows[0].Name
                            }

                            var toJsonStr = JSON.stringify(callbackMsg);
                            //console.log("[0]"+toJsonStr);
                            ws.send(toJsonStr);
                        }
                        else
                        {
                            var callbackMsg = {
                                eventName: "Login",
                                data: "fail"
                            }

                            var toJsonStr = JSON.stringify(callbackMsg);
                            //console.log("[1]"+toJsonStr);
                            ws.send(toJsonStr);
                        }
                    }



                });
            }
            //Register
            else if(toJson.eventName == "Register")
            {
                var dataFromClient ={
                    eventName: "Register",
                    data: toJson.data
                }

                var splitStr = dataFromClient.data.split('#');
                var userID = splitStr[0];
                var password = splitStr[1];
                var name = splitStr[2];

                var sqlInsert = "INSERT INTO UserData (UserID, Password, Name, Money) VALUES ('"+userID+"', '"+password+"', '"+name+"', '0')"; //Register

                db.all(sqlInsert, (err, rows)=>{
                    if(err)
                    {
                        var callbackMsg = {
                            eventName:"Register",
                            data:"fail"
                        }

                        var toJsonStr = JSON.stringify(callbackMsg);
                        //console.log("[0]"+toJsonStr);
                        ws.send(toJsonStr);
                    }
                    else
                    {
                        var callbackMsg = {
                            eventName:"Register",
                            data:"registersuccess"
                        }

                        var toJsonStr = JSON.stringify(callbackMsg);
                        //console.log("[1]"+toJsonStr);
                        ws.send(toJsonStr);
                    }


                });
            }
            else if (toJson.eventName == "Sendmessage") //Edittttttttttttttttttttttttttttt
            {
                console.log("EVENT SEND MESSAGE");

                /*var dataFromClient ={
                    eventName: "Sendmessage",
                    data: message,
                    
                }


                console.log("Send from client :"+ data);*/
                Boardcast(data);
            }
            
            


        });//message
        
    }

    console.log("client connected.");
    wsList.push(ws);

    

    ws.on("close", ()=>{
        console.log("client disconnected.");
        //wsList = ArrayRemove(wsList, ws);
        for(var i = 0; i < wsList.length; i++)
        {
            if(wsList[i] == ws)
            {
                wsList.splice(i, 1);
                break;
            }
        }

        for(var i = 0; i < roomList.length; i++)
        {
            for(var j = 0; j , roomList[i].wsList.length; j++)
            {
                if(roomList[i].wsList[j] == ws)
                {
                    roomList[i].wsList.splice(j, 1);
                    break;
                }
            }
        }

    });
});


    function ArrayRemove(arr, value)
    {
        return arr.filter((element)=>{
        return element != value;
        });
    }

    function Boardcast(data)      //Editttttttttttttttttttttt
    {
        for(var i = 0; i < wsList.length; i++)
        {
            var callbackMsg = {
                eventName:"Sendmessage",
                data: data
            }

            console.log("Send Success");
            var toJsonStr = JSON.stringify(callbackMsg);
            wsList[i].send(toJsonStr);
        }
        
    }


});