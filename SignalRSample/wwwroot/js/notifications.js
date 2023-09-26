var notificationInput = document.getElementById("notificationInput");
var notificationCounter = document.getElementById("notificationCounter");
var sendButton = document.getElementById("sendButton");
var messageList = document.getElementById("messageList");

// create connection
var connectionNotifications = new signalR.HubConnectionBuilder()
    .withUrl("/hubs/notification")
    .build();

sendButton.disabled = true;


// connect to methods that hub invokes aka receiver notifications from hub
connectionNotifications.on("LoadNotifications", function (messages, counter){
    messageList.innerHTML = "";
    notificationCounter.innerHTML = "<span>(" + counter + ")</span>";
    for (let i = messages.length - 1; i >= 0; i--) {
        var li = document.createElement("li");
        li.textContent = "Notification - " + messages[i];
        messageList.appendChild(li);
    }
});

sendButton.addEventListener("click", function (event) {
    var msg = notificationInput.value;
    connectionNotifications.send("SendMessage", msg).then(function () {
        notificationInput.value = "";
    });
    event.preventDefault();
});

//start connection
function fulfilled() {
    sendButton.disabled = false;

    console.log("Connection to User Hub Successful");
    connectionNotifications.send("LoadMessages")

}

function rejected() {
    sendButton.disabled = true;

    console.log("Connection to User Hub failed")
}

connectionNotifications.start().then(fulfilled, rejected)