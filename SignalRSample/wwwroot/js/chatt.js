var connectionC = new signalR.HubConnectionBuilder()
    .withUrl("/hubs/chat")
    .withAutomaticReconnect([0, 1000, 5000, null])
    .build();



connectionC.on("ReceiveUserConnected", function (userId, userName) {
    addMessage(`${userName} has open a connection`)
});

connectionC.on("ReceiveUserDisconnected", function (userId, userName) {
    addMessage(`${userName} has closed a connection `)
});

connectionC.on("ReceiveAddRoomMessage", function (maxRoom, roomId, roomName, userId, userName) {
    addMessage(`${userName} has Created room ${roomName} `)
    fillRoomDropDown();
});


connectionC.on("ReceiveDeleteRoomMessage", function (deleted, selected, roomName, userId, userName) {
    addMessage(`${userName} has Removed room ${roomName} `)
    fillRoomDropDown();
});

connectionC.on("ReceivePublicMessage", function (roomId, message, roomName, userId, userName) {
    addMessage(`[Public Message - ${roomName}]: ${userName} says ${message}`)
    fillRoomDropDown();
});

connectionC.on("ReceivePrivateMessage", function (senderId, senderName, receiverId, receiverName, message, chatId) {
    addMessage(`[Private Message ${senderName} -> ${receiverName} ] says ${message}`)
    fillRoomDropDown();
})

document.addEventListener('DOMContentLoaded', (event) => {
    fillRoomDropDown();
    fillUserDropDown();
})

function sendPublicMessage() {
    let inputMsg = document.getElementById('txtPublicMessage');
    let ddlSelRoom = document.getElementById('ddlSelRoom');
    let message = inputMsg.value;
    let roomId = ddlSelRoom.value;
    let roomName = ddlDelRoom.options[ddlSelRoom.selectedIndex].text;

    connectionC.send("SendPublicMessage", Number(roomId), message, roomName);
    inputMsg.value = '';

}

function sendPrivateMessage(){
    let inputMsg = document.getElementById('txtPrivateMessage');
    let message = inputMsg.value;

    let ddlSelUser = document.getElementById('ddlSelUser');
    let receiverId = ddlSelUser.value;
    let receiverName = ddlSelUser.options[ddlSelUser.selectedIndex].text;

    connectionC.send("SendPrivateMessage", receiverId, message, receiverName);
    inputMsg.value = '';
}
function addnewRoom(maxRoom) {

    let createRoomName = document.getElementById('createRoomName');

    var roomName = createRoomName.value;

    if (roomName == null && roomName == '') {
        return;
    }
    /*POST*/
    $.ajax({
        url: '/ChatRooms/PostChatRoom',
        dataType: "json",
        type: "POST",
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify({ id: 0, name: roomName }),
        async: true,
        processData: false,
        cache: false,
        success: function (json) {

            /*ADD ROOM COMPLETED SUCCESSFULLY*/
            connectionC.send("SendAddRoomMessage", maxRoom, json.id, json.name);
            createRoomName.value = '';

        },
        error: function (xhr) {
            alert('error');
        }
    })
}

function deleteRoom() {

    let ddlDelRoom = document.getElementById('ddlDelRoom');

    let roomId = ddlDelRoom.value;
    let roomName = ddlDelRoom.options[ddlDelRoom.selectedIndex].text;

    let text = `Do you want to delete Chat Room ${roomName}?`;
    if (confirm(text) == false) {
        return;
    }

    if (roomName == null && roomName == '') {
        return;
    }


    /*DELETE*/
    $.ajax({
        url: `/ChatRooms/DeleteChatRooms/${roomId}`,
        dataType: "json",
        type: "DELETE",
        contentType: 'application/json;',
        async: true,
        processData: false,
        cache: false,
        success: function (json) {

            /*ADD ROOM COMPLETED SUCCESSFULLY*/
            connectionC.send("SendDeleteRoomMessage", json.deleted, json.selected, roomName);
            fillRoomDropDown();

        },
        error: function (xhr) {
            alert('error');
        }
    })
}

function fillUserDropDown() {

    $.getJSON('/ChatRooms/GetChatUser')
        .done(function (json) {

            var ddlSelUser = document.getElementById("ddlSelUser");

            ddlSelUser.innerText = null;

            json.forEach(function (item) {
                var newOption = document.createElement("option");
                newOption.text = item.userName;
                newOption.value = item.id;
                ddlSelUser.add(newOption);
            });

        })
        .fail(function (jqxhr, textStatus, error) {

            var err = textStatus + ", " + error;
            console.log("Request Failed: " + jqxhr.detail);
        });

}

function fillRoomDropDown() {

    $.getJSON('/ChatRooms/GetChatRoom')
        .done(function (json) {
            var ddlDelRoom = document.getElementById("ddlDelRoom");
            var ddlSelRoom = document.getElementById("ddlSelRoom");

            ddlDelRoom.innerText = null;
            ddlSelRoom.innerText = null;

            json.forEach(function (item) {
                var newOption = document.createElement("option");

                newOption.text = item.name;
                newOption.value = item.id;
                ddlDelRoom.add(newOption);


                var newOption1 = document.createElement("option");

                newOption1.text = item.name;
                newOption1.value = item.id;
                ddlSelRoom.add(newOption1);

            });

        })
        .fail(function (jqxhr, textStatus, error) {

            var err = textStatus + ", " + error;
            console.log("Request Failed: " + jqxhr.detail);
        });

}

function addMessage(msg) {
    if (msg == null || msg == '') { 
        return;
    }

    var ui = document.getElementById('messagesList');
    var li = document.createElement("li");
    ui.appendChild(li);
    li.textContent = msg;
}

//start connection
function fulfilled() {
    console.log("Connection to User Hub Successful");

}

function rejected() {
    console.log("Connection to User Hub failed");
}

connectionC.start().then(fulfilled, rejected);