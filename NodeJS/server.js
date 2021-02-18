var websocket = require('ws');

var callbackInitServer = ()=>{
    console.log("seawserver is running.");
}

var wss = new websocket.Server({port:5501}, callbackInitServer);

var wsList = [];
var roomList = [];

/*
{
    roomName: "xxx",
}
*/

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
                            console.log("isLeaveSuccess")
                            break;
                        }
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

         });
        
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

function Boardcast(data)
{
    for(var i = 0; i < wsList.length; i++)
    {
        wsList[i].send(data);
    }
}

