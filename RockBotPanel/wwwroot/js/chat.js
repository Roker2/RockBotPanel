"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable send button until connection is established
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (user, message) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var encodedMsg = user + " says " + msg;
    /*var li = document.createElement("li");
    li.textContent = encodedMsg;
    document.getElementById("messagesList").appendChild(li);*/
    var messageBox = document.createElement("div");
    messageBox.classList.add("alert");
    messageBox.classList.add("alert-dark");
    var messageBoxHeader = document.createElement("h4");
    messageBoxHeader.classList.add("alert-heading");
    messageBoxHeader.innerHTML = user;
    messageBox.appendChild(messageBoxHeader);
    messageBox.innerHTML += msg;
    document.getElementById("messagesList").appendChild(messageBox);
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;
    connection.invoke("SendMessage", user, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
    document.getElementById("messageInput").value = "";
});