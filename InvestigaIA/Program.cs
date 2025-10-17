using InvestigaIA.Model.Utilities;
using Microsoft.AspNetCore.SignalR.Client;

var connection = new HubConnectionBuilder()
           .WithUrl("ws://localhost:5014/gamehub")
           .Build();

connection.On<string>("ChatUpdate", message =>
{
    Console.WriteLine($"ChatUpdate: {message}");
});

connection.On<string>("ReceiveMessage", message =>
{
    Console.WriteLine($"ReceiveMessage: {message}");
});

connection.On<string>("Send", message =>
{
    Console.WriteLine($"Send: {message}");
});



connection.On<MessageAnswer>("MessageAnswer", message =>
{
    Console.WriteLine(message.Text);
});



await connection.StartAsync();



Console.WriteLine("Connected!");

var Gameid = Console.ReadLine();


await connection.InvokeAsync("JoinGame", Gameid);

var chatId = Console.ReadLine();    


await connection.InvokeAsync("OpenChat", chatId);



while (true) { 

    var message = Console.ReadLine();
    if (message == "exit") break;
    await connection.InvokeAsync("SendMessage", Gameid, chatId, message);
}






await connection.StopAsync();