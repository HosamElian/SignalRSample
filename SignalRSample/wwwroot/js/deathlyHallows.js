var cloakCounter = document.getElementById("cloakCounter");
var stoneCounter = document.getElementById("stoneCounter");
var wandCounter = document.getElementById("wandCounter");

// create connection
var connectionDeathlyHallows = new signalR.HubConnectionBuilder()
    .withUrl("/hubs/dealthyhallows")
    .build();


// connect to methods that hub invokes aka receiver notifications from hub
connectionDeathlyHallows.on("updateDeathlyHallowCount", (cloak, stone, wand) => {


    cloakCounter.innerText = cloak.toString();
    stoneCounter.innerText = stone.toString();
    wandCounter.innerText = wand.toString();
});

function GetRaceStatusOnClient() {
    connectionDeathlyHallows.invoke("GetRaceStatus").then((reaceCounter) => {
        cloakCounter.innerText = reaceCounter.cloak.toString();
        stoneCounter.innerText = reaceCounter.stone.toString();
        wandCounter.innerText = reaceCounter.wand.toString();
    });
}

//start connection
function fulfilled() {
    GetRaceStatusOnClient();
    console.log("Connection to User Hub Successful");

}

function rejected() {
    console.log("Connection to User Hub failed")
}

connectionDeathlyHallows.start().then(fulfilled, rejected)