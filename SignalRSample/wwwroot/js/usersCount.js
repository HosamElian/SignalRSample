// create connection
var connectionUserCount = new signalR.HubConnectionBuilder()
    //.configureLogging(signalR.LogLevel.Information)
    .withUrl("/hubs/userCount", signalR.HttpTransportType.WebSockets)
    .withAutomaticReconnect()
    .build();


// connect to methods that hub invokes aka receiver notifications from hub

connectionUserCount.on("updateTotalViews", (value) => {
    var newCountSpan = document.getElementById("totalViewsCounter");
    newCountSpan.innerText = value.toString();
});

connectionUserCount.on("updateTotalUsers", (value) => {
    var newCountSpan = document.getElementById("totalUsersCounter");
    newCountSpan.innerText = value.toString();
});

//invokes hub methods aka send notifications to hub
function newWindowLoadedOnClient() {
    connectionUserCount.send("NewWindowLoaded");
}

//start connection
function fulfilled() {
    console.log("Connection to User Hub Successful");
    newWindowLoadedOnClient();
}

function rejected() {
    console.log("Connection to User Hub failed");
}

// client side event
connectionUserCount.onclose((error) => {
    //document.body.style.background = "red";
});

connectionUserCount.onreconnected((connectionId) => {
    //document.body.style.background = "green";

});
connectionUserCount.onreconnected((error) => {
    //document.body.style.background = "orange";

});

connectionUserCount.start().then(fulfilled, rejected)