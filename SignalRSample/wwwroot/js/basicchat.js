var sendMessage = document.getElementById("sendMessage");
var messageList = document.getElementById("messagesList");

// create connection
var connectionChat = new signalR.HubConnectionBuilder()
    .withUrl("/hubs/basicchat")
    .build();

sendMessage.disable = true;

// connect to methods that hub invokes aka receiver notifications from hub
connectionChat.on("MessageReceived", function (user, message) {
    var li = document.createElement("li");
    messageList.appendChild(li);
    li.textContent = `${user} - ${message}`;

});

sendMessage.addEventListener("click", function (event) {
    var sender = document.getElementById("senderEmail").value;
    var message = document.getElementById("chatMessage").value;
    var receiver = document.getElementById("receiverEmail").value;
    if (receiver.length > 0) {
        connectionChat.send("SendMessageToReceiver", sender, receiver, message).catch(function (err) {
            return console.error(err.toString());

        });
    }
    else {

    connectionChat.send("SendMessageToAll", sender, message).catch(function (err) {
        return console.error(err.toString());

    });
    }

    event.preventDefault();

})

//start connection
function fulfilled() {
    sendMessage.disable = false;

    console.log("Connection to User Hub Successful");

}

function rejected() {
    console.log("Connection to User Hub failed");
}

connectionChat.start().then(fulfilled, rejected);